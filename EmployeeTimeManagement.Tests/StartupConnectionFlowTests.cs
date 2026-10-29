using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmployeeTimeManagement.Database;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class StartupConnectionFlowTests
    {
        private const string Secret = "Server=db.example.com;Database=wimpy;User ID=app;Password=hunter2;";

        private string folder;
        private string storePath;
        private string envPath;

        // What the fake database accepts, what the user was asked, and what they answer.
        private bool databaseUp;
        private int tests;
        private readonly List<StartupProblem> prompts = new List<StartupProblem>();
        private readonly Queue<StartupChoice> answers = new Queue<StartupChoice>();
        private int createCalls;
        private bool createSaves;

        [SetUp]
        public void CreateFolder()
        {
            folder = Path.Combine(Path.GetTempPath(), "StartupConnectionFlowTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            storePath = Path.Combine(folder, "data", "connection.dat");
            envPath = Path.Combine(folder, ".env");
            databaseUp = true;
            tests = 0;
            createCalls = 0;
            createSaves = true;
            prompts.Clear();
            answers.Clear();
        }

        [TearDown]
        public void DeleteFolder()
        {
            foreach (string file in Directory.GetFiles(folder, "*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }

            Directory.Delete(folder, true);
        }

        private static byte[] Scramble(byte[] bytes)
        {
            return bytes.Select(b => (byte)(b ^ 0x5A)).Reverse().ToArray();
        }

        private static byte[] Unscramble(byte[] bytes)
        {
            return bytes.Reverse().Select(b => (byte)(b ^ 0x5A)).ToArray();
        }

        private ConnectionStringStore Store()
        {
            return new ConnectionStringStore(storePath, envPath, Scramble, Unscramble);
        }

        private StartupConnectionFlow Flow()
        {
            return new StartupConnectionFlow(
                Store(),
                (string candidate, out string error) =>
                {
                    tests++;
                    error = databaseUp ? null : "Unable to connect";
                    return databaseUp;
                },
                (problem, detail) =>
                {
                    prompts.Add(problem);
                    return answers.Dequeue();
                },
                () =>
                {
                    createCalls++;
                    if (createSaves)
                    {
                        Store().Save(Secret);
                    }

                    return createSaves;
                });
        }

        private void PutEnv()
        {
            File.WriteAllText(envPath, ConnectionStringStore.EnvKey + "=" + Secret);
        }

        [Test]
        public void EnvWithPassingTestContinuesStraightIn()
        {
            PutEnv();

            Assert.That(Flow().Run().Connected, Is.True);
            Assert.That(prompts, Is.Empty);
        }

        [Test]
        public void EnvWithFailingTestOffersRetryOrContinue()
        {
            PutEnv();
            databaseUp = false;
            answers.Enqueue(StartupChoice.ContinueForNow);

            StartupResult result = Flow().Run();

            Assert.That(prompts, Is.EqualTo(new[] { StartupProblem.ConnectionFailed }));
            Assert.That(result.Connected, Is.False);
        }

        [Test]
        public void RetryTestsAgain()
        {
            PutEnv();
            databaseUp = false;
            answers.Enqueue(StartupChoice.Retry);
            answers.Enqueue(StartupChoice.Retry);

            var flow = new StartupConnectionFlow(
                Store(),
                (string candidate, out string error) =>
                {
                    tests++;
                    databaseUp = tests >= 2;
                    error = databaseUp ? null : "Unable to connect";
                    return databaseUp;
                },
                (problem, detail) =>
                {
                    prompts.Add(problem);
                    return answers.Dequeue();
                },
                () => false);

            Assert.That(flow.Run().Connected, Is.True);
            Assert.That(tests, Is.EqualTo(2));
            Assert.That(prompts.Count, Is.EqualTo(1));
        }

        [Test]
        public void NoEnvOffersCreateOrContinue()
        {
            answers.Enqueue(StartupChoice.ContinueForNow);

            StartupResult result = Flow().Run();

            Assert.That(prompts, Is.EqualTo(new[] { StartupProblem.NotSetUp }));
            Assert.That(result.Connected, Is.False);
            Assert.That(createCalls, Is.EqualTo(0));
        }

        [Test]
        public void CreateThatSavesContinuesIn()
        {
            answers.Enqueue(StartupChoice.Create);

            StartupResult result = Flow().Run();

            Assert.That(createCalls, Is.EqualTo(1));
            Assert.That(result.Connected, Is.True);
        }

        [Test]
        public void CancellingCreateAsksAgain()
        {
            createSaves = false;
            answers.Enqueue(StartupChoice.Create);
            answers.Enqueue(StartupChoice.ContinueForNow);

            StartupResult result = Flow().Run();

            Assert.That(prompts, Is.EqualTo(new[] { StartupProblem.NotSetUp, StartupProblem.NotSetUp }));
            Assert.That(result.Connected, Is.False);
        }

        [Test]
        public void AFoundEnvIsEncryptedIntoTheStoreAndRemovedEvenWhenTheTestFails()
        {
            PutEnv();
            databaseUp = false;
            answers.Enqueue(StartupChoice.ContinueForNow);

            Flow().Run();

            Assert.That(File.Exists(envPath), Is.False, "The plain-text .env was left behind.");
            Assert.That(File.ReadAllText(storePath), Does.Not.Contain("hunter2"));
            Assert.That(Store().Load(), Is.EqualTo(Secret));
        }

        [Test]
        public void ContinueForNowReportsTheFailureReason()
        {
            PutEnv();
            databaseUp = false;
            answers.Enqueue(StartupChoice.ContinueForNow);

            Assert.That(Flow().Run().Reason, Does.Contain("Unable to connect"));
        }

        [Test]
        public void AnUnreadableStoreOffersCreate()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(storePath));
            File.WriteAllBytes(storePath, new byte[] { 1, 2, 3 });
            var flow = new StartupConnectionFlow(
                new ConnectionStringStore(storePath, envPath, Scramble, b => { throw new System.Security.Cryptography.CryptographicException("wrong machine"); }),
                (string candidate, out string error) => { error = null; return true; },
                (problem, detail) => { prompts.Add(problem); return StartupChoice.ContinueForNow; },
                () => false);

            Assert.That(flow.Run().Connected, Is.False);
            Assert.That(prompts, Is.EqualTo(new[] { StartupProblem.NotSetUp }));
        }
    }
}

using System;
using System.Collections.Generic;
using EmployeeTimeManagement.Models;
using NUnit.Framework;

namespace EmployeeTimeManagement.Tests
{
    [TestFixture]
    public class LeaveBookingTests
    {
        private static LeaveBookingRequest ValidRequest()
        {
            return new LeaveBookingRequest
            {
                EmployeeID = 1,
                LeaveType = "PTO",
                StartDate = new DateTime(2026, 3, 10),
                EndDate = new DateTime(2026, 3, 12),
                Reason = "Annual leave",
                ContractStartDate = new DateTime(2025, 1, 1),
                ContractEndDate = null,
                ExistingAbsences = new List<Absence>()
            };
        }

        [Test]
        public void AValidBookingIsAccepted()
        {
            LeaveBookingResult result = LeaveBooking.Build(ValidRequest());

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Error, Is.Null);
            Assert.That(result.Absence, Is.Not.Null);
            Assert.That(result.Absence.LeaveType, Is.EqualTo(LeaveType.PTO));
            Assert.That(result.Absence.EmployeeID, Is.EqualTo(1));
            Assert.That(result.Absence.StartDate, Is.EqualTo(new DateTime(2026, 3, 10)));
            Assert.That(result.Absence.EndDate, Is.EqualTo(new DateTime(2026, 3, 12)));
            Assert.That(result.Absence.Reason, Is.EqualTo("Annual leave"));
        }

        [Test]
        public void ABlankReasonIsStoredAsNullRatherThanEmpty()
        {
            LeaveBookingRequest request = ValidRequest();
            request.Reason = "   ";

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.Absence.Reason, Is.Null);
        }

        [Test]
        public void AWOLCannotBeChosen()
        {
            LeaveBookingRequest request = ValidRequest();
            request.LeaveType = "AWOL";

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Absence, Is.Null);
            Assert.That(result.Error, Does.Contain("AWOL"));
        }

        [Test]
        public void ABlankLeaveTypeIsRefused()
        {
            LeaveBookingRequest request = ValidRequest();
            request.LeaveType = null;

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Absence, Is.Null);
        }

        [Test]
        public void AnUnrecognisedLeaveTypeIsRefused()
        {
            LeaveBookingRequest request = ValidRequest();
            request.LeaveType = "GARDENING";

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Absence, Is.Null);
        }

        [Test]
        public void AnEndDateBeforeTheStartDateIsRefused()
        {
            LeaveBookingRequest request = ValidRequest();
            request.StartDate = new DateTime(2026, 3, 12);
            request.EndDate = new DateTime(2026, 3, 10);

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Absence, Is.Null);
            Assert.That(result.Error, Does.Contain("end date"));
        }

        [Test]
        public void ASingleDayRangeIsAllowed()
        {
            LeaveBookingRequest request = ValidRequest();
            request.StartDate = new DateTime(2026, 3, 10);
            request.EndDate = new DateTime(2026, 3, 10);

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void AStartDateBeforeTheContractStartIsRefused()
        {
            LeaveBookingRequest request = ValidRequest();
            request.ContractStartDate = new DateTime(2026, 3, 11);

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Absence, Is.Null);
            Assert.That(result.Error, Does.Contain("employment"));
        }

        [Test]
        public void AnEndDateAfterTheContractEndIsRefused()
        {
            LeaveBookingRequest request = ValidRequest();
            request.ContractEndDate = new DateTime(2026, 3, 11);

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Absence, Is.Null);
            Assert.That(result.Error, Does.Contain("employment"));
        }

        [Test]
        public void AnEmployeeWithNoContractRowIsAlwaysEmployed()
        {
            LeaveBookingRequest request = ValidRequest();
            request.ContractStartDate = null;
            request.ContractEndDate = null;

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ARangeOverlappingAnExistingAbsenceOfTheSameLeaveTypeIsRefused()
        {
            LeaveBookingRequest request = ValidRequest();
            request.ExistingAbsences = new List<Absence>
            {
                new Absence { LeaveType = LeaveType.PTO, StartDate = new DateTime(2026, 3, 11), EndDate = new DateTime(2026, 3, 15) }
            };

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Absence, Is.Null);
            Assert.That(result.Error, Does.Contain("overlap"));
        }

        [Test]
        public void ARangeOverlappingAnExistingAbsenceOfADifferentLeaveTypeIsRefused()
        {
            LeaveBookingRequest request = ValidRequest();
            request.ExistingAbsences = new List<Absence>
            {
                new Absence { LeaveType = LeaveType.Sick, StartDate = new DateTime(2026, 3, 11), EndDate = new DateTime(2026, 3, 15) }
            };

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Absence, Is.Null);
        }

        [Test]
        public void ARangeOverlappingAnExistingAWOLRowIsRefused()
        {
            LeaveBookingRequest request = ValidRequest();
            request.ExistingAbsences = new List<Absence>
            {
                new Absence { LeaveType = LeaveType.AWOL, StartDate = new DateTime(2026, 3, 12), EndDate = new DateTime(2026, 3, 12) }
            };

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Absence, Is.Null);
        }

        [Test]
        public void ARangeThatEndsTheDayAnExistingAbsenceStartsDoesNotOverlap()
        {
            LeaveBookingRequest request = ValidRequest();
            request.StartDate = new DateTime(2026, 3, 8);
            request.EndDate = new DateTime(2026, 3, 10);
            request.ExistingAbsences = new List<Absence>
            {
                new Absence { LeaveType = LeaveType.PTO, StartDate = new DateTime(2026, 3, 11), EndDate = new DateTime(2026, 3, 15) }
            };

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ARangeThatStartsTheDayAfterAnExistingAbsenceEndsDoesNotOverlap()
        {
            LeaveBookingRequest request = ValidRequest();
            request.StartDate = new DateTime(2026, 3, 16);
            request.EndDate = new DateTime(2026, 3, 18);
            request.ExistingAbsences = new List<Absence>
            {
                new Absence { LeaveType = LeaveType.PTO, StartDate = new DateTime(2026, 3, 11), EndDate = new DateTime(2026, 3, 15) }
            };

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void AMaternityAbsenceOfExactlyFourMonthsDoesNotWarn()
        {
            LeaveBookingRequest request = ValidRequest();
            request.LeaveType = "MATERNITY";
            request.StartDate = new DateTime(2026, 1, 1);
            request.EndDate = new DateTime(2026, 5, 1);

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Warning, Is.Null);
        }

        [Test]
        public void AMaternityAbsenceLongerThanFourMonthsWarns()
        {
            LeaveBookingRequest request = ValidRequest();
            request.LeaveType = "MATERNITY";
            request.StartDate = new DateTime(2026, 1, 1);
            request.EndDate = new DateTime(2026, 5, 2);

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Warning, Is.Not.Null);
        }

        [Test]
        public void APTOAbsenceLongerThanFourMonthsDoesNotWarn()
        {
            LeaveBookingRequest request = ValidRequest();
            request.LeaveType = "PTO";
            request.StartDate = new DateTime(2026, 1, 1);
            request.EndDate = new DateTime(2026, 6, 1);

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Warning, Is.Null);
        }

        [Test]
        public void ABookingCostingMoreThanTheAvailableBalanceIsRefusedByDefault()
        {
            LeaveBookingRequest request = ValidRequest();
            request.AvailableBalance = 2;

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Absence, Is.Null);
            Assert.That(result.RequiresOverride, Is.True);
        }

        [Test]
        public void TheOverBalanceRefusalNamesTheBalanceAvailableAndTheCost()
        {
            LeaveBookingRequest request = ValidRequest();
            request.AvailableBalance = 2;

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.Error, Does.Contain("3"));
            Assert.That(result.Error, Does.Contain("2"));
        }

        [Test]
        public void ABookingCostingExactlyTheAvailableBalanceIsAcceptedWithoutAnOverride()
        {
            LeaveBookingRequest request = ValidRequest();
            request.AvailableBalance = 3;

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Absence.OverrideReason, Is.Null);
        }

        [Test]
        public void AnOverBalanceBookingWithNoOverrideReasonWritesNothing()
        {
            LeaveBookingRequest request = ValidRequest();
            request.AvailableBalance = 2;
            request.OverrideReason = "   ";

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Absence, Is.Null);
            Assert.That(result.RequiresOverride, Is.True);
        }

        [Test]
        public void AnOverBalanceBookingWithAnOverrideReasonIsAccepted()
        {
            LeaveBookingRequest request = ValidRequest();
            request.AvailableBalance = 2;
            request.OverrideReason = "Special arrangement with the owner";

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Absence.OverrideReason, Is.EqualTo("Special arrangement with the owner"));
        }

        [Test]
        public void TheOverrideReasonIsTrimmedAndKeptSeparateFromTheAbsenceReason()
        {
            LeaveBookingRequest request = ValidRequest();
            request.Reason = "Annual leave";
            request.AvailableBalance = 2;
            request.OverrideReason = "  Advanced leave agreed with owner  ";

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.Absence.Reason, Is.EqualTo("Annual leave"));
            Assert.That(result.Absence.OverrideReason, Is.EqualTo("Advanced leave agreed with owner"));
        }

        [Test]
        public void AnOverrideReasonTypedForABookingThatDidNotNeedOneIsNotStored()
        {
            LeaveBookingRequest request = ValidRequest();
            request.AvailableBalance = 10;
            request.OverrideReason = "Not actually needed";

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Absence.OverrideReason, Is.Null);
        }

        [Test]
        public void ANullAvailableBalanceSkipsTheOverBalanceCheckEntirely()
        {
            LeaveBookingRequest request = ValidRequest();
            request.LeaveType = "MATERNITY";
            request.StartDate = new DateTime(2026, 1, 1);
            request.EndDate = new DateTime(2026, 1, 3);
            request.AvailableBalance = null;

            LeaveBookingResult result = LeaveBooking.Build(request);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Absence.OverrideReason, Is.Null);
        }
    }
}

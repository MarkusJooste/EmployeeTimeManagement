using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // The list F1 opens: keys that work everywhere, the screens, and the open screen's own.
    internal class KeyboardShortcutsForm : Form
    {
        private static readonly KeyValuePair<string, string>[] Everywhere =
        {
            Pair("Enter", "Next field (on a button: press it)"),
            Pair("Shift+Enter", "Previous field"),
            Pair("Down / Up", "Next / previous field, from a text box"),
            Pair("Tab / Shift+Tab", "Next / previous field or button"),
            Pair("Ctrl+F", "Search box on this screen"),
            Pair("Enter or Down", "From a search box into its list"),
            Pair("Up", "From the top of a list back to its search box"),
            Pair("Space", "Tick a box, or press a button"),
            Pair("Alt+Down", "Open a drop-down list"),
            Pair("Esc", "Close a window like this one")
        };

        private readonly TableLayoutPanel table = new TableLayoutPanel();

        public KeyboardShortcutsForm(string screenName, IEnumerable<KeyValuePair<string, string>> screens,
            IEnumerable<KeyValuePair<string, string>> thisScreen)
        {
            Text = "Keyboard shortcuts";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            AutoScroll = true;
            ClientSize = new Size(520, 560);

            WimpyTheme.Apply(this);

            table.ColumnCount = 2;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.AutoSize = true;
            table.Dock = DockStyle.Top;
            table.Padding = new Padding(20, 12, 20, 12);

            List<KeyValuePair<string, string>> own = thisScreen.ToList();
            if (own.Count > 0)
            {
                AddSection(screenName);
                AddRows(own);
            }

            AddSection("Every screen");
            AddRows(Everywhere);

            AddSection("Go to a screen");
            AddRows(screens);

            var close = new Button { Text = "Close", Width = 100, Height = 30, DialogResult = DialogResult.OK };
            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 50,
                Padding = new Padding(0, 8, 16, 0)
            };
            buttons.Controls.Add(close);

            Controls.Add(table);
            Controls.Add(buttons);
            AcceptButton = close;
            CancelButton = close;

            WimpyTheme.StyleButton(close, ButtonRole.Secondary);

            // The theme restyles labels as they are added, so their fonts are set once
            // they are all in.
            foreach (KeyValuePair<Label, Font> font in fonts)
            {
                font.Key.Font = font.Value;
            }

            // Tall enough to show everything without scrolling, unless the screen is not.
            int wanted = table.PreferredSize.Height + buttons.Height;
            int room = Screen.FromPoint(Cursor.Position).WorkingArea.Height - 80;
            ClientSize = new Size(ClientSize.Width, System.Math.Min(wanted, room));

            Shown += (sender, e) => AutoScrollPosition = Point.Empty;
        }

        private readonly List<KeyValuePair<Label, Font>> fonts = new List<KeyValuePair<Label, Font>>();

        private Label NewLabel(string text, Font font, Padding margin)
        {
            var label = new Label { Text = text, AutoSize = true, Font = font, Margin = margin };
            fonts.Add(new KeyValuePair<Label, Font>(label, font));
            return label;
        }

        private void AddSection(string title)
        {
            Label heading = NewLabel(title, WimpyTheme.Heading(12F), new Padding(0, table.RowCount == 0 ? 0 : 14, 0, 4));
            heading.ForeColor = WimpyTheme.Red;

            table.RowCount++;
            table.Controls.Add(heading, 0, table.RowCount - 1);
            table.SetColumnSpan(heading, 2);
        }

        private void AddRows(IEnumerable<KeyValuePair<string, string>> rows)
        {
            foreach (KeyValuePair<string, string> row in rows)
            {
                table.RowCount++;
                table.Controls.Add(NewLabel(row.Key, WimpyTheme.BodyBold, new Padding(0, 3, 8, 3)), 0, table.RowCount - 1);
                table.Controls.Add(NewLabel(row.Value, WimpyTheme.Body, new Padding(0, 3, 0, 3)), 1, table.RowCount - 1);
            }
        }

        private static KeyValuePair<string, string> Pair(string keys, string action)
        {
            return new KeyValuePair<string, string>(keys, action);
        }
    }
}

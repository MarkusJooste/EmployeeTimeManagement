using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // How a button is drawn: Primary is the solid red action, Secondary the white button
    // with a red outline for cancelling or removing, Nav a sidebar link.
    public enum ButtonRole
    {
        Primary,
        Secondary,
        Nav
    }

    // The app's look, taken from wimpy.co.za: deep red header and navigation, bright red
    // square buttons, bold slab-serif headings and a charcoal footer colour for table headers.
    // Forms are styled at runtime, so the designer files keep their plain layout.
    public static class WimpyTheme
    {
        public static readonly Color Red = Color.FromArgb(190, 6, 19);
        public static readonly Color BrightRed = Color.FromArgb(227, 7, 19);
        public static readonly Color PressedRed = Color.FromArgb(150, 4, 14);
        public static readonly Color Charcoal = Color.FromArgb(45, 45, 45);
        public static readonly Color Text = Color.FromArgb(33, 37, 41);
        public static readonly Color MutedText = Color.FromArgb(108, 112, 117);
        public static readonly Color Page = Color.FromArgb(246, 245, 243);
        public static readonly Color Card = Color.White;
        public static readonly Color Line = Color.FromArgb(225, 222, 218);
        public static readonly Color Tint = Color.FromArgb(252, 232, 233);
        public static readonly Color AlternateRow = Color.FromArgb(250, 249, 248);
        public static readonly Color Disabled = Color.FromArgb(214, 212, 209);

        // The website's headings are Zilla Slab. Rockwell, which comes with Office, is the
        // closest slab serif Windows usually has, and Georgia is on every Windows machine.
        private static readonly string HeadingFamily = FirstInstalled("Zilla Slab", "Rockwell", "Georgia");
        private static readonly string BodyFamily = FirstInstalled("Segoe UI", "Microsoft Sans Serif");

        // 8.25pt keeps labels the width the designer laid them out at; tables get 9pt, as
        // their columns have the room.
        public static readonly Font Body = new Font(BodyFamily, 8.25F);
        public static readonly Font BodyBold = new Font(BodyFamily, 8.25F, FontStyle.Bold);
        public static readonly Font TableBody = new Font(BodyFamily, 9F);
        public static readonly Font TableBold = new Font(BodyFamily, 9F, FontStyle.Bold);

        public static Font Heading(float size)
        {
            return new Font(HeadingFamily, size, FontStyle.Bold);
        }

        // The forms already styled. Styling works from what the designer set, so a second pass
        // would read the theme's own fonts as designed ones and shrink the headings again.
        private static readonly ConditionalWeakTable<Control, object> Styled = new ConditionalWeakTable<Control, object>();

        // Styles a form or control and everything inside it, including controls added later.
        // A form that styles itself in its constructor is left alone when it is shown.
        public static void Apply(Control root)
        {
            object styled;
            if (Styled.TryGetValue(root, out styled))
            {
                return;
            }

            Styled.Add(root, null);

            if (root is Form || root is UserControl)
            {
                if (!IsSet(root, "BackColor"))
                {
                    root.BackColor = Page;
                }

                root.ForeColor = Text;
            }

            StyleChildren(root);
        }

        public static void StyleButton(Button button, ButtonRole role)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.UseVisualStyleBackColor = false;
            button.Cursor = Cursors.Hand;
            button.Tag = role;
            button.FlatAppearance.BorderSize = role == ButtonRole.Nav ? 0 : 1;

            if (role == ButtonRole.Nav)
            {
                button.Font = new Font(BodyFamily, 10F, FontStyle.Bold);
                button.TextAlign = ContentAlignment.MiddleLeft;
                button.Padding = new Padding(18, 0, 0, 0);
            }
            else
            {
                // Bold where the text fits, so a short button does not wrap its label.
                int room = button.Width - button.Padding.Horizontal - 8;
                button.Font = TextRenderer.MeasureText(button.Text, BodyBold).Width <= room ? BodyBold : Body;
            }

            ColourButton(button);

            // Flat buttons draw their disabled text grey on the same red, so a disabled button
            // turns grey instead, where it reads as unavailable.
            button.EnabledChanged -= ButtonEnabledChanged;
            button.EnabledChanged += ButtonEnabledChanged;
        }

        // Marks the selected sidebar link: white with red text, like the website's white buttons.
        public static void SetNavSelected(Button button, bool selected)
        {
            button.BackColor = selected ? Card : Red;
            button.ForeColor = selected ? Red : Color.White;
            button.FlatAppearance.MouseOverBackColor = selected ? Card : PressedRed;
            button.FlatAppearance.MouseDownBackColor = selected ? Card : PressedRed;
        }

        // A toggle among buttons, such as the Daily / Weekly / Monthly period: the chosen one
        // is solid red, the rest outlined.
        public static void SetToggleSelected(Button button, bool selected)
        {
            StyleButton(button, selected ? ButtonRole.Primary : ButtonRole.Secondary);
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Card;
            grid.BorderStyle = BorderStyle.None;
            DrawEdge(grid);
            grid.GridColor = Line;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 32;
            grid.RowTemplate.Height = 26;
            grid.Font = TableBody;

            DataGridViewCellStyle header = grid.ColumnHeadersDefaultCellStyle;
            header.BackColor = Charcoal;
            header.ForeColor = Color.White;
            header.SelectionBackColor = Charcoal;
            header.SelectionForeColor = Color.White;
            header.Font = TableBold;
            header.Padding = new Padding(4, 0, 0, 0);

            // A narrow column cuts its heading short rather than wrapping it onto two lines.
            header.WrapMode = DataGridViewTriState.False;

            DataGridViewCellStyle cells = grid.DefaultCellStyle;
            cells.BackColor = Card;
            cells.ForeColor = Text;
            cells.SelectionBackColor = Tint;
            cells.SelectionForeColor = Text;
            cells.Padding = new Padding(2, 0, 0, 0);

            grid.AlternatingRowsDefaultCellStyle.BackColor = AlternateRow;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Tint;
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Text;

            grid.RowHeadersDefaultCellStyle.BackColor = Charcoal;
            grid.RowHeadersDefaultCellStyle.SelectionBackColor = PressedRed;
            grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                var buttonColumn = column as DataGridViewButtonColumn;
                if (buttonColumn != null)
                {
                    buttonColumn.FlatStyle = FlatStyle.Flat;
                    buttonColumn.DefaultCellStyle.ForeColor = BrightRed;
                    buttonColumn.DefaultCellStyle.Font = TableBold;
                }
            }
        }

        private static void StyleChildren(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                StyleControl(control);
            }

            parent.ControlAdded -= ChildAdded;
            parent.ControlAdded += ChildAdded;
        }

        private static void ChildAdded(object sender, ControlEventArgs e)
        {
            StyleControl(e.Control);
        }

        private static void StyleControl(Control control)
        {
            if (control is Button)
            {
                var button = (Button)control;
                StyleButton(button, button.Tag is ButtonRole ? (ButtonRole)button.Tag : RoleFor(button));
            }
            else if (control is DataGridView)
            {
                StyleGrid((DataGridView)control);
            }
            else if (control is Label)
            {
                StyleLabel((Label)control);
            }
            else if (control is GroupBox)
            {
                // Children are styled first and get explicit fonts and colours, so the heading
                // font and red set on the box do not pass down to them.
                StyleChildren(control);
                control.Font = Heading(10F);
                control.ForeColor = Red;
                return;
            }
            else if (control is TextBox || control is ComboBox || control is DateTimePicker
                || control is CheckBox || control is RadioButton || control is NumericUpDown || control is ListBox)
            {
                SetBodyFont(control);
                if (!IsSet(control, "ForeColor"))
                {
                    control.ForeColor = Text;
                }
            }
            else if (control is Panel && IsSet(control, "BackColor") && control.BackColor == Color.White
                && ((Panel)control).BorderStyle == BorderStyle.FixedSingle)
            {
                // A white boxed panel is a card; a flat edge suits the square look better.
                ((Panel)control).BorderStyle = BorderStyle.None;
            }
            else if (control is Panel && ((Panel)control).BorderStyle == BorderStyle.FixedSingle)
            {
                // A boxed section becomes a white card on the page.
                ((Panel)control).BorderStyle = BorderStyle.None;
                control.BackColor = Card;
                DrawEdge(control);
            }

            if (control.Controls.Count > 0 && !(control is DataGridView))
            {
                StyleChildren(control);
            }
        }

        private static void StyleLabel(Label label)
        {
            Font designed = label.Font;
            bool explicitFont = IsSet(label, "Font");
            bool explicitColour = IsSet(label, "ForeColor");

            if (explicitFont && designed.Size >= 16F)
            {
                // Big figures and page titles.
                label.Font = Heading(designed.Size - 2F);
                if (!explicitColour)
                {
                    label.ForeColor = label.Parent is Form ? Text : Red;
                }

                return;
            }

            if (explicitFont && (designed.Bold || designed.Size >= 10F))
            {
                label.Font = Heading(Math.Max(10F, designed.Size));
                if (!explicitColour)
                {
                    label.ForeColor = Text;
                }

                return;
            }

            SetBodyFont(label);
            if (!explicitColour)
            {
                label.ForeColor = Text;
            }
            else if (label.ForeColor == Color.DimGray || label.ForeColor == Color.Gray)
            {
                label.ForeColor = MutedText;
            }
            else if (label.ForeColor == Color.Firebrick || label.ForeColor == Color.Red)
            {
                label.ForeColor = BrightRed;
            }
        }

        // Cancelling, closing and removing are outlined so the red button on a screen is the
        // action that moves the work forward.
        private static ButtonRole RoleFor(Button button)
        {
            string text = (button.Text ?? string.Empty).ToLowerInvariant();
            string[] secondary = { "cancel", "close", "remove", "delete", "clear", "back", "logout", "log out" };
            return secondary.Any(word => text.Contains(word)) ? ButtonRole.Secondary : ButtonRole.Primary;
        }

        private static void ButtonEnabledChanged(object sender, EventArgs e)
        {
            ColourButton((Button)sender);
        }

        private static void ColourButton(Button button)
        {
            ButtonRole role = button.Tag is ButtonRole ? (ButtonRole)button.Tag : ButtonRole.Primary;

            if (role == ButtonRole.Nav)
            {
                SetNavSelected(button, button.BackColor == Card);
                return;
            }

            if (!button.Enabled)
            {
                button.BackColor = Disabled;
                button.ForeColor = MutedText;
                button.FlatAppearance.BorderColor = Disabled;
                return;
            }

            if (role == ButtonRole.Primary)
            {
                button.BackColor = BrightRed;
                button.ForeColor = Color.White;
                button.FlatAppearance.BorderColor = BrightRed;
                button.FlatAppearance.MouseOverBackColor = Red;
                button.FlatAppearance.MouseDownBackColor = PressedRed;
            }
            else
            {
                button.BackColor = Card;
                button.ForeColor = BrightRed;
                button.FlatAppearance.BorderColor = BrightRed;
                button.FlatAppearance.MouseOverBackColor = Tint;
                button.FlatAppearance.MouseDownBackColor = Tint;
            }
        }

        // A thin grey edge in place of the black FixedSingle border, which WinForms cannot
        // colour, so white sections stay apart on the page.
        public static void DrawEdge(Control control)
        {
            control.Paint -= PaintEdge;
            control.Paint += PaintEdge;
            control.Resize -= InvalidateOnResize;
            control.Resize += InvalidateOnResize;
        }

        private static void PaintEdge(object sender, PaintEventArgs e)
        {
            var control = (Control)sender;
            using (var pen = new Pen(Line))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, control.Width - 1, control.Height - 1);
            }
        }

        private static void InvalidateOnResize(object sender, EventArgs e)
        {
            ((Control)sender).Invalidate();
        }

        private static void SetBodyFont(Control control)
        {
            control.Font = IsSet(control, "Font") && control.Font.Bold ? BodyBold : Body;
        }

        // Whether the designer set the property on this control, rather than it inheriting
        // the value from its parent.
        private static bool IsSet(Control control, string property)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(control)[property];
            return descriptor != null && descriptor.ShouldSerializeValue(control);
        }

        private static string FirstInstalled(params string[] families)
        {
            foreach (string family in families)
            {
                if (FontFamily.Families.Any(installed => string.Equals(installed.Name, family, StringComparison.OrdinalIgnoreCase)))
                {
                    return family;
                }
            }

            return FontFamily.GenericSansSerif.Name;
        }
    }
}

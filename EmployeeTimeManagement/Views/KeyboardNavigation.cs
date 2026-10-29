using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // A key that presses one of a screen's buttons. When onlyIn is given, the key only does
    // so while the cursor is inside that control, such as Enter on a list. With no button it
    // only lists a key the screen handles itself.
    internal sealed class Shortcut
    {
        public Shortcut(Keys keys, Button button, Control onlyIn = null, string description = null)
        {
            Keys = keys;
            Button = button;
            OnlyIn = onlyIn;
            Description = description;
        }

        public Keys Keys { get; private set; }
        public Button Button { get; private set; }
        public Control OnlyIn { get; private set; }

        // What the key does, for the shortcuts list; the button's own text if not given.
        public string Description { get; private set; }

        public string Explain()
        {
            string what = Description ?? Button.Text;
            return OnlyIn == null ? what : what + " (in the list)";
        }
    }

    // A screen with keys of its own, listed on the F1 shortcuts list under "This screen".
    internal interface IHasShortcuts
    {
        IEnumerable<Shortcut> Shortcuts { get; }
    }

    // Moving around a screen without the mouse: Enter steps through the fields as Tab does,
    // the arrow keys move between text boxes, and a search box leads into its list.
    internal static class KeyboardNavigation
    {
        public const string SearchBoxName = "txtSearch";

        // The control the cursor is in, looking inside the screens shown in the main window.
        public static Control Focused(ContainerControl root)
        {
            Control control = root.ActiveControl;

            while (control is ContainerControl && ((ContainerControl)control).ActiveControl != null)
            {
                control = ((ContainerControl)control).ActiveControl;
            }

            return control;
        }

        // Presses the button of the first shortcut for this key that is available now: its
        // button is showing and enabled, and the cursor is where the shortcut applies.
        public static bool RunShortcut(IHasShortcuts screen, Keys keyData, Control focused)
        {
            foreach (Shortcut shortcut in screen.Shortcuts)
            {
                if (shortcut.Keys != keyData)
                {
                    continue;
                }

                if (shortcut.OnlyIn != null && (focused == null || !(shortcut.OnlyIn == focused || shortcut.OnlyIn.Contains(focused))))
                {
                    continue;
                }

                // A key the screen handles itself is only listed here, with no button.
                if (shortcut.Button != null && shortcut.Button.Visible && shortcut.Button.Enabled)
                {
                    shortcut.Button.PerformClick();
                    return true;
                }
            }

            return false;
        }

        // Enter and Shift+Enter, and Down and Up in a text box, move to the next or previous
        // field. A search box leads down into its list, and Up from a list's first row goes
        // back to the search box. Lists, drop-downs and dates keep their own use of the keys.
        public static bool Move(Control focused, Keys keyData)
        {
            if (focused == null || focused is IDataGridViewEditingControl)
            {
                return false;
            }

            if (focused is DataGridView)
            {
                return keyData == Keys.Up && LeaveListUpwards((DataGridView)focused);
            }

            // The text part of a number box is inside the box.
            Control field = focused.Parent is UpDownBase ? focused.Parent : focused;
            bool isTextBox = field is TextBox && (!((TextBox)field).Multiline || ((TextBox)field).ReadOnly);

            if (field.Name == SearchBoxName && (keyData == Keys.Enter || keyData == Keys.Down))
            {
                return EnterList(field);
            }

            bool forward;
            if (keyData == Keys.Enter || keyData == (Keys.Shift | Keys.Enter))
            {
                if (!IsField(field))
                {
                    return false;
                }

                forward = keyData == Keys.Enter;
            }
            else if ((keyData == Keys.Down || keyData == Keys.Up) && isTextBox)
            {
                forward = keyData == Keys.Down;
            }
            else
            {
                return false;
            }

            Form screen = field.FindForm();
            return screen != null && screen.SelectNextControl(field, forward, true, true, false);
        }

        // Puts the cursor in the screen's search box, with its text selected to type over.
        public static bool FocusSearch(Control screen)
        {
            TextBox search = FindVisible<TextBox>(screen, box => box.Name == SearchBoxName);
            if (search == null)
            {
                return false;
            }

            search.Focus();
            search.SelectAll();
            return true;
        }

        // Lets Tab leave the screen's read-only lists for the next control, rather than going
        // from column to column inside them. Lists that are typed into keep Tab for their
        // cells; Ctrl+Tab leaves those.
        public static void LetTabLeaveLists(Control screen)
        {
            foreach (Control child in screen.Controls)
            {
                var list = child as DataGridView;
                if (list != null && list.ReadOnly)
                {
                    list.StandardTab = true;
                }

                LetTabLeaveLists(child);
            }
        }

        // Puts the cursor where a newly opened screen is used from: its search box, or else
        // its first field.
        public static void FocusFirstField(Form screen)
        {
            if (!FocusSearch(screen))
            {
                screen.SelectNextControl(null, true, true, true, true);
            }
        }

        // Keys that pass from one field to the next. A text box that takes several lines
        // keeps Enter for a new line.
        private static bool IsField(Control control)
        {
            var textBox = control as TextBoxBase;
            if (textBox != null)
            {
                return !textBox.Multiline || textBox.ReadOnly;
            }

            var comboBox = control as ComboBox;
            if (comboBox != null)
            {
                return !comboBox.DroppedDown;
            }

            return control is DateTimePicker || control is UpDownBase || control is CheckBox || control is RadioButton;
        }

        // From a search box into the nearest list around it, on its first row if none is
        // chosen yet.
        private static bool EnterList(Control search)
        {
            for (Control around = search.Parent; around != null; around = around.Parent)
            {
                DataGridView list = FindVisible<DataGridView>(around, grid => true);
                if (list == null)
                {
                    continue;
                }

                if (list.Rows.Count == 0)
                {
                    return false;
                }

                list.Focus();
                if (list.CurrentCell == null)
                {
                    DataGridViewCell first = list.Rows[0].Cells.Cast<DataGridViewCell>().FirstOrDefault(cell => cell.Visible);
                    if (first != null)
                    {
                        list.CurrentCell = first;
                    }
                }

                return true;
            }

            return false;
        }

        // Up from a list's first row goes back to the search box above it, if it has one.
        private static bool LeaveListUpwards(DataGridView list)
        {
            if (list.CurrentCell != null && list.CurrentCell.RowIndex > 0)
            {
                return false;
            }

            for (Control around = list.Parent; around != null; around = around.Parent)
            {
                TextBox search = FindVisible<TextBox>(around, box => box.Name == SearchBoxName);
                if (search != null)
                {
                    search.Focus();
                    search.SelectAll();
                    return true;
                }
            }

            return false;
        }

        private static T FindVisible<T>(Control root, Func<T, bool> wanted) where T : Control
        {
            foreach (Control child in root.Controls)
            {
                var match = child as T;
                if (match != null && match.Visible && match.Enabled && wanted(match))
                {
                    return match;
                }

                T inside = child.Visible ? FindVisible(child, wanted) : null;
                if (inside != null)
                {
                    return inside;
                }
            }

            return null;
        }

        // A key as it is written on the shortcuts list, such as "Ctrl+S" or "Enter".
        public static string Describe(Keys keys)
        {
            var text = new StringBuilder();

            if ((keys & Keys.Control) != 0)
            {
                text.Append("Ctrl+");
            }

            if ((keys & Keys.Shift) != 0)
            {
                text.Append("Shift+");
            }

            if ((keys & Keys.Alt) != 0)
            {
                text.Append("Alt+");
            }

            Keys key = keys & Keys.KeyCode;
            switch (key)
            {
                case Keys.Escape:
                    text.Append("Esc");
                    break;
                case Keys.Return:
                    text.Append("Enter");
                    break;
                default:
                    text.Append(key >= Keys.D0 && key <= Keys.D9 ? ((char)('0' + (key - Keys.D0))).ToString() : key.ToString());
                    break;
            }

            return text.ToString();
        }
    }
}

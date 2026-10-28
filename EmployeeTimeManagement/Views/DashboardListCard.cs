using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // One list on the dashboard: a heading with a count, then who needs attention and why.
    // The Store column only shows for the owner, who sees more than one store.
    public class DashboardListCard : UserControl
    {
        private readonly string title;
        private readonly Label lblTitle;
        private readonly ListView lvAlerts;
        private readonly ColumnHeader colEmployee;
        private readonly ColumnHeader colStore;
        private readonly ColumnHeader colNote;

        public DashboardListCard(string title, bool showStore)
        {
            this.title = title;
            Dock = DockStyle.Fill;
            Margin = new Padding(0, 0, 12, 12);
            Padding = new Padding(1);
            BackColor = WimpyTheme.Line;

            lblTitle = new Label();
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 42;
            lblTitle.Padding = new Padding(10, 0, 0, 0);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.BackColor = WimpyTheme.Card;
            lblTitle.ForeColor = WimpyTheme.Text;
            lblTitle.Font = WimpyTheme.Heading(10.5F);
            lblTitle.Text = title;

            colEmployee = new ColumnHeader { Text = "Employee", Width = 115 };
            colStore = new ColumnHeader { Text = "Store", Width = showStore ? 90 : 0 };
            colNote = new ColumnHeader { Text = "Note", Width = 120 };

            lvAlerts = new ListView();
            lvAlerts.Dock = DockStyle.Fill;
            lvAlerts.View = View.Details;
            lvAlerts.FullRowSelect = true;
            lvAlerts.MultiSelect = false;
            lvAlerts.HideSelection = false;
            lvAlerts.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvAlerts.BorderStyle = BorderStyle.None;
            lvAlerts.Font = WimpyTheme.TableBody;
            lvAlerts.ForeColor = WimpyTheme.Text;

            // Charcoal headers to match the tables on the other screens.
            lvAlerts.OwnerDraw = true;
            lvAlerts.DrawColumnHeader += DrawHeader;
            lvAlerts.DrawItem += (sender, e) => e.DrawDefault = true;
            lvAlerts.DrawSubItem += (sender, e) => e.DrawDefault = true;

            // Notes such as a list of missed days can be longer than the column.
            lvAlerts.ShowItemToolTips = true;
            lvAlerts.Columns.AddRange(new[] { colEmployee, colStore, colNote });
            lvAlerts.Resize += (sender, e) => FitNoteColumn();

            // Fill docks before Top, so the list is added first and takes the space left.
            Controls.Add(lvAlerts);
            Controls.Add(lblTitle);
        }

        public void ShowAlerts(List<DashboardAlert> alerts, string allClearText)
        {
            lvAlerts.Items.Clear();

            if (alerts.Count == 0)
            {
                lblTitle.Text = allClearText;
                return;
            }

            lblTitle.Text = title + " (" + alerts.Count + ")";

            foreach (DashboardAlert alert in alerts)
            {
                var item = new ListViewItem(alert.EmployeeName);
                item.SubItems.Add(alert.StoreName);
                item.SubItems.Add(alert.Note);
                item.ToolTipText = string.IsNullOrEmpty(alert.Note) ? alert.EmployeeName : alert.EmployeeName + ": " + alert.Note;
                lvAlerts.Items.Add(item);
            }

            FitNoteColumn();
        }

        // Says the list could not be read, rather than leaving it empty, which would read
        // as "nothing to worry about".
        public void ShowError(string message)
        {
            lvAlerts.Items.Clear();
            lblTitle.Text = title + ": could not load";
            System.Diagnostics.Trace.TraceError("Could not load dashboard list '" + title + "': " + message);
        }

        public void ShowMessage(string message)
        {
            lvAlerts.Items.Clear();
            lblTitle.Text = title + ": " + message;
        }

        private void DrawHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (var background = new SolidBrush(WimpyTheme.Charcoal))
            {
                e.Graphics.FillRectangle(background, e.Bounds);
            }

            if (e.Bounds.Width == 0)
            {
                return;
            }

            Rectangle text = Rectangle.Inflate(e.Bounds, -6, 0);
            TextRenderer.DrawText(e.Graphics, e.Header.Text, WimpyTheme.TableBold, text, Color.White,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        // The Note column takes whatever width the other two leave.
        private void FitNoteColumn()
        {
            int width = lvAlerts.ClientSize.Width - colEmployee.Width - colStore.Width;
            colNote.Width = Math.Max(80, width);
        }
    }
}

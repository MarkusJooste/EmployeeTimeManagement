using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // One labelled fact in the details panel: a small caption over its value, themed to
    // match the surface it sits on. Used for ID number, mobile, start date, job,
    // department and the contract's ending.
    public partial class FactBox : UserControl
    {
        public FactBox()
        {
            InitializeComponent();
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            BackColor = Theme.Surface;
            lblCaption.Font = Theme.SmallFont;
            lblCaption.ForeColor = Theme.InkMuted;
            lblValue.Font = Theme.BodyFont;
            lblValue.ForeColor = Theme.Ink;
        }

        // The word above the value, naming what the value is.
        public string Caption
        {
            get { return lblCaption.Text; }
            set { lblCaption.Text = value; }
        }

        // The fact itself. It can run to a second line, for a Former employee's ending, and
        // longer than the box has room for since the reason for leaving is free text. The
        // label ellipsises what does not fit; the tooltip carries the rest.
        public string Value
        {
            get { return lblValue.Text; }

            set
            {
                lblValue.Text = value;
                tipValue.SetToolTip(lblValue, value);
            }
        }
    }
}

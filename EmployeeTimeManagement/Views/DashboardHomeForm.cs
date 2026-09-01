using EmployeeTimeManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    public partial class DashboardHomeForm : Form
    {
        public DashboardHomeForm()
        {
            InitializeComponent();
            LoadDashboardInformation();
        }

        private void LoadDashboardInformation()
        {
            if (CurrentUser.Manager == null)
            {
                return;
            }

            lblWelcome.Text = $"Welcome back, {CurrentUser.Manager.ManagerName}";

            lblStore.Text = $"Store ID: {CurrentUser.StoreID}";

            // TODO: Database statistics
            lblEmployeeCount.Text = "0";
            lblHoursToday.Text = "0";
            lblLeaveToday.Text = "0";
            lblAwolToday.Text = "0";
        }
    }
}

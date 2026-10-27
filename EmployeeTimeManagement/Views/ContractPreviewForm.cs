using PdfiumViewer;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EmployeeTimeManagement.Views
{
    // Shows a filled-in contract exactly as it will print, with the viewer's own zoom,
    // print and save buttons.
    public class ContractPreviewForm : Form
    {
        private readonly PdfViewer viewer;
        private readonly PdfDocument document;

        public ContractPreviewForm(byte[] pdf, string title)
        {
            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(900, 900);
            MinimumSize = new Size(500, 400);
            ShowIcon = false;
            ShowInTaskbar = false;

            document = PdfDocument.Load(new MemoryStream(pdf));

            viewer = new PdfViewer();
            viewer.Dock = DockStyle.Fill;
            viewer.ShowBookmarks = false;
            viewer.ShowToolbar = true;
            viewer.ZoomMode = PdfViewerZoomMode.FitWidth;
            viewer.Document = document;

            Controls.Add(viewer);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            viewer.Document = null;
            document.Dispose();
            base.OnFormClosed(e);
        }
    }
}

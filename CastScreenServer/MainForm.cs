using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CastScreenServer
{
    public partial class MainForm : Form
    {
        public static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private object _locker = new object();
        private ReaderWriterLock _rwLocker = new ReaderWriterLock();
        private MemoryStream _capturedScreen = new MemoryStream();

        private HttpListener screenCastServer = new HttpListener();

        public MainForm()
        {
            InitializeComponent();

            this.FormClosed += MainForm_FormClosed;

            _logger.Info("app started");
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            NLog.LogManager.Shutdown();
        }

        private void btnStartStop_ClickAsync(object sender, EventArgs e)
        {
            if (!string.Equals(btnStartStop.Tag.ToString(), "start", StringComparison.OrdinalIgnoreCase))
            {
                btnStartStop.Text = "Start Cast";
                btnStartStop.Tag = "start";
                _logger.Info("Screen cast stopped");

                return;
            }

            GetScreenImage();

            try
            {
                _logger.Info("Starting cast...");
                btnStartStop.Text = "Stop Cast";
                btnStartStop.Tag = "stop";
                //await start
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Can't start casting. Details: {0}", ex.Message);
                throw;
            }
        }

        private void GetScreenImage()
        {
            var screenImage = ScreenCaptureHelper.CaptureScreen(Screen.PrimaryScreen.Bounds, true);
            screenImage.Save(_capturedScreen, System.Drawing.Imaging.ImageFormat.Png);

            //using (var file = new System.IO.FileStream(@"C:\Users\PRNC\workspace\screen.png", FileMode.Create, FileAccess.Write))
            //{
            //    screenImage.Save(file, System.Drawing.Imaging.ImageFormat.Png);
            //}
            
        }

        private async Task LoopCaptureScreen()
        {
            while
        }
    }
}

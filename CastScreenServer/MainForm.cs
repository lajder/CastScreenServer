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

        private bool _isStarted;
        private object _locker = new object();
        private ReaderWriterLock _rwLocker = new ReaderWriterLock();
        private MemoryStream _capturedScreen = new MemoryStream();

        private HttpListener _screenCastServer = new HttpListener();

        List<Tuple<string, string>> _netInterfaces;

        public MainForm()
        {
            InitializeComponent();

            _netInterfaces = GetNetworkInterfaces();
            foreach (var netInter in _netInterfaces)
            {
                cbInterface.Items.Add(netInter.Item2 + " - " + netInter.Item1);
            }
            cbInterface.SelectedIndex = 0;
            //cbInterface.SelectedIndex = cbInterface.Items.Count - 1;


            this.FormClosed += MainForm_FormClosed;
            _logger.Info("app started");
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            NLog.LogManager.Shutdown();
        }

        private async void btnStartStop_ClickAsync(object sender, EventArgs e)
        {
            //CancellationTokenSource tokenSource = null;

            if (!string.Equals(btnStartStop.Tag.ToString(), "start", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Info("Stopping screen cast...");
                _isStarted = false;
                btnStartStop.Text = "Start Cast";
                btnStartStop.Tag = "start";
                //tokenSource?.Cancel();

                return;
            }

            try
            {
                _logger.Info("Starting cast...");
                btnStartStop.Text = "Stop Cast";
                btnStartStop.Tag = "stop";

                StartServer();

                _isStarted = true;

                Task[] tasks = new Task[2];
                tasks[0] = Task.Run(StartScreenCapture);
                tasks[1] = Task.Run(ListenHttpRequests);

                Task.WaitAll(tasks);

                //tokenSource = new CancellationTokenSource();
                //CancellationToken cancelToken = tokenSource.Token;

                //var task = Task.Run(async () =>
                //{
                //    cancelToken.ThrowIfCancellationRequested();

                //    while (true)
                //    {
                //        GetScreenImage();
                //        cancelToken.ThrowIfCancellationRequested();
                //        await Task.Delay(40);
                //    }

                //}, tokenSource.Token);

                //try
                //{
                //    await task;
                //}
                //catch (OperationCanceledException cancEx)
                //{
                //    _logger.Info(cancEx, "Task canceled. Details: {0}", cancEx.Message);
                //}
                //finally
                //{
                //    tokenSource.Dispose();
                //}
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Can't start casting. Details: {0}", ex.Message);
                throw;
            }
        }

        private List<Tuple<string, string>> GetNetworkInterfaces()
        {
            List<Tuple<string, string>> netInterfaces = new List<Tuple<string, string>>();

            foreach (var netInter in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var unicastAddr in netInter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastAddr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        netInterfaces.Add(Tuple.Create(netInter.Name, unicastAddr.Address.ToString()));
                    }
                }
            }

            return netInterfaces;
        }

        private void StartServer()
        {
            _logger.Info("Starting server...");
            var selectedAddr = _netInterfaces.ElementAt(cbInterface.SelectedIndex).Item2;
            string serverUrl = $"http://{selectedAddr}:{numPort.Value}/";
            txtURL.Text = serverUrl;
            _screenCastServer.Prefixes.Clear();
            _screenCastServer.Prefixes.Add(serverUrl);
            _screenCastServer.Start();
            _logger.Info("Server started. URL: {0}", serverUrl);
        }

        private async Task ListenHttpRequests()
        {
            while (_isStarted)
            {
                var context = await _screenCastServer.GetContextAsync();
                var requestPath = context.Request.Url.LocalPath;

                byte[] responseData = new byte[0];

                if (requestPath.Contains("screen"))
                {
                    responseData = _capturedScreen.ToArray();
                }
                else if (requestPath.Contains("bootstrap"))
                {
                    string pagePath = Path.Combine(Application.StartupPath, "Webserver", requestPath);
                    responseData = File.ReadAllBytes(pagePath);
                }
                else if (requestPath == "/")
                {
                    string pagePath = Path.ChangeExtension(Path.Combine(Application.StartupPath, "Webserver", "index"), "html");
                    responseData = File.ReadAllBytes(pagePath);
                }

                context.Response.StatusCode = 200;
                try
                {
                    await context.Response.OutputStream.WriteAsync(responseData, 0, responseData.Length);
                }
                catch
                {

                }

                context.Response.Close();
            }
        }

        private async Task StartScreenCapture()
        {
            while (_isStarted)
            {
                var screenImage = ScreenCaptureHelper.CaptureScreen(Screen.PrimaryScreen.Bounds, true);
                screenImage.Save(_capturedScreen, System.Drawing.Imaging.ImageFormat.Png);
                await Task.Delay(40);
            }

            //using (var file = new System.IO.FileStream(@"C:\Users\PRNC\workspace\screen.png", FileMode.Create, FileAccess.Write))
            //{
            //    screenImage.Save(file, System.Drawing.Imaging.ImageFormat.Png);
            //}
        }
    }
}

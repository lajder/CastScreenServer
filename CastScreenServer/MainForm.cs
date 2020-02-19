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

        private bool _autoStart = false;
        private bool _isStarted;
        private byte[] _capturedScreen;
        private string[] _startArgs;

        private HttpListener _screenCastServer = new HttpListener();

        List<Tuple<string, string>> _netInterfaces;

        public MainForm()
        {
            InitializeComponent();

            this.FormClosed += MainForm_FormClosed;
            this.Load += MainForm_Load;

            _netInterfaces = GetNetworkInterfaces();

            foreach (var netInter in _netInterfaces)
            {
                cbInterface.Items.Add(netInter.Item2 + " - " + netInter.Item1);
            }
            cbInterface.SelectedIndex = 0;

            try
            {
                _startArgs = Environment.GetCommandLineArgs();
                if (_startArgs.Length > 2)
                {
                    string customIP = _startArgs[1];
                    string customPort = _startArgs[2];

                    System.Text.RegularExpressions.Regex ipAddrRegex = new System.Text.RegularExpressions.Regex(@"\b(?:(?:2(?:[0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9])\.){3}(?:(?:2([0-4][0-9]|5[0-5])|[0-1]?[0-9]?[0-9]))\b");
                    System.Text.RegularExpressions.Regex portRegex = new System.Text.RegularExpressions.Regex(@"\d{2,4}");
                    if (!ipAddrRegex.IsMatch(customIP)) throw new Exception("Invalid IP adress format");
                    if (!portRegex.IsMatch(customPort)) throw new Exception("Invalid Port number format");

                    decimal decPortNumber = Convert.ToDecimal(customPort);
                    numPort.Value = decPortNumber;

                    var customInterface = Tuple.Create("custom", customIP);
                    _netInterfaces.Add(customInterface);
                    cbInterface.Items.Add(customInterface.Item2 + " - " + customInterface.Item1);
                    cbInterface.SelectedIndex = cbInterface.Items.Count - 1;

                    _autoStart = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Can't get command line arguments. Details: {0}", ex.Message);
            }
            
            _logger.Info("app started");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (_autoStart) btnStartStop_ClickAsync(this, new EventArgs());
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _screenCastServer.Close();
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
                //_screenCastServer.Stop();
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
                    responseData = _capturedScreen;
                }
                else if (requestPath.Contains("bootstrap"))
                {
                    string pagePath = Path.Combine(Application.StartupPath, "Webserver", "bootstrap.min.css");
                    responseData = File.ReadAllBytes(pagePath);
                    context.Response.ContentType = "text/css";
                }
                else if (requestPath == "/")
                {
                    string pagePath = Path.ChangeExtension(Path.Combine(Application.StartupPath, "Webserver", "index"), "html");
                    responseData = File.ReadAllBytes(pagePath);
                    context.Response.ContentType = "text/html";
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

                using (var stream = new MemoryStream())
                {
                    screenImage.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    _capturedScreen = stream.ToArray();
                }

                await Task.Delay(40);
            }
        }
    }
}

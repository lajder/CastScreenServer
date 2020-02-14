using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScreenRecorderLib;

namespace CastScreenServer
{
    public partial class MainForm : Form
    {
        private RecorderOptions _recOptions;
        private Recorder _recorder;
        private Stream _outStream;

        public MainForm()
        {
            InitializeComponent();

            _recOptions = new RecorderOptions
            {
                RecorderMode = RecorderMode.Video,
                IsThrottlingDisabled = false,
                IsHardwareEncodingEnabled = true,
                IsLowLatencyEnabled = true,
                IsMp4FastStartEnabled = true,
                IsFragmentedMp4Enabled = true,
                AudioOptions = new AudioOptions
                {
                    Bitrate = AudioBitrate.bitrate_96kbps,
                    Channels = AudioChannels.Mono,
                    IsAudioEnabled = true
                },
                VideoOptions = new VideoOptions
                {
                    BitrateMode = BitrateControlMode.UnconstrainedVBR,
                    Bitrate = 4000 * 1000,
                    Framerate = 30,
                    IsFixedFramerate = false,
                    EncoderProfile = H264Profile.Main
                },
                MouseOptions = new MouseOptions
                {
                    IsMouseClicksDetected = true,
                    IsMousePointerEnabled = true,
                    MouseClickDetectionMode = MouseDetectionMode.Polling
                },
                DisplayOptions = new DisplayOptions
                {
                    MonitorDeviceName = System.Windows.Forms.Screen.PrimaryScreen.DeviceName
                }
            };

            _recorder = Recorder.CreateRecorder(_recOptions);
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {

        }

        private void GetScreenVideo()
        {
        }
    }
}

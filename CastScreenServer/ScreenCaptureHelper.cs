using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace CastScreenServer
{
    public class ScreenCaptureHelper
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyHeight, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, int diFlags);

        private const Int32 CURSOR_SHOWING = 0x0001;
        private const Int32 DI_NORMAL = 0x0003;

        [StructLayout(LayoutKind.Sequential)]
        private struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINTAPI
        {
            public int x;
            public int y;
        }

        public static Bitmap CaptureScreen(Rectangle bounds, bool showMouse)
        {
            Bitmap screenCapture = new Bitmap(bounds.Width, bounds.Height);

            try
            {
                using (Graphics graphics = Graphics.FromImage(screenCapture))
                {
                    graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);

                    if (showMouse)
                    {
                        CURSORINFO mouseCursor;
                        mouseCursor.cbSize = Marshal.SizeOf(typeof(CURSORINFO));

                        if (GetCursorInfo(out mouseCursor))
                        {
                            if (mouseCursor.flags == CURSOR_SHOWING)
                            {
                                var hdc = graphics.GetHdc();
                                DrawIconEx(hdc, mouseCursor.ptScreenPos.x - bounds.X, mouseCursor.ptScreenPos.y - bounds.Y, mouseCursor.hCursor, 0, 0, 0, IntPtr.Zero, DI_NORMAL);
                                graphics.ReleaseHdc();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainForm._logger.Error(ex, "Can't capture screen. Details: {0}", ex.Message);
                screenCapture = null;
            }

            return screenCapture;
        }

    }
}

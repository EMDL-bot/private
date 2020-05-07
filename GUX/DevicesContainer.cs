using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Runtime.InteropServices;

namespace GUX
{
    public partial class DevicesContainer : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        public static void ReDock(IntPtr handle, int x, int y, int width, int height)
        {
            IntPtr HWND_TOPMOST = new IntPtr(-1);
            const short SWP_NOACTIVATE = 0x0010;

            SetWindowPos(handle, HWND_TOPMOST, x, y, width, height, SWP_NOACTIVATE);
        }

        private Task Exec(string cmd)
        {
            try
            {
                return Task.Factory.StartNew(() =>
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = "/c " + cmd,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    process.WaitForExit();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private Task AttachDevice(int index)
        {
            return Task.Factory.StartNew(() =>
            {
                Process proc = new Process();
                proc.StartInfo.FileName = $"\"{@"C:\Program Files\Microvirt\MEmu\MEmu.exe"}\"";
                proc.StartInfo.Arguments = "MEmu_" + index;
                proc.Start();

                proc.WaitForInputIdle();

                while (proc.MainWindowHandle == IntPtr.Zero)
                {
                    Thread.Sleep(100);
                    proc.Refresh();
                }

                Invoke((MethodInvoker)delegate
                {
                    var panel = new PanelControl();
                    panel.Size = new Size(350, 250);
                    panel.MinimumSize = new Size(350, 250);
                    panel.MaximumSize = new Size(350, 250);
                    panel.AutoSize = true;
                    panel.Dock = DockStyle.Left;

                    panel.Tag = proc.MainWindowHandle;
                    flowLayoutPanel1.Controls.Add(panel);
                    SetParent(proc.MainWindowHandle, panel.Handle);

                    SetInterval((action) =>
                    {
                        panel.Invoke((MethodInvoker)delegate
                        {
                            Rectangle r = panel.RectangleToScreen(panel.ClientRectangle);
                            ReDock((IntPtr)panel.Tag, r.X, r.Y, r.Width, r.Height);
                            MoveWindow((IntPtr)panel.Tag, 0, 0, r.Width, r.Height, true);
                        });
                    }, 1000);
                });
            });
        }

        public System.Timers.Timer SetInterval(Action<System.Timers.Timer> Act, int Interval)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += (sender, args) => Act(timer);
            timer.AutoReset = true;
            timer.Interval = Interval;
            timer.Start();

            return timer;
        }

        private void Panel_SizeChanged(object sender, EventArgs e)
        {
            
        }

        public DevicesContainer()
        {
            InitializeComponent();
        }

        private async void DevicesContainer_Load(object sender, EventArgs e)
        {
            await Exec("TASKKILL /T /F /IM MEmuConsole.exe /IM MEmu.exe /IM MEmuSVC.exe /IM MEmuHeadless.exe");
            await AttachDevice(8).ContinueWith(async (ad) =>
            {
                if (ad.IsCompleted)
                {
                    await AttachDevice(1).ContinueWith(async (ad2) =>
                    {
                        if (ad2.IsCompleted)
                        {
                            await AttachDevice(3).ContinueWith(async (ad3) =>
                            {
                                if (ad3.IsCompleted)
                                {
                                    await AttachDevice(6);
                                }
                            });
                        }
                    });
                }
            });
        }

        private async void DevicesContainer_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            await Exec("TASKKILL /T /F /IM MEmuConsole.exe /IM MEmu.exe /IM MEmuSVC.exe /IM MEmuHeadless.exe");
            Application.ExitThread();
        }
    }
}
using System;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using System.Runtime.InteropServices;
using GUX.Core;
using DevExpress.XtraLayout;
using System.Collections.Generic;
using System.Management;

namespace GUX
{
    public partial class DevicesContainer : DevExpress.XtraEditors.XtraForm
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        public string actionsLogFile { get; set; }
        public string errorsLogFile { get; set; }
        public string blockedLogFile { get; set; }
        public string currentDirectory { get; set; }
        public CancellationToken token { get; set; }

    private System.Timers.Timer _interval = null;

        private async void Log(string message, string index, string loglevel, bool blocked = false)
        {
            try
            {
                var logtype = loglevel == "info" ? "ok" : loglevel.ToLower();
                using (FileStream fs = new FileStream(Application.StartupPath + "\\Actions Logs\\" + actionsLogFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        await sw.WriteLineAsync($"{DateTime.UtcNow.ToLongTimeString()}: [{index}] - ({logtype}) | {message.ToLower()}");
                    }
                }
                if (blocked)
                {
                    BlockedLog(message, index);
                    return;
                }
                if (logtype == "error")
                    ErrorLog(message, index);

            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        private async void BlockedLog(string message, string index)
        {
            try
            {
                using (FileStream fs = new FileStream(Application.StartupPath + "\\Blocked Logs\\" + blockedLogFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        await sw.WriteLineAsync($"{DateTime.UtcNow.ToLongTimeString()}: [{currentDirectory}/{index}] | {message.ToLower()}");
                    }
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        private async void ErrorLog(string message, string index)
        {
            try
            {
                using (FileStream fs = new FileStream(Application.StartupPath + "\\Error Logs\\" + errorsLogFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        await sw.WriteLineAsync($"{DateTime.UtcNow.ToLongTimeString()}: [{index}] | {message.ToLower()}");
                    }
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
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
                    while (!process.StandardOutput.EndOfStream)
                    {
                        var line = process.StandardOutput.ReadLine();
                        Console.WriteLine($"EXEC Output : {line}");
                    }
                    process.WaitForExit();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private PanelControl DevicePanel(string name, IntPtr tag)
        {
            var panel = new PanelControl();
            panel.Name = name;
            var bounds = Screen.PrimaryScreen.Bounds;
            var size = new Size(bounds.Width / 5, bounds.Height / 5);
            panel.Size = size;
            panel.MinimumSize = size;
            panel.MaximumSize = size;
            panel.Tag = tag;
            return panel;
        }

        private LayoutControlItem LCI(PanelControl panel, string name, string text, int tag)
        {
            LayoutControlItem item = new LayoutControlItem(DevicesLayoutControl, panel);
            item.Name = name;
            item.TextAlignMode = TextAlignModeItem.AutoSize;
            item.TextLocation = DevExpress.Utils.Locations.Top;
            item.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
            item.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
            item.TextVisible = true;
            item.AllowHtmlStringInCaption = true;
            item.Text = text;
            item.Tag = tag;
            return item;
        }

        public Task<bool> AttachDevice(string index)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    Process proc = new Process();
                    proc.StartInfo.FileName = $"\"{@"C:\Program Files\Microvirt\MEmu\MEmu.exe"}\"";
                    proc.StartInfo.Arguments = "MEmu_" + index;
                    proc.Start();

                    proc.WaitForInputIdle();

                    while (proc.MainWindowHandle == IntPtr.Zero)
                    {
                        Thread.Sleep(Globals.DRIVER_WAIT_POLLING_INTERVAL);
                        proc.Refresh();
                    }

                    Invoke((MethodInvoker) delegate
                    {
                        var panel = DevicePanel($"Panel_{proc.Id}_{index}", proc.MainWindowHandle);
                        var item = LCI(panel, $"LCI_{proc.Id}_{index}", $"<b>{currentDirectory}</b>/{index}", proc.Id);
                        
                        DevicesGroupLayout.AddItem(item);
                        SetParent(proc.MainWindowHandle, panel.Handle);
                    });

                    Thread.Sleep(5000);
                    Log("attach device completed!", index, "info");
                    return true;
                }
                catch (Exception x)
                {
                    Console.WriteLine(x.Message);
                    //Log("attach device failed!", index, "error");
                    return false;
                }
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

        private Task KillProcessAndChildren(int pid)
        {
            return Task.Factory.StartNew(() =>
            {
                if (pid == 0)
                    return;

                ManagementObjectSearcher searcher = new ManagementObjectSearcher
                        ("Select * From Win32_Process Where ParentProcessID=" + pid);
                ManagementObjectCollection moc = searcher.Get();
                foreach (ManagementObject mo in moc)
                {
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                }
                try
                {
                    Process proc = Process.GetProcessById(pid);
                    proc.Kill();
                }
                catch (ArgumentException)
                {
                    // Process already exited.
                }
            }); 
        }

        private Task StopDevices()
        {
            return Task.Factory.StartNew(() =>
            {
                DevicesLayoutControl.Invoke((MethodInvoker)async delegate
                {
                    try
                    {
                        if (_interval != null) _interval.Close();
                        if (_interval != null) _interval.Dispose();
                        DevicesGroupLayout.Items.ConvertToTypedList().ForEach((item) =>
                        {
                            var pid = (int)item.Tag;
                            //await KillProcessAndChildren(pid);
                            Process.GetProcessById(pid)?.Kill();
                        });
                        DevicesGroupLayout.Clear(true);
                        await Final();
                        Log("stop devices completed!", "-", "info");
                    }
                    catch (Exception x)
                    {
                        Console.WriteLine(x.Message);
                    }
                });
            });
        }

        private Task StopDevice(string index)
        {
            return Task.Factory.StartNew(() =>
            {
                DevicesLayoutControl.Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        _interval?.Close();
                        _interval?.Dispose();
                        var item = (LayoutControlItem)DevicesGroupLayout.Items.ConvertToTypedList().Find(i => i.Name.EndsWith($"_{index}"));
                        if(item != null)
                        {
                            var pid = (int)item.Tag;
                            //await KillProcessAndChildren(pid);

                            item.Control?.Dispose();
                            DevicesGroupLayout.Remove(item);
                            item?.Dispose();

                            Process.GetProcessById(pid)?.Kill();

                            if (autoFitDevicesButton.Down)
                                _interval = SetInterval(async (timer) => {
                                    await FitDevices();
                                }, 3000);

                            Log("stop device completed!", index, "info");
                        }
                    }
                    catch (Exception x)
                    {
                        Console.WriteLine(x.Message);
                    }
                });
            });
        }

        private Task FitDevices()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    DevicesLayoutControl.Invoke((MethodInvoker)delegate
                    {
                        try
                        {
                            DevicesGroupLayout.Items.ConvertToTypedList().ForEach((item) =>
                            {
                                var control = ((LayoutControlItem)item).Control;
                                if (control != null)
                                    control.Invoke((MethodInvoker)delegate
                                    {
                                        Rectangle r = control.RectangleToScreen(control.ClientRectangle);
                                        MoveWindow((IntPtr)control.Tag, 0, 0, r.Width, r.Height, true);
                                    });
                            });
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine(x.Message);
                        }
                    });
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }

        private async void DevicesContainer_Load(object sender, EventArgs e)
        {
            autoFitDevicesButton.Down = true;
            await Final();
        }

        private async void DevicesContainer_FormClosing(object sender, FormClosingEventArgs e)
        {
            await Final();
            this.Close();
        }

        private async void stopDevicesButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            await StopDevices();
        }

        public Task Stop()
        {
            return StopDevices();
        }

        public Task Stop(string index)
        {
            return StopDevice(index);
        }

        public Task Fit()
        {
            return FitDevices();
        }

        public Task Final()
        {
            return Exec("TASKKILL /T /F /IM MEmuConsole.exe /IM MEmu.exe /IM MEmuSVC.exe /IM MEmuHeadless.exe /IM adb.exe");/*.ContinueWith((kk) => {
                if (kk.IsCompleted)
                {
                    DevicesLayoutControl.Invoke((MethodInvoker)delegate
                    {
                        try
                        {
                            DevicesGroupLayout.Clear(true);
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine(x.Message);
                        }
                    });
                }
            });*/
        }

        private void DevicesLayoutControlItemsChanged(object sender, EventArgs e)
        {
            DevicesCountLabel.Caption = $"{DevicesGroupLayout.Items.Count}";
        }

        private async void fitDevicesButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            await FitDevices();
        }

        private void autoFitDevicesButton_DownChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _interval?.Close();
            _interval?.Dispose();
            if (autoFitDevicesButton.Down)
                _interval = SetInterval(async (timer) => {
                    await FitDevices();
                }, 3000);
        }
    }
}
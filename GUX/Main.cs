using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using AppiumHelper;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSplashScreen;
using FastColoredTextBoxNS;
using GmailDemo;
using GUX.Core;
using GUX.Extensions;
using GUX.Services;
using GUX.UC;
using Npgsql;
using Polly;
using Polly.Bulkhead;
using XanderUI;

namespace GUX
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {
        private List<string> Devices;
        private DataTable _Devices = new DataTable();
        private DataRow[] _Rows;
        private Thread _Thread;
        private ManualResetEvent _ManualResetEvent = new ManualResetEvent(true);
        private string _conString = "";
        private PostgreSqlHelper _psqlHelper;
        private AppiumHelperService _Service;
        private Scheduler _Scheduler;
        private _CONFIG _Config;
        private DevicesContainer iDevices = null;
        private string actionsLogFile = "",
            errorsLogFile = "",
            blockedLogFile = "",
            disabledLogFile = "",
            warningLogFile = "";
        private PerformanceCounter ram = new PerformanceCounter("Memory", "% Committed Bytes In Use", String.Empty, Environment.MachineName);
        private PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total", Environment.MachineName);
        private CancellationTokenSource _CancellationTokenSource;
        private AsyncBulkheadPolicy _AsyncBulkheadPolicy;
        private List<Task> TasksList;
        private bool helper = false, alreadyExists = false;
        private string currentDirectory = "", currentDateFilter = "", currentKeyword = "";
        private int maxRetries = 3;
        DevExpress.XtraBars.Ribbon.GalleryItem _downItem = null;
        DefaultBoolean _downItemChecked = DefaultBoolean.Default;

        private SkinElement GetItemCheckedElement()
        {
            SkinElement elem = EditorsSkins.GetSkin(DevExpress.LookAndFeel.UserLookAndFeel.Default)[EditorsSkins.SkinTileItemChecked];
            if (elem == null)
                elem = EditorsSkins.GetSkin(DevExpress.XtraEditors.Controls.DefaultSkinProvider.Default)[EditorsSkins.SkinTileItemChecked];
            return elem;
        }

        public Main()
        {
            InitializeComponent();
            Devices = new List<string>();
            _Service = new AppiumHelperService();
            _Scheduler = new Scheduler();
            this.DoubleBuffered = true;
            //this.SetStyle(ControlStyles.ResizeRedraw, true);
            directoryCLB.DoubleBuffer();
            iStackPanel.AutoScroll = true;
            ((WindowsUISeparator)UIButtonPanel.Buttons[6]).Appearance.ForeColor = Color.FromArgb(37, 37, 38);
            ImageCollection images = new ImageCollection();
            images.ImageSize = new Size(64, 32);
            var fakeImage = new Bitmap(64, 32);
            images.AddImage(fakeImage);
            images.AddImage(GetSemiTransparentImage());
            images.AddImage(fakeImage);
            UIButtonPanel.ButtonBackgroundImages = images;
            Globals.SchServices = new SchedulerService();
            iDevices = new DevicesContainer();
            try
            {
                var element = GetItemCheckedElement();
                Image image = element.Image.Image;
                Size originalSize = image.Size;
                Size newImageSize = new Size(originalSize.Width / 2, originalSize.Height / 2);
                image = new Bitmap(image, newImageSize);
                element.SetActualImage(image, true);
                LookAndFeelHelper.ForceDefaultLookAndFeelChanged();
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        public void ApplyDropShadow()
        {
            try
            {
                new Dropshadow(this)
                {
                    ShadowBlur = 15,
                    ShadowSpread = -5,
                    ShadowColor = Color.FromArgb(85, 0, 0, 0)
                }.RefreshShadow();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                    process.WaitForExit();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private Task LoadDevices()
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
                            Arguments = "/c adb devices",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();

                    while (!process.StandardOutput.EndOfStream)
                    {
                        var line = process.StandardOutput.ReadLine();
                        Devices.Add(line);
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

        private Task SetFileWatcher(bool init = false)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    BeginInvoke((MethodInvoker)delegate
                    {
                        fctbActions.Clear();
                        fctbErrors.Clear();
                        fctbWarning.Clear();

                        if (!Directory.Exists(Application.StartupPath + "\\Action Logs\\"))
                            Directory.CreateDirectory(Application.StartupPath + "\\Actions Logs\\");

                        if (!Directory.Exists(Application.StartupPath + "\\Error Logs\\"))
                            Directory.CreateDirectory(Application.StartupPath + "\\Error Logs\\");

                        if (!Directory.Exists(Application.StartupPath + "\\Warning Logs\\"))
                            Directory.CreateDirectory(Application.StartupPath + "\\Warning Logs\\");

                        if (!Directory.Exists(Application.StartupPath + "\\Blocked Logs\\"))
                            Directory.CreateDirectory(Application.StartupPath + "\\Blocked Logs\\");

                        if (!Directory.Exists(Application.StartupPath + "\\Disabled Logs\\"))
                            Directory.CreateDirectory(Application.StartupPath + "\\Disabled Logs\\");

                        if (init)
                        {
                            fctbBlocked.Clear();
                            fctbDisabled.Clear();

                            blockedLogFile = "GUX.BlockedLog-" + DateTime.UtcNow.ToString("yyyyMMdd-hh-mm") + ".log";
                            blockedLogFSW.Path = Application.StartupPath + "\\Blocked Logs\\";
                            blockedLogFSW.Filter = blockedLogFile;

                            disabledLogFile = "GUX.DisabledLog-" + DateTime.UtcNow.ToString("yyyyMMdd-hh-mm") + ".log";
                            disabledLogFSW.Path = Application.StartupPath + "\\Disabled Logs\\";
                            disabledLogFSW.Filter = disabledLogFile;
                        }

                        actionsLogFile = "GUX.ActionLog-" + DateTime.UtcNow.ToString("yyyyMMdd-hh-mm") + ".log";
                        actionLogFSW.Path = Application.StartupPath + "\\Actions Logs\\";
                        actionLogFSW.Filter = actionsLogFile;

                        errorsLogFile = "GUX.ErrorLog-" + DateTime.UtcNow.ToString("yyyyMMdd-hh-mm") + ".log";
                        errorLogFSW.Path = Application.StartupPath + "\\Error Logs\\";
                        errorLogFSW.Filter = errorsLogFile;

                        warningLogFile = "GUX.WarningLog-" + DateTime.UtcNow.ToString("yyyyMMdd-hh-mm") + ".log";
                        warningLogFSW.Path = Application.StartupPath + "\\Warning Logs\\";
                        warningLogFSW.Filter = warningLogFile;
                    });
                }
                catch (Exception x)
                {
                    Console.WriteLine("File Watchers : " + x.Message);
                }
            });
        }

        private void StopFileWatcher()
        {
            ((ISupportInitialize)actionLogFSW).EndInit();
            ((ISupportInitialize)errorLogFSW).EndInit();
        }

        private void LoadFont()
        {
            //PrivateFontCollection pfc = new PrivateFontCollection();
            //pfc.AddFontFile(Application.StartupPath + "\\ANDROID ROBOT.ttf");
            //var font = new Font(pfc.Families[0], 20, FontStyle.Regular);
            ////gux_title.Height = font.Height;
            ////gux_title.Font = font;
            //var font2 = new Fonts(8);
            //appiumHint.Font = font2.FontAwesome;
            //timer.Tick += Timer_Tick; ; // don't freeze the ui
            //timer.Interval = 1024;
            //timer.Enabled = true;
            ////var fonto = new Fonts(10);
            //appiumHint.Text = new Fonts.fa(font2).circle;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                ramLabel.Caption = String.Format("{0:##0}%", ram.NextValue());
                cpuLabel.Caption = String.Format("{0:##0}%", cpu.NextValue());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                timer.Enabled = false;
            }
        }

        private string DeviceNameGenerator(string index)
        {
            const int h = 21503;
            int i = int.Parse(index);
            return "127.0.0.1:" + ((i > 0) ? (i * 10) + h : h);
        }

        private Task LoadDbConfig()
        {
            return Task.Factory.StartNew(() =>
            {
                IFormatter formatter = new BinaryFormatter();
                using (var stream = new FileStream(Application.StartupPath + "\\" + this.Tag.ToString(), FileMode.Open, FileAccess.Read))
                {
                    var _db = (_DB)formatter.Deserialize(stream);
                    _conString = _db.ConnectionString();
                }
            });
        }

        private Task LoadConfig()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    if (File.Exists(Application.StartupPath + "\\" + saveSettingsButton.Tag.ToString()))
                    {
                        IFormatter formatter = new BinaryFormatter();

                        using (var stream = new FileStream(Application.StartupPath + "\\" + saveSettingsButton.Tag.ToString(), FileMode.Open, FileAccess.Read))
                        {
                            _Config = (_CONFIG)formatter.Deserialize(stream);
                        }

                        if (_Config != null)
                        {
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                try
                                {
                                    this.portNum.Value = Globals.SERVER_PORT = _Config._SERVER_PORT;
                                    this.testDriverToNum.Value = Globals.TEST_DRIVER_TIMEOUT = _Config._TEST_DRIVER_TIMEOUT;
                                    this.waitNum.Value = Globals.DRIVER_WAIT_TIMEOUT = _Config._DRIVER_WAIT_TIMEOUT;
                                    this.pollingNum.Value = Globals.DRIVER_WAIT_POLLING_INTERVAL = _Config._DRIVER_WAIT_POLLING_INTERVAL;
                                    this.driverLetterText.Text = Globals.DRIVE_LETTER = _Config._DRIVE_LETTER;
                                    this.vmsDirectoryText.Text = Globals.VMS_DIRECTORY = _Config._VMS_DIRECTORY;
                                    this.failedAutoRetrySwitch.SwitchState = (Globals.FAILED_AUTO_RETRY = _Config._FAILED_AUTO_RETRY) ? XUISwitch.State.On : XUISwitch.State.Off;
                                    this.failedMAxRetriesNum.Value = Globals.FAILED_MAX_RETRIES = _Config._FAILED_MAX_RETRIES;
                                    this.failedActionsMaxRetriesNum.Value = Globals.FAILED_ACTION_MAX_RETRIES = _Config._FAILED_ACTION_MAX_RETRIES;
                                    this.concurrentThreadsNum.Value = Globals.THREADS_CONCURRENCY = _Config._THREADS_CONCURRENCY;
                                    this.threadsIntervalNum.Value = Globals.THREADS_INTERVAL = _Config._THREADS_INTERVAL;
                                    this.appTopMostSwitch.SwitchState = (Globals.UI_TOP_MOST = _Config._UI_TOP_MOST) ? XUISwitch.State.On : XUISwitch.State.Off;
                                    this.appTransparentSwitch.SwitchState = (Globals.UI_TRANSPARENT = _Config._UI_TRANSPARENT) ? XUISwitch.State.On : XUISwitch.State.Off;

                                    Globals.DEFAULT_SCENARIO = _Config._DEFAULT_SCENARIO;
                                    var scenario = new ImageComboBoxItem()
                                    {
                                        Value = $"{Globals.DEFAULT_SCENARIO.name[0]}|{Globals.DEFAULT_SCENARIO.ID}",
                                        Description = Globals.DEFAULT_SCENARIO.name
                                    };
                                    this.defaultScenarioGallery.Properties.Items.Add(scenario);
                                    this.defaultScenarioGallery.SelectedIndex = 0;

                                    this.warmupAutoRunSwitch.SwitchState = (Globals.WARMUP_AUTORUN = _Config._WARMUP_AUTORUN) ? XUISwitch.State.On : XUISwitch.State.Off;
                                    this.warmupInboxFolderSwitch.SwitchState = (Globals.WARMUP_PROCEED_INBOX_FOLDER = _Config._WARMUP_PROCEED_INBOX_FOLDER) ? XUISwitch.State.On : XUISwitch.State.Off;
                                    this.treatedInboxEmailsMaxNum.Value = Globals.WARMUP_MAX_TREATED_INBOX_EMAILS = _Config._WARMUP_MAX_TREATED_INBOX_EMAILS;
                                }
                                catch (Exception c)
                                {
                                    Console.WriteLine(c.Message);
                                }
                            });
                        }
                    }
                }
                catch (Exception x)
                {
                    Console.WriteLine(x.Message);
                }
            });
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var handle = SplashScreenManager.ShowOverlayForm(this, new DevExpress.XtraSplashScreen.OverlayWindowOptions()
            {
                SkinName = "Visual Studio 2013 Dark",
                FadeIn = true,
                FadeOut = true,
                Image = global::GUX.Properties.Resources.loader
            });

            outputDockPanel.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            accordionControl1.SelectElement(accordionProcess);
            ScenarioTablePanel.AutoScroll = true;

            await SetFileWatcher(true);
            await LoadDbConfig();
            await LoadConfig();
            //
            ApplyDropShadow();

            _psqlHelper = new PostgreSqlHelper(_conString);

            iDevices.Show();

            await LoadLocations();

            await _Service.StartAppium().ContinueWith((sa) =>
            {
                if (sa.IsCompleted)
                {
                    BeginInvoke((MethodInvoker)async delegate
                    {
                        appiumServerStatus.ImageOptions.SvgImage = global::GUX.Properties.Resources.actions_checkcircled;
                        var img = new Bitmap(global::GUX.Properties.Resources.android_logo, new Size(24, 24));
                        BeginInvoke((MethodInvoker)delegate
                        {
                            iAlert.Show(this, "", "Server Running.", img);
                        });
                        await Task.Run(() =>
                        {
                            //toast.ShowNotification("b8c36c4b-00e7-4c8d-ad53-1aefa75929aa");
                            SplashScreenManager.CloseOverlayForm(handle);
                        });
                    });
                }
            });
        }

        private void Log_TextChanged(object sender, TextChangedEventArgs e)
        {
            var control = (sender as FastColoredTextBox);
            var error = new SolidBrush(Color.FromArgb(255, (byte)255, (byte)82, (byte)85));
            var success = new SolidBrush(Color.FromArgb(255, (byte)140, (byte)209, (byte)157));
            var warning = new SolidBrush(Color.FromArgb(255, (byte)255, (byte)169, (byte)39));

            Style ErrorStyle = new TextStyle(error, null, FontStyle.Bold);
            Style SuccessStyle = new TextStyle(success, null, FontStyle.Bold);
            Style WarnStyle = new TextStyle(warning, null, FontStyle.Bold);

            control.ClearStylesBuffer();
            e.ChangedRange.SetStyle(ErrorStyle, @"\berror\b|\bfailed\b|\bfalse\b", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(SuccessStyle, @"\bsuccess\b|\bok\b|\btrue\b|\bdone\b", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(WarnStyle, @"\bwarning\b|\bdisabled\b|\bblocked\b", RegexOptions.Multiline);
        }

        #region : CallAPI
        /*
         try
            {
                // In the class
                HttpClient client = new HttpClient();

                // Put the following code where you want to initialize the class
                // It can be the static constructor or a one-time initializer
                client.BaseAddress = new Uri("http://localhost:64304/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));


                // Assuming http://localhost:4354/api/ as BaseAddress 
                await client.GetAsync("GUX/5926739b-f34a-4c40-bcdd-fe73540e4c65").ContinueWith((gux) => {
                    if (gux.IsCompleted)
                    {
                        if (gux.Result.IsSuccessStatusCode)
                        {
                            gux.Result.Content.ReadAsAsync(typeof(List<string>)).ContinueWith((data) =>
                            {
                                if (data.IsCompleted)
                                {
                                    var o = String.Join("\n", ((List<string>)data.Result).ToArray());
                                    XtraMessageBox.Show(o);
                                }
                            });
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
             */
        #endregion

        private Task LoadLocations()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    DataTable dt = _psqlHelper.DirectQuery("select distinct(directory) from devices;");

                    directoryCLB.BeginInvoke((MethodInvoker)delegate
                    {
                        foreach (DataRow data in dt.Rows)
                        {
                            directoryCLB.Items.Add(data["directory"]);
                        }
                    });

                    _psqlHelper.Dispose();
                }
                catch (NpgsqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            });
        }

        private Task LoadDevices(string location)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    _Devices = _psqlHelper.DirectQuery("select d.* from devices d where d.directory in ('" + location.Replace(",", "','") + "') order by d.id;");
                    if (_Devices == null) return;
                    var hasRows = _Devices.Rows.Count > 0;
                    var dirs = location.Split(',');
                    iStackPanel.BeginInvoke((MethodInvoker)delegate
                    {
                        iStackPanel.SuspendLayout();
                        iStackPanel.Controls.Clear();
                        if (hasRows)
                        {
                            foreach (var dir in dirs)
                            {
                                var LV = new iLV();

                                _Rows = _Devices.Select("directory = '" + dir + "'");

                                var dt = new DataTable();
                                dt.Columns.Add("index");
                                dt.Columns.Add("device");

                                foreach (var row in _Rows)
                                {
                                    dt.Rows.Add(row["index"], DeviceNameGenerator(row["index"].ToString()));
                                }

                                LV.HtmlTitle = string.Format("<b>{0}</b>", dir.ToUpper());
                                LV.DataSource = dt;
                                LV.Tag = dir;
                                LV.Height = iStackPanel.Height - 80;

                                iStackPanel.Controls.Add(LV);
                                //iStackPanel.SetStretched(LV, true);
                            }
                        }
                        iStackPanel.ResumeLayout();
                    });

                    noDevicLabel.BeginInvoke((MethodInvoker)delegate
                    {
                        noDevicLabel.Visible = !hasRows;
                    });

                    _psqlHelper.Dispose();
                }
                catch (NpgsqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            });
        }

        private async void IStackPanel_SizeChanged(object sender, EventArgs e)
        {
            var container = (sender as DevExpress.Utils.Layout.StackPanel);
            await Task.Factory.StartNew(() =>
            {
                container.BeginInvoke((MethodInvoker)delegate
                {
                    container.SuspendLayout();
                    if (container.Controls.Count > 0)
                    {
                        if (container.Controls[0].Height != container.Height - 80)
                            container.Controls.OfType<iLV>().ToList().ForEach((LV) =>
                            {
                                LV.Height = container.Height - 80;
                            });
                    }
                    container.ResumeLayout();
                });
            });
        }

        private Task<DataTable> LoadSeeds(string email)
        {
            return Task<DataTable>.Factory.StartNew(() =>
            {
                try
                {
                    var seeds = _psqlHelper.DirectQuery("select s.email, s.password, s.recovery, s.proxy from seeds s where s.email ='" + email + "';");
                    return seeds;
                }
                catch (NpgsqlException ex)
                {
                    XtraMessageBox.Show(ex.Message);
                    return null;
                }
            });
        }

        private DataTable _LoadSeeds(string email)
        {
            try
            {
                var seeds = _psqlHelper.DirectQuery("select s.email, s.password, s.recovery, s.proxy from seeds s where s.email ='" + email + "';");
                return seeds;
            }
            catch (NpgsqlException ex)
            {
                XtraMessageBox.Show(ex.Message);
                return null;
            }
        }

        private void SetupButton_Click(object sender, EventArgs e)
        {
            MainTabControl.SelectedTabPage = ProcessXtraTabPage;
        }

        private Task<bool> CheckExistsDevice(GmailUnitTest g, string device)
        {
            try
            {
                return g.IsNotExists(device).ContinueWith((io) =>
                {
                    if (io.IsCompleted)
                        return io.Result;
                    else
                        return false;
                });
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                return null;
            }
        }

        private Task CheckOfflineDevice(GmailUnitTest g, string device, System.Timers.Timer timer, Task task)
        {
            try
            {
                if (_CancellationTokenSource.IsCancellationRequested)
                {
                    var index = g.index;
                    timer.Stop();
                    timer.Dispose();
                    g.StopProcess();
                    StopVM(index).Wait();
                    Console.WriteLine($"{index}, Cancellation Requested!");
                    task.Dispose();
                    return null;
                }

                return g.IsOffline(device).ContinueWith((io) =>
                {
                    if (io.IsCompleted)
                    {
                        if (io.Result)
                        {
                            var index = g.index;
                            timer.Stop();
                            timer.Dispose();
                            g.StopProcess();
                            StopVM(index).Wait();
                            Log(device + " is Offline! Process stopped.", index, "error");
                            task.Dispose();
                        }
                    }
                });
            }
            catch (Exception c)
            {
                timer.Stop();
                timer.Dispose();
                Console.WriteLine(c.Message);
                return null;
            }
        }

        private void ActionButton_Click(object sender, EventArgs e)
        {
            MainTabControl.SelectedTabPage = ActionsXtraTabPage;
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

        private Task RenameFolder()
        {
            var drive = Globals.DRIVE_LETTER + @":\My Drive\";
            var old_dir = drive + Globals.VMS_DIRECTORY;
            var new_dir = currentDirectory;
            return Task.Factory.StartNew(() =>
            {
                string temp = "";
                RE:
                try
                {
                    iDevices.Final().Wait();
                    temp = drive + new_dir + DateTime.Now.ToBinary().ToString();
                    Directory.Move(old_dir, temp);
                }
                catch (Exception x)
                {
                    Console.WriteLine(x.Message);
                    if (x.Message.ToLower().Contains("denied"))
                    {
                        iDevices.Final().Wait();
                        goto RE;
                    }
                }
                return temp;
            }).ContinueWith((md) =>
            {
                if (md.IsCompleted)
                {
                    Log(old_dir + " => " + md.Result + " successed!", "!", "info");
                    helper = true;
                }
            });
        }

        private Task StartActionsProcess(string folder, string keyword, string date, bool isRetry = false)
        {
            helper = false;
            var exceptions = new List<string>();

            var LV = iStackPanel.Controls.OfType<iLV>().Single(l => l.Tag.ToString() == folder);

            var targets = LV.CheckedIndices;
            var actionPath = Application.StartupPath + "\\Actions Logs\\" + actionsLogFile;
            var errorPath = Application.StartupPath + "\\Error Logs\\" + errorsLogFile;
            var disabledPath = Application.StartupPath + "\\Disabled Logs\\" + disabledLogFile;
            var blockedPath = Application.StartupPath + "\\Blocked Logs\\" + blockedLogFile;
            var threadsCon = Globals.THREADS_CONCURRENCY;
            var inter = Globals.THREADS_INTERVAL * 1000;

            var drive = Globals.DRIVE_LETTER + @":\My Drive\";
            var directory = folder;
            var _directory = drive + Globals.VMS_DIRECTORY;
            var temp = "";

            return Task.Factory.StartNew(() =>
            {
                if (!isRetry)
                {
                    if (!Directory.Exists(drive))
                    {
                        Log("Directory " + drive + " not found!", "!!", "error");
                        helper = true;
                        goto end;
                    }
                    var folders = Directory.GetDirectories(drive).Where(d => d.Contains(directory));
                    if (folders.Count() == 0)
                    {
                        Log("Directory " + directory + " not found!", "!!", "error");
                        helper = true;
                        goto end;
                    }
                    else
                    {
                        try
                        {
                            temp = folders.First();

                            var directoryTask = Task.Factory.StartNew(() =>
                            {
                                if (!Directory.Exists(_directory))
                                {
                                    Directory.Move(temp, _directory);
                                    Log(temp + " => " + _directory + " successed!", "!", "info");
                                }
                                else
                                {
                                    alreadyExists = true;
                                    Log("Directory " + _directory + " already exists!", "!", "error");
                                    helper = true;
                                }
                            }).ContinueWith((md) =>
                            {
                                if (md.IsCompleted)
                                {
                                    if (!alreadyExists)
                                    {
                                        var exec = Exec("TASKKILL /T /F /IM MEmuConsole.exe /IM MEmuSVC.exe /IM MEmuHeadless.exe & START /MIN MEmuConsole.exe");
                                        exec.Wait();
                                        if (exec.IsCompleted)
                                        {
                                            Task.Delay(3000).Wait();
                                        }
                                    }
                                }
                            });

                            directoryTask.Wait();
                            Thread.Sleep(5000);
                            //Task.Delay(10000).Wait();
                            Exec("TASKKILL /T /F /IM MEmuConsole.exe /IM MEmu.exe /IM MEmuSVC.exe /IM MEmuHeadless.exe").Wait();

                            iDevices.Invoke((MethodInvoker)delegate
                            {
                                iDevices.currentDirectory = folder;
                                iDevices.actionsLogFile = actionsLogFile;
                                iDevices.errorsLogFile = errorsLogFile;
                                iDevices.blockedLogFile = blockedLogFile;
                            });
                        }
                        catch (Exception x)
                        {
                            Log(x.Message, "?", "error");
                            helper = true;
                            goto end;
                        }
                    }
                }

                if (!alreadyExists)
                {
                    _CancellationTokenSource = new CancellationTokenSource();
                    iDevices.token = _CancellationTokenSource.Token;
                    LV.Invoke((MethodInvoker)async delegate
                    {
                        try
                        {
                            if (maxRetries == 0)
                            {
                                Log("max retries has been reached!", "!", "warning");
                                var directoryTask = Task.Factory.StartNew(() =>
                                {
                                    temp = drive + directory + DateTime.Now.ToBinary().ToString();
                                    Directory.Move(_directory, temp);
                                }).ContinueWith((md) =>
                                {
                                    if (md.IsCompleted)
                                    {
                                        Log(_directory + " => " + temp + " successed!", "!", "info");
                                        helper = true;
                                    }
                                });
                                directoryTask.Wait();
                                return;
                            }
                            else
                            {
                                _AsyncBulkheadPolicy = Policy.BulkheadAsync((int)threadsCon, Int32.MaxValue);
                                TasksList = new List<Task>();
                                var ui = TaskScheduler.FromCurrentSynchronizationContext();
                                foreach (var item in targets)
                                {
                                    var device = _Devices.Rows[item];
                                    var index = device["index"].ToString();
                                    var _device = DeviceNameGenerator(index);

                                    GmailUnitTest _gunit = new GmailUnitTest()
                                    {
                                        ServerPort = Globals.SERVER_PORT,
                                        TestDriverTO = Globals.TEST_DRIVER_TIMEOUT,
                                        WaitTO = Globals.DRIVER_WAIT_TIMEOUT,
                                        WaitPolling = Globals.DRIVER_WAIT_POLLING_INTERVAL,
                                        testRetries = 10,
                                        device = _device,
                                        index = index,
                                        actionLogPath = actionPath,
                                        errorLogPath = errorPath,
                                        disabledLogPath = disabledPath,
                                        blockedLogPath = blockedPath,
                                        actionRetries = Globals.FAILED_ACTION_MAX_RETRIES,
                                        cancelationToken = _CancellationTokenSource.Token,
                                        scenario = Globals.DEFAULT_SCENARIO.actions,
                                        maxTreatedInboxEmails = Globals.WARMUP_MAX_TREATED_INBOX_EMAILS
                                    };

                                    var account = device["account"].ToString();
                                    var stopWatch = new Stopwatch();
                                    var seeds = _LoadSeeds(account).Rows[0];
                                    var t = _AsyncBulkheadPolicy.ExecuteAsync(async () =>
                                    {
                                        try
                                        {
                                            await Task.Delay(inter);
                                            await iDevices.AttachDevice(index).ContinueWith((st) =>
                                            {
                                                st.Wait();
                                                if (st.IsCompleted)
                                                {
                                                    if (st.Result)
                                                    {
                                                        stopWatch.Start();
                                                        //iDevices.Fit();
                                                        var deviceStopWatch = new Stopwatch();
                                                        deviceStopWatch.Start();

                                                        var check = true;

                                                        while (check)
                                                        {
                                                            if (deviceStopWatch.Elapsed.Seconds >= 60)
                                                            {
                                                                _gunit.Dispose();
                                                                deviceStopWatch.Stop();
                                                                Log(device + " device not found! Process stopped.", index, "error");
                                                                StopVM(index).Wait();
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                var ced = CheckExistsDevice(_gunit, _device).ContinueWith((cdne) =>
                                                                {
                                                                    if (cdne.IsCompleted)
                                                                    {
                                                                        check = cdne.Result;
                                                                    }
                                                                });
                                                                ced.Wait();
                                                            }
                                                        }

                                                        if (!check)
                                                        {
                                                            var tdk = _gunit.TestDriver().ContinueWith((td) =>
                                                            {
                                                                if (td.IsCompleted)
                                                                {
                                                                    if (td.Status == TaskStatus.Canceled)
                                                                        return;
                                                                    //iDevices.Fit();
                                                                    Log("Test Driver : " + td.Result, index, "info");
                                                                    if (td.Result)
                                                                    {
                                                                        var timer = SetInterval(new Action<System.Timers.Timer>((tt) =>
                                                                        {
                                                                            try
                                                                            {
                                                                                CheckOfflineDevice(_gunit, _device, tt, st).Wait();
                                                                            }
                                                                            catch (Exception x)
                                                                            {
                                                                                Console.WriteLine(x.Message);
                                                                                if (tt != null) tt.Dispose();
                                                                            }
                                                                        }), 3000);
                                                                        var spm = _gunit.SpamProcess(Globals.DEFAULT_SCENARIO.keyword, Globals.DEFAULT_SCENARIO.date).ContinueWith((sp) =>
                                                                        {
                                                                            //sp.Wait();
                                                                            //iDevices.Fit();
                                                                            if (sp.IsCompleted)
                                                                            {
                                                                                if (sp.Status == TaskStatus.Canceled)
                                                                                    return;
                                                                                if (sp.Result.ToString() == "spam actions successed!")
                                                                                {
                                                                                    //iDevices.Fit();
                                                                                    if (Globals.WARMUP_PROCEED_INBOX_FOLDER)
                                                                                    {
                                                                                        _gunit.actionRetries = Globals.FAILED_ACTION_MAX_RETRIES;
                                                                                        _gunit.maxTreatedInboxEmails = Globals.WARMUP_MAX_TREATED_INBOX_EMAILS;
                                                                                        var inbx = _gunit.InboxProcess(Globals.DEFAULT_SCENARIO.keyword, Globals.DEFAULT_SCENARIO.date).ContinueWith((inb) =>
                                                                                        {
                                                                                            //inb.Wait();
                                                                                            if (inb.IsCompleted)
                                                                                            {
                                                                                                if (inb.Status == TaskStatus.Canceled)
                                                                                                    return;
                                                                                                if (inb.Result.ToString() != "inbox actions successed!")
                                                                                                {
                                                                                                    timer?.Dispose();
                                                                                                    _gunit?.Dispose();

                                                                                                    //iDevices.Fit();

                                                                                                    Log("inbox actions failed!", index, "error");
                                                                                                    //iDevices.Fit();
                                                                                                    return;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    var wstp = stopWatch.Elapsed;
                                                                                                    Log(string.Format("warmup done! [{0:D2}:{1:D2}:{2:D3}]", wstp.Minutes, wstp.Seconds, wstp.Milliseconds), index, "info");
                                                                                                    stopWatch.Stop();
                                                                                                    stopWatch.Reset();

                                                                                                    timer?.Dispose();
                                                                                                    _gunit?.Dispose();

                                                                                                    //iDevices.Fit();
                                                                                                }
                                                                                            }
                                                                                        });
                                                                                        if (inbx != null && (inbx.Status == TaskStatus.RanToCompletion || inbx.Status != TaskStatus.Running)) inbx.Wait();
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        timer?.Dispose();
                                                                                        _gunit?.Dispose();
                                                                                        //iDevices.Fit();
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    timer?.Dispose();
                                                                                    _gunit?.Dispose();

                                                                                    Log("spam actions failed!", index, "error");
                                                                                    //iDevices.Fit();
                                                                                    return;
                                                                                }
                                                                            }
                                                                        });
                                                                        if (spm != null && (spm.Status == TaskStatus.RanToCompletion || spm.Status != TaskStatus.Running)) spm.Wait();
                                                                    }
                                                                    else
                                                                    {
                                                                        Log("driver timedout!", index, "error");
                                                                        stopWatch.Stop();

                                                                        timer?.Dispose();
                                                                        _gunit?.Dispose();
                                                                        //iDevices.Fit();

                                                                        return;
                                                                    }
                                                                }
                                                            });
                                                            if (tdk != null && (tdk.Status == TaskStatus.RanToCompletion || tdk.Status != TaskStatus.Running)) tdk.Wait();
                                                        }
                                                    }
                                                }
                                            });

                                            await Task.Delay(1500).ContinueWith((tt) =>
                                            {
                                                tt.Wait();
                                                if (tt.IsCompleted)
                                                {
                                                    _gunit.KillDriver();
                                                    timer?.Dispose();
                                                    _gunit?.Dispose();
                                                    var svm = StopVM(index);
                                                    svm.Wait();
                                                }
                                            });
                                            Console.WriteLine(_device + ": Process finished!");
                                            return;
                                        }
                                        catch (Exception c)
                                        {
                                            timer?.Dispose();
                                            _gunit?.Dispose();
                                            Console.WriteLine(c.Message);
                                            return;
                                        }
                                    });
                                    TasksList.Add(t);
                                    await Task.Delay(inter);
                                }
                                var _tasks = Task.WhenAll(TasksList);
                                await _tasks.ContinueWith((_t) =>
                                {
                                    if (_t.IsCompleted)
                                    {
                                        try
                                        {
                                            Log("DONE", "!", "info");
                                            if (Globals.FAILED_AUTO_RETRY)
                                                fctbErrors.BeginInvoke((MethodInvoker)delegate
                                                {
                                                    StopFileWatcher();
                                                    if (fctbErrors.Text.Length > 0)
                                                    {
                                                        helper = false;
                                                        BeginInvoke((MethodInvoker)delegate
                                                        {
                                                            FailedWarmupTasksRetryButton.Tag = folder;
                                                            FailedWarmupTasksRetryButton.PerformClick();
                                                            maxRetries--;
                                                        });
                                                    }
                                                    else
                                                    {
                                                        helper = true;
                                                        return;
                                                    }
                                                });
                                            else
                                            {
                                                helper = true;
                                                return;
                                            }
                                        }
                                        catch (Exception x)
                                        {
                                            Log(x.Message, "?", "error");
                                            helper = true;
                                            return;
                                        }
                                    }
                                });
                            }
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine(x.Message);
                            helper = true;
                            return;
                        }
                    });
                }
                end:
                Console.WriteLine("End Process");
            });
        }

        private Task StartProcess(string folder, bool isRetry = false)
        {
            helper = false;
            var exceptions = new List<string>();

            var LV = iStackPanel.Controls.OfType<iLV>().Single(l => l.Tag.ToString() == folder);

            var targets = LV.CheckedIndices;
            var actionPath = Application.StartupPath + "\\Actions Logs\\" + actionsLogFile;
            var errorPath = Application.StartupPath + "\\Error Logs\\" + errorsLogFile;
            var disabledPath = Application.StartupPath + "\\Disabled Logs\\" + disabledLogFile;
            var blockedPath = Application.StartupPath + "\\Blocked Logs\\" + blockedLogFile;
            var threadsCon = Globals.THREADS_CONCURRENCY;
            var inter = Globals.THREADS_INTERVAL * 1000;

            var drive = Globals.DRIVE_LETTER + @":\My Drive\";
            var directory = folder;
            var _directory = drive + Globals.VMS_DIRECTORY;
            var temp = "";

            return Task.Factory.StartNew(() =>
            {
                if (!isRetry)
                {
                    if (!Directory.Exists(drive))
                    {
                        Log("Directory " + drive + " not found!", "!!", "error");
                        helper = true;
                        goto end;
                    }
                    var folders = Directory.GetDirectories(drive).Where(d => d.Contains(directory));
                    if (folders.Count() == 0)
                    {
                        Log("Directory " + directory + " not found!", "!!", "error");
                        helper = true;
                        goto end;
                    }
                    else
                    {
                        try
                        {
                            temp = folders.First();

                            var directoryTask = Task.Factory.StartNew(() =>
                            {
                                if (!Directory.Exists(_directory))
                                {
                                    Directory.Move(temp, _directory);
                                    Log(temp + " => " + _directory + " successed!", "!", "info");
                                }
                                else
                                {
                                    alreadyExists = true;
                                    Log("Directory " + _directory + " already exists!", "!", "error");
                                    helper = true;
                                }
                            }).ContinueWith((md) =>
                            {
                                if (md.IsCompleted)
                                {
                                    if (!alreadyExists)
                                    {
                                        var exec = Exec("TASKKILL /T /F /IM MEmuConsole.exe /IM MEmuSVC.exe /IM MEmuHeadless.exe & START /MIN MEmuConsole.exe");
                                        exec.Wait();
                                        if (exec.IsCompleted)
                                        {
                                            Task.Delay(3000).Wait();
                                        }
                                    }
                                }
                            });

                            directoryTask.Wait();
                            Thread.Sleep(5000);
                            //Task.Delay(10000).Wait();
                            Exec("TASKKILL /T /F /IM MEmuConsole.exe /IM MEmu.exe /IM MEmuSVC.exe /IM MEmuHeadless.exe").Wait();

                            iDevices.Invoke((MethodInvoker)delegate
                            {
                                iDevices.currentDirectory = folder;
                                iDevices.actionsLogFile = actionsLogFile;
                                iDevices.errorsLogFile = errorsLogFile;
                                iDevices.blockedLogFile = blockedLogFile;
                            });
                        }
                        catch (Exception x)
                        {
                            Log(x.Message, "?", "error");
                            helper = true;
                            goto end;
                        }
                    }
                }

                if (!alreadyExists)
                {
                    _CancellationTokenSource = new CancellationTokenSource();
                    iDevices.token = _CancellationTokenSource.Token;
                    LV.Invoke((MethodInvoker)async delegate
                    {
                        try
                        {
                            if (maxRetries == 0)
                            {
                                var directoryTask = Task.Factory.StartNew(() =>
                                {
                                    temp = drive + directory + DateTime.Now.ToBinary().ToString();
                                    Directory.Move(_directory, temp);
                                }).ContinueWith((md) =>
                                {
                                    if (md.IsCompleted)
                                    {
                                        Log(_directory + " => " + temp + " successed!", "!", "info");
                                        helper = true;
                                    }
                                });
                                directoryTask.Wait();
                                return;
                            }
                            else
                            {
                                _AsyncBulkheadPolicy = Policy.BulkheadAsync((int)threadsCon, Int32.MaxValue);
                                TasksList = new List<Task>();
                                var ui = TaskScheduler.FromCurrentSynchronizationContext();
                                foreach (var item in targets)
                                {
                                    var device = _Devices.Rows[item];
                                    var index = device["index"].ToString();
                                    var _device = DeviceNameGenerator(index);

                                    GmailUnitTest _gunit = new GmailUnitTest()
                                    {
                                        ServerPort = Globals.SERVER_PORT,
                                        TestDriverTO = Globals.TEST_DRIVER_TIMEOUT,
                                        WaitTO = Globals.DRIVER_WAIT_TIMEOUT,
                                        WaitPolling = Globals.DRIVER_WAIT_POLLING_INTERVAL,
                                        testRetries = 10,
                                        device = _device,
                                        index = index,
                                        actionLogPath = actionPath,
                                        errorLogPath = errorPath,
                                        disabledLogPath = disabledPath,
                                        blockedLogPath = blockedPath,
                                        actionRetries = Globals.FAILED_ACTION_MAX_RETRIES,
                                        scenario = Globals.DEFAULT_SCENARIO.actions,
                                        maxTreatedInboxEmails = Globals.WARMUP_MAX_TREATED_INBOX_EMAILS,
                                        cancelationToken = _CancellationTokenSource.Token
                                    };

                                    var account = device["account"].ToString();
                                    var stopWatch = new Stopwatch();
                                    var seeds = _LoadSeeds(account).Rows[0];
                                    var proxy = seeds["proxy"].ToString().Split(':');
                                    var t = _AsyncBulkheadPolicy.ExecuteAsync(async () =>
                                    {
                                        try
                                        {
                                            await Task.Delay(inter);
                                            await iDevices.AttachDevice(index).ContinueWith((st) =>
                                            {
                                                st.Wait();
                                                if (st.IsCompleted)
                                                {
                                                    if (st.Result)
                                                    {
                                                        stopWatch.Start();
                                                        //iDevices.Fit();
                                                        var deviceStopWatch = new Stopwatch();
                                                        deviceStopWatch.Start();

                                                        var check = true;

                                                        while (check)
                                                        {
                                                            if (deviceStopWatch.Elapsed.Seconds >= 60)
                                                            {
                                                                _gunit.Dispose();
                                                                deviceStopWatch.Stop();
                                                                Log(device + " device not found! Process stopped.", index, "error");
                                                                StopVM(index).Wait();
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                var ced = CheckExistsDevice(_gunit, _device).ContinueWith((cdne) =>
                                                                {
                                                                    if (cdne.IsCompleted)
                                                                    {
                                                                        check = cdne.Result;
                                                                    }
                                                                });
                                                                ced.Wait();
                                                            }
                                                        }

                                                        if (!check)
                                                        {
                                                            var tdk = _gunit.TestDriver().ContinueWith((td) =>
                                                            {
                                                                if (td.IsCompleted)
                                                                {
                                                                    if (td.Status == TaskStatus.Canceled)
                                                                        return;
                                                                    //iDevices.Fit();
                                                                    Log("Test Driver : " + td.Result, index, "info");
                                                                    if (td.Result)
                                                                    {
                                                                        var timer = SetInterval(new Action<System.Timers.Timer>((tt) =>
                                                                        {
                                                                            try
                                                                            {
                                                                                CheckOfflineDevice(_gunit, _device, tt, st).Wait();
                                                                            }
                                                                            catch (Exception x)
                                                                            {
                                                                                Console.WriteLine(x.Message);
                                                                                if (tt != null) tt.Dispose();
                                                                            }
                                                                        }), 3000);
                                                                        var ptk = _gunit.SetupProxy(proxy[0], proxy.Count() > 1 ? Convert.ToInt16(proxy[1]) : 92).ContinueWith((pt) =>
                                                                        {
                                                                            //pt.Wait();
                                                                            //iDevices.Fit();
                                                                            if (pt.IsCompleted)
                                                                            {
                                                                                if (pt.Status == TaskStatus.Canceled)
                                                                                    return;
                                                                                if (pt.Result.ToString() == "setup proxy successed!")
                                                                                {
                                                                                    //iDevices.Fit();
                                                                                    _gunit.actionRetries = Globals.FAILED_ACTION_MAX_RETRIES;
                                                                                    var sitk = _gunit.SignIn(new string[] { seeds["email"].ToString(), seeds["password"].ToString(), seeds["recovery"].ToString() }).ContinueWith((sit) =>
                                                                                    {
                                                                                        if (sit.IsCompleted)
                                                                                        {
                                                                                            if (sit.Status == TaskStatus.Canceled)
                                                                                                return;
                                                                                            if (sit.Result.ToString() != "signin successed!")
                                                                                            {
                                                                                                timer?.Dispose();
                                                                                                _gunit?.Dispose();

                                                                                                Log("signin failed!", index, "error");
                                                                                                //iDevices.Fit();
                                                                                                return;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                var stp = stopWatch.Elapsed;
                                                                                                Log(string.Format("setup done! [{0:D2}:{1:D2}:{2:D3}]", stp.Minutes, stp.Seconds, stp.Milliseconds), index, "info");
                                                                                                stopWatch.Stop();
                                                                                                stopWatch.Reset();
                                                                                                //iDevices.Fit();
                                                                                                if (Globals.WARMUP_AUTORUN)
                                                                                                {
                                                                                                    _gunit.actionRetries = Globals.FAILED_ACTION_MAX_RETRIES;
                                                                                                    _gunit.scenario = Globals.DEFAULT_SCENARIO.actions;
                                                                                                    stopWatch.Start();
                                                                                                    var spm = _gunit.SpamProcess(Globals.DEFAULT_SCENARIO.keyword, Globals.DEFAULT_SCENARIO.date).ContinueWith((sp) =>
                                                                                                    {
                                                                                                        //sp.Wait();
                                                                                                        //iDevices.Fit();
                                                                                                        if (sp.IsCompleted)
                                                                                                        {
                                                                                                            if (sp.Status == TaskStatus.Canceled)
                                                                                                                return;
                                                                                                            if (sp.Result.ToString() == "spam actions successed!")
                                                                                                            {
                                                                                                                //iDevices.Fit();
                                                                                                                if (Globals.WARMUP_PROCEED_INBOX_FOLDER)
                                                                                                                {
                                                                                                                    _gunit.actionRetries = Globals.FAILED_ACTION_MAX_RETRIES;
                                                                                                                    _gunit.maxTreatedInboxEmails = Globals.WARMUP_MAX_TREATED_INBOX_EMAILS;
                                                                                                                    var inbx = _gunit.InboxProcess(Globals.DEFAULT_SCENARIO.keyword, Globals.DEFAULT_SCENARIO.date).ContinueWith((inb) =>
                                                                                                                    {
                                                                                                                        //inb.Wait();
                                                                                                                        if (inb.IsCompleted)
                                                                                                                        {
                                                                                                                            if (inb.Status == TaskStatus.Canceled)
                                                                                                                                return;
                                                                                                                            if (inb.Result.ToString() != "inbox actions successed!")
                                                                                                                            {
                                                                                                                                timer?.Dispose();
                                                                                                                                _gunit?.Dispose();
                                                                                                                                Log("inbox actions failed!", index, "error");
                                                                                                                                //iDevices.Fit();
                                                                                                                                return;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                var wstp = stopWatch.Elapsed;
                                                                                                                                Log(string.Format("warmup done! [{0:D2}:{1:D2}:{2:D3}]", wstp.Minutes, wstp.Seconds, wstp.Milliseconds), index, "info");
                                                                                                                                stopWatch.Stop();
                                                                                                                                stopWatch.Reset();
                                                                                                                                timer?.Dispose();
                                                                                                                                _gunit?.Dispose();
                                                                                                                                //iDevices.Fit();
                                                                                                                            }
                                                                                                                        }
                                                                                                                    });
                                                                                                                    if (inbx != null && (inbx.Status == TaskStatus.RanToCompletion || inbx.Status != TaskStatus.Running)) inbx.Wait();
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    timer?.Dispose();
                                                                                                                    _gunit?.Dispose();
                                                                                                                    //iDevices.Fit();
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                Log("spam actions failed!", index, "error");
                                                                                                                timer?.Dispose();
                                                                                                                _gunit?.Dispose();
                                                                                                                //iDevices.Fit();
                                                                                                                return;
                                                                                                            }
                                                                                                        }
                                                                                                    });
                                                                                                    if (spm != null && (spm.Status == TaskStatus.RanToCompletion || spm.Status != TaskStatus.Running)) spm.Wait();
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    timer?.Dispose();
                                                                                                    _gunit?.Dispose();
                                                                                                    //iDevices.Fit();
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    });
                                                                                    if (sitk != null && (sitk.Status == TaskStatus.RanToCompletion || sitk.Status != TaskStatus.Running))
                                                                                        sitk.Wait();
                                                                                }
                                                                                else
                                                                                {
                                                                                    timer?.Dispose();
                                                                                    _gunit?.Dispose();
                                                                                    //iDevices.Fit();
                                                                                    Log("proxy setup failed!", index, "error");
                                                                                    return;
                                                                                }
                                                                            }
                                                                        });
                                                                        if (ptk != null && (ptk.Status == TaskStatus.RanToCompletion || ptk.Status != TaskStatus.Running)) ptk.Wait();
                                                                    }
                                                                    else
                                                                    {
                                                                        Log("driver timedout!", index, "error");
                                                                        stopWatch.Stop();
                                                                        timer?.Dispose();
                                                                        _gunit?.Dispose();
                                                                        //iDevices.Fit(); return;
                                                                    }
                                                                }
                                                            });
                                                            if (tdk != null && (tdk.Status == TaskStatus.RanToCompletion || tdk.Status != TaskStatus.Running)) tdk.Wait();
                                                        }
                                                    }
                                                }
                                            });
                                            await Task.Delay(1500).ContinueWith((tt) =>
                                            {
                                                tt.Wait();
                                                if (tt.IsCompleted)
                                                {
                                                    _gunit.KillDriver();
                                                    timer?.Dispose();
                                                    _gunit?.Dispose();
                                                    var svm = StopVM(index);
                                                    svm.Wait();
                                                }
                                            });
                                            Console.WriteLine(_device + ": Process finished!");
                                            return;
                                        }
                                        catch (Exception c)
                                        {
                                            timer?.Dispose();
                                            _gunit?.Dispose();
                                            Console.WriteLine(c.Message);
                                            return;
                                        }
                                    });
                                    TasksList.Add(t);
                                    await Task.Delay(inter);
                                }
                                var _tasks = Task.WhenAll(TasksList);
                                await _tasks.ContinueWith((_t) =>
                                {
                                    if (_t.IsCompleted)
                                    {
                                        try
                                        {
                                            Log("DONE", "!", "info");
                                            if (Globals.FAILED_AUTO_RETRY)
                                                fctbErrors.BeginInvoke((MethodInvoker)delegate
                                                {
                                                    StopFileWatcher();
                                                    if (fctbErrors.Text.Length > 0)
                                                    {
                                                        helper = false;
                                                        BeginInvoke((MethodInvoker)delegate
                                                        {
                                                            FailedTasksRetryButton.Tag = folder;
                                                            FailedTasksRetryButton.PerformClick();
                                                            maxRetries--;
                                                        });
                                                    }
                                                    else
                                                    {
                                                        helper = true;
                                                        return;
                                                    }
                                                });
                                            else
                                            {
                                                helper = true;
                                                return;
                                            }
                                        }
                                        catch (Exception x)
                                        {
                                            Log(x.Message, "?", "error");
                                            helper = true;
                                            return;
                                        }
                                    }
                                });
                            }
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine(x.Message);
                            helper = true;
                            return;
                        }
                    });
                }
                end:
                Console.WriteLine("End Process");
            });
        }

        private Task StartCheckProcess(string folder, bool isRetry = false)
        {
            helper = false;
            var exceptions = new List<string>();

            var LV = iStackPanel.Controls.OfType<iLV>().Single(l => l.Tag.ToString() == folder);

            var targets = LV.CheckedIndices;
            var actionPath = Application.StartupPath + "\\Actions Logs\\" + actionsLogFile;
            var errorPath = Application.StartupPath + "\\Error Logs\\" + errorsLogFile;
            var disabledPath = Application.StartupPath + "\\Disabled Logs\\" + disabledLogFile;
            var blockedPath = Application.StartupPath + "\\Blocked Logs\\" + blockedLogFile;
            var threadsCon = Globals.THREADS_CONCURRENCY;
            var inter = Globals.THREADS_INTERVAL * 1000;

            var drive = Globals.DRIVE_LETTER + @":\My Drive\";
            var directory = folder;
            var _directory = drive + Globals.VMS_DIRECTORY;
            var temp = "";

            return Task.Factory.StartNew(() =>
            {
                if (!isRetry)
                {
                    if (!Directory.Exists(drive))
                    {
                        Log("Directory " + drive + " not found!", "!!", "error");
                        helper = true;
                        goto end;
                    }
                    var folders = Directory.GetDirectories(drive).Where(d => d.Contains(directory));
                    if (folders.Count() == 0)
                    {
                        Log("Directory " + directory + " not found!", "!!", "error");
                        helper = true;
                        goto end;
                    }
                    else
                    {
                        try
                        {
                            temp = folders.First();

                            var directoryTask = Task.Factory.StartNew(() =>
                            {
                                if (!Directory.Exists(_directory))
                                {
                                    Directory.Move(temp, _directory);
                                    Log(temp + " => " + _directory + " successed!", "!", "info");
                                }
                                else
                                {
                                    alreadyExists = true;
                                    Log("Directory " + _directory + " already exists!", "!", "error");
                                    helper = true;
                                }
                            }).ContinueWith((md) =>
                            {
                                if (md.IsCompleted)
                                {
                                    if (!alreadyExists)
                                    {
                                        var exec = Exec("TASKKILL /T /F /IM MEmuConsole.exe /IM MEmuSVC.exe /IM MEmuHeadless.exe & START /MIN MEmuConsole.exe");
                                        exec.Wait();
                                        if (exec.IsCompleted)
                                        {
                                            Task.Delay(3000).Wait();
                                        }
                                    }
                                }
                            });

                            directoryTask.Wait();
                            Thread.Sleep(5000);
                            //Task.Delay(10000).Wait();
                            Exec("TASKKILL /T /F /IM MEmuConsole.exe /IM MEmu.exe /IM MEmuSVC.exe /IM MEmuHeadless.exe").Wait();

                            iDevices.Invoke((MethodInvoker)delegate
                            {
                                iDevices.currentDirectory = folder;
                                iDevices.actionsLogFile = actionsLogFile;
                                iDevices.errorsLogFile = errorsLogFile;
                                iDevices.blockedLogFile = blockedLogFile;
                            });
                        }
                        catch (Exception x)
                        {
                            Log(x.Message, "?", "error");
                            helper = true;
                            goto end;
                        }
                    }
                }

                if (!alreadyExists)
                {
                    _CancellationTokenSource = new CancellationTokenSource();
                    iDevices.token = _CancellationTokenSource.Token;
                    LV.Invoke((MethodInvoker)async delegate
                    {
                        try
                        {
                            if (maxRetries == 0)
                            {
                                var directoryTask = Task.Factory.StartNew(() =>
                                {
                                    temp = drive + directory + DateTime.Now.ToBinary().ToString();
                                    Directory.Move(_directory, temp);
                                }).ContinueWith((md) =>
                                {
                                    if (md.IsCompleted)
                                    {
                                        Log(_directory + " => " + temp + " successed!", "!", "info");
                                        helper = true;
                                    }
                                });
                                directoryTask.Wait();
                                return;
                            }
                            else
                            {
                                _AsyncBulkheadPolicy = Policy.BulkheadAsync((int)threadsCon, Int32.MaxValue);
                                TasksList = new List<Task>();
                                var ui = TaskScheduler.FromCurrentSynchronizationContext();
                                foreach (var item in targets)
                                {
                                    var device = _Devices.Rows[item];
                                    var index = device["index"].ToString();
                                    var _device = DeviceNameGenerator(index);

                                    GmailUnitTest _gunit = new GmailUnitTest()
                                    {
                                        ServerPort = Globals.SERVER_PORT,
                                        TestDriverTO = Globals.TEST_DRIVER_TIMEOUT,
                                        WaitTO = Globals.DRIVER_WAIT_TIMEOUT,
                                        WaitPolling = Globals.DRIVER_WAIT_POLLING_INTERVAL,
                                        testRetries = 10,
                                        device = _device,
                                        index = index,
                                        actionLogPath = actionPath,
                                        errorLogPath = errorPath,
                                        disabledLogPath = disabledPath,
                                        blockedLogPath = blockedPath,
                                        actionRetries = Globals.FAILED_ACTION_MAX_RETRIES,
                                        cancelationToken = _CancellationTokenSource.Token
                                    };

                                    var account = device["account"].ToString();
                                    var stopWatch = new Stopwatch();
                                    var t = _AsyncBulkheadPolicy.ExecuteAsync(async () =>
                                    {
                                        try
                                        {
                                            await Task.Delay(inter);
                                            await iDevices.AttachDevice(index).ContinueWith((st) =>
                                            {
                                                st.Wait();
                                                if (st.IsCompleted)
                                                {
                                                    if (st.Status == TaskStatus.Canceled)
                                                        return;
                                                    if (st.Result)
                                                    {
                                                        stopWatch.Start();
                                                        //iDevices.Fit();
                                                        var deviceStopWatch = new Stopwatch();
                                                        deviceStopWatch.Start();

                                                        var check = true;

                                                        while (check)
                                                        {
                                                            if (deviceStopWatch.Elapsed.Seconds >= 60)
                                                            {
                                                                _gunit.Dispose();
                                                                deviceStopWatch.Stop();
                                                                Log(device + " device not found! Process stopped.", index, "error");
                                                                StopVM(index).Wait();
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                var ced = CheckExistsDevice(_gunit, _device).ContinueWith((cdne) =>
                                                                {
                                                                    if (cdne.IsCompleted)
                                                                    {
                                                                        check = cdne.Result;
                                                                    }
                                                                });
                                                                ced.Wait();
                                                            }
                                                        }

                                                        if (!check)
                                                        {
                                                            var tdk = _gunit.TestDriver().ContinueWith((td) =>
                                                            {
                                                                if (td.IsCompleted)
                                                                {
                                                                    if (td.Status == TaskStatus.Canceled)
                                                                        return;

                                                                    //iDevices.Fit();
                                                                    Log("Test Driver : " + td.Result, index, "info");
                                                                    if (td.Result)
                                                                    {
                                                                        var timer = SetInterval(new Action<System.Timers.Timer>((tt) =>
                                                                        {
                                                                            try
                                                                            {
                                                                                CheckOfflineDevice(_gunit, _device, tt, st).Wait();
                                                                            }
                                                                            catch (Exception x)
                                                                            {
                                                                                Console.WriteLine(x.Message);
                                                                                if (tt != null) tt.Dispose();
                                                                            }
                                                                        }), 3000);
                                                                        var ptk = _gunit.CheckProcess().ContinueWith((pt) =>
                                                                        {
                                                                            //pt.Wait();
                                                                            //iDevices.Fit();
                                                                            if (pt.IsCompleted)
                                                                            {
                                                                                if (pt.Status == TaskStatus.Canceled)
                                                                                    return;
                                                                                timer?.Dispose();
                                                                                _gunit?.Dispose();
                                                                                //iDevices.Fit();
                                                                                return;
                                                                            }
                                                                        });
                                                                        if (ptk != null && (ptk.Status == TaskStatus.RanToCompletion || ptk.Status != TaskStatus.Running)) ptk.Wait();
                                                                    }
                                                                    else
                                                                    {
                                                                        Log("driver timedout!", index, "error");
                                                                        stopWatch.Stop();
                                                                        timer?.Dispose();
                                                                        _gunit?.Dispose();
                                                                        //iDevices.Fit();
                                                                        return;
                                                                    }
                                                                }
                                                            });
                                                            if (tdk != null && (tdk.Status == TaskStatus.RanToCompletion || tdk.Status != TaskStatus.Running)) tdk.Wait();
                                                        }
                                                    }
                                                }
                                            }, _CancellationTokenSource.Token);
                                            await Task.Delay(1500).ContinueWith((tt) =>
                                            {
                                                tt.Wait();
                                                if (tt.IsCompleted)
                                                {
                                                    _gunit.KillDriver();
                                                    timer?.Dispose();
                                                    _gunit?.Dispose();
                                                    var svm = StopVM(index);
                                                    svm.Wait();
                                                }
                                            });
                                            Console.WriteLine(_device + ": Process finished!");
                                            return;
                                        }
                                        catch (Exception c)
                                        {
                                            timer?.Dispose();
                                            _gunit?.Dispose();
                                            Console.WriteLine(c.Message);
                                            return;
                                        }
                                    });
                                    TasksList.Add(t);
                                    await Task.Delay(inter);
                                }
                                var _tasks = Task.WhenAll(TasksList);
                                await _tasks.ContinueWith((_t) =>
                                {
                                    if (_t.IsCompleted)
                                    {
                                        try
                                        {
                                            Log("DONE", "!", "info");
                                            if (Globals.FAILED_AUTO_RETRY)
                                                fctbErrors.BeginInvoke((MethodInvoker)delegate
                                                {
                                                    StopFileWatcher();
                                                    if (fctbErrors.Text.Length > 0)
                                                    {
                                                        helper = false;
                                                        BeginInvoke((MethodInvoker)delegate
                                                        {
                                                            FailedCheckTasksRetryButton.Tag = folder;
                                                            FailedCheckTasksRetryButton.PerformClick();
                                                            maxRetries--;
                                                        });
                                                    }
                                                    else
                                                    {
                                                        helper = true;
                                                        return;
                                                    }
                                                });
                                            else
                                            {
                                                helper = true;
                                                return;
                                            }
                                        }
                                        catch (Exception x)
                                        {
                                            Log(x.Message, "?", "error");
                                            helper = true;
                                            return;
                                        }
                                    }
                                });
                            }
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine(x.Message);
                            helper = true;
                            return;
                        }
                    });
                }
                end:
                Console.WriteLine("End Process");
            });
        }

        private Task StartCheckDriverProcess(string folder, bool isRetry = false)
        {
            helper = false;
            var exceptions = new List<string>();

            var LV = iStackPanel.Controls.OfType<iLV>().Single(l => l.Tag.ToString() == folder);

            var targets = LV.CheckedIndices;
            var actionPath = Application.StartupPath + "\\Actions Logs\\" + actionsLogFile;
            var errorPath = Application.StartupPath + "\\Error Logs\\" + errorsLogFile;
            var disabledPath = Application.StartupPath + "\\Disabled Logs\\" + disabledLogFile;
            var blockedPath = Application.StartupPath + "\\Blocked Logs\\" + blockedLogFile;
            var threadsCon = Globals.THREADS_CONCURRENCY;
            var inter = Globals.THREADS_INTERVAL * 1000;

            var drive = Globals.DRIVE_LETTER + @":\My Drive\";
            var directory = folder;
            var _directory = drive + Globals.VMS_DIRECTORY;
            var temp = "";

            return Task.Factory.StartNew(() =>
            {
                if (!isRetry)
                {
                    if (!Directory.Exists(drive))
                    {
                        Log("Directory " + drive + " not found!", "!!", "error");
                        helper = true;
                        goto end;
                    }
                    var folders = Directory.GetDirectories(drive).Where(d => d.Contains(directory));
                    if (folders.Count() == 0)
                    {
                        Log("Directory " + directory + " not found!", "!!", "error");
                        helper = true;
                        goto end;
                    }
                    else
                    {
                        try
                        {
                            temp = folders.First();

                            var directoryTask = Task.Factory.StartNew(() =>
                            {
                                if (!Directory.Exists(_directory))
                                {
                                    Directory.Move(temp, _directory);
                                    Log(temp + " => " + _directory + " successed!", "!", "info");
                                }
                                else
                                {
                                    alreadyExists = true;
                                    Log("Directory " + _directory + " already exists!", "!", "error");
                                    helper = true;
                                }
                            }).ContinueWith((md) =>
                            {
                                if (md.IsCompleted)
                                {
                                    if (!alreadyExists)
                                    {
                                        var exec = Exec("TASKKILL /T /F /IM MEmuConsole.exe /IM MEmuSVC.exe /IM MEmuHeadless.exe & START /MIN MEmuConsole.exe");
                                        exec.Wait();
                                        if (exec.IsCompleted)
                                        {
                                            Task.Delay(3000).Wait();
                                        }
                                    }
                                }
                            });

                            directoryTask.Wait();
                            Thread.Sleep(5000);
                            //Task.Delay(10000).Wait();
                            Exec("TASKKILL /T /F /IM MEmuConsole.exe /IM MEmu.exe /IM MEmuSVC.exe /IM MEmuHeadless.exe").Wait();

                            iDevices.Invoke((MethodInvoker)delegate
                            {
                                iDevices.currentDirectory = folder;
                                iDevices.actionsLogFile = actionsLogFile;
                                iDevices.errorsLogFile = errorsLogFile;
                                iDevices.blockedLogFile = blockedLogFile;
                            });
                        }
                        catch (Exception x)
                        {
                            Log(x.Message, "?", "error");
                            helper = true;
                            goto end;
                        }
                    }
                }

                if (!alreadyExists)
                {
                    _CancellationTokenSource = new CancellationTokenSource();
                    iDevices.token = _CancellationTokenSource.Token;
                    LV.Invoke((MethodInvoker)async delegate
                    {
                        try
                        {
                            if (maxRetries == 0)
                            {
                                var directoryTask = Task.Factory.StartNew(() =>
                                {
                                    temp = drive + directory + DateTime.Now.ToBinary().ToString();
                                    Directory.Move(_directory, temp);
                                }).ContinueWith((md) =>
                                {
                                    if (md.IsCompleted)
                                    {
                                        Log(_directory + " => " + temp + " successed!", "!", "info");
                                        helper = true;
                                    }
                                });
                                directoryTask.Wait();
                                return;
                            }
                            else
                            {
                                _AsyncBulkheadPolicy = Policy.BulkheadAsync((int)threadsCon, Int32.MaxValue);
                                TasksList = new List<Task>();
                                var ui = TaskScheduler.FromCurrentSynchronizationContext();
                                foreach (var item in targets)
                                {
                                    var device = _Devices.Rows[item];
                                    var index = device["index"].ToString();
                                    var _device = DeviceNameGenerator(index);

                                    GmailUnitTest _gunit = new GmailUnitTest()
                                    {
                                        ServerPort = Globals.SERVER_PORT,
                                        TestDriverTO = Globals.TEST_DRIVER_TIMEOUT,
                                        WaitTO = Globals.DRIVER_WAIT_TIMEOUT,
                                        WaitPolling = Globals.DRIVER_WAIT_POLLING_INTERVAL,
                                        testRetries = 10,
                                        device = _device,
                                        index = index,
                                        actionLogPath = actionPath,
                                        errorLogPath = errorPath,
                                        disabledLogPath = disabledPath,
                                        blockedLogPath = blockedPath,
                                        actionRetries = Globals.FAILED_ACTION_MAX_RETRIES,
                                        cancelationToken = _CancellationTokenSource.Token
                                    };

                                    var account = device["account"].ToString();
                                    var stopWatch = new Stopwatch();
                                    var t = _AsyncBulkheadPolicy.ExecuteAsync(async () =>
                                    {
                                        try
                                        {
                                            await Task.Delay(inter);
                                            await iDevices.AttachDevice(index).ContinueWith((st) =>
                                            {
                                                st.Wait();
                                                if (st.IsCompleted)
                                                {
                                                    if (st.Result)
                                                    {
                                                        stopWatch.Start();
                                                        //iDevices.Fit();
                                                        var deviceStopWatch = new Stopwatch();
                                                        deviceStopWatch.Start();

                                                        var check = true;

                                                        while (check)
                                                        {
                                                            if (deviceStopWatch.Elapsed.Seconds >= 60)
                                                            {
                                                                _gunit.Dispose();
                                                                deviceStopWatch.Stop();
                                                                Log(device + " device not found! Process stopped.", index, "error");
                                                                StopVM(index).Wait();
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                var ced = CheckExistsDevice(_gunit, _device).ContinueWith((cdne) =>
                                                                {
                                                                    if (cdne.IsCompleted)
                                                                    {
                                                                        check = cdne.Result;
                                                                    }
                                                                });
                                                                ced.Wait();
                                                            }
                                                        }

                                                        if (!check)
                                                        {
                                                            Log("check driver process started", index, "info");
                                                            var tdk = _gunit.TestDriver().ContinueWith((td) =>
                                                            {
                                                                if (td.IsCompleted)
                                                                {
                                                                    timer?.Dispose();
                                                                    _gunit?.Dispose();
                                                                    //iDevices.Fit();
                                                                    Log("Test Driver : " + td.Result, index, "info");
                                                                    if (!td.Result)
                                                                    {
                                                                        Log("driver timedout!", index, "error", true);
                                                                        stopWatch.Stop();
                                                                        timer?.Dispose();
                                                                        _gunit?.Dispose();
                                                                        //iDevices.Fit();
                                                                        return;
                                                                    }
                                                                }
                                                            });
                                                            tdk.Wait();
                                                        }
                                                    }
                                                }
                                            }, _CancellationTokenSource.Token);

                                            await Task.Delay(1500).ContinueWith((tt) =>
                                            {
                                                tt.Wait();
                                                if (tt.IsCompleted)
                                                {
                                                    _gunit.KillDriver();
                                                    timer?.Dispose();
                                                    _gunit?.Dispose();
                                                    var svm = StopVM(index);
                                                    svm.Wait();
                                                }
                                            });
                                            Console.WriteLine(_device + ": Process finished!");
                                            return;
                                        }
                                        catch (Exception c)
                                        {
                                            timer?.Dispose();
                                            _gunit?.Dispose();
                                            Console.WriteLine(c.Message);
                                            return;
                                        }
                                    });
                                    TasksList.Add(t);
                                    await Task.Delay(inter);
                                }
                                var _tasks = Task.WhenAll(TasksList);
                                await _tasks.ContinueWith((_t) =>
                                {
                                    if (_t.IsCompleted)
                                    {
                                        try
                                        {
                                            Log("DONE", "!", "info");
                                            helper = true;
                                            return;
                                        }
                                        catch (Exception x)
                                        {
                                            Log(x.Message, "?", "error");
                                            helper = true;
                                            return;
                                        }
                                    }
                                });
                            }
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine(x.Message);
                            helper = true;
                            return;
                        }
                    });
                }
                end:
                Console.WriteLine("End Process");
            });
        }

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

        private Task StopServer(bool log = true)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = "/c TASKKILL /F /IM node.exe",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    process.WaitForExit();
                    if (log)
                        Log("Appium server stopped", "!", "warning");
                }
                catch (Exception c)
                {
                    Console.WriteLine(c.Message);
                    if (log)
                        Log("stop appium server failed!", "!", "error");
                }
            });
        }

        private Task StopVM(string index)
        {
            return iDevices.Stop(index);
        }

        private Task<List<string>> TakeLastLines(string text, int count)
        {
            return Task.Factory.StartNew(() =>
            {
                List<string> lines = new List<string>();
                var match = Regex.Match(text, "^.*$", RegexOptions.Multiline | RegexOptions.RightToLeft);

                while (match.Success && lines.Count < count)
                {
                    lines.Insert(0, match.Value);
                    match = match.NextMatch();
                }

                return lines;
            });
        }

        private async void fsw_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        await sr.ReadToEndAsync().ContinueWith(async (response) =>
                        {
                            if (response.IsCompleted)
                            {
                                await TakeLastLines(response.Result, 100).ContinueWith((tl) =>
                                {
                                    if (tl.IsCompleted)
                                        fctbActions.BeginInvoke((MethodInvoker)delegate
                                        {
                                            fctbActions.Text = String.Join("\n", tl.Result);
                                            fctbActions.GoEnd();
                                        });
                                });
                            }
                        });
                    }
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        private void tooltip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            e.DrawText();
        }

        private async void fswServer_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        await sr.ReadToEndAsync().ContinueWith(async (response) =>
                        {
                            if (response.IsCompleted)
                            {
                                await TakeLastLines(response.Result, 100).ContinueWith((tl) =>
                                {
                                    if (tl.IsCompleted)
                                        fctbErrors.BeginInvoke((MethodInvoker)delegate
                                        {
                                            fctbErrors.Text = String.Join("\n", tl.Result);
                                            fctbErrors.GoEnd();
                                        });
                                });
                            }

                        });
                    }
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        private void lvTimer_Tick(object sender, EventArgs e)
        {
            iStackPanel.BeginInvoke((MethodInvoker)delegate
            {
                var count = iStackPanel.Controls.OfType<iLV>().Select(c => c.CheckedItemsCount).Sum();
                selectedLabel.Text = count + " selected";

                UIButtonPanel.Buttons.ForEach((button) =>
                {
                    if (button.Properties.Caption != "Reload" && button.Properties.Caption != "Install")
                        button.Properties.Visible = (count > 0);
                });
            });
        }

        private async void saveSettingsButton_Click(object sender, EventArgs e)
        {
            try
            {
                var ctrl = sender as SimpleButton;
                var conf = new _CONFIG()
                {
                    _SERVER_PORT = Globals.SERVER_PORT = (Int32)portNum.Value,
                    _TEST_DRIVER_TIMEOUT = Globals.TEST_DRIVER_TIMEOUT = (Int32)testDriverToNum.Value,
                    _DRIVER_WAIT_TIMEOUT = Globals.DRIVER_WAIT_TIMEOUT = (Int32)waitNum.Value,
                    _DRIVER_WAIT_POLLING_INTERVAL = Globals.DRIVER_WAIT_POLLING_INTERVAL = (Int32)pollingNum.Value,
                    _THREADS_CONCURRENCY = Globals.THREADS_CONCURRENCY = (Int32)concurrentThreadsNum.Value,
                    _THREADS_INTERVAL = Globals.THREADS_INTERVAL = (Int32)threadsIntervalNum.Value,
                    _DRIVE_LETTER = Globals.DRIVE_LETTER = string.IsNullOrWhiteSpace(driverLetterText.Text) ? "D" : driverLetterText.Text,
                    _VMS_DIRECTORY = Globals.VMS_DIRECTORY = string.IsNullOrWhiteSpace(vmsDirectoryText.Text) ? "VMS" : vmsDirectoryText.Text,
                    _FAILED_AUTO_RETRY = Globals.FAILED_AUTO_RETRY = failedAutoRetrySwitch.SwitchState == XUISwitch.State.On,
                    _FAILED_MAX_RETRIES = Globals.FAILED_MAX_RETRIES = (Int32)failedMAxRetriesNum.Value,
                    _FAILED_ACTION_MAX_RETRIES = Globals.FAILED_ACTION_MAX_RETRIES = (Int32)failedActionsMaxRetriesNum.Value,
                    _UI_TOP_MOST = Globals.UI_TOP_MOST = appTopMostSwitch.SwitchState == XUISwitch.State.On,
                    _UI_TRANSPARENT = Globals.UI_TRANSPARENT = appTransparentSwitch.SwitchState == XUISwitch.State.On,
                    _DEFAULT_SCENARIO = Globals.DEFAULT_SCENARIO = await GetScenario(Convert.ToInt32(defaultScenarioGallery.EditValue.ToString().Split('|')[1])),
                    _WARMUP_AUTORUN = Globals.WARMUP_AUTORUN = warmupAutoRunSwitch.SwitchState == XUISwitch.State.On,
                    _WARMUP_PROCEED_INBOX_FOLDER = Globals.WARMUP_PROCEED_INBOX_FOLDER = warmupInboxFolderSwitch.SwitchState == XUISwitch.State.On,
                    _WARMUP_MAX_TREATED_INBOX_EMAILS = Globals.WARMUP_MAX_TREATED_INBOX_EMAILS = (Int32)treatedInboxEmailsMaxNum.Value
                };

                IFormatter formatter = new BinaryFormatter();
                using (var stream = new FileStream(Application.StartupPath + "\\" + ctrl.Tag.ToString(), FileMode.Create, FileAccess.Write))
                    formatter.Serialize(stream, conf);

                BeginInvoke((MethodInvoker)delegate
                {
                    iAlert.Show(this, "Settings", "Settings saved with success!");
                });
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        private async void directoryCLB_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            await DirectoryChangedAsync();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                _CancellationTokenSource?.Cancel();
                _AsyncBulkheadPolicy?.Dispose();

                if (TasksList != null)
                {
                    TasksList.Clear();
                    TasksList = null;
                }

                _Service?.StopAppium();

                StopServer(false).ConfigureAwait(false);
                iDevices?.Stop().ConfigureAwait(false);
                RenameFolder().ConfigureAwait(false);
                Application.ExitThread();
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                Application.ExitThread();
            }
        }

        private DevExpress.Utils.Svg.SvgImage GetResource(string action)
        {
            DevExpress.Utils.Svg.SvgImage _Image = null;
            switch (action)
            {
                case "open":
                    _Image = global::GUX.Properties.Resources.OpenAct;
                    break;
                case "click link":
                    _Image = global::GUX.Properties.Resources.ClickAct;
                    break;
                case "archive":
                    _Image = global::GUX.Properties.Resources.ArchiveAct;
                    break;
                case "not spam":
                    _Image = global::GUX.Properties.Resources.NotSpamAct;
                    break;
                case "reply":
                    _Image = global::GUX.Properties.Resources.ReplyAct;
                    break;
                case "delete":
                    _Image = global::GUX.Properties.Resources.DeleteAct;
                    break;
                case "add star":
                    _Image = global::GUX.Properties.Resources.StarAct;
                    break;
                case "flag":
                    _Image = global::GUX.Properties.Resources.FlagAct;
                    break;
                case "forward":
                    _Image = global::GUX.Properties.Resources.ForwardAct;
                    break;
                case "pin":
                    _Image = global::GUX.Properties.Resources.PinAct;
                    break;
                case "send email":
                    _Image = global::GUX.Properties.Resources.SendAct;
                    break;
                case "snooz":
                    _Image = global::GUX.Properties.Resources.SnoozAct;
                    break;
                case "important":
                    _Image = global::GUX.Properties.Resources.ImportantAct;
                    break;
                case "scroll":
                    _Image = global::GUX.Properties.Resources.ScrollAct;
                    break;
            }
            return _Image;
        }

        private Task<DataTable> GetScenarios()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var dt = _psqlHelper.DirectQuery("select * from scenario;");
                    _psqlHelper.Dispose();
                    return dt;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                    return null;
                }
            });
        }

        private Task<Scenario> GetScenario(int id)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var dt = _psqlHelper.DirectQuery($"select * from scenario where id = {id};").Rows[0];
                    _psqlHelper.Dispose();
                    return new Scenario()
                    {
                        ID = (int)dt["id"],
                        name = (string)dt["name"],
                        actions = (string)dt["actions"],
                        keyword = (string)dt["keyword"],
                        date = (string)dt["date_filter"]
                    };
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                    return null;
                }
            });
        }

        private Task ReloadScenarios()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    DataTable dt = _psqlHelper.DirectQuery("select * from scenario;");
                    var group = dt.Rows.OfType<DataRow>().ToList().GroupBy(c => c["name"]);

                    galleryControl2.BeginInvoke((MethodInvoker)delegate
                    {
                        galleryControl2.Gallery.Destroy();
                    });

                    foreach (var item in group)
                    {
                        var name = item.First()["name"].ToString();
                        var actions = item.First()["actions"].ToString().Split(',');
                        var keyword = item.First()["keyword"].ToString();
                        var date = item.First()["date_filter"].ToString();

                        var stt = new SuperToolTip();
                        stt.Items.AddTitle(name);
                        stt.Items.Add($"Keyword: {keyword}");
                        stt.Items.Add($"Date Filter: {date}");

                        var CaptionControl = new iTools();
                        CaptionControl.Title = name;
                        CaptionControl.DataID = (int)item.First()["id"];
                        CaptionControl.ButtonClick += CaptionControl_ButtonClick;
                        CaptionControl.ToolTip = stt;

                        var galGroup = new DevExpress.XtraBars.Ribbon.GalleryItemGroup()
                        {
                            Caption = name
                        };
                        galGroup.CaptionAlignment = DevExpress.XtraBars.Ribbon.GalleryItemGroupCaptionAlignment.Center;
                        galGroup.Tag = (int)item.First()["id"];
                        galGroup.CaptionControl = CaptionControl;
                        galGroup.CaptionAlignment = DevExpress.XtraBars.Ribbon.GalleryItemGroupCaptionAlignment.Stretch;

                        foreach (var act in actions)
                        {
                            var action = new DevExpress.XtraBars.Ribbon.GalleryItem()
                            {
                                Caption = act
                            };
                            action.ImageOptions.SvgImage = GetResource(act.ToLower());
                            galGroup.Items.Add(action);
                        }

                        galleryControl2.BeginInvoke((MethodInvoker)delegate
                        {
                            galleryControl2.Gallery.Groups.Add(galGroup);
                        });
                    }

                    _psqlHelper.Dispose();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            });
        }

        private async void CaptionControl_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            var id = (sender as iTools).DataID;
            var dialog = XtraMessageBox.Show("Are you sure?", "Delete scenario", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var dt = _psqlHelper.DirectNonQuery($"delete from scenario where id = {id};");
                        BeginInvoke((MethodInvoker)delegate
                        {
                            iAlert.Show(this, dt == 1 ? "Delete scenario successed!" : "Delete scenario failed!", "Scenario");
                        });

                        _psqlHelper.Dispose();
                    }
                    catch (NpgsqlException ex)
                    {
                        XtraMessageBox.Show(ex.Message);
                    }
                }).ContinueWith(async (ads) =>
                {
                    if (ads.IsCompleted)
                        await ReloadScenarios();
                });
        }

        private async void scenarioReloadButton_Click(object sender, EventArgs e)
        {
            await ReloadScenarios();
        }

        private async void scenarioSave_Click(object sender, EventArgs e)
        {
            try
            {
                var name = scenarioName.Text.Trim();
                var actions = string.Join(",", galleryControl1.Gallery.GetCheckedItems().Select(cc => cc.Caption));
                var keyword = ScenarioKeywordFilter.Text.Trim();
                var type = (string)ScenarioDateFilterType.EditValue;
                var val = ScenarioDateFilterValue.Value;
                var kind = ScenarioDateFilterKind.SelectedText[0].ToString().ToLower();
                var date = ScenarioDateFilterType.EditValue != null ? type + val + kind : "";
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var dt = _psqlHelper.DirectNonQuery($"insert into scenario values (DEFAULT,'{name}','{actions}','{keyword}','{date}');");
                        BeginInvoke((MethodInvoker)delegate
                        {
                            iAlert.Show(this, dt == 1 ? "Create scenario successed!" : "Create scenario failed!", "Scenario");
                        });
                        _psqlHelper.Dispose();
                    }
                    catch (NpgsqlException ex)
                    {
                        XtraMessageBox.Show(ex.Message);
                    }
                }).ContinueWith(async (ads) =>
                {
                    if (ads.IsCompleted)
                        await ReloadScenarios();
                });
            }
            catch (Exception c)
            {
                XtraMessageBox.Show(c.Message);
            }
        }

        private void scenarioReset_Click(object sender, EventArgs e)
        {
            galleryControl1.Gallery.GetAllItems().ForEach((item) =>
            {
                item.Checked = false;
            });
            scenarioName.ResetText();
        }

        private void selectAllDeviceCb_CheckedChanged(object sender)
        {
            var t = new Thread((ThreadStart)delegate
            {
                iStackPanel.BeginInvoke((MethodInvoker)async delegate
                {
                    var listViews = iStackPanel.Controls.OfType<iLV>();
                    foreach (var lv in listViews)
                    {
                        await Task.Factory.StartNew(() =>
                        {
                            lv.BeginInvoke((MethodInvoker)delegate
                            {
                                if (selectAllDeviceCb.Checked)
                                    lv.CheckAll();
                                else
                                    lv.UnCheckAll();
                            });
                        }).ContinueWith((ck) =>
                        {
                            //lv.ItemChecked += ListView_ItemChecked;
                            BeginInvoke((MethodInvoker)delegate
                            {
                                var lvs = iStackPanel.Controls.OfType<iLV>().Select(c => c.CheckedItemsCount);
                                selectedLabel.Text = lvs.Sum() + " selected";
                            });
                        });
                    }
                });
            });
            t.Priority = ThreadPriority.Highest;
            t.IsBackground = true;
            t.Start();
        }

        private Image GetSemiTransparentImage()
        {
            var semiTransparentImage = new Bitmap(64, 32);
            for (int x = 0; x < semiTransparentImage.Width; x++)
            {
                for (int y = 0; y < semiTransparentImage.Height; y++)
                {
                    Color color = semiTransparentImage.GetPixel(x, y);
                    semiTransparentImage.SetPixel(x, y, Color.FromArgb(30, color.R, color.G, color.B));
                }
            }
            return semiTransparentImage;
        }

        private Task<Image> Placeholder(string keyword, string color, string background, int height = 500, int width = 500, double fontsize = 290, double DY = 10.5, int X = 50, int Y = 50)
        {
            return Task.Factory.StartNew(() =>
            {
                Image placeholder = null;
                var backgrounds = new string[] {
            "#21E7FF",
            "#B9EBE6",
            "#C7FFEF",
            "#B5EBD1",
            "#5DFF91",
            "#BFFCFF",
            "#B9EBE5",
            "#C7FFEF",
            "#94F0C6",
            "#5CFF9A"
                };
                var backcolor = backgrounds[new Random().Next(0, backgrounds.Count() - 1)];
                byte[] bytes = new ASCIIEncoding().GetBytes($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{width}\" height=\"{height}\"><rect width=\"{width}\" height=\"{height}\" fill=\"{backcolor}\"/><text fill=\"rgba(0,0,0,1)\" font-family=\"sans-serif\" font-weight=\"bolder\" x=\"{width / 2}\" y=\"{(height / 2) + (height / 6)}\" font-size=\"{width / 2}\" text-anchor=\"middle\">{keyword[0]}</text></svg>");
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    using (var xmlReader = XmlReader.Create(ms))
                    {
                        var svgImage = SvgLoader.ParseDocument(xmlReader);
                        var img = SvgBitmap.Create(svgImage);
                        placeholder = img.Render(null, 1.0D);
                    }
                }

                return placeholder;
            });
        }

        private async void accordionSettings_Click(object sender, EventArgs e)
        {
            MainTabControl.SelectedTabPage = SettingsXtraTabPage;
            SplashScreenManager.ShowDefaultWaitForm(this, true, true, false, 750, "Please Wait", "Fetching...");
            await GetScenarios().ContinueWith((gs) =>
            {
                if (gs.IsCompleted)
                {
                    defaultScenarioGallery.BeginInvoke((MethodInvoker)async delegate
                    {
                        defaultScenarioGallery.Properties.Items.Clear();
                        defaultScenarioGallery.Properties.SmallImages = null;
                        defaultScenarioGallery.Properties.LargeImages = null;
                        var imageList = new ImageList();
                        int index = 0;
                        var items = new List<ImageComboBoxItem>();
                        foreach (DataRow dt in gs.Result.Rows)
                        {
                            var img = await Placeholder(dt["name"].ToString(), "", "");
                            imageList.Images.Add(img);
                            var scenario = new ImageComboBoxItem()
                            {
                                Description = dt["name"].ToString(),
                                Value = $"{dt["name"].ToString()[0]}|{(int)dt["id"]}"
                            };
                            items.Add(scenario);
                            index++;
                        }
                        defaultScenarioGallery.Properties.Items.AddRange(items);
                    });
                    SplashScreenManager.CloseDefaultWaitForm();
                }
            });
        }

        private Task DirectoryChangedAsync()
        {
            try
            {
                var items = directoryCLB.CheckedItems;
                directoryCLB.Enabled = false;
                directoryCLB.SuspendLayout();
                var dir = string.Join(",", items.OfType<CheckedListBoxItem>().Select(c => c.ToString()));

                return Task.Factory.StartNew(async () =>
                {
                    BeginInvoke((MethodInvoker)delegate
                    {
                        SplashScreenManager.ShowDefaultWaitForm(this, true, true, false, 750, "Please Wait", "Fetching...");
                    });
                    await LoadDevices(dir).ContinueWith((ld) =>
                    {
                        if (ld.IsCompleted)
                            directoryCLB.BeginInvoke((MethodInvoker)delegate
                            {
                                directoryCLB.Enabled = true;
                                directoryCLB.ResumeLayout();
                                SplashScreenManager.CloseForm();
                            });
                    });
                });
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
                return null;
            }
        }

        private async void UIButtonPanel_ButtonClick(object sender, ButtonEventArgs e)
        {
            var actionPath = Application.StartupPath + "\\Actions Logs\\" + actionsLogFile;
            var errorPath = Application.StartupPath + "\\Error Logs\\" + errorsLogFile;
            var disabledPath = Application.StartupPath + "\\Disabled Logs\\" + actionsLogFile;
            var blockedPath = Application.StartupPath + "\\Blocked Logs\\" + errorsLogFile;

            var threadsCon = Globals.THREADS_CONCURRENCY;
            var inter = Globals.THREADS_INTERVAL * 1000;
            var targets = new iLV().CheckedItems;

            helper = true;

            switch (e.Button.Properties.Caption)
            {
                case "Reload":
                    await DirectoryChangedAsync();
                    break;
                case "Start":
                    if (!_Service.IsRunning())
                    {
                        var handle = SplashScreenManager.ShowOverlayForm(this, new DevExpress.XtraSplashScreen.OverlayWindowOptions()
                        {
                            SkinName = "Visual Studio 2013 Dark",
                            FadeIn = true,
                            FadeOut = true,
                            Image = global::GUX.Properties.Resources.loader
                        });

                        await _Service.StartAppium().ContinueWith((sa) =>
                        {
                            if (sa.IsCompleted)
                            {
                                BeginInvoke((MethodInvoker)async delegate
                                {
                                    appiumServerStatus.ImageOptions.SvgImage = global::GUX.Properties.Resources.actions_checkcircled;
                                    var img = new Bitmap(global::GUX.Properties.Resources.android_logo, new Size(24, 24));
                                    BeginInvoke((MethodInvoker)delegate
                                    {
                                        iAlert.Show(this, "", "Server Running.", img);
                                    });
                                    await Task.Run(() =>
                                    {
                                        //toast.ShowNotification("b8c36c4b-00e7-4c8d-ad53-1aefa75929aa");
                                        SplashScreenManager.CloseOverlayForm(handle);
                                    });
                                });
                            }
                        });
                    }
                    break;
                case "Install":
                    iPopupMenu.ShowPopup(iBarManager, new Point(MousePosition.X, MousePosition.Y));
                    break;
                case "Warmup":
                    if (Globals.DEFAULT_SCENARIO == null)
                    {
                        XtraMessageBox.Show("Please add a default scenario in settings", "Default Scenario");
                    }
                    else
                    {
                        await SetFileWatcher();
                        var wuTasks = new List<Task>();
                        alreadyExists = false;
                        var keyword = Globals.DEFAULT_SCENARIO.keyword;
                        var date = Globals.DEFAULT_SCENARIO.date;
                        await Task.Factory.StartNew(() =>
                        {
                            directoryCLB.BeginInvoke((MethodInvoker)async delegate
                            {
                                var dirs = directoryCLB.CheckedItems.OfType<CheckedListBoxItem>().Select(c => c.ToString());
                                progressBar.Visibility = DevExpress.XtraBars.BarItemVisibility.OnlyInRuntime;
                                foreach (var dir in dirs)
                                {
                                    if (alreadyExists)
                                        break;
                                    currentDirectory = dir;
                                    currentDateFilter = date;
                                    currentKeyword = keyword;
                                    maxRetries = Globals.FAILED_MAX_RETRIES;
                                    var t = StartActionsProcess(dir, keyword, date).ContinueWith(async (sp) =>
                                    {
                                        iStackPanel.BeginInvoke((MethodInvoker)delegate
                                        {
                                            iStackPanel.Controls.OfType<iLV>().ToList().ForEach((lv) =>
                                            {
                                                lv.BeginInvoke((MethodInvoker)delegate
                                                {
                                                    lv.InProgress = (lv.PlainTextTitle == dir);
                                                });
                                            });
                                        });
                                        while (!helper)
                                        {
                                            //
                                        }
                                        Log(dir + " task completed!", "!", "info");
                                        await RenameFolder();
                                        iStackPanel.BeginInvoke((MethodInvoker)delegate
                                        {
                                            var lv = iStackPanel.Controls.OfType<iLV>().Single(l => l.PlainTextTitle == currentDirectory);
                                            lv.InProgress = false;
                                        });
                                    });
                                    await t;
                                    wuTasks.Add(t);
                                    await Task.Delay(2000);
                                }
                                await Task.WhenAll(wuTasks).ContinueWith((all) =>
                                {
                                    if (all.IsCompleted)
                                    {
                                        Invoke((MethodInvoker)delegate
                                        {
                                            progressBar.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                                        });

                                        iStackPanel.BeginInvoke((MethodInvoker)delegate
                                        {
                                            var lv = iStackPanel.Controls.OfType<iLV>().Single(l => l.PlainTextTitle == currentDirectory);
                                            lv.InProgress = false;
                                        });
                                    }
                                });
                            });
                        });
                    }
                    break;
                case "Check":
                    checkPopupMenu.ShowPopup(iBarManager, MousePosition);
                    break;
                case "Setup":
                    await SetFileWatcher();
                    var tsk = new List<Task>();
                    alreadyExists = false;
                    await Task.Factory.StartNew(() =>
                    {
                        directoryCLB.BeginInvoke((MethodInvoker)async delegate
                        {
                            var dirs = directoryCLB.CheckedItems.OfType<CheckedListBoxItem>().Select(c => c.ToString());
                            progressBar.Visibility = DevExpress.XtraBars.BarItemVisibility.OnlyInRuntime;
                            foreach (var dir in dirs)
                            {
                                if (alreadyExists)
                                    break;
                                currentDirectory = dir;
                                maxRetries = Globals.FAILED_MAX_RETRIES;
                                var t = StartProcess(dir).ContinueWith(async (sp) =>
                                {
                                    iStackPanel.BeginInvoke((MethodInvoker)delegate
                                    {
                                        iStackPanel.Controls.OfType<iLV>().ToList().ForEach((lv) =>
                                        {
                                            lv.BeginInvoke((MethodInvoker)delegate
                                            {
                                                lv.InProgress = (lv.PlainTextTitle == dir);
                                            });
                                        });
                                    });
                                    while (!helper)
                                    {
                                        //
                                    }
                                    Log(dir + " task completed!", "!", "info");
                                    await RenameFolder();
                                    iStackPanel.BeginInvoke((MethodInvoker)delegate
                                    {
                                        var lv = iStackPanel.Controls.OfType<iLV>().Single(l => l.PlainTextTitle == currentDirectory);
                                        lv.InProgress = false;
                                    });
                                });
                                await t;
                                tsk.Add(t);
                                await Task.Delay(2000);
                            }
                            await Task.WhenAll(tsk).ContinueWith((all) =>
                            {
                                if (all.IsCompleted)
                                {
                                    Invoke((MethodInvoker)delegate
                                    {
                                        progressBar.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                                    });

                                    iStackPanel.BeginInvoke((MethodInvoker)delegate
                                    {
                                        var lv = iStackPanel.Controls.OfType<iLV>().Single(l => l.PlainTextTitle == currentDirectory);
                                        lv.InProgress = false;
                                    });
                                }

                                //if (Globals.WARMUP_AUTORUN)
                                //    UIButtonPanel.Invoke((MethodInvoker)delegate
                                //    {
                                //        UIButtonPanel.Buttons.Owner.PerformClick(UIButtonPanel.Buttons["Warmup"]);
                                //    });
                            });
                        });
                    });
                    break;
                case "Stop":
                    try
                    {
                        _CancellationTokenSource?.Cancel();

                        _AsyncBulkheadPolicy?.Dispose();

                        if (TasksList != null)
                        {
                            TasksList.Clear();
                            TasksList = null;
                        }

                        _Service?.StopAppium();

                        Globals.scheduleTasks.Clear();
                        Globals.SchServices.Dispose();

                        //await StopAll(false);

                        await iDevices?.Stop().ContinueWith(async (st) =>
                        {
                            if (st.IsCompleted)
                                await RenameFolder();
                        });

                        await StopServer().ContinueWith((ss) =>
                        {
                            if (ss.IsCompleted)
                            {
                                BeginInvoke((MethodInvoker)delegate
                                {
                                    appiumServerStatus.ImageOptions.SvgImage = global::GUX.Properties.Resources.actions_deletecircled;
                                });
                            }
                        });

                    }
                    catch (Exception c)
                    {
                        Console.WriteLine(c.Message);
                    }
                    break;
                case "Fit":
                    await iDevices?.Fit();
                    break;
                case "Close":
                    await iDevices?.Stop().ContinueWith(async (st) =>
                    {
                        if (st.IsCompleted)
                            await RenameFolder();
                    });
                    break;
            }
        }

        private void isTransSwitch_SwitchStateChanged(object sender, EventArgs e)
        {
            this.Opacity = appTransparentSwitch.SwitchState == XUISwitch.State.On ? 0.95D : 1D;
        }

        private async void FailedTasksRetryButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (currentDirectory != "")
                {
                    //var itemIndex = directoryCLB.FindStringExact(currentDirectory, 0);
                    //var dirs = directoryCLB.CheckedItems.OfType<CheckedListBoxItem>().ToList();
                    //var dirIndex = dirs.IndexOf(directoryCLB.Items[itemIndex]);
                    var LV = iStackPanel.Controls.OfType<iLV>().Single(l => l.Tag.ToString() == currentDirectory);

                    if (LV.DataSource != null)
                    {
                        LV.UnCheckAll();
                        if (fctbErrors.Lines.Count > 0)
                        {
                            foreach (var line in fctbErrors.Lines)
                            {
                                if (line.Length > 0)
                                {
                                    int number;
                                    var output = line.Split('[', ']');
                                    if (output.Count() > 0)
                                    {
                                        if (int.TryParse(output[1], out number))
                                        {
                                            LV.SetItemChecked(number - 1, true);
                                            if (maxRetries == 0)
                                            {
                                                Log("max retries has been reached!", number.ToString(), "warning", true);
                                            }
                                        }
                                    }
                                }
                            }
                            if (LV.CheckedItemsCount > 0)
                            {
                                if (_AsyncBulkheadPolicy != null)
                                    _AsyncBulkheadPolicy.Dispose();
                                if (TasksList != null)
                                    TasksList = null;

                                await SetFileWatcher();
                                Log("Select failed indices", "!", "info");
                                await StartProcess(currentDirectory, true);
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
        }

        private async void blockedLogFSW_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {

                        await sr.ReadToEndAsync().ContinueWith(async (response) =>
                        {
                            if (response.IsCompleted)
                            {
                                await TakeLastLines(response.Result, 100).ContinueWith((tl) =>
                                {
                                    if (tl.IsCompleted)
                                        fctbBlocked.BeginInvoke((MethodInvoker)delegate
                                        {
                                            fctbBlocked.Text = String.Join("\n", tl.Result);
                                            fctbBlocked.GoEnd();
                                        });
                                });
                            }

                        });
                    }
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        private async void disabledLogFSW_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        await sr.ReadToEndAsync().ContinueWith(async (response) =>
                        {
                            if (response.IsCompleted)
                            {
                                await TakeLastLines(response.Result, 100).ContinueWith((tl) =>
                                {
                                    if (tl.IsCompleted)
                                        fctbDisabled.BeginInvoke((MethodInvoker)delegate
                                        {
                                            fctbDisabled.Text = String.Join("\n", tl.Result);
                                            fctbDisabled.GoEnd();
                                        });
                                });
                            }
                        });
                    }
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        private void ProcessXtraTabPage_Paint(object sender, PaintEventArgs e)
        {
            var background = new SolidBrush(Color.FromArgb(255, (byte)28, (byte)28, (byte)28));
            e.Graphics.FillRectangle(background, e.ClipRectangle);
        }

        private void logsTabControl_CustomHeaderButtonClick(object sender, DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventArgs e)
        {
            //iPopupMenu.ShowPopup(iBarManager, new Point(MousePosition.X, MousePosition.Y));
        }

        private async void warningLogFSW_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        await sr.ReadToEndAsync().ContinueWith(async (response) =>
                        {
                            if (response.IsCompleted)
                            {
                                await TakeLastLines(response.Result, 100).ContinueWith((tl) =>
                                {
                                    if (tl.IsCompleted)
                                        fctbWarning.BeginInvoke((MethodInvoker)delegate
                                        {
                                            fctbWarning.Text = String.Join("\n", tl.Result);
                                            fctbWarning.GoEnd();
                                        });
                                });
                            }
                        });
                    }
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        private async void FailedCheckTasksRetryButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (currentDirectory != "")
                {
                    var LV = iStackPanel.Controls.OfType<iLV>().Single(l => l.Tag.ToString() == currentDirectory);

                    if (LV.DataSource != null)
                    {
                        LV.UnCheckAll();
                        if (fctbWarning.Lines.Count > 0)
                        {
                            foreach (var line in fctbWarning.Lines)
                            {
                                if (line.Length > 0)
                                {
                                    int number;
                                    var output = line.Split('[', ']');
                                    if (output.Count() > 0)
                                    {
                                        if (int.TryParse(output[1], out number))
                                        {
                                            LV.SetItemChecked(number - 1, true);
                                            if (maxRetries == 0)
                                            {
                                                Log("max retries has been reached!", number.ToString(), "warning", true);
                                            }
                                        }
                                    }
                                }
                            }
                            if (LV.CheckedItemsCount > 0)
                            {
                                if (_AsyncBulkheadPolicy != null)
                                    _AsyncBulkheadPolicy.Dispose();
                                if (TasksList != null)
                                    TasksList = null;

                                await SetFileWatcher();
                                Log("Select warmup failed indices", "!", "info");
                                await StartCheckProcess(currentDirectory, true);
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
        }

        private void defaultScenarioGallery_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(Convert.ToInt32(defaultScenarioGallery.EditValue.ToString().Split('|')[1]));
        }

        private async void DevicesDriverCheck_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            await SetFileWatcher();
            var tskss = new List<Task>();
            alreadyExists = false;
            await Task.Factory.StartNew(() =>
            {
                directoryCLB.BeginInvoke((MethodInvoker)async delegate
                {
                    var dirs = directoryCLB.CheckedItems.OfType<CheckedListBoxItem>().Select(c => c.ToString());
                    progressBar.Visibility = DevExpress.XtraBars.BarItemVisibility.OnlyInRuntime;
                    foreach (var dir in dirs)
                    {
                        if (alreadyExists)
                            break;
                        currentDirectory = dir;
                        maxRetries = Globals.FAILED_MAX_RETRIES;
                        var t = StartCheckDriverProcess(dir).ContinueWith(async (sp) =>
                        {
                            iStackPanel.BeginInvoke((MethodInvoker)delegate
                            {
                                iStackPanel.Controls.OfType<iLV>().ToList().ForEach((lv) =>
                                {
                                    lv.BeginInvoke((MethodInvoker)delegate
                                    {
                                        lv.InProgress = (lv.PlainTextTitle == dir);
                                    });
                                });
                            });
                            while (!helper)
                            {
                                //
                            }
                            Log(dir + " task completed!", "!", "info");
                            await RenameFolder();
                            iStackPanel.BeginInvoke((MethodInvoker)delegate
                            {
                                var lv = iStackPanel.Controls.OfType<iLV>().Single(l => l.PlainTextTitle == currentDirectory);
                                lv.InProgress = false;
                            });
                        });
                        await t;
                        tskss.Add(t);
                        await Task.Delay(2000);
                    }
                    await Task.WhenAll(tskss).ContinueWith((all) =>
                    {
                        if (all.IsCompleted)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                progressBar.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            });

                            iStackPanel.BeginInvoke((MethodInvoker)delegate
                            {
                                var lv = iStackPanel.Controls.OfType<iLV>().Single(l => l.PlainTextTitle == currentDirectory);
                                lv.InProgress = false;
                            });
                        }
                    });
                });
            });
        }

        private async void simpleButton1_Click(object sender, EventArgs e)
        {
            var k = iL.Text.Trim();
            var w = Convert.ToInt32(iW.Text);
            var h = Convert.ToInt32(iH.Text);
            var f = Convert.ToInt32(iF.Text);
            var dy = (double)iDY.Value;
            var x = Convert.ToInt32(iX.Text);
            var y = Convert.ToInt32(iY.Text);
            //await Placeholder(k, "", "", h, w, f, dy, x, y);
        }

        private void warmupAutoRunSwitch_SwitchStateChanged(object sender, EventArgs e)
        {
            if (defaultScenarioGallery.SelectedIndex < 0)
                warmupAutoRunSwitch.SwitchState = XUISwitch.State.Off;
        }

        private async void GoogleAccountsCheck_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            await SetFileWatcher();
            var tskss = new List<Task>();
            alreadyExists = false;
            await Task.Factory.StartNew(() =>
            {
                directoryCLB.BeginInvoke((MethodInvoker)async delegate
                {
                    var dirs = directoryCLB.CheckedItems.OfType<CheckedListBoxItem>().Select(c => c.ToString());
                    progressBar.Visibility = DevExpress.XtraBars.BarItemVisibility.OnlyInRuntime;
                    foreach (var dir in dirs)
                    {
                        if (alreadyExists)
                            break;
                        currentDirectory = dir;
                        maxRetries = Globals.FAILED_MAX_RETRIES;
                        var t = StartCheckProcess(dir).ContinueWith(async (sp) =>
                        {
                            iStackPanel.BeginInvoke((MethodInvoker)delegate
                            {
                                iStackPanel.Controls.OfType<iLV>().ToList().ForEach((lv) =>
                                {
                                    lv.BeginInvoke((MethodInvoker)delegate
                                    {
                                        lv.InProgress = (lv.PlainTextTitle == dir);
                                    });
                                });
                            });
                            while (!helper)
                            {
                                //
                            }
                            Log(dir + " task completed!", "!", "info");
                            await RenameFolder();
                            iStackPanel.BeginInvoke((MethodInvoker)delegate
                            {
                                var lv = iStackPanel.Controls.OfType<iLV>().Single(l => l.PlainTextTitle == currentDirectory);
                                lv.InProgress = false;
                            });
                        });
                        await t;
                        tskss.Add(t);
                        await Task.Delay(2000);
                    }
                    await Task.WhenAll(tskss).ContinueWith((all) =>
                    {
                        if (all.IsCompleted)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                progressBar.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                            });

                            iStackPanel.BeginInvoke((MethodInvoker)delegate
                            {
                                var lv = iStackPanel.Controls.OfType<iLV>().Single(l => l.PlainTextTitle == currentDirectory);
                                lv.InProgress = false;
                            });
                        }
                    });
                });
            });
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var lines = fctbActions.FindLines("^((?!warning).)*$", RegexOptions.Multiline);
            if (lines.Count > 0)
                fctbActions.RemoveLines(lines);
        }

        private async void FailedWarmupTasksRetryButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (currentDirectory != "")
                {
                    //var itemIndex = directoryCLB.FindStringExact(currentDirectory, 0);
                    //var dirs = directoryCLB.CheckedItems.OfType<CheckedListBoxItem>().ToList();
                    //var dirIndex = dirs.IndexOf(directoryCLB.Items[itemIndex]);

                    var LV = iStackPanel.Controls.OfType<iLV>().Single(l => l.Tag.ToString() == currentDirectory);

                    if (LV.DataSource != null)
                    {
                        LV.UnCheckAll();
                        if (fctbErrors.Lines.Count > 0)
                        {
                            foreach (var line in fctbErrors.Lines)
                            {
                                if (line.Length > 0)
                                {
                                    int number;
                                    var output = line.Split('[', ']');
                                    if (output.Count() > 0)
                                    {
                                        if (int.TryParse(output[1], out number))
                                        {
                                            LV.SetItemChecked(number - 1, true);
                                            if (maxRetries == 0)
                                            {
                                                Log("max retries has been reached!", number.ToString(), "warning", true);
                                            }
                                        }
                                    }
                                }
                            }
                            if (LV.CheckedItemsCount > 0)
                            {
                                if (_AsyncBulkheadPolicy != null)
                                    _AsyncBulkheadPolicy.Dispose();
                                if (TasksList != null)
                                    TasksList = null;

                                await SetFileWatcher();
                                Log("Select warmup failed indices", "!", "info");
                                await StartActionsProcess(currentDirectory, currentKeyword, currentDateFilter, true);
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
        }

        private void isTopMostSwitch_SwitchStateChanged(object sender, EventArgs e)
        {
            this.TopMost = appTopMostSwitch.SwitchState == XUISwitch.State.On;
        }

        private void Gallery_ItemClick(object sender, DevExpress.XtraBars.Ribbon.GalleryItemClickEventArgs e)
        {
            if (_downItem == e.Item)
            {
                if (_downItemChecked == DefaultBoolean.True && e.Item.Checked)
                    e.Item.Checked = false;
            }
            _downItem = null;
            _downItemChecked = DefaultBoolean.Default;
        }

        private void Gallery_ItemCheckedChanged(object sender, DevExpress.XtraBars.Ribbon.GalleryItemEventArgs e)
        {
            if (_downItem == null)
                return;
            if (!e.Item.Checked)
            {
                if (_downItem != e.Item)
                {
                    e.Item.Checked = true;
                }
            }
        }

        private void Gallery_MouseDown(object sender, MouseEventArgs e)
        {
            DevExpress.XtraBars.Ribbon.ViewInfo.RibbonHitInfo hi = galleryControl1.CalcHitInfo(galleryControl1.PointToClient(Control.MousePosition));
            if (hi.InGalleryItem)
            {
                _downItem = hi.GalleryItem;
                _downItemChecked = hi.GalleryItem.Checked ? DefaultBoolean.True : DefaultBoolean.False;
            }
        }
    }
}
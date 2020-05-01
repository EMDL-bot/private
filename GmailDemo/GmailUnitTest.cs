using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium;
using System.Threading.Tasks;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using System.IO;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.Service.Options;
using System.Collections;
using System.Linq;
using System.Timers;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Appium.MultiTouch;
using System.Drawing;
using OpenQA.Selenium.Appium.Interfaces;

namespace GmailDemo
{
    [TestClass]
    public class GmailUnitTest : IDisposable
    {
        //private AppiumDriver<AndroidElement> driver;
        private AndroidDriver<AndroidElement> driver;
        public string device { get; set; }
        public string actionLogPath { get; set; }
        public string errorLogPath { get; set; }
        public string blockedLogPath { get; set; }
        public string disabledLogPath { get; set; }
        public string directory { get; set; }
        public string index { get; set; }
        public List<string> error;
        public CancellationToken cancelationToken;
        public int testRetries { get; set; }
        public int actionRetries { get; set; }
        public bool Disposed { get; private set; }
        public string scenario { get; set; }

        private string ServerURI;
        public int ServerPort { get { return ServerPort; } set { this.ServerURI = "http://127.0.0.1:" + value + "/wd/hub"; } }
        public int TestDriverTO { get; set; }
        public int WaitTO { get; set; }
        public int WaitPolling { get; set; }


        private string InstallGmailApp(string path)
        {
            try
            {
                //if (!driver.IsAppInstalled(""))
                driver.InstallApp(path);
                return "done";
            }
            catch (Exception e)
            {
                return "error, " + e.Message;
            }
        }

        public Task InitAppiumServer(int port = 100)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var process = Process.Start(@"appium.bat");
                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.ErrorDataReceived += Process_ErrorDataReceived;
                    //Thread.Sleep(5000);
                }
                catch (Exception x)
                {
                    Log(x.Message, "error");
                }
            });
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log(e.Data, "error");
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log(e.Data, "info");
        }

        public void KillDriver()
        {
            if (driver != null)
                driver.Quit();
        }

        public Task StopProcess()
        {
            return Task.Factory.StartNew(() =>
            {
                KillDriver();
                this.Dispose();
            });
        }

        private Task<List<string>> LoadDevices()
        {
            try
            {
                var devices = new List<string>();
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
                        devices.Add(line);
                    }

                    process.WaitForExit();
                    return devices;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private Task ReloadDevice()
        {
            try
            {
                var devices = new List<string>();
                return Task.Factory.StartNew(() =>
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = "/c adb disconnect " + device + " && adb connect " + device,
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

        private Task<bool> isOnline()
        {
            return LoadDevices().ContinueWith((list) =>
            {
                if (list.IsCompleted)
                {
                    var devices = list.Result;
                    return devices.Contains(device + " device");
                }
                return false;
            });
        }

        private string InitDriver(int timeout = 90, bool isGmail = true)
        {
            cancelationToken.ThrowIfCancellationRequested();
            try
            {
                AppiumOptions options = new AppiumOptions();
                options.PlatformName = "Android";
                options.AddAdditionalCapability("deviceName", this.device);
                options.AddAdditionalCapability("udid", this.device);
                options.AddAdditionalCapability("systemPort", new Random().Next(21000, 26000));
                options.AddAdditionalCapability("platformName", "android");
                options.AddAdditionalCapability("automationName", "UiAutomator2");
                if (isGmail)
                {
                    options.AddAdditionalCapability("appPackage", "com.google.android.gm");
                    options.AddAdditionalCapability("appActivity", ".ConversationListActivityGmail");
                }
                options.AddAdditionalCapability("noReset", true);
                options.AddAdditionalCapability("printPageSourceOnFindFailure", false);

                driver = new AndroidDriver<AndroidElement>(new Uri(this.ServerURI), options);
                //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);

                Log("init driver", "success");
                return "done";
            }
            catch (Exception x)
            {
                Log(x.Message, "error");
                return "";
            }
        }

        public Task<string> TestAppiumServer()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    /*Environment.SetEnvironmentVariable(AppiumServiceConstants.NodeBinaryPath, @"C:\Program Files\nodejs\node.exe");
                    Environment.SetEnvironmentVariable(AppiumServiceConstants.AppiumBinaryPath, @"C:\Users\Mehdi\AppData\Roaming\npm\node_modules\appium\build\lib\appium.js");
                    Environment.SetEnvironmentVariable("JAVA_HOME", @"C:\Program Files\Java\jdk-13.0.2");
                    Environment.SetEnvironmentVariable("ANDROID_HOME", @"C:\Users\Mehdi\AppData\Local\Android\Sdk");
                    AppiumLocalService _appiumLocalService = new AppiumServiceBuilder().UsingPort(100).Build();*/

                    AppiumOptions options = new AppiumOptions();
                    options.PlatformName = "Android";
                    options.AddAdditionalCapability("deviceName", this.device);
                    options.AddAdditionalCapability("udid", this.device);
                    options.AddAdditionalCapability("systemPort", new Random().Next(21000, 26000));
                    options.AddAdditionalCapability("platformName", "android");
                    options.AddAdditionalCapability("automationName", "UiAutomator2");
                    options.AddAdditionalCapability("appPackage", "com.android.settings");
                    options.AddAdditionalCapability("appActivity", ".Settings");
                    options.AddAdditionalCapability("noReset", true);
                    options.AddAdditionalCapability("noSign", true);
                    options.AddAdditionalCapability("printPageSourceOnFindFailure", false);

                    driver = new AndroidDriver<AndroidElement>(new Uri(this.ServerURI), options);
                    return "success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                /*AppiumOptions options = new AppiumOptions();
                options.PlatformName = "Android";
                options.AddAdditionalCapability("deviceName", this.device);
                options.AddAdditionalCapability("udid", this.device);
                options.AddAdditionalCapability("systemPort", new Random().Next(21000, 26000));
                options.AddAdditionalCapability("platformName", "android");
                options.AddAdditionalCapability("automationName", "UiAutomator2");
                options.AddAdditionalCapability("noReset", true);
                options.AddAdditionalCapability("printPageSourceOnFindFailure", false);

                driver = new AndroidDriver<AndroidElement>(new Uri(this.ServerURI), options);*/
            });
        }

        public Task<bool> TestDriver()
        {
            return Task.Factory.StartNew(() =>
            {
                bool result = false;

                /*var timer = new System.Timers.Timer();
                timer.Elapsed += new ElapsedEventHandler(myEvent);
                timer.Interval = 60000;
                timer.Enabled = true;*/

                var sw = new Stopwatch();

                if (cancelationToken.IsCancellationRequested)
                {
                    Log("test driver: operation canceled!", "info");
                    return false;
                }

                sw.Start();

                AppiumOptions options = new AppiumOptions();
                options.PlatformName = "Android";
                options.AddAdditionalCapability("deviceName", this.device);
                options.AddAdditionalCapability("udid", this.device);
                options.AddAdditionalCapability("systemPort", new Random().Next(30000, 40000));
                options.AddAdditionalCapability("platformName", "android");
                options.AddAdditionalCapability("automationName", "UiAutomator2");
                ////options.AddAdditionalCapability("uiautomator2ServerInstallTimeout", 60000);
                ////options.AddAdditionalCapability("uiautomator2ServerLaunchTimeout", 60000);
                options.AddAdditionalCapability("noReset", true);

                while (true)
                {
                    if (result || sw.ElapsedMilliseconds >= (this.TestDriverTO * 1000))
                        break;
                    else
                    {
                        try
                        {
                            driver = new AndroidDriver<AndroidElement>(new Uri(this.ServerURI), options);
                            result = driver.SessionDetails.Count > 0;
                            Console.WriteLine(index + " : SessionDetails = " + driver.SessionDetails.Count);
                        }
                        catch (Exception c)
                        {
                            Console.WriteLine(index + " : " + c.Message);
                            result = false;
                        }
                        Thread.Sleep(this.WaitPolling);
                    }
                }
                sw.Stop();
                if (driver != null)
                    driver.Quit();
                return result;
            });
        }

        private string ProxySetup(string host, int port = 92)
        {
            if (this.actionRetries == 0)
            {
                Log("max actions retries reached!", "error");
                BlockedLog("max actions retries reached!");
                return "max actions retries reached!";
            }

            cancelationToken.ThrowIfCancellationRequested();
            try
            {
                bool isFail = false;
                cancelationToken.ThrowIfCancellationRequested();
                AppiumOptions options = new AppiumOptions();
                options.PlatformName = "Android";
                options.AddAdditionalCapability("deviceName", this.device);
                options.AddAdditionalCapability("udid", this.device);
                var systemPort = Convert.ToInt32(this.device.Split(':')[1]);
                options.AddAdditionalCapability("systemPort", new Random().Next(30000, 40000));
                options.AddAdditionalCapability("platformName", "android");
                options.AddAdditionalCapability("automationName", "UiAutomator2");
                options.AddAdditionalCapability("appPackage", "com.android.settings");
                options.AddAdditionalCapability("appActivity", ".Settings");
                options.AddAdditionalCapability("ignoreUnimportantViews", true);
                options.AddAdditionalCapability("noReset", true);

                /*KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>("--log-level", "error");
                var args = new OptionCollector().AddArguments(keyValuePair);

                var appium = new AppiumServiceBuilder();
                //appium.WithLogFile(new FileInfo(appiumLogPath));
                var service = appium.UsingPort(10102).WithArguments(args).Build(); //new Uri(this.ServerURI)

                service.Start();

                while (!service.IsRunning)
                {
                    Console.WriteLine("waiting..");
                }*/

                driver = new AndroidDriver<AndroidElement>(new Uri(this.ServerURI), options);
                //driver.IgnoreUnimportantViews(true);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(this.WaitTO));
                wait.PollingInterval = TimeSpan.FromMilliseconds(this.WaitPolling);
                wait.Message = "PROXY WAIT TIMEDOUT!";

                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.LinearLayout[@resource-id='com.android.settings:id/dashboard_tile']")));
                    //Thread.Sleep(3000);
                    var net = driver.FindElementsByXPath("//android.widget.LinearLayout[@resource-id='com.android.settings:id/dashboard_tile']");
                    net[0].Click();
                    Log("click networks button", "info");
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("PROXY WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                    {
                        this.actionRetries--;
                        ProxySetup(host, port);
                    }
                    else
                    {
                        Log("network button : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.LinearLayout[contains(@content-desc,'Connected')]")));
                    //Thread.Sleep(3000);
                    var wifi = driver.FindElementsByXPath("//android.widget.LinearLayout[contains(@content-desc,'Connected')]");
                    new Actions(driver).ClickAndHold(wifi[0]).Perform();
                    Log("long press wifi button", "info");
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("PROXY WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                    {
                        this.actionRetries--;
                        ProxySetup(host, port);
                    }
                    else
                    {
                        Log("wifi button : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.TextView[@resource-id='android:id/title'][@text='Modify network']")));
                    //Thread.Sleep(3000);
                    var edit = driver.FindElementsByXPath("//android.widget.TextView[@resource-id='android:id/title'][@text='Modify network']");
                    edit[0].Click();
                    Log("click modify network button", "info");
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("PROXY WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                    {
                        this.actionRetries--;
                        ProxySetup(host, port);
                    }
                    else
                    {
                        Log("modify network button : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                bool allReadySet = false;

                if (!allReadySet)
                {
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        var adv = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("com.android.settings:id/wifi_advanced_togglebox")));
                        //Thread.Sleep(3000);
                        //ad.
                        //var adv = driver.FindElementById("com.android.settings:id/wifi_advanced_togglebox");
                        if (adv.GetAttribute("checked") == "false")
                        {
                            adv.Click();
                            Log("click toggle options box", "info");
                        }
                        else
                        {
                            goto entry;
                        }
                    }
                    catch (Exception x)
                    {
                        if (x.Message.Contains("PROXY WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                        {
                            this.actionRetries--;
                            ProxySetup(host, port);
                        }
                        else
                        {
                            Log("toggle options box : " + x.Message, "error");
                            isFail = true;
                            goto done;
                        }
                    }
                }

                if (!allReadySet)
                {
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("com.android.settings:id/proxy_settings")));
                        //Thread.Sleep(3000);
                        var proxy = driver.FindElementById("com.android.settings:id/proxy_settings");
                        proxy.Click();
                        Log("click proxy settings button", "info");
                    }
                    catch (Exception x)
                    {
                        if (x.Message.Contains("PROXY WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                        {
                            this.actionRetries--;
                            ProxySetup(host, port);
                        }
                        else
                        {
                            Log("proxy settings button : " + x.Message, "error");
                            isFail = true;
                            goto done;
                        }
                    }
                }

                if (!allReadySet)
                {
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.CheckedTextView[@resource-id='android:id/text1'][@text='Manual']")));
                        //Thread.Sleep(3000);
                        var type = driver.FindElementsByXPath("//android.widget.CheckedTextView[@resource-id='android:id/text1'][@text='Manual']");
                        type[0].Click();
                        Log("click manual option button", "info");
                    }
                    catch (Exception x)
                    {
                        if (x.Message.Contains("PROXY WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                        {
                            this.actionRetries--;
                            ProxySetup(host, port);
                        }
                        else
                        {
                            Log("manual option button : " + x.Message, "error");
                            isFail = true;
                            goto done;
                        }
                    }
                }
                entry:
                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("com.android.settings:id/proxy_hostname")));
                    //Thread.Sleep(3000);
                    var proxyHost = driver.FindElementById("com.android.settings:id/proxy_hostname");
                    proxyHost.SendKeys(host);
                    Log("sendkeys proxy hostname : " + host, "info");
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("PROXY WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                    {
                        this.actionRetries--;
                        ProxySetup(host, port);
                    }
                    else
                    {
                        Log("sendkeys proxy hostname : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("com.android.settings:id/proxy_port")));
                    //Thread.Sleep(3000);
                    var proxyPort = driver.FindElementById("com.android.settings:id/proxy_port");
                    proxyPort.SendKeys(port.ToString());
                    Log("sendkeys proxy port : " + port, "info");
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("PROXY WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                    {
                        this.actionRetries--;
                        ProxySetup(host, port);
                    }
                    else
                    {
                        Log("sendkeys proxy port : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("android:id/button1")));
                    //Thread.Sleep(3000);
                    var save = driver.FindElementById("android:id/button1");
                    save.Click();
                    Log("click save button", "info");
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("PROXY WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                    {
                        this.actionRetries--;
                        ProxySetup(host, port);
                    }
                    else
                    {
                        Log("save button : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                done:
                driver.Quit();
                return isFail ? "setup proxy failed!" : "setup proxy successed!";
            }
            catch (Exception e)
            {
                driver.Quit();
                Log(e.Message, "error");
                return e.Message;
            }
        }

        public string GmailLogin(string[] creds)
        {
            if (this.actionRetries == 0)
            {
                Log("max actions retries reached!", "error");
                BlockedLog("max actions retries reached!");
                return "max actions retries reached!";
            }

            bool isFail = false;
            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                AppiumOptions options = new AppiumOptions();
                options.PlatformName = "Android";
                options.AddAdditionalCapability("deviceName", this.device);
                options.AddAdditionalCapability("udid", this.device);
                var systemPort = Convert.ToInt32(this.device.Split(':')[1]);
                options.AddAdditionalCapability("systemPort", new Random().Next(30000, 40000));
                options.AddAdditionalCapability("platformName", "android");
                options.AddAdditionalCapability("automationName", "UiAutomator2");
                options.AddAdditionalCapability("appPackage", "com.android.settings");
                options.AddAdditionalCapability("appActivity", ".Settings$AccountSettingsActivity");
                options.AddAdditionalCapability("ignoreUnimportantViews", true);
                options.AddAdditionalCapability("noReset", true);

                driver = new AndroidDriver<AndroidElement>(new Uri(this.ServerURI), options);
            }
            catch (Exception x)
            {
                return "driver error, " + x.Message;
            }

            //driver.IgnoreUnimportantViews(true);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(this.WaitTO)); //new WebDriverWait(driver, TimeSpan.FromSeconds(this.WaitTO));
            wait.PollingInterval = TimeSpan.FromMilliseconds(this.WaitPolling);
            wait.Message = "SIGN-IN WAIT TIMEDOUT!";


            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.XPath("//android.widget.TextView[@resource-id='android:id/title']")));
                var account = driver.FindElementsByXPath("//android.widget.TextView[@resource-id='android:id/title']");
                if (account.Count > 0)
                {
                    if (account[0].Text == "Google")
                        goto tour;
                    else
                    {
                        account[0].Click();
                        Log("click add account button", "info");
                    }
                }

            }
            catch (Exception x)
            {
                if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                {
                    this.actionRetries--;
                    return GmailLogin(creds);
                }
                else
                {
                    Log("add account button : " + x.Message, "error");
                    isFail = true;
                    goto done;
                }
            }

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.TextView[@resource-id='android:id/title'][@text='Google']")));
                var google = driver.FindElementsByXPath("//android.widget.TextView[@resource-id='android:id/title'][@text='Google']");
                if (google.Count > 0)
                {
                    google[0].Click();
                    Log("click google button", "info");
                }

            }
            catch (Exception x)
            {
                if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                {
                    this.actionRetries--;
                    return GmailLogin(creds);
                }
                else
                {
                    Log("google button : " + x.Message, "error");
                    isFail = true;
                    goto done;
                }
            }

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.EditText[@resource-id='identifierId']")));
                var id = driver.FindElementsByXPath("//android.widget.EditText[@resource-id='identifierId']");
                id[0].SendKeys(creds[0]);
                Log("sendkeys identifier id : " + creds[0], "info");
            }
            catch (Exception x)
            {
                if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                {
                    this.actionRetries--;
                    return GmailLogin(creds);
                }
                else
                {
                    Log("identifier id : " + x.Message, "error");
                    isFail = true;
                    goto done;
                }
            }

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.Button[@resource-id='identifierNext']")));
                var next = driver.FindElementsByXPath("//android.widget.Button[@resource-id='identifierNext']");
                next[0].Click();
                Log("click next button", "info");
            }
            catch (Exception x)
            {
                if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                {
                    this.actionRetries--;
                    return GmailLogin(creds);
                }
                else
                {
                    Log("next button : " + x.Message, "error");
                    isFail = true;
                    goto done;
                }
            }

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.view.View[@resource-id='password']//android.widget.EditText")));
                var pw = driver.FindElementsByXPath("//android.view.View[@resource-id='password']//android.widget.EditText");
                pw[0].SendKeys(creds[1]);
                Log("sendkeys password : *******", "info");
            }
            catch (Exception x)
            {
                if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                {
                    this.actionRetries--;
                    return GmailLogin(creds);
                }
                else
                {
                    Log("sendkeys password : " + x.Message, "error");
                    isFail = true;
                    goto done;
                }
            }

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.Button[@resource-id='passwordNext']")));
                var next = driver.FindElementsByXPath("//android.widget.Button[@resource-id='passwordNext']");
                next[0].Click();
                Log("click password next button", "info");
            }
            catch (Exception x)
            {
                if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                {
                    this.actionRetries--;
                    return GmailLogin(creds);
                }
                else
                {
                    Log("password next button : " + x.Message, "error");
                    isFail = true;
                    goto done;
                }
            }

            //try
            //{
            //    cancelationToken.ThrowIfCancellationRequested();
            //    var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            //    var errorPassword = subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.view.View[@resource-id='password']//android.widget.EditText")));
            //    if (errorPassword.Displayed)
            //    {
            //        Log(creds[0] + " - wrong password!", "warning");
            //        DisabledLog(creds[0] + " - wrong password!");
            //        goto done;
            //    }
            //}
            //catch (Exception x)
            //{
            //    Console.WriteLine("Wrong password check : " + x.Message);
            //}

            bool recoveryRequired = false;

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//android.view.View[@resource-id='headingText']")));
                var header = driver.FindElementsByXPath("//android.view.View[@resource-id='headingText']");
                if (header[0].Displayed)
                {
                    if (header[0].Text == "Account disabled")
                    {
                        Log(creds[0] + " - account disabled!", "warning");
                        DisabledLog(creds[0] + " - account disabled!");
                        goto done;
                    }

                    recoveryRequired = !(header[0].Text.Contains("Welcome") || header[0].Text.Contains("Hi "));
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }

            if (recoveryRequired)
            {
                var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.view.View[@text = 'Confirm your recovery email']")));
                    var recovery = driver.FindElementsByXPath("//android.view.View[@text = 'Confirm your recovery email']");
                    recovery[0].Click();
                    Log("click recovery button", "info");
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                    {
                        this.actionRetries--;
                        return GmailLogin(creds);
                    }
                    else
                    {
                        Log("recovery button : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.EditText[@resource-id='knowledge-preregistered-email-response']")));
                    var recoveryArea = driver.FindElementsByXPath("//android.widget.EditText[@resource-id='knowledge-preregistered-email-response']");
                    recoveryArea[0].SendKeys(creds[2]);
                    Log("sendkeys recovery : " + creds[2], "info");
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                    {
                        this.actionRetries--;
                        return GmailLogin(creds);
                    }
                    else
                    {
                        Log("sendkeys recovery : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.Button[@text='Next']")));
                    var next = driver.FindElementsByXPath("//android.widget.Button[@text='Next']");
                    next[0].Click();
                    Log("click recovery next button : " + creds[2], "info");
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                    {
                        this.actionRetries--;
                        return GmailLogin(creds);
                    }
                    else
                    {
                        Log("recovery next button : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                //try
                //{
                //    cancelationToken.ThrowIfCancellationRequested();
                //    subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                //    var errorRecovery = subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.EditText[@resource-id='knowledge-preregistered-email-response']")));
                //    if (errorRecovery.Displayed)
                //    {
                //        Log(creds[2] + " - wrong recovery email!", "warning");
                //        DisabledLog(creds[2] + " - wrong recovery email!");
                //        goto done;
                //    }
                //}
                //catch (Exception x)
                //{
                //    Console.WriteLine("Wrong recovery check : " + x.Message);
                //}
            }

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//android.view.View[@resource-id='headingText']")));
                var header = driver.FindElementsByXPath("//android.view.View[@resource-id='headingText']");
                if (header[0].Displayed)
                {
                    if (header[0].Text == "Account disabled")
                    {
                        Log(creds[0] + " - account disabled!", "warning");
                        DisabledLog(creds[0] + " - account disabled!");
                        goto done;
                    }
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.EditText[@resource-id='idvreenablePhoneNumberId']")));
                var pv = driver.FindElementsByXPath("//android.widget.EditText[@resource-id='idvreenablePhoneNumberId']");
                pv[0].Clear();
                Log(creds[0] + " - Phone Verification Required!", "warning");
                BlockedLog(creds[0] + " - Phone Verification Required!");
                goto done;
            }
            catch (Exception e)
            {
                Console.WriteLine("Phone verification : " + e.Message);
            }

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.Button[@resource-id='signinconsentNext']")));
                var consent = driver.FindElementsByXPath("//android.widget.Button[@resource-id='signinconsentNext'][@text='I agree']");
                consent[0].Click();
                Log("click signin consent next button", "info");
            }
            catch (Exception x)
            {
                if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                {
                    this.actionRetries--;
                    return GmailLogin(creds);
                }
                else
                {
                    Log("signin consent next button : " + x.Message, "error");
                    isFail = true;
                    goto done;
                }
            }

            var scroll = new TouchAction(driver);

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                var subwait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                var frame = subwait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//android.widget.FrameLayout[@resource-id='com.google.android.gms:id/minute_maid']")));
                if (frame.Count > 0)
                {
                    try
                    {
                        var size = driver.Manage().Window.Size;

                        int anchor = (int)(size.Width * 0.5);
                        int startPoint = (int)(size.Height * 0.6);
                        int endPoint = (int)(size.Height * 0.1);

                        scroll.Press(anchor, startPoint).Wait(2000)
                        .MoveTo(anchor, endPoint).Release().Perform();
                        //scroll.Press(anchor, startPoint).Wait(150)
                        //.MoveTo(anchor, endPoint).Release().Perform();
                        scroll.Cancel();
                        Console.WriteLine("end scrolling..");
                    }
                    catch (Exception c)
                    {
                        Console.WriteLine("scroll : " + c.Message);
                    }

                    subwait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.Button[@resource-id='signinconsentNext']")));
                    var consent = driver.FindElementsByXPath("//android.widget.Button[@resource-id='signinconsentNext'][@text='I agree']");
                    consent[0].Click();
                    Log("click signin consent next button 2", "info");
                    //try
                    //{
                    //    cancelationToken.ThrowIfCancellationRequested();
                    //    var consent = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.Button[@resource-id='signinconsentNext'][@text='I agree'][@clickable='true']")));
                    //    var coord = consent.Location;

                    //    Console.WriteLine("===== button INFO : ======");
                    //    Console.WriteLine("button text : " + consent.Text);
                    //    Console.WriteLine("button size : " + consent.Size);
                    //    Console.WriteLine("button selected : " + consent.Selected);
                    //    Console.WriteLine("button displayed : " + consent.Displayed);
                    //    Console.WriteLine("button enabled : " + consent.Enabled);
                    //    Console.WriteLine("button Location : " + coord);
                    //    Console.WriteLine("===== end button INFO : ======");

                    //    consent.Click();
                    //    new TouchAction(driver).Tap(coord.X, coord.Y, 5).Perform();
                    //    Log("tap signin consent button", "info");
                    //}
                    //catch (Exception x)
                    //{
                    //    Console.WriteLine("Consent button 2 : " + x.Message);
                    //}
                }
            }
            catch (Exception x)
            {
                Console.WriteLine("new account features : " + x.Message);
            }

            scroll.Cancel();

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementLocated(By.XPath("//android.widget.Button"), "MORE"));
                var more = driver.FindElementsByXPath("//android.widget.Button[@text='MORE']");
                more[0].Click();
                Log("click more button", "info");
            }
            catch (Exception x)
            {
                Console.WriteLine("More button : " + x.Message);
            }

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElementLocated(By.XPath("//android.widget.Button"), "ACCEPT"));
                var accept = driver.FindElementsByXPath("//android.widget.Button[@text='ACCEPT']");
                accept[0].Click();
                Log("click accept button", "info");
            }
            catch (Exception x)
            {
                if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                {
                    this.actionRetries--;
                    return GmailLogin(creds);
                }
                else
                {
                    Log("accept button : " + x.Message, "error");
                    isFail = true;
                    goto done;
                }
            }


            scroll.Cancel();
            tour:
            Task.Delay(1000).Wait();
            driver.CloseApp();
            driver.ActivateApp("com.google.android.gm");

            var _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                _wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("com.google.android.gm:id/conversation_list_folder_header")));
                var header = driver.FindElementById("com.google.android.gm:id/conversation_list_folder_header");
                if (header != null)
                {
                    goto done;
                }
            }
            catch (Exception x)
            {
                Console.WriteLine("check gmail is open : " + x.Message);
            }

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("com.google.android.gm:id/welcome_tour_got_it")));
                var welcome = driver.FindElementById("com.google.android.gm:id/welcome_tour_got_it");
                welcome.Click();
                Log("click welcome button", "info");
            }
            catch (Exception x)
            {
                if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                {
                    this.actionRetries--;
                    return GmailLogin(creds);
                }
                else
                {
                    Log("welcome button : " + x.Message, "error");
                    isFail = true;
                    goto done;
                }
            }

            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("com.google.android.gm:id/owner")));
                var done = driver.FindElementById("com.google.android.gm:id/action_done");
                done.Click();
                Log("click done button", "info");
            }
            catch (Exception x)
            {
                if (x.Message.Contains("SIGN-IN WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                {
                    this.actionRetries--;
                    return GmailLogin(creds);
                }
                else
                {
                    Log("done button : " + x.Message, "error");
                    isFail = true;
                    goto done;
                }
            }

            done:
            driver.Quit();
            return isFail ? "sign-in failed!" : "signin successed!";

        }

        private void Sleep(int seconds = 3, bool random = false)
        {
            var sec = random ? new Random().Next(1, 20) : seconds;
            Log($"waiting {seconds} seconds..", "info");
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }

        public string SpamActions(string keyword, string date)
        {
            if (this.actionRetries == 0)
            {
                Log("max spam actions retries reached!", "error");
                BlockedLog("max spam actions retries reached!");
                return "max spam actions retries reached!";
            }

            bool isFail = false;
            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                AppiumOptions options = new AppiumOptions();
                options.PlatformName = "Android";
                options.AddAdditionalCapability("deviceName", this.device);
                options.AddAdditionalCapability("udid", this.device);
                var systemPort = Convert.ToInt32(this.device.Split(':')[1]);
                options.AddAdditionalCapability("systemPort", new Random().Next(30000, 40000));
                options.AddAdditionalCapability("platformName", "android");
                options.AddAdditionalCapability("automationName", "UiAutomator2");
                options.AddAdditionalCapability("appPackage", "com.google.android.gm");
                options.AddAdditionalCapability("appActivity", ".ConversationListActivityGmail");
                options.AddAdditionalCapability("ignoreUnimportantViews", true);
                options.AddAdditionalCapability("noReset", true);

                driver = new AndroidDriver<AndroidElement>(new Uri(this.ServerURI), options);
            }
            catch (Exception x)
            {
                return "driver error, " + x.Message;
            }

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(this.WaitTO));
            wait.PollingInterval = TimeSpan.FromMilliseconds(this.WaitPolling);
            wait.Message = "SPAM ACTIONS WAIT TIMEDOUT!";
            keyword = keyword.Replace("\"", "");

            if (driver != null)
            {
                var i = 1;
                process:
                //var now = date != "" ? DateTime.Parse(date).ToString("yyyy/MM/dd") : DateTime.UtcNow.ToString("yyyy/MM/dd");

                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("com.google.android.gm:id/conversation_list_folder_header")));
                    var header = driver.FindElementById("com.google.android.gm:id/conversation_list_folder_header");
                    if (header != null)
                    {
                        header.Click();
                        Log("click search header", "info");
                    }
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("SPAM ACTIONS WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                    {
                        this.actionRetries--;
                        return SpamActions(keyword, date);
                    }
                    else
                    {
                        Log("click search header : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("com.google.android.gm:id/open_search_view_edit_text")));
                    var searchArea = driver.FindElementById("com.google.android.gm:id/open_search_view_edit_text");
                    if (searchArea != null)
                    {
                        var keys = $"in:spam from:{keyword.ToLower()}" + (date != "" ? " " + date : "");
                        searchArea.ClearCache();
                        searchArea.SendKeys(keys);
                        searchArea.ClearCache();
                        searchArea.Click();
                        new Actions(driver).SendKeys(Keys.Enter).Perform();
                        Log("send keys : " + keys, "info");
                    }
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("SPAM ACTIONS WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                    {
                        this.actionRetries--;
                        return SpamActions(keyword, date);
                    }
                    else
                    {
                        Log("search area : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                try
                {
                    var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    cancelationToken.ThrowIfCancellationRequested();
                    var empty = subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("com.google.android.gm:id/empty_text")));
                    //var empty = driver.FindElementById("com.google.android.gm:id/empty_text");
                    if (empty.Count > 0)
                    {
                        if (empty[0].Text != "")
                        {
                            Log("no emails Founds in SPAM folder!", "warning");
                            goto done;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write("Empty text : " + e.Message);
                }

                //OPEN EMAIL
                ReadOnlyCollection<AndroidElement> emails = null;
                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("com.google.android.gm:id/viewified_conversation_item_view")));
                    emails = driver.FindElementsById("com.google.android.gm:id/viewified_conversation_item_view");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Conversation list : " + e.Message);
                }

                if (emails != null && emails.Count > 0)
                {
                    var emailsCount = emails.Count;
                    Log("Displayed Emails: " + emailsCount, "info");


                    while (!isFail)
                    {
                        ReadOnlyCollection<AndroidElement> emailTime = null;

                        try
                        {
                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//android.view.ViewGroup[@resource-id='com.google.android.gm:id/viewified_conversation_item_view']/android.widget.TextView[@resource-id='com.google.android.gm:id/date']")));
                            emailTime = driver.FindElementsByXPath("//android.view.ViewGroup[@resource-id='com.google.android.gm:id/viewified_conversation_item_view']/android.widget.TextView[@resource-id='com.google.android.gm:id/date']");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Email Time : " + e.Message);
                        }

                        if (emailTime != null && emailTime.Count > 0)
                        {
                            Log("Working on Email No: " + i, "info");
                            //READ EMAIL AND SCROLL

                            try
                            {
                                cancelationToken.ThrowIfCancellationRequested();
                                var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                                var webView = subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//android.view.View[contains(@resource-id, 'm#msg-f:')]")));

                                if (webView.Displayed)
                                {

                                    //var coord = webView.Location;
                                    //var dim = webView.Size;
                                    //var REC = new Rectangle(coord, dim);
                                    //
                                    //var scrollDOWN = new TouchAction(driver)
                                    //                   .Press(REC.X, REC.Y)
                                    //                   .Wait(1500)
                                    //                   .MoveTo(REC.X, 0)
                                    //                   .Release();
                                    //
                                    //scrollDOWN.Perform();
                                    //Console.WriteLine("end scroll down.");
                                    //var scrollUP = new TouchAction(driver)
                                    //                   .Press(REC.X, 0)
                                    //                   .Wait(1500)
                                    //                   .MoveTo(REC.X, REC.Y)
                                    //                   .Release();
                                    //
                                    //scrollUP.Perform();
                                    //Console.WriteLine("end up down.");
                                    //scroll.Press(anchor, startPoint).Wait(1500)
                                    //.MoveTo(anchor, endPoint).Release().Perform();
                                    //
                                    //scroll.Press(anchor, startPoint).Wait(150)
                                    //.MoveTo(anchor, endPoint).Release().Perform();
                                    //
                                    //Console.WriteLine("end scrolling..");
                                    //scroll.Cancel();

                                }
                            }
                            catch (Exception c)
                            {
                                Log("Scrolling : " + c.Message, "warning");
                            }

                            //GET CURRENT SUBJECT
                            AndroidElement senderName = null;

                            try
                            {
                                cancelationToken.ThrowIfCancellationRequested();
                                var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(7));
                                subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("com.google.android.gm:id/sender_name")));
                                senderName = driver.FindElementById("com.google.android.gm:id/sender_name");
                            }
                            catch (Exception e)
                            {
                                Log("Sender Name : " + e.Message, "error");
                            }

                            try
                            {
                                if (senderName != null)
                                {
                                    var sender = senderName.Text.ToLower();
                                    Log("Sender Name: " + sender, "info");
                                    if (sender.Contains(keyword.ToLower()))
                                    {
                                        //MARK AS NOT SPAM
                                        ReadOnlyCollection<AndroidElement> notJunkButton = null;

                                        try
                                        {
                                            cancelationToken.ThrowIfCancellationRequested();
                                            //var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.Button[@resource-id = 'com.google.android.gm:id/warning_banner_primary_button'][@text = 'Report not spam']")));
                                            notJunkButton = driver.FindElementsByXPath("//android.widget.Button[@resource-id = 'com.google.android.gm:id/warning_banner_primary_button'][@text = 'Report not spam']");
                                            Log("not junk count : " + notJunkButton.Count, "");
                                        }
                                        catch (Exception c)
                                        {
                                            Console.WriteLine("Not junk Button : " + c.Message);
                                        }

                                        try
                                        {
                                            if (notJunkButton == null)
                                            {
                                                senderName.Click();
                                                notJunkButton = driver.FindElementsByXPath("//android.widget.Button[@resource-id = 'com.google.android.gm:id/warning_banner_primary_button'][@text = 'Report not spam']");
                                            }
                                            notJunkButton[0].Click();
                                            Log("Email marked as not junk", "info");
                                        }
                                        catch (Exception c)
                                        {
                                            Log("mark as not junk : " + c.Message, "error");
                                            isFail = true;
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            cancelationToken.ThrowIfCancellationRequested();
                                            var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                                            subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("(//*[@content-desc='Delete'])[1]")));

                                            var deleteButton = driver.FindElementsByXPath("(//*[@content-desc='Delete'])[1]");
                                            deleteButton[0].Click();
                                            Log("Email Deleted!", "info");
                                        }
                                        catch (Exception c)
                                        {
                                            Console.WriteLine("Delete Button : " + c.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    Log("sender name has NULL", "warning");
                                }
                            }
                            catch (Exception e)
                            {
                                Log(e.Message, "error");
                                break;
                            }
                        }
                        else
                        {
                            Log("No Emails Found!", "info");
                            break;
                        }
                        i++;

                        try
                        {
                            driver.PressKeyCode(AndroidKeyCode.Back);
                        }
                        catch (Exception c)
                        {
                            Console.WriteLine("error, Navigate back : " + c.Message);
                        }

                        goto process;
                    }
                }
            }
            else
            {
                Console.WriteLine("Error while initializing driver!");
            }

            done:
            driver.Quit();
            return isFail ? "spam actions failed!" : "spam actions successed!";
        }

        public string InboxActions(string keyword, string date)
        {
            bool isFail = false;
            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                AppiumOptions options = new AppiumOptions();
                options.PlatformName = "Android";
                options.AddAdditionalCapability("deviceName", this.device);
                options.AddAdditionalCapability("udid", this.device);
                var systemPort = Convert.ToInt32(this.device.Split(':')[1]);
                options.AddAdditionalCapability("systemPort", new Random().Next(30000, 40000));
                options.AddAdditionalCapability("platformName", "android");
                options.AddAdditionalCapability("automationName", "UiAutomator2");

                //options.AddAdditionalCapability("uiautomator2ServerInstallTimeout", 60000);
                //options.AddAdditionalCapability("uiautomator2ServerLaunchTimeout", 60000);

                options.AddAdditionalCapability("appPackage", "com.google.android.gm");
                options.AddAdditionalCapability("appActivity", ".ConversationListActivityGmail");
                options.AddAdditionalCapability("noReset", true);
                options.AddAdditionalCapability("printPageSourceOnFindFailure", false);

                driver = new AndroidDriver<AndroidElement>(new Uri(this.ServerURI), options);
            }
            catch (Exception x)
            {
                return "driver error, " + x.Message;
            }

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(this.WaitTO));
            wait.PollingInterval = TimeSpan.FromMilliseconds(this.WaitPolling);
            wait.Message = "INBOX ACTIONS WAIT TIMEDOUT!";
            keyword = keyword.Replace("\"", "");

            if (driver != null)
            {
                process:
                var now = DateTime.UtcNow.ToString("yyyy/MM/dd");

                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("com.google.android.gm:id/conversation_list_folder_header")));
                    var header = driver.FindElementById("com.google.android.gm:id/conversation_list_folder_header");
                    if (header != null)
                    {
                        header.Click();
                        Log("click search header", "info");
                    }
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("INBOX ACTIONS WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                        return InboxActions(keyword, date);
                    else
                    {
                        Log("click search header : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("com.google.android.gm:id/open_search_view_edit_text")));
                    var searchArea = driver.FindElementById("com.google.android.gm:id/open_search_view_edit_text");
                    if (searchArea != null)
                    {
                        var keys = "in:inbox from:" + keyword.ToLower() + " is:unread";
                        searchArea.SendKeys(keys);
                        new Actions(driver).SendKeys(Keys.Enter).Perform();
                        Log("send keys : " + keys, "info");
                    }
                }
                catch (Exception x)
                {
                    if (x.Message.Contains("INBOX ACTIONS WAIT TIMEDOUT!") || x.Message.Contains("element not found") || x.Message.Contains("could not be located on the page") || x.Message.Contains("socket hang up") || x.Message.Contains("'normalizeTagNames'") || x.Message.Contains("Index was out of range") || x.Message.Contains("error occurred while processing the command") || x.Message.Contains("StaleObjectException") || x.Message.Contains("session is either terminated or not started"))
                        return InboxActions(keyword, date);
                    else
                    {
                        Log("search area : " + x.Message, "error");
                        isFail = true;
                        goto done;
                    }
                }

                try
                {
                    var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    cancelationToken.ThrowIfCancellationRequested();
                    var empty = subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("com.google.android.gm:id/empty_text")));
                    //var empty = driver.FindElementById("com.google.android.gm:id/empty_text");
                    if (empty.Count > 0)
                    {
                        if (empty[0].Text != "")
                        {
                            Log("no emails Founds in INBOX folder!", "warning");
                            goto done;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }

                //OPEN EMAIL
                ReadOnlyCollection<AndroidElement> emails = null;
                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("com.google.android.gm:id/viewified_conversation_item_view")));
                    emails = driver.FindElementsById("com.google.android.gm:id/viewified_conversation_item_view");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Conversation list : " + e.Message);
                }

                if (emails != null && emails.Count > 0)
                {
                    var emailsCount = emails.Count;
                    Log("Displayed Emails: " + emailsCount, "info");
                    var i = 1;

                    while (!isFail)
                    {
                        ReadOnlyCollection<AndroidElement> emailTime = null;

                        try
                        {
                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//android.view.ViewGroup[@resource-id='com.google.android.gm:id/viewified_conversation_item_view']/android.widget.TextView[@resource-id='com.google.android.gm:id/date']")));
                            emailTime = driver.FindElementsByXPath("//android.view.ViewGroup[@resource-id='com.google.android.gm:id/viewified_conversation_item_view']/android.widget.TextView[@resource-id='com.google.android.gm:id/date']");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Email Time : " + e.Message);
                        }

                        if (emailTime != null && emailTime.Count > 0)
                        {
                            var time = emailTime[0].Text;
                            var isValidTime = Time12hFormat.Validate(time);
                            if (isValidTime)
                            {
                                Log("Working on Email No: " + i, "info");
                                //READ EMAIL AND SCROLL

                                /*try
                                {
                                    cancelationToken.ThrowIfCancellationRequested();
                                    var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                                    var webView = subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//android.webkit.WebView")));

                                    if (webView.Displayed)
                                    {
                                        var random = (new Random().Next() * ((5 - 1) + 1)) + 1;
                                        for (var r = 1; r <= random; r++)
                                        {
                                            new Actions(driver).SendKeys(Keys.PageDown).Perform();
                                        }
                                        for (var r = 1; r <= random; r++)
                                        {
                                            new Actions(driver).SendKeys(Keys.PageUp).Perform();
                                        }
                                        new Actions(driver).SendKeys(Keys.PageUp).Perform();
                                    }
                                }
                                catch (Exception c)
                                {
                                    Log("Scrolling : " + c.Message);
                                }*/

                                //GET CURRENT SUBJECT
                                AndroidElement senderName = null;

                                try
                                {
                                    cancelationToken.ThrowIfCancellationRequested();
                                    var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(7));
                                    subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("com.google.android.gm:id/sender_name")));
                                    senderName = driver.FindElementById("com.google.android.gm:id/sender_name");
                                }
                                catch (Exception e)
                                {
                                    Log("Sender Name : " + e.Message, "error");
                                }

                                try
                                {
                                    if (senderName != null)
                                    {
                                        var sender = senderName.Text;
                                        Log("Sender Name: " + sender, "info");
                                        if (sender.Contains(keyword))
                                        {
                                            var act = new Random().Next((5 - 1) + 1) + 1;
                                            emailTime[0].Click();
                                            isFail = doAction(act, wait);
                                            //isFail = Archive(wait);
                                        }
                                        else
                                        {
                                            emailTime[0].Click();
                                            isFail = Archive(wait);
                                            Log("Email Archived!", "info");
                                        }
                                    }
                                    else
                                    {
                                        Log("sender name has NULL", "warning");
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log(e.Message, "error");
                                }
                            }
                            else
                            {
                                Log("No (Today) Emails Found!", "info");
                                break;
                            }
                        }
                        else
                        {
                            Log("No Emails Found!", "info");
                            break;
                        }
                        i++;

                        try
                        {
                            driver.PressKeyCode(AndroidKeyCode.Back);
                        }
                        catch (Exception c)
                        {
                            Console.WriteLine("error, Navigate back : " + c.Message);
                        }

                        goto process;
                    }

                }
            }
            else
            {
                Console.WriteLine("Error while initializing driver!");
            }

            done:
            driver.Quit();
            return isFail ? "inbox actions failed!" : "inbox actions successed!";
        }

        private bool Archive(WebDriverWait wait)
        {
            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("com.google.android.gm:id/archive")));
                var archiveAction = driver.FindElementsById("com.google.android.gm:id/archive");
                if (archiveAction.Count > 0)
                {
                    archiveAction[0].Click();
                    Log("Email archived", "info");
                    return false;
                }
                else
                {
                    Log("Archive Button Not Found", "error");
                    return true;
                }
            }
            catch (Exception x)
            {
                Log("archive action : " + x.Message, "error");
                return true;
            }
        }

        private bool doAction(int action, WebDriverWait wait)
        {
            bool isFailed = false;
            switch (action)
            {
                case 1:
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.widget.ImageView[@content-desc=\"Reply\"]")));
                        var replyButton = driver.FindElementByXPath("//android.widget.ImageView[@content-desc=\"Reply\"]");
                        if (replyButton != null)
                        {
                            replyButton.Click();
                            Log("click reply button", "info");

                            new Actions(driver).SendKeys("Thank you so much!").Perform();

                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("com.google.android.gm:id/send")));
                            var sendButton = driver.FindElementById("com.google.android.gm:id/send");
                            if (sendButton != null)
                            {
                                sendButton.Click();
                                Log("send reply", "info");
                                isFailed = false;
                            }
                            else
                            {
                                Log("Send Button Not Found!", "error");
                                isFailed = true;
                            }
                        }
                    }
                    catch (Exception x)
                    {
                        Log("reply action : " + x.Message, "error");
                        isFailed = true;
                    }

                    isFailed = Archive(wait);
                    break;
                case 2:
                    isFailed = Archive(wait);
                    break;
                case 3:
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("com.google.android.gm:id/conversation_header_star")));
                        var starAction = driver.FindElementsById("com.google.android.gm:id/conversation_header_star");
                        if (starAction.Count > 0)
                        {
                            starAction[0].Click();
                            Log("perform star action", "info");
                            isFailed = false;
                        }
                        else
                        {
                            Log("star button not found!", "error");
                            isFailed = true;
                        }
                    }
                    catch (Exception x)
                    {
                        Log("Star action : " + x.Message, "error");
                        isFailed = true;
                    }
                    isFailed = Archive(wait);
                    break;
                case 4:
                    try
                    {
                        //MORE OPTIONS BUTTON:
                        cancelationToken.ThrowIfCancellationRequested();
                        wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("(//android.widget.ImageView[@content-desc=\"More options\"])[2]")));
                        var moreOptions = driver.FindElementsByXPath("(//android.widget.ImageView[@content-desc=\"More options\"])[2]");
                        if (moreOptions.Count > 0)
                        {
                            moreOptions[0].Click();
                            Log("click more options button", "info");

                            //MARK IMPORTANT:
                            cancelationToken.ThrowIfCancellationRequested();
                            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("(//android.widget.TextView[Contains(@text, \"important\")])[1]")));
                            var importantAction = driver.FindElementsByXPath("(//android.widget.TextView[Contains(@text, \"important\")])[1]");
                            if (importantAction.Count > 0)
                            {
                                if (importantAction[0].Text == "Mark important")
                                {
                                    importantAction[0].Click();
                                    Log("click mark important button", "info");
                                }
                                else
                                {
                                    moreOptions[0].Click();
                                }
                                isFailed = false;
                            }
                        }
                    }
                    catch (Exception x)
                    {
                        Log("mark important action : " + x.Message, "error");
                        isFailed = true;
                    }
                    isFailed = Archive(wait);
                    break;
                case 5:
                    try
                    {
                        cancelationToken.ThrowIfCancellationRequested();
                        var subWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                        subWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath("//android.webkit.WebView")));
                        var webView = driver.FindElementsByXPath("//android.webkit.WebView");
                        if (webView.Count > 0)
                        {
                            webView[0].Click();
                            Log("open link in browser", "info");
                            isFailed = false;
                        }
                        try
                        {
                            if (webView[0].Displayed)
                            {

                            }
                        }
                        catch (Exception e)
                        {
                            driver.PressKeyCode(AndroidKeyCode.Back);
                            Log("back to Gmail app", "info");
                        }
                    }
                    catch (Exception x)
                    {
                        Log("click link : " + x.Message, "error");
                    }
                    isFailed = Archive(wait);
                    break;
            }

            return isFailed;
        }

        private void doJunk(string keyword, string date)
        {
            if (driver != null)
            {

                var header = driver.FindElementById("com.google.android.gm:id/conversation_list_folder_header");
                header.Click();

                var searchArea = driver.FindElementById("com.google.android.gm:id/open_search_view_edit_text");
                searchArea.SendKeys("is:spam From:" + keyword.ToLower());
                new Actions(driver).SendKeys(Keys.Enter);

                //OPEN EMAIL
                var email = driver.FindElementsById("com.google.android.gm:id/viewified_conversation_item_view");
                var emailsCount = email.Count;
                Console.WriteLine("Displayed Emails: " + emailsCount);
                var i = 1;
                process:
                while (true)
                {
                    var emailTime = driver.FindElementsByXPath("//android.view.ViewGroup[@resource-id='com.google.android.gm:id/viewified_conversation_item_view']/android.widget.TextView[@resource-id='com.google.android.gm:id/date']");
                    if (emailTime.Count > 0)
                    {
                        var time = emailTime[0].Text;
                        var isValidTime = Time12hFormat.Validate(time);
                        if (isValidTime)
                        {
                            Console.WriteLine("Working on Email No: " + i);
                            //READ EMAIL AND SCROLL
                            var webView = driver.FindElementsByXPath("//android.webkit.WebView");

                            if (webView.Count > 0)
                            {
                                webView[0].Click();
                                var random = (new Random().Next() * ((5 - 1) + 1)) + 1;
                                for (var r = 1; r <= random; r++)
                                {
                                    new Actions(driver).SendKeys(Keys.PageDown);
                                }
                                for (var r = 1; r <= random; r++)
                                {
                                    new Actions(driver).SendKeys(Keys.PageUp);
                                }
                                new Actions(driver).SendKeys(Keys.PageUp);
                            }
                            //GET CURRENT SUBJECT
                            var senderName = driver.FindElementById("com.google.android.gm:id/sender_name");
                            var sender = senderName.Text;
                            if (sender.Contains(keyword))
                            {
                                //MARK AS NOT SPAM
                                var notJunkButton = driver.FindElementsByXPath("//android.widget.Button[@resource-id = 'com.google.android.gm:id/warning_banner_primary_button']");
                                if (notJunkButton.Count == 0)
                                {
                                    senderName.Click();
                                    notJunkButton = driver.FindElementsByXPath("//android.widget.Button[@resource-id = 'com.google.android.gm:id/warning_banner_primary_button']");
                                }
                                notJunkButton[0].Click();
                                Console.WriteLine("Email marked as not junk");
                            }
                            else
                            {
                                //MARK AS NOT SPAM
                                var deleteButton = driver.FindElementsByXPath("(//*[@content-desc='Delete'])[1]");
                                deleteButton[0].Click();
                                Console.WriteLine("Email deleted");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No (Today) Emails Found!");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Emails Found!");
                        break;
                    }
                    i++;
                    goto process;
                }
            }
            else
            {
                Console.WriteLine("Error while initializing driver!");
            }
        }

        public string CheckAccounts()
        {
            bool isFail = false;
            try
            {
                cancelationToken.ThrowIfCancellationRequested();
                AppiumOptions options = new AppiumOptions();
                options.PlatformName = "Android";
                options.AddAdditionalCapability("deviceName", this.device);
                options.AddAdditionalCapability("udid", this.device);
                var systemPort = Convert.ToInt32(this.device.Split(':')[1]);
                options.AddAdditionalCapability("systemPort", new Random().Next(30000, 40000));
                options.AddAdditionalCapability("platformName", "android");
                options.AddAdditionalCapability("automationName", "UiAutomator2");

                //options.AddAdditionalCapability("uiautomator2ServerInstallTimeout", 60000);
                //options.AddAdditionalCapability("uiautomator2ServerLaunchTimeout", 60000);

                options.AddAdditionalCapability("appPackage", "com.google.android.gm");
                options.AddAdditionalCapability("appActivity", ".ConversationListActivityGmail");
                options.AddAdditionalCapability("noReset", true);
                options.AddAdditionalCapability("printPageSourceOnFindFailure", false);

                driver = new AndroidDriver<AndroidElement>(new Uri(this.ServerURI), options);
            }
            catch (Exception x)
            {
                return "driver error, " + x.Message;
            }

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(this.WaitTO));
            wait.PollingInterval = TimeSpan.FromMilliseconds(this.WaitPolling);
            wait.Message = "CHECK ACCOUNT WAIT TIMEDOUT!";

            if (driver != null)
            {
                try
                {
                    cancelationToken.ThrowIfCancellationRequested();
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.Id("com.google.android.gm:id/conversation_list_folder_header")));
                    var header = driver.FindElementById("com.google.android.gm:id/conversation_list_folder_header");
                    if (header != null)
                    {
                        Log("gmail account found", "info");
                    }
                }
                catch (Exception)
                {
                    Log("gmail account not found", "warning");
                    BlockedLog("gmail account not found");
                }
            }
            else
            {
                Console.WriteLine("Error while initializing driver!");
                isFail = true;
            }

            driver.Quit();
            return isFail ? "check account failed!" : "check account successed!";
        }

        private async void Log(string message, string loglevel)
        {
            try
            {
                var logtype = loglevel == "info" ? "ok" : loglevel.ToLower();
                using (FileStream fs = new FileStream(actionLogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        await sw.WriteLineAsync($"{DateTime.UtcNow.ToLongTimeString()}: [{index}] - ({logtype}) | {message.ToLower()}");
                    }
                }
                if (logtype == "error")
                    ErrorLog(message);
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        private async void ErrorLog(string message)
        {
            try
            {
                using (FileStream fs = new FileStream(errorLogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
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

        private async void BlockedLog(string message)
        {
            try
            {
                using (FileStream fs = new FileStream(blockedLogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        await sw.WriteLineAsync($"{DateTime.UtcNow.ToLongTimeString()}: [{directory}/{index}] | {message.ToLower()}");
                    }
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        private async void DisabledLog(string message)
        {
            try
            {
                using (FileStream fs = new FileStream(disabledLogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        await sw.WriteLineAsync($"{DateTime.UtcNow.ToLongTimeString()}: [{directory}/{index}] | {message.ToLower()}");
                    }
                }
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        [TestMethod]
        public Task TestGmail()
        {
            error = new List<string>();
            return Task.Factory.StartNew(() =>
            {
                var driverResponse = InitDriver();
                if (driverResponse != "done")
                    error.Add(driverResponse);
                else
                {

                }
            });
        }

        [TestMethod]
        public Task InstallGmail(string path)
        {
            error = new List<string>();
            return Task.Factory.StartNew(() =>
            {
                var initResponse = InitDriver(20, false);
                if (initResponse == "done")
                {
                    var installResponse = InstallGmailApp(path);
                    Log(installResponse, "");
                }
            });
        }

        [TestMethod]
        public Task<string> InboxProcess(string keyword, string date)
        {
            error = new List<string>();
            return Task<string>.Factory.StartNew(() =>
            {
                try
                {
                    if (cancelationToken.IsCancellationRequested)
                    {
                        Log("Operation Canceled!", "warning");
                        return "Operation Canceled!";
                    }
                    var doInbox = InboxActions(keyword, date);
                    Log(doInbox, doInbox.Contains("failed") ? "error" : "success");
                    return doInbox;
                }
                catch (OperationCanceledException e)
                {
                    Log("Operation Canceled!", "warning");
                    driver.Quit();
                    return "Operation Canceled!";
                }
            }, cancelationToken);
        }

        [TestMethod]
        public Task<string> SpamProcess(string keyword, string date)
        {
            error = new List<string>();
            return Task<string>.Factory.StartNew(() =>
            {
                try
                {
                    if (cancelationToken.IsCancellationRequested)
                    {
                        Log("Operation Canceled!", "warning");
                        return "Operation Canceled!";
                    }
                    var doSpam = SpamActions(keyword, date);
                    Log(doSpam, doSpam.Contains("failed") ? "error" : "success");
                    return doSpam;
                }
                catch (OperationCanceledException e)
                {
                    Log("Operation Canceled!", "warning");
                    driver.Quit();
                    return "Operation Canceled!";
                }
            }, cancelationToken);
        }

        [TestMethod]
        public Task<string> CheckProcess()
        {
            error = new List<string>();
            return Task<string>.Factory.StartNew(() =>
            {
                try
                {
                    if (cancelationToken.IsCancellationRequested)
                    {
                        Log("Operation Canceled!", "warning");
                        return "Operation Canceled!";
                    }
                    var doCheck = CheckAccounts();
                    Log(doCheck, doCheck.Contains("failed") ? "error" : "success");
                    return doCheck;
                }
                catch (OperationCanceledException e)
                {
                    Log("Operation Canceled!", "warning");
                    driver.Quit();
                    return "Operation Canceled!";
                }
            }, cancelationToken);
        }

        [TestMethod]
        public Task Testing()
        {
            error = new List<string>();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    if (cancelationToken.IsCancellationRequested)
                    {
                        Log("Operation Canceled!", "warning");
                    }

                    InitDriver(60, true);
                    driver.PressKeyCode(AndroidKeyCode.Back);
                }
                catch (OperationCanceledException e)
                {
                    Log("Operation Canceled!", "warning");
                    driver.Quit();
                }
            }, cancelationToken);
        }

        [TestMethod]
        public Task<string> SetupProxy(string host, int port = 92)
        {
            error = new List<string>();
            return Task<string>.Factory.StartNew(() =>
            {
                try
                {
                    if (cancelationToken.IsCancellationRequested)
                    {
                        Log("Operation Canceled!", "warning");
                        return "Operation Canceled!";
                    }
                    var updateProxyResponse = ProxySetup(host, port);
                    Log(updateProxyResponse, updateProxyResponse.Contains("failed") ? "error" : "success");
                    return updateProxyResponse;
                }
                catch (OperationCanceledException e)
                {
                    Log("Operation Canceled!", "warning");
                    driver.Quit();
                    return "Operation Canceled!";
                }
            }, cancelationToken);
        }

        [TestMethod]
        public Task<string> SignIn(string[] creds)
        {
            error = new List<string>();
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    if (cancelationToken.IsCancellationRequested)
                    {
                        Log("Operation Canceled!", "warning");
                        return "Operation Canceled!";
                    }
                    cancelationToken.ThrowIfCancellationRequested();
                    var signInResponse = GmailLogin(creds);
                    Log(signInResponse, signInResponse.Contains("failed") ? "error" : "success");
                    return signInResponse;
                }
                catch (OperationCanceledException e)
                {
                    Log("Operation Canceled!", "warning");
                    driver.Quit();
                    return "Operation Canceled!";
                }
            }, cancelationToken);
        }

        [TestMethod]
        public Task<bool> IsNotExists(string device)
        {
            try
            {
                var devices = new List<string>();
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
                        devices.Add(line);
                    }

                    process.WaitForExit();
                    return devices.Where(str => str.Contains(device)).Count() == 0;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        [TestMethod]
        public Task<bool> IsOffline(string device)
        {
            try
            {
                var devices = new List<string>();
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
                        devices.Add(line);
                    }

                    process.WaitForExit();
                    return /*devices.Contains(device + " offline") || */devices.Where(str => str.Contains(device)).Count() == 0;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void Dispose()
        => Dispose(true);

        protected void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}

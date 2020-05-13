using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Service;
using System.Threading.Tasks;
using System.Collections.Generic;
using OpenQA.Selenium.Appium.Service.Options;

namespace AppiumHelper
{
    [TestClass]
    public class AppiumHelperService
    {
        private AppiumLocalService service;

        private Task InitServices()
        {
            try
            {
                return Task.Factory.StartNew(() =>
                {
                    KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>("--log-level", "error");
                    var args = new OptionCollector().AddArguments(keyValuePair);

                    var appium = new AppiumServiceBuilder();
                    //appium.WithLogFile(new FileInfo(appiumLogPath));
                    service = appium.UsingPort(10102).WithArguments(args).Build(); //new Uri("http://127.0.0.1:10102/wd/hub")

                    service.Start();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        [TestMethod]
        public Task StartAppium()
        {
            return InitServices();
        }

        [TestMethod]
        public void StopAppium()
        {
            if (service.IsRunning)
            {
                service.Dispose();
            }
        }

        [TestMethod]
        public bool IsRunning()
        {
            return service.IsRunning;
        }
    }
}

using GUX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GUX.Services
{
    public class SchedulerService
    {
        private List<Timer> timers = new List<Timer>();
        public void ScheduleTask(int hour, int min, double intervalInHour, Action task, string info = "")
        {
            DateTime now = DateTime.Now;
            DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, hour, min, 0, 0);
            if (now > firstRun)
            {
                firstRun = firstRun.AddDays(1);
            }
            TimeSpan timeToGo = firstRun - now;
            if (timeToGo <= TimeSpan.Zero)
            {
                timeToGo = TimeSpan.Zero;
            }
            var timer = new Timer(x =>
            {
                task.Invoke();
            }, null, timeToGo, TimeSpan.FromHours(intervalInHour));

            timers.Add(timer);

            Globals.scheduleTasks.Add(new ScheduleTask
            {
                Timer = timer,
                Name = "NULL",
                Info = info
            });
        }

        public void Dispose(Timer t)
        {
            try
            {
                timers.Remove(t);
                t.Dispose();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                foreach (var timer in timers)
                {
                    timer.Dispose();
                }
                timers.Clear();
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUX.Core
{
    public class Scheduler
    {
        public void IntervalInSeconds(int hour, int sec, double interval, Action task, string info)
        {
            interval = interval / 3600;
            Globals.SchServices.ScheduleTask(hour, sec, interval, task, info);

        }
        public void IntervalInMinutes(int hour, int min, double interval, Action task, string info)
        {
            interval = interval / 60;
            Globals.SchServices.ScheduleTask(hour, min, interval, task, info);
        }
        public void IntervalInHours(int hour, int min, double interval, Action task, string info)
        {
            Globals.SchServices.ScheduleTask(hour, min, interval, task, info);
        }
        public void IntervalInDays(int hour, int min, double interval, Action task, string info)
        {
            interval = interval * 24;
            Globals.SchServices.ScheduleTask(hour, min, interval, task, info);
        }
    }
}

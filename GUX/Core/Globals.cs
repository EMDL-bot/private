using GUX.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUX.Core
{
    public static class Globals
    {
        public static List<ScheduleTask> scheduleTasks = new List<ScheduleTask>();
        public static SchedulerService SchServices;

        public static int SERVER_PORT { get; set; }
        public static int TEST_DRIVER_TIMEOUT { get; set; }
        public static int DRIVER_WAIT_TIMEOUT { get; set; }
        public static int DRIVER_WAIT_POLLING_INTERVAL { get; set; }

        public static bool UI_TOP_MOST { get; set; }
        public static bool UI_TRANSPARENT { get; set; }

        public static int THREADS_CONCURRENCY { get; set; }
        public static int THREADS_INTERVAL { get; set; }

        public static string DRIVE_LETTER { get; set; }
        public static string VMS_DIRECTORY { get; set; }
        public static bool FAILED_AUTO_RETRY { get; set; }
        public static int FAILED_MAX_RETRIES { get; set; }
        public static int FAILED_ACTION_MAX_RETRIES { get; set; }

        public static Scenario DEFAULT_SCENARIO { get; set; }
        public static bool WARMUP_AUTORUN { get; set; }
        public static bool WARMUP_PROCEED_INBOX_FOLDER { get; set; }
        public static int WARMUP_MAX_TREATED_INBOX_EMAILS { get; set; }
    }
}

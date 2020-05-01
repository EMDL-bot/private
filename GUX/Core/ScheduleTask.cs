using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GUX.Core
{
    public class ScheduleTask
    {
        public Timer Timer { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
    }
}

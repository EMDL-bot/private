using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUX.Core
{
    [Serializable]
    public class Scenario
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string actions { get; set; }
        public string keyword { get; set; }
        public string date { get; set; }
    }
}

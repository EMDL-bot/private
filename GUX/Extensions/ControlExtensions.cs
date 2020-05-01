using GUX.UC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUX.Extensions
{
    public static class ControlExtensions
    {
        

        public static void DoubleBuffer(this Control control)
        {
            if (SystemInformation.TerminalServerSession) return;
            System.Reflection.PropertyInfo dbProp = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            dbProp.SetValue(control, true, null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUX.UC
{
    public partial class iListView : ListView
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int MOUSEWHEEL = 0x020A;
        private const int KEYDOWN = 0x0100;
        private enum ScrollBarDir { SB_HORZ = 0, SB_VERT = 1, SB_CTL = 2, SB_BOTH = 3 }
        public event ScrollEventHandler Scroll;

        

        public iListView()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        public iListView(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected virtual void OnScroll(ScrollEventArgs e)
        {
            ScrollEventHandler handler = this.Scroll;
            if (handler != null) handler(this, e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            ShowScrollBar(this.Handle, (int)ScrollBarDir.SB_BOTH, false);

            if (m.Msg == MOUSEWHEEL)
            {
                OnScroll(new ScrollEventArgs((ScrollEventType)(m.WParam.ToInt32() & 0xffff), -1, 0, ScrollOrientation.VerticalScroll));
            }

            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }
}

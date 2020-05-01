using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUX.UC
{
    public partial class iCheckedList : CheckedListBox
    {
        private Color borderColor = Color.FromArgb(30, 30, 30);

        public Color BorderColor
        {
            get
            {
                return this.borderColor;
            }
            set
            {
                this.borderColor = value;
                this.Invalidate();
            }
        }

        public iCheckedList()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            InitializeComponent();
        }

        public iCheckedList(IContainer container)
        {
            container.Add(this);
            this.DrawMode = DrawMode.OwnerDrawFixed;
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // breakpoint on the below line doesn't happen
            base.OnPaint(e);
            Pen pen = new Pen(this.borderColor);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.DrawRectangle(pen, 2, 2, this.Width + 1, this.Height + 1);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;

namespace GUX.UC
{
    public partial class iTools : DevExpress.XtraEditors.XtraUserControl
    {
        public iTools()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public string Title
        {
            get { return this.iButton.Text; }
            set { this.iButton.Text = value.ToUpper(); }
        }

        public int DataID { get; set; }

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks update/delete button")]
        public event ButtonPressedEventHandler ButtonClick;

        public SuperToolTip ToolTip
        {
            get
            {
                return iButton.SuperTip;
            }
            set
            {
                iButton.SuperTip = value;
            }
        }

        private void iButton_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            this.ButtonClick?.Invoke(this, e);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GUX.Core;
using Transitions;

namespace GUX.UC
{
    public partial class iButton : UserControl
    {
        public string icon { get { return iconLabel.Text; } set { iconLabel.Text = value; } }
        public string text { get { return textLabel.Text; } set { textLabel.Text = value; } }
        public Font font { set { iconLabel.Font = value; } }
        public iButton()
        {
            InitializeComponent();
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(36, 36, 36);
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;

namespace GUX
{
    public partial class iSplash : SplashScreen
    {
        public iSplash()
        {
            InitializeComponent();
            this.labelCopyright.Text = "Copyright © 2019-" + DateTime.Now.Year.ToString();
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum SplashScreenCommand
        {
        }

        private void iSplash_Load(object sender, EventArgs e)
        {

        }
    }
}
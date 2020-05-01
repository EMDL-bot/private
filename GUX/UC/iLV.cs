using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.XtraEditors.BaseCheckedListBoxControl;
using DevExpress.Utils.Controls;

namespace GUX.UC
{
    public partial class iLV : DevExpress.XtraEditors.XtraUserControl, IXtraResizableControl
    {
        public iLV()
        {
            InitializeComponent();
        }

        public object DataSource
        {
            get
            {
                return this.iCheckedList.DataSource;
            }
            set
            {
                this.iCheckedList.DataSource = value;
            }
        }

        public string HtmlTitle
        {
            get
            {
                return this.iLabel.Text;
            }
            set
            {
                this.iLabel.Text = value;
            }
        }

        public string PlainTextTitle
        {
            get
            {
                return this.iLabel.PlainText;
            }
        }

        public CheckedItemCollection CheckedItems
        {
            get
            {
                return iCheckedList.CheckedItems;
            }
        }

        public int CheckedItemsCount
        {
            get
            {
                return this.iCheckedList.CheckedItemsCount;
            }
        }

        public CheckedIndexCollection CheckedIndices
        {
            get
            {
                return this.iCheckedList.CheckedIndices;
            }
        }

        public bool InProgress
        {
            get
            {
                return this.iSpinner.Visible;
            }
            set
            {
                this.iSpinner.Visible = value;
            }
        }

        public void CheckAll()
        {
            this.iCheckedList.CheckAll();
        }

        public void UnCheckAll()
        {
            this.iCheckedList.UnCheckAll();
        }

        public void SetItemChecked(int index, bool value)
        {
            this.iCheckedList.SetItemChecked(index, value);
        }

        public void ResetDataSource()
        {
            if (this.iCheckedList.DataSource != null)
            {
                this.DataSource = null;
            }
        }

        private void iSelectAll_CheckedChanged(object sender)
        {
            if (this.iSelectAll.Checked)
                this.CheckAll();
            else
                this.UnCheckAll();
        }

        private void iLV_Load(object sender, EventArgs e)
        {
            
        }
    }
}

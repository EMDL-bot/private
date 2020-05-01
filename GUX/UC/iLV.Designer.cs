namespace GUX.UC
{
    partial class iLV
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.XtraEditors.TableLayout.ItemTemplateBase itemTemplateBase1 = new DevExpress.XtraEditors.TableLayout.ItemTemplateBase();
            DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition1 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
            DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
            DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition3 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
            DevExpress.XtraEditors.TableLayout.TemplatedItemElement templatedItemElement1 = new DevExpress.XtraEditors.TableLayout.TemplatedItemElement();
            DevExpress.XtraEditors.TableLayout.TemplatedItemElement templatedItemElement2 = new DevExpress.XtraEditors.TableLayout.TemplatedItemElement();
            DevExpress.XtraEditors.TableLayout.TemplatedItemElement templatedItemElement3 = new DevExpress.XtraEditors.TableLayout.TemplatedItemElement();
            DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition1 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
            DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
            DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
            DevExpress.XtraEditors.TableLayout.TableSpan tableSpan1 = new DevExpress.XtraEditors.TableLayout.TableSpan();
            this.iCheckedList = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.iLabel = new DevExpress.XtraEditors.LabelControl();
            this.iSelectAll = new MetroSet_UI.Controls.MetroSetCheckBox();
            this.iSpinner = new XanderUI.XUICircleProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.iCheckedList)).BeginInit();
            this.SuspendLayout();
            // 
            // iCheckedList
            // 
            this.iCheckedList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.iCheckedList.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.iCheckedList.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.iCheckedList.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.iCheckedList.Appearance.Options.UseBackColor = true;
            this.iCheckedList.Appearance.Options.UseBorderColor = true;
            this.iCheckedList.Appearance.Options.UseForeColor = true;
            this.iCheckedList.AppearanceSelected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.iCheckedList.AppearanceSelected.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.iCheckedList.AppearanceSelected.Options.UseBackColor = true;
            this.iCheckedList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.iCheckedList.CheckOnClick = true;
            this.iCheckedList.HighlightedItemStyle = DevExpress.XtraEditors.HighlightStyle.Skinned;
            this.iCheckedList.HorzScrollStep = 2;
            this.iCheckedList.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
            this.iCheckedList.ItemAutoHeight = true;
            this.iCheckedList.ItemHeight = 30;
            this.iCheckedList.Location = new System.Drawing.Point(0, 27);
            this.iCheckedList.Name = "iCheckedList";
            this.iCheckedList.ShowFocusRect = false;
            this.iCheckedList.Size = new System.Drawing.Size(165, 168);
            this.iCheckedList.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.iCheckedList.TabIndex = 121;
            tableColumnDefinition1.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
            tableColumnDefinition1.Length.Value = 18D;
            tableColumnDefinition2.Length.Value = 32D;
            tableColumnDefinition3.Length.Value = 146D;
            itemTemplateBase1.Columns.Add(tableColumnDefinition1);
            itemTemplateBase1.Columns.Add(tableColumnDefinition2);
            itemTemplateBase1.Columns.Add(tableColumnDefinition3);
            templatedItemElement1.FieldName = null;
            templatedItemElement1.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
            templatedItemElement1.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            templatedItemElement1.ImageOptions.SvgImage = global::GUX.Properties.Resources.app;
            templatedItemElement1.ImageOptions.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.CommonPalette;
            templatedItemElement1.RowIndex = 1;
            templatedItemElement1.StretchHorizontal = true;
            templatedItemElement1.StretchVertical = true;
            templatedItemElement1.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
            templatedItemElement1.TextVisible = false;
            templatedItemElement2.AnchorAlignment = DevExpress.Utils.AnchorAlignment.Left;
            templatedItemElement2.Appearance.Normal.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            templatedItemElement2.Appearance.Normal.Options.UseFont = true;
            templatedItemElement2.ColumnIndex = 1;
            templatedItemElement2.FieldName = "index";
            templatedItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
            templatedItemElement2.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            templatedItemElement2.RowIndex = 1;
            templatedItemElement2.Text = "index";
            templatedItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
            templatedItemElement3.ColumnIndex = 2;
            templatedItemElement3.FieldName = "device";
            templatedItemElement3.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
            templatedItemElement3.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
            templatedItemElement3.RowIndex = 1;
            templatedItemElement3.Text = "device";
            templatedItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
            itemTemplateBase1.Elements.Add(templatedItemElement1);
            itemTemplateBase1.Elements.Add(templatedItemElement2);
            itemTemplateBase1.Elements.Add(templatedItemElement3);
            itemTemplateBase1.Name = "Device";
            tableRowDefinition1.Length.Value = 4D;
            tableRowDefinition2.Length.Value = 18D;
            tableRowDefinition3.Length.Value = 4D;
            itemTemplateBase1.Rows.Add(tableRowDefinition1);
            itemTemplateBase1.Rows.Add(tableRowDefinition2);
            itemTemplateBase1.Rows.Add(tableRowDefinition3);
            tableSpan1.RowSpan = 3;
            itemTemplateBase1.Spans.Add(tableSpan1);
            this.iCheckedList.Templates.Add(itemTemplateBase1);
            // 
            // iLabel
            // 
            this.iLabel.AllowHtmlString = true;
            this.iLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.iLabel.Location = new System.Drawing.Point(72, 6);
            this.iLabel.Name = "iLabel";
            this.iLabel.Size = new System.Drawing.Size(20, 14);
            this.iLabel.TabIndex = 122;
            this.iLabel.Text = "<b>G04</b>";
            // 
            // iSelectAll
            // 
            this.iSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iSelectAll.BackColor = System.Drawing.Color.Transparent;
            this.iSelectAll.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.iSelectAll.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.iSelectAll.Checked = false;
            this.iSelectAll.CheckSignColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.iSelectAll.CheckState = MetroSet_UI.Enums.CheckState.Unchecked;
            this.iSelectAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.iSelectAll.DisabledBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(85)))), ((int)(((byte)(85)))));
            this.iSelectAll.Font = new System.Drawing.Font("Calibri", 9F);
            this.iSelectAll.Location = new System.Drawing.Point(142, 5);
            this.iSelectAll.Name = "iSelectAll";
            this.iSelectAll.SignStyle = MetroSet_UI.Enums.SignStyle.Shape;
            this.iSelectAll.Size = new System.Drawing.Size(20, 16);
            this.iSelectAll.Style = MetroSet_UI.Design.Style.Custom;
            this.iSelectAll.StyleManager = null;
            this.iSelectAll.TabIndex = 123;
            this.iSelectAll.ThemeAuthor = "Narwin";
            this.iSelectAll.ThemeName = "MetroDark";
            this.iSelectAll.CheckedChanged += new MetroSet_UI.Controls.MetroSetCheckBox.CheckedChangedEventHandler(this.iSelectAll_CheckedChanged);
            // 
            // iSpinner
            // 
            this.iSpinner.AnimationSpeed = 7;
            this.iSpinner.FilledColor = System.Drawing.SystemColors.Highlight;
            this.iSpinner.FilledColorAlpha = 190;
            this.iSpinner.FilledThickness = 3;
            this.iSpinner.IsAnimated = true;
            this.iSpinner.Location = new System.Drawing.Point(5, 3);
            this.iSpinner.Name = "iSpinner";
            this.iSpinner.Percentage = 50;
            this.iSpinner.ShowText = false;
            this.iSpinner.Size = new System.Drawing.Size(16, 16);
            this.iSpinner.TabIndex = 124;
            this.iSpinner.TextColor = System.Drawing.Color.Gray;
            this.iSpinner.TextSize = 7;
            this.iSpinner.UnFilledColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.iSpinner.UnfilledThickness = 3;
            this.iSpinner.UseWaitCursor = true;
            this.iSpinner.Visible = false;
            // 
            // iLV
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseBorderColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.iSpinner);
            this.Controls.Add(this.iSelectAll);
            this.Controls.Add(this.iLabel);
            this.Controls.Add(this.iCheckedList);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(165, 195);
            this.Name = "iLV";
            this.Size = new System.Drawing.Size(165, 195);
            this.Load += new System.EventHandler(this.iLV_Load);
            ((System.ComponentModel.ISupportInitialize)(this.iCheckedList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.CheckedListBoxControl iCheckedList;
        private DevExpress.XtraEditors.LabelControl iLabel;
        private MetroSet_UI.Controls.MetroSetCheckBox iSelectAll;
        private XanderUI.XUICircleProgressBar iSpinner;
    }
}

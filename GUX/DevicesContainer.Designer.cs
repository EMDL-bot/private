namespace GUX
{
    partial class DevicesContainer
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions1 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            DevExpress.Utils.Controls.SnapOptions snapOptions1 = new DevExpress.Utils.Controls.SnapOptions();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.autoFitDevicesButton = new DevExpress.XtraBars.BarButtonItem();
            this.fitDevicesButton = new DevExpress.XtraBars.BarButtonItem();
            this.stopDevicesButton = new DevExpress.XtraBars.BarButtonItem();
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.DevicesCountLabel = new DevExpress.XtraBars.BarStaticItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barEditItem1 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemColorEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
            this.DevicesLayoutControl = new DevExpress.XtraLayout.LayoutControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.DevicesGroupLayout = new DevExpress.XtraLayout.LayoutControlGroup();
            this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DevicesLayoutControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DevicesGroupLayout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(363, 369);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(380, 14);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1,
            this.bar3});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.stopDevicesButton,
            this.DevicesCountLabel,
            this.fitDevicesButton,
            this.barEditItem1,
            this.autoFitDevicesButton});
            this.barManager1.MaxItemId = 8;
            this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemColorEdit1});
            this.barManager1.StatusBar = this.bar3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.autoFitDevicesButton),
            new DevExpress.XtraBars.LinkPersistInfo(this.fitDevicesButton),
            new DevExpress.XtraBars.LinkPersistInfo(this.stopDevicesButton, true)});
            this.bar1.Text = "Tools";
            // 
            // autoFitDevicesButton
            // 
            this.autoFitDevicesButton.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
            this.autoFitDevicesButton.Caption = "Auto Fit";
            this.autoFitDevicesButton.Id = 7;
            this.autoFitDevicesButton.ImageOptions.SvgImage = global::GUX.Properties.Resources.fitboundstotext;
            this.autoFitDevicesButton.Name = "autoFitDevicesButton";
            this.autoFitDevicesButton.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.autoFitDevicesButton_DownChanged);
            // 
            // fitDevicesButton
            // 
            this.fitDevicesButton.Caption = "Fit";
            this.fitDevicesButton.Id = 4;
            this.fitDevicesButton.ImageOptions.SvgImage = global::GUX.Properties.Resources.squeeze;
            this.fitDevicesButton.Name = "fitDevicesButton";
            this.fitDevicesButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.fitDevicesButton_ItemClick);
            // 
            // stopDevicesButton
            // 
            this.stopDevicesButton.Caption = "Stop";
            this.stopDevicesButton.Id = 2;
            this.stopDevicesButton.ImageOptions.SvgImage = global::GUX.Properties.Resources.actions_forbid;
            this.stopDevicesButton.Name = "stopDevicesButton";
            this.stopDevicesButton.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.stopDevicesButton.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.stopDevicesButton_ItemClick);
            // 
            // bar3
            // 
            this.bar3.BarName = "Status bar";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.DevicesCountLabel)});
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            this.bar3.Text = "Status bar";
            // 
            // DevicesCountLabel
            // 
            this.DevicesCountLabel.Caption = "15";
            this.DevicesCountLabel.Id = 3;
            this.DevicesCountLabel.ImageOptions.SvgImage = global::GUX.Properties.Resources.electronics_phoneandroid2;
            this.DevicesCountLabel.Name = "DevicesCountLabel";
            this.DevicesCountLabel.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(810, 28);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 444);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(810, 28);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 28);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 416);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(810, 28);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 416);
            // 
            // barEditItem1
            // 
            this.barEditItem1.Caption = "barEditItem1";
            this.barEditItem1.Edit = this.repositoryItemColorEdit1;
            this.barEditItem1.Id = 6;
            this.barEditItem1.Name = "barEditItem1";
            // 
            // repositoryItemColorEdit1
            // 
            this.repositoryItemColorEdit1.AutoHeight = false;
            this.repositoryItemColorEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemColorEdit1.Name = "repositoryItemColorEdit1";
            // 
            // DevicesLayoutControl
            // 
            this.DevicesLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DevicesLayoutControl.Location = new System.Drawing.Point(0, 28);
            this.DevicesLayoutControl.Name = "DevicesLayoutControl";
            this.DevicesLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-43, 70, 1249, 713);
            this.DevicesLayoutControl.Root = this.Root;
            this.DevicesLayoutControl.Size = new System.Drawing.Size(810, 416);
            this.DevicesLayoutControl.TabIndex = 5;
            this.DevicesLayoutControl.Text = "layoutControl1";
            this.DevicesLayoutControl.ItemAdded += new System.EventHandler(this.DevicesLayoutControlItemsChanged);
            this.DevicesLayoutControl.ItemRemoved += new System.EventHandler(this.DevicesLayoutControlItemsChanged);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.DevicesGroupLayout});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(810, 416);
            this.Root.TextVisible = false;
            // 
            // DevicesGroupLayout
            // 
            this.DevicesGroupLayout.AllowHtmlStringInCaption = true;
            this.DevicesGroupLayout.CustomHeaderButtons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("Close", true, buttonImageOptions1, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, false, true, null, -1)});
            this.DevicesGroupLayout.CustomizationFormText = "Devices";
            this.DevicesGroupLayout.GroupBordersVisible = false;
            this.DevicesGroupLayout.GroupStyle = DevExpress.Utils.GroupStyle.Card;
            this.DevicesGroupLayout.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Flow;
            this.DevicesGroupLayout.Location = new System.Drawing.Point(0, 0);
            this.DevicesGroupLayout.Name = "DevicesGroupLayout";
            this.DevicesGroupLayout.OptionsItemText.AlignControlsWithHiddenText = true;
            this.DevicesGroupLayout.OptionsItemText.TextAlignMode = DevExpress.XtraLayout.TextAlignModeGroup.AutoSize;
            this.DevicesGroupLayout.ShowTabPageCloseButton = true;
            this.DevicesGroupLayout.Size = new System.Drawing.Size(790, 396);
            this.DevicesGroupLayout.Tag = "LayoutRootGroupForRestore";
            this.DevicesGroupLayout.Text = "Devices";
            // 
            // DevicesContainer
            // 
            this.AllowMdiBar = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.behaviorManager1.SetBehaviors(this, new DevExpress.Utils.Behaviors.Behavior[] {
            ((DevExpress.Utils.Behaviors.Behavior)(DevExpress.Utils.Behaviors.Common.SnapWindowBehavior.Create(typeof(DevExpress.Utils.BehaviorSource.SnapWindowBehaviorSourceForForm), snapOptions1))),
            ((DevExpress.Utils.Behaviors.Behavior)(DevExpress.Utils.Behaviors.Common.PersistenceBehavior.Create(typeof(DevExpress.Utils.BehaviorSource.PersistenceBehaviorSourceForForm), null, DevExpress.Utils.Behaviors.Common.Storage.Registry, DevExpress.Utils.DefaultBoolean.Default)))});
            this.ClientSize = new System.Drawing.Size(810, 472);
            this.Controls.Add(this.DevicesLayoutControl);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.DoubleBuffered = true;
            this.HtmlText = "Devices <size=-3><backcolor=#252526>  BETA  </backcolor></size>";
            this.IconOptions.SvgImage = global::GUX.Properties.Resources.vectorpaint;
            this.Name = "DevicesContainer";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DevicesContainer_FormClosing);
            this.Load += new System.EventHandler(this.DevicesContainer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DevicesLayoutControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DevicesGroupLayout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem stopDevicesButton;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.BarStaticItem DevicesCountLabel;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl DevicesLayoutControl;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlGroup DevicesGroupLayout;
        private DevExpress.XtraBars.BarButtonItem fitDevicesButton;
        private DevExpress.Utils.Behaviors.BehaviorManager behaviorManager1;
        private DevExpress.XtraBars.BarButtonItem autoFitDevicesButton;
        private DevExpress.XtraBars.BarEditItem barEditItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemColorEdit repositoryItemColorEdit1;
    }
}
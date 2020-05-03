namespace GUX.UC
{
    partial class iTools
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
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions1 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.iButton = new DevExpress.XtraEditors.ButtonEdit();
            ((System.ComponentModel.ISupportInitialize)(this.iButton.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // iButton
            // 
            this.iButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iButton.EditValue = ". . .";
            this.iButton.Location = new System.Drawing.Point(0, 0);
            this.iButton.Margin = new System.Windows.Forms.Padding(0);
            this.iButton.Name = "iButton";
            this.iButton.Properties.AllowFocused = false;
            this.iButton.Properties.Appearance.Font = new System.Drawing.Font("Calibri", 10F);
            this.iButton.Properties.Appearance.Options.UseFont = true;
            this.iButton.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            editorButtonImageOptions1.SvgImage = global::GUX.Properties.Resources.removesheetrows;
            editorButtonImageOptions1.SvgImageSize = new System.Drawing.Size(14, 14);
            this.iButton.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions1, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Delete Scenario", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.iButton.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.iButton.Size = new System.Drawing.Size(188, 20);
            this.iButton.TabIndex = 1;
            this.iButton.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.iButton_ButtonClick);
            // 
            // iTools
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.iButton);
            this.DoubleBuffered = true;
            this.Name = "iTools";
            this.Size = new System.Drawing.Size(188, 20);
            ((System.ComponentModel.ISupportInitialize)(this.iButton.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ButtonEdit iButton;
    }
}

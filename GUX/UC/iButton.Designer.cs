namespace GUX.UC
{
    partial class iButton
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
            this.iconLabel = new System.Windows.Forms.Label();
            this.textLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // iconLabel
            // 
            this.iconLabel.BackColor = System.Drawing.Color.Transparent;
            this.iconLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.iconLabel.Location = new System.Drawing.Point(0, 0);
            this.iconLabel.Name = "iconLabel";
            this.iconLabel.Size = new System.Drawing.Size(25, 29);
            this.iconLabel.TabIndex = 0;
            this.iconLabel.Text = "O";
            this.iconLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconLabel.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.iconLabel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            // 
            // textLabel
            // 
            this.textLabel.BackColor = System.Drawing.Color.Transparent;
            this.textLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.textLabel.Location = new System.Drawing.Point(21, 0);
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(76, 29);
            this.textLabel.TabIndex = 1;
            this.textLabel.Text = "label2";
            this.textLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.textLabel.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.textLabel.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            // 
            // iButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Controls.Add(this.iconLabel);
            this.Controls.Add(this.textLabel);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Name = "iButton";
            this.Size = new System.Drawing.Size(97, 29);
            this.Enter += new System.EventHandler(this.OnMouseEnter);
            this.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label iconLabel;
        private System.Windows.Forms.Label textLabel;
    }
}

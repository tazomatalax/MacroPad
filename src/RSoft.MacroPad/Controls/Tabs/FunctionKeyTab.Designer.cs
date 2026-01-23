namespace RSoft.MacroPad.Controls.Tabs
{
    partial class FunctionKeyTab
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
            this.rbF13 = new System.Windows.Forms.RadioButton();
            this.rbF14 = new System.Windows.Forms.RadioButton();
            this.rbF15 = new System.Windows.Forms.RadioButton();
            this.rbF16 = new System.Windows.Forms.RadioButton();
            this.rbF17 = new System.Windows.Forms.RadioButton();
            this.rbF18 = new System.Windows.Forms.RadioButton();
            this.lblInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rbF13
            // 
            this.rbF13.AutoSize = true;
            this.rbF13.Location = new System.Drawing.Point(4, 30);
            this.rbF13.Name = "rbF13";
            this.rbF13.Size = new System.Drawing.Size(140, 17);
            this.rbF13.TabIndex = 0;
            this.rbF13.TabStop = true;
            this.rbF13.Text = "F13 (Key 1 / Button 1)";
            this.rbF13.UseVisualStyleBackColor = true;
            this.rbF13.Click += new System.EventHandler(this.KeyChanged);
            // 
            // rbF14
            // 
            this.rbF14.AutoSize = true;
            this.rbF14.Location = new System.Drawing.Point(4, 53);
            this.rbF14.Name = "rbF14";
            this.rbF14.Size = new System.Drawing.Size(140, 17);
            this.rbF14.TabIndex = 0;
            this.rbF14.TabStop = true;
            this.rbF14.Text = "F14 (Key 2 / Button 2)";
            this.rbF14.UseVisualStyleBackColor = true;
            this.rbF14.Click += new System.EventHandler(this.KeyChanged);
            // 
            // rbF15
            // 
            this.rbF15.AutoSize = true;
            this.rbF15.Location = new System.Drawing.Point(4, 76);
            this.rbF15.Name = "rbF15";
            this.rbF15.Size = new System.Drawing.Size(140, 17);
            this.rbF15.TabIndex = 0;
            this.rbF15.TabStop = true;
            this.rbF15.Text = "F15 (Key 3 / Button 3)";
            this.rbF15.UseVisualStyleBackColor = true;
            this.rbF15.Click += new System.EventHandler(this.KeyChanged);
            // 
            // rbF16
            // 
            this.rbF16.AutoSize = true;
            this.rbF16.Location = new System.Drawing.Point(200, 30);
            this.rbF16.Name = "rbF16";
            this.rbF16.Size = new System.Drawing.Size(101, 17);
            this.rbF16.TabIndex = 0;
            this.rbF16.TabStop = true;
            this.rbF16.Text = "F16 (Knob Left)";
            this.rbF16.UseVisualStyleBackColor = true;
            this.rbF16.Click += new System.EventHandler(this.KeyChanged);
            // 
            // rbF17
            // 
            this.rbF17.AutoSize = true;
            this.rbF17.Location = new System.Drawing.Point(200, 53);
            this.rbF17.Name = "rbF17";
            this.rbF17.Size = new System.Drawing.Size(107, 17);
            this.rbF17.TabIndex = 0;
            this.rbF17.TabStop = true;
            this.rbF17.Text = "F17 (Knob Right)";
            this.rbF17.UseVisualStyleBackColor = true;
            this.rbF17.Click += new System.EventHandler(this.KeyChanged);
            // 
            // rbF18
            // 
            this.rbF18.AutoSize = true;
            this.rbF18.Location = new System.Drawing.Point(200, 76);
            this.rbF18.Name = "rbF18";
            this.rbF18.Size = new System.Drawing.Size(106, 17);
            this.rbF18.TabIndex = 0;
            this.rbF18.TabStop = true;
            this.rbF18.Text = "F18 (Knob Click)";
            this.rbF18.UseVisualStyleBackColor = true;
            this.rbF18.Click += new System.EventHandler(this.KeyChanged);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(4, 4);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(376, 13);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "Function keys (F13-F18) for workflow automation. F18 cycles modes when pressed.";
            // 
            // FunctionKeyTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.rbF18);
            this.Controls.Add(this.rbF15);
            this.Controls.Add(this.rbF17);
            this.Controls.Add(this.rbF14);
            this.Controls.Add(this.rbF16);
            this.Controls.Add(this.rbF13);
            this.MaximumSize = new System.Drawing.Size(1800, 120);
            this.MinimumSize = new System.Drawing.Size(800, 120);
            this.Name = "FunctionKeyTab";
            this.Size = new System.Drawing.Size(800, 120);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.RadioButton rbF13;
        private System.Windows.Forms.RadioButton rbF14;
        private System.Windows.Forms.RadioButton rbF15;
        private System.Windows.Forms.RadioButton rbF16;
        private System.Windows.Forms.RadioButton rbF17;
        private System.Windows.Forms.RadioButton rbF18;
        private System.Windows.Forms.Label lblInfo;
    }
}

namespace CastScreenServer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbInterface = new System.Windows.Forms.ComboBox();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.btnStartStop = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.SuspendLayout();
            // 
            // cbInterface
            // 
            this.cbInterface.FormattingEnabled = true;
            this.cbInterface.Location = new System.Drawing.Point(55, 74);
            this.cbInterface.Name = "cbInterface";
            this.cbInterface.Size = new System.Drawing.Size(282, 23);
            this.cbInterface.TabIndex = 0;
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(361, 75);
            this.numPort.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(49, 23);
            this.numPort.TabIndex = 1;
            this.numPort.Value = new decimal(new int[] {
            8546,
            0,
            0,
            0});
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(55, 161);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(242, 23);
            this.txtURL.TabIndex = 2;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(441, 317);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(75, 23);
            this.btnStartStop.TabIndex = 5;
            this.btnStartStop.Tag = "start";
            this.btnStartStop.Text = "Start Cast";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_ClickAsync);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 437);
            this.Controls.Add(this.numPort);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.txtURL);
            this.Controls.Add(this.cbInterface);
            this.Name = "MainForm";
            this.Text = "CastScreenServer";
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbInterface;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Button btnStartStop;
    }
}


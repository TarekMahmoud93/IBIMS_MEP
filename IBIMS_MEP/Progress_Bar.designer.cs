namespace IBIMS_MEP
{
    partial class Fpb
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
            this.pbr = new System.Windows.Forms.ProgressBar();
            this.Lb = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pbr
            // 
            this.pbr.Location = new System.Drawing.Point(12, 26);
            this.pbr.Name = "pbr";
            this.pbr.Size = new System.Drawing.Size(506, 36);
            this.pbr.Step = 1;
            this.pbr.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbr.TabIndex = 0;
            // 
            // Lb
            // 
            this.Lb.AutoSize = true;
            this.Lb.BackColor = System.Drawing.Color.Transparent;
            this.Lb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Lb.Font = new System.Drawing.Font("Lucida Bright", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Lb.Location = new System.Drawing.Point(222, 80);
            this.Lb.Name = "Lb";
            this.Lb.Size = new System.Drawing.Size(63, 19);
            this.Lb.TabIndex = 1;
            this.Lb.Text = "label1";
            this.Lb.UseMnemonic = false;
            // 
            // Fpb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 124);
            this.ControlBox = false;
            this.Controls.Add(this.Lb);
            this.Controls.Add(this.pbr);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Fpb";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Progress_Bar";
            this.Load += new System.EventHandler(this.Progress_Bar_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ProgressBar pbr;
        public System.Windows.Forms.Label Lb;
    }
}
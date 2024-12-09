namespace Dekosis___KYS
{
    partial class Form4
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form4));
            this.button1 = new System.Windows.Forms.Button();
            this.cameraBox = new Emgu.CV.UI.ImageBox();
            this.camStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.bitirButon = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.cameraBox)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 49);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cameraBox
            // 
            this.cameraBox.Enabled = false;
            this.cameraBox.Location = new System.Drawing.Point(79, 12);
            this.cameraBox.Name = "cameraBox";
            this.cameraBox.Size = new System.Drawing.Size(640, 480);
            this.cameraBox.TabIndex = 2;
            this.cameraBox.TabStop = false;
            // 
            // camStart
            // 
            this.camStart.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.camStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.818182F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.camStart.Image = ((System.Drawing.Image)(resources.GetObject("camStart.Image")));
            this.camStart.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.camStart.Location = new System.Drawing.Point(725, 12);
            this.camStart.Name = "camStart";
            this.camStart.Size = new System.Drawing.Size(105, 61);
            this.camStart.TabIndex = 3;
            this.camStart.Text = "Kamerayı Aç";
            this.camStart.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.camStart.UseVisualStyleBackColor = true;
            this.camStart.Click += new System.EventHandler(this.camStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label1.Font = new System.Drawing.Font("Verdana", 13.74545F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(794, 178);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(2, 27);
            this.label1.TabIndex = 4;
            // 
            // okButton
            // 
            this.okButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.818182F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.okButton.Image = ((System.Drawing.Image)(resources.GetObject("okButton.Image")));
            this.okButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.okButton.Location = new System.Drawing.Point(1049, 271);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(95, 65);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "Tamam";
            this.okButton.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // bitirButon
            // 
            this.bitirButon.Cursor = System.Windows.Forms.Cursors.Default;
            this.bitirButon.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bitirButon.Font = new System.Drawing.Font("Verdana", 9.818182F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.bitirButon.Image = ((System.Drawing.Image)(resources.GetObject("bitirButon.Image")));
            this.bitirButon.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.bitirButon.Location = new System.Drawing.Point(1049, 424);
            this.bitirButon.Name = "bitirButon";
            this.bitirButon.Size = new System.Drawing.Size(95, 80);
            this.bitirButon.TabIndex = 6;
            this.bitirButon.Text = "Dersi Bitir";
            this.bitirButon.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.bitirButon.UseVisualStyleBackColor = true;
            this.bitirButon.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1156, 516);
            this.Controls.Add(this.bitirButon);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.camStart);
            this.Controls.Add(this.cameraBox);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form4";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Yoklama";
            ((System.ComponentModel.ISupportInitialize)(this.cameraBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private Emgu.CV.UI.ImageBox cameraBox;
        private System.Windows.Forms.Button camStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button bitirButon;
    }
}
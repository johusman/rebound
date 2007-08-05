namespace Rebound
{
    partial class frmRebound
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
            if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRebound));
            this.btnProcess = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblProcessing = new System.Windows.Forms.Label();
            this.rmsOutput = new Rebound.RMSControl();
            this.rmsInput = new Rebound.RMSControl();
            this.prgProcessing = new System.Windows.Forms.ProgressBar();
            this.rabDirect = new System.Windows.Forms.RadioButton();
            this.rabImpulseReponse = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.water1 = new Rebound.WaterControl();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnProcess
            // 
            this.btnProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProcess.Location = new System.Drawing.Point(109, 65);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 23);
            this.btnProcess.TabIndex = 1;
            this.btnProcess.Text = "Process...";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblProcessing);
            this.groupBox1.Controls.Add(this.rmsOutput);
            this.groupBox1.Controls.Add(this.rmsInput);
            this.groupBox1.Controls.Add(this.prgProcessing);
            this.groupBox1.Location = new System.Drawing.Point(12, 219);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(190, 155);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Progress";
            // 
            // lblProcessing
            // 
            this.lblProcessing.AutoSize = true;
            this.lblProcessing.Location = new System.Drawing.Point(6, 134);
            this.lblProcessing.Name = "lblProcessing";
            this.lblProcessing.Size = new System.Drawing.Size(0, 13);
            this.lblProcessing.TabIndex = 5;
            // 
            // rmsOutput
            // 
            this.rmsOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rmsOutput.Gain = 4;
            this.rmsOutput.Incremental = true;
            this.rmsOutput.Location = new System.Drawing.Point(6, 69);
            this.rmsOutput.Name = "rmsOutput";
            this.rmsOutput.Rms = null;
            this.rmsOutput.Size = new System.Drawing.Size(177, 40);
            this.rmsOutput.TabIndex = 4;
            // 
            // rmsInput
            // 
            this.rmsInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rmsInput.Gain = 2;
            this.rmsInput.Incremental = false;
            this.rmsInput.Location = new System.Drawing.Point(6, 19);
            this.rmsInput.Name = "rmsInput";
            this.rmsInput.Rms = null;
            this.rmsInput.Size = new System.Drawing.Size(177, 44);
            this.rmsInput.TabIndex = 3;
            // 
            // prgProcessing
            // 
            this.prgProcessing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prgProcessing.Location = new System.Drawing.Point(7, 115);
            this.prgProcessing.Name = "prgProcessing";
            this.prgProcessing.Size = new System.Drawing.Size(177, 16);
            this.prgProcessing.Step = 1;
            this.prgProcessing.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.prgProcessing.TabIndex = 2;
            // 
            // rabDirect
            // 
            this.rabDirect.AutoSize = true;
            this.rabDirect.Location = new System.Drawing.Point(9, 42);
            this.rabDirect.Name = "rabDirect";
            this.rabDirect.Size = new System.Drawing.Size(53, 17);
            this.rabDirect.TabIndex = 6;
            this.rabDirect.Text = "Direct";
            this.rabDirect.UseVisualStyleBackColor = true;
            // 
            // rabImpulseReponse
            // 
            this.rabImpulseReponse.AutoSize = true;
            this.rabImpulseReponse.Checked = true;
            this.rabImpulseReponse.Location = new System.Drawing.Point(9, 19);
            this.rabImpulseReponse.Name = "rabImpulseReponse";
            this.rabImpulseReponse.Size = new System.Drawing.Size(121, 17);
            this.rabImpulseReponse.TabIndex = 7;
            this.rabImpulseReponse.TabStop = true;
            this.rabImpulseReponse.Text = "By impulse response";
            this.rabImpulseReponse.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.rabImpulseReponse);
            this.groupBox2.Controls.Add(this.rabDirect);
            this.groupBox2.Controls.Add(this.btnProcess);
            this.groupBox2.Location = new System.Drawing.Point(12, 380);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(190, 97);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Processing";
            // 
            // water1
            // 
            this.water1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.water1.Gain = 0;
            this.water1.Location = new System.Drawing.Point(12, 12);
            this.water1.Name = "water1";
            this.water1.RenderingMode = Rebound.WaterControl.RenderingModeEnum.WaveHeight;
            this.water1.Size = new System.Drawing.Size(190, 195);
            this.water1.TabIndex = 2;
            // 
            // frmRebound
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 489);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.water1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmRebound";
            this.Text = "Rebound";
            this.ResizeEnd += new System.EventHandler(this.frmRebound_ResizeEnd);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnProcess;
        private WaterControl water1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar prgProcessing;
        private RMSControl rmsInput;
        private RMSControl rmsOutput;
        private System.Windows.Forms.RadioButton rabImpulseReponse;
        private System.Windows.Forms.RadioButton rabDirect;
        private System.Windows.Forms.Label lblProcessing;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}


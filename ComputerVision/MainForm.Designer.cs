namespace ComputerVision
{
    partial class MainForm
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
            this.panelSource = new System.Windows.Forms.Panel();
            this.panelDestination = new System.Windows.Forms.Panel();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbGrayscale = new System.Windows.Forms.ComboBox();
            this.btNegative = new System.Windows.Forms.Button();
            this.btReset = new System.Windows.Forms.Button();
            this.btGrayscale = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.tbBrightness = new System.Windows.Forms.TrackBar();
            this.lbBrightness = new System.Windows.Forms.Label();
            this.lbContrast = new System.Windows.Forms.Label();
            this.tbContrast = new System.Windows.Forms.TrackBar();
            this.btHistoEqGs = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbContrast)).BeginInit();
            this.SuspendLayout();
            // 
            // panelSource
            // 
            this.panelSource.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelSource.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panelSource.Location = new System.Drawing.Point(12, 12);
            this.panelSource.Name = "panelSource";
            this.panelSource.Size = new System.Drawing.Size(320, 240);
            this.panelSource.TabIndex = 0;
            // 
            // panelDestination
            // 
            this.panelDestination.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelDestination.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelDestination.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panelDestination.Location = new System.Drawing.Point(348, 12);
            this.panelDestination.Name = "panelDestination";
            this.panelDestination.Size = new System.Drawing.Size(320, 240);
            this.panelDestination.TabIndex = 1;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(12, 439);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 2;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cbGrayscale);
            this.panel1.Controls.Add(this.btHistoEqGs);
            this.panel1.Controls.Add(this.btNegative);
            this.panel1.Controls.Add(this.btReset);
            this.panel1.Controls.Add(this.btGrayscale);
            this.panel1.Location = new System.Drawing.Point(348, 271);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(320, 190);
            this.panel1.TabIndex = 3;
            // 
            // cbGrayscale
            // 
            this.cbGrayscale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGrayscale.FormattingEnabled = true;
            this.cbGrayscale.Items.AddRange(new object[] {
            "AVG",
            "Formula"});
            this.cbGrayscale.Location = new System.Drawing.Point(84, 3);
            this.cbGrayscale.Name = "cbGrayscale";
            this.cbGrayscale.Size = new System.Drawing.Size(121, 21);
            this.cbGrayscale.TabIndex = 14;
            // 
            // btNegative
            // 
            this.btNegative.Location = new System.Drawing.Point(3, 32);
            this.btNegative.Name = "btNegative";
            this.btNegative.Size = new System.Drawing.Size(75, 23);
            this.btNegative.TabIndex = 13;
            this.btNegative.Text = "Negative";
            this.btNegative.UseVisualStyleBackColor = true;
            this.btNegative.Click += new System.EventHandler(this.btNegative_Click);
            // 
            // btReset
            // 
            this.btReset.Location = new System.Drawing.Point(240, 3);
            this.btReset.Name = "btReset";
            this.btReset.Size = new System.Drawing.Size(75, 23);
            this.btReset.TabIndex = 13;
            this.btReset.Text = "Reset";
            this.btReset.UseVisualStyleBackColor = true;
            this.btReset.Click += new System.EventHandler(this.btReset_Click);
            // 
            // btGrayscale
            // 
            this.btGrayscale.Location = new System.Drawing.Point(3, 3);
            this.btGrayscale.Name = "btGrayscale";
            this.btGrayscale.Size = new System.Drawing.Size(75, 23);
            this.btGrayscale.TabIndex = 13;
            this.btGrayscale.Text = "Grayscale";
            this.btGrayscale.UseVisualStyleBackColor = true;
            this.btGrayscale.Click += new System.EventHandler(this.btGrayscale_Click);
            // 
            // tbBrightness
            // 
            this.tbBrightness.LargeChange = 10;
            this.tbBrightness.Location = new System.Drawing.Point(12, 275);
            this.tbBrightness.Maximum = 255;
            this.tbBrightness.Minimum = -255;
            this.tbBrightness.Name = "tbBrightness";
            this.tbBrightness.Size = new System.Drawing.Size(320, 45);
            this.tbBrightness.TabIndex = 15;
            this.tbBrightness.TickFrequency = 10;
            this.tbBrightness.ValueChanged += new System.EventHandler(this.tbBrightness_ValueChanged);
            // 
            // lbBrightness
            // 
            this.lbBrightness.AutoSize = true;
            this.lbBrightness.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBrightness.Location = new System.Drawing.Point(12, 255);
            this.lbBrightness.Name = "lbBrightness";
            this.lbBrightness.Size = new System.Drawing.Size(75, 17);
            this.lbBrightness.TabIndex = 15;
            this.lbBrightness.Text = "Brightness";
            // 
            // lbContrast
            // 
            this.lbContrast.AutoSize = true;
            this.lbContrast.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbContrast.Location = new System.Drawing.Point(12, 323);
            this.lbContrast.Name = "lbContrast";
            this.lbContrast.Size = new System.Drawing.Size(61, 17);
            this.lbContrast.TabIndex = 15;
            this.lbContrast.Text = "Contrast";
            // 
            // tbContrast
            // 
            this.tbContrast.LargeChange = 10;
            this.tbContrast.Location = new System.Drawing.Point(12, 343);
            this.tbContrast.Maximum = 255;
            this.tbContrast.Minimum = -255;
            this.tbContrast.Name = "tbContrast";
            this.tbContrast.Size = new System.Drawing.Size(320, 45);
            this.tbContrast.TabIndex = 15;
            this.tbContrast.TickFrequency = 10;
            this.tbContrast.ValueChanged += new System.EventHandler(this.tbContrast_ValueChanged);
            // 
            // btHistoEqGs
            // 
            this.btHistoEqGs.Location = new System.Drawing.Point(3, 61);
            this.btHistoEqGs.Name = "btHistoEqGs";
            this.btHistoEqGs.Size = new System.Drawing.Size(75, 23);
            this.btHistoEqGs.TabIndex = 13;
            this.btHistoEqGs.Text = "Histo Eq Gs";
            this.btHistoEqGs.UseVisualStyleBackColor = true;
            this.btHistoEqGs.Click += new System.EventHandler(this.btHistoEqGs_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 473);
            this.Controls.Add(this.tbContrast);
            this.Controls.Add(this.tbBrightness);
            this.Controls.Add(this.lbContrast);
            this.Controls.Add(this.lbBrightness);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.panelDestination);
            this.Controls.Add(this.panelSource);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbContrast)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelSource;
        private System.Windows.Forms.Panel panelDestination;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btGrayscale;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btNegative;
        private System.Windows.Forms.TrackBar tbBrightness;
        private System.Windows.Forms.Label lbBrightness;
        private System.Windows.Forms.ComboBox cbGrayscale;
        private System.Windows.Forms.Label lbContrast;
        private System.Windows.Forms.TrackBar tbContrast;
        private System.Windows.Forms.Button btReset;
        private System.Windows.Forms.Button btHistoEqGs;
    }
}


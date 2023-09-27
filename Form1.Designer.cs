namespace AGV_YOLOV5
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            btnOpen = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            pictureBox2 = new PictureBox();
            btnTemp = new Button();
            txtCOM = new TextBox();
            txtCOML = new TextBox();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // btnOpen
            // 
            btnOpen.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOpen.BackColor = Color.Turquoise;
            btnOpen.Location = new Point(1185, 469);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(67, 24);
            btnOpen.TabIndex = 1;
            btnOpen.Text = "YOLO";
            btnOpen.UseVisualStyleBackColor = false;
            btnOpen.Click += btnOpen_Click;
            // 
            // timer1
            // 
            timer1.Interval = 30;
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.Location = new Point(-1, 1);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(1265, 680);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
            // 
            // btnTemp
            // 
            btnTemp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnTemp.BackColor = Color.Turquoise;
            btnTemp.Location = new Point(1185, 503);
            btnTemp.Name = "btnTemp";
            btnTemp.Size = new Size(67, 24);
            btnTemp.TabIndex = 4;
            btnTemp.Text = "温度采集";
            btnTemp.UseVisualStyleBackColor = false;
            btnTemp.Click += btnTemp_Click;
            // 
            // txtCOM
            // 
            txtCOM.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            txtCOM.BackColor = Color.SlateGray;
            txtCOM.BorderStyle = BorderStyle.None;
            txtCOM.Font = new Font("Microsoft YaHei UI Light", 7.5F, FontStyle.Regular, GraphicsUnit.Point);
            txtCOM.ForeColor = Color.Cornsilk;
            txtCOM.Location = new Point(1149, 508);
            txtCOM.Name = "txtCOM";
            txtCOM.Size = new Size(37, 14);
            txtCOM.TabIndex = 6;
            txtCOM.Text = "COM3";
            // 
            // txtCOML
            // 
            txtCOML.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            txtCOML.BackColor = Color.SlateGray;
            txtCOML.BorderStyle = BorderStyle.None;
            txtCOML.Font = new Font("Microsoft YaHei UI Light", 7.5F, FontStyle.Regular, GraphicsUnit.Point);
            txtCOML.ForeColor = Color.Cornsilk;
            txtCOML.Location = new Point(1149, 542);
            txtCOML.Name = "txtCOML";
            txtCOML.Size = new Size(37, 14);
            txtCOML.TabIndex = 8;
            txtCOML.Text = "COM5";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.BackColor = Color.Turquoise;
            button1.Location = new Point(1185, 537);
            button1.Name = "button1";
            button1.Size = new Size(67, 24);
            button1.TabIndex = 7;
            button1.Text = "光线采集";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(1264, 681);
            Controls.Add(txtCOML);
            Controls.Add(button1);
            Controls.Add(txtCOM);
            Controls.Add(btnTemp);
            Controls.Add(btnOpen);
            Controls.Add(pictureBox2);
            Name = "Form1";
            Text = "自动驾驶车载感知层";
            FormClosing += Form1_FormClosing;
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnOpen;
        private System.Windows.Forms.Timer timer1;
        private PictureBox pictureBox2;
        private Button btnTemp;
        private TextBox txtCOM;
        private TextBox txtCOML;
        private Button button1;
    }
}
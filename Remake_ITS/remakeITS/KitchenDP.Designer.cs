namespace remakeITS
{
    partial class KitchenDP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KitchenDP));
            this.panel15 = new System.Windows.Forms.Panel();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.lb_activeuser = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btn_minimize = new System.Windows.Forms.Label();
            this.btn_exitKD = new System.Windows.Forms.Label();
            this.btn_maximize = new System.Windows.Forms.Label();
            this.panel15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // panel15
            // 
            this.panel15.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel15.BackColor = System.Drawing.Color.Crimson;
            this.panel15.Controls.Add(this.btn_maximize);
            this.panel15.Controls.Add(this.btn_minimize);
            this.panel15.Controls.Add(this.btn_exitKD);
            this.panel15.Controls.Add(this.lb_activeuser);
            this.panel15.Controls.Add(this.pictureBox4);
            this.panel15.Location = new System.Drawing.Point(0, 0);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(1237, 43);
            this.panel15.TabIndex = 19;
            this.panel15.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel15_MouseMove);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(7, 6);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(42, 33);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 30;
            this.pictureBox4.TabStop = false;
            // 
            // lb_activeuser
            // 
            this.lb_activeuser.AutoSize = true;
            this.lb_activeuser.BackColor = System.Drawing.Color.Transparent;
            this.lb_activeuser.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_activeuser.ForeColor = System.Drawing.Color.White;
            this.lb_activeuser.Location = new System.Drawing.Point(49, 12);
            this.lb_activeuser.Name = "lb_activeuser";
            this.lb_activeuser.Size = new System.Drawing.Size(161, 25);
            this.lb_activeuser.TabIndex = 29;
            this.lb_activeuser.Text = "Kitchen Display";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Black;
            this.flowLayoutPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 46);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1232, 598);
            this.flowLayoutPanel1.TabIndex = 20;
            // 
            // btn_minimize
            // 
            this.btn_minimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_minimize.AutoSize = true;
            this.btn_minimize.BackColor = System.Drawing.Color.Crimson;
            this.btn_minimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_minimize.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_minimize.ForeColor = System.Drawing.Color.Transparent;
            this.btn_minimize.Location = new System.Drawing.Point(1141, 6);
            this.btn_minimize.Name = "btn_minimize";
            this.btn_minimize.Size = new System.Drawing.Size(25, 25);
            this.btn_minimize.TabIndex = 116;
            this.btn_minimize.Text = "_";
            this.btn_minimize.Click += new System.EventHandler(this.btn_minimize_Click_1);
            this.btn_minimize.MouseEnter += new System.EventHandler(this.btn_minimize_MouseEnter);
            this.btn_minimize.MouseLeave += new System.EventHandler(this.btn_minimize_MouseLeave);
            // 
            // btn_exitKD
            // 
            this.btn_exitKD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_exitKD.AutoSize = true;
            this.btn_exitKD.BackColor = System.Drawing.Color.Crimson;
            this.btn_exitKD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_exitKD.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.btn_exitKD.ForeColor = System.Drawing.Color.Transparent;
            this.btn_exitKD.Location = new System.Drawing.Point(1204, 10);
            this.btn_exitKD.Name = "btn_exitKD";
            this.btn_exitKD.Size = new System.Drawing.Size(27, 25);
            this.btn_exitKD.TabIndex = 115;
            this.btn_exitKD.Text = "X";
            this.btn_exitKD.Click += new System.EventHandler(this.btn_exitKD_Click_1);
            this.btn_exitKD.MouseEnter += new System.EventHandler(this.btn_exitKD_MouseEnter);
            this.btn_exitKD.MouseLeave += new System.EventHandler(this.btn_exitKD_MouseLeave);
            // 
            // btn_maximize
            // 
            this.btn_maximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_maximize.AutoSize = true;
            this.btn_maximize.BackColor = System.Drawing.Color.Crimson;
            this.btn_maximize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_maximize.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_maximize.ForeColor = System.Drawing.Color.Transparent;
            this.btn_maximize.Location = new System.Drawing.Point(1171, 7);
            this.btn_maximize.Name = "btn_maximize";
            this.btn_maximize.Size = new System.Drawing.Size(33, 29);
            this.btn_maximize.TabIndex = 117;
            this.btn_maximize.Text = "🗖";
            this.btn_maximize.Click += new System.EventHandler(this.btn_maximize_Click);
            this.btn_maximize.MouseEnter += new System.EventHandler(this.btn_maximize_MouseEnter);
            this.btn_maximize.MouseLeave += new System.EventHandler(this.btn_maximize_MouseLeave);
            // 
            // KitchenDP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkRed;
            this.ClientSize = new System.Drawing.Size(1238, 647);
            this.Controls.Add(this.panel15);
            this.Controls.Add(this.flowLayoutPanel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "KitchenDP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KitchenDP";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KitchenDP_FormClosing);
            this.Load += new System.EventHandler(this.KitchenDP_Load);
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel15;
        private System.Windows.Forms.PictureBox pictureBox4;
        public System.Windows.Forms.Label lb_activeuser;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label btn_minimize;
        private System.Windows.Forms.Label btn_exitKD;
        private System.Windows.Forms.Label btn_maximize;
    }
}
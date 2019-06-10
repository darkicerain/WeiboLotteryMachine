namespace WeiboLotteryMachine.Win
{
    partial class MainFormView
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label labelInterval;
            this.groupBoxSet = new System.Windows.Forms.GroupBox();
            this.textBoxInterval = new System.Windows.Forms.TextBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.pictureBoxHeader = new System.Windows.Forms.PictureBox();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.textBoxYdmUser = new System.Windows.Forms.TextBox();
            this.textBoxYdmPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            labelInterval = new System.Windows.Forms.Label();
            this.groupBoxSet.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHeader)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(86, 38);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(47, 12);
            label1.TabIndex = 0;
            label1.Text = " 账号：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(92, 69);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(41, 12);
            label2.TabIndex = 1;
            label2.Text = "密码：";
            // 
            // labelInterval
            // 
            labelInterval.AutoSize = true;
            labelInterval.Location = new System.Drawing.Point(332, 38);
            labelInterval.Name = "labelInterval";
            labelInterval.Size = new System.Drawing.Size(65, 12);
            labelInterval.TabIndex = 8;
            labelInterval.Text = "时间间隔：";
            // 
            // groupBoxSet
            // 
            this.groupBoxSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSet.Controls.Add(this.label3);
            this.groupBoxSet.Controls.Add(this.textBoxYdmPassword);
            this.groupBoxSet.Controls.Add(this.textBoxYdmUser);
            this.groupBoxSet.Controls.Add(this.textBoxInterval);
            this.groupBoxSet.Controls.Add(labelInterval);
            this.groupBoxSet.Controls.Add(this.buttonStart);
            this.groupBoxSet.Controls.Add(this.pictureBoxHeader);
            this.groupBoxSet.Controls.Add(this.buttonStop);
            this.groupBoxSet.Controls.Add(this.buttonLogin);
            this.groupBoxSet.Controls.Add(this.textBoxPassword);
            this.groupBoxSet.Controls.Add(this.textBoxUsername);
            this.groupBoxSet.Controls.Add(label2);
            this.groupBoxSet.Controls.Add(label1);
            this.groupBoxSet.Location = new System.Drawing.Point(13, 13);
            this.groupBoxSet.Name = "groupBoxSet";
            this.groupBoxSet.Size = new System.Drawing.Size(775, 109);
            this.groupBoxSet.TabIndex = 0;
            this.groupBoxSet.TabStop = false;
            this.groupBoxSet.Text = "未登录";
            // 
            // textBoxInterval
            // 
            this.textBoxInterval.Enabled = false;
            this.textBoxInterval.Location = new System.Drawing.Point(407, 35);
            this.textBoxInterval.Name = "textBoxInterval";
            this.textBoxInterval.Size = new System.Drawing.Size(53, 21);
            this.textBoxInterval.TabIndex = 9;
            this.textBoxInterval.Text = "30";
            // 
            // buttonStart
            // 
            this.buttonStart.Enabled = false;
            this.buttonStart.Location = new System.Drawing.Point(633, 21);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(104, 35);
            this.buttonStart.TabIndex = 7;
            this.buttonStart.Text = "开始转发";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // pictureBoxHeader
            // 
            this.pictureBoxHeader.Location = new System.Drawing.Point(12, 25);
            this.pictureBoxHeader.Name = "pictureBoxHeader";
            this.pictureBoxHeader.Size = new System.Drawing.Size(70, 70);
            this.pictureBoxHeader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxHeader.TabIndex = 6;
            this.pictureBoxHeader.TabStop = false;
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(633, 62);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(104, 35);
            this.buttonStop.TabIndex = 5;
            this.buttonStop.Text = "停止转发";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonLogin
            // 
            this.buttonLogin.Enabled = false;
            this.buttonLogin.Location = new System.Drawing.Point(245, 35);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(62, 52);
            this.buttonLogin.TabIndex = 4;
            this.buttonLogin.Text = "登录";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(139, 66);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(100, 21);
            this.textBoxPassword.TabIndex = 3;
            this.textBoxPassword.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(139, 35);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(100, 21);
            this.textBoxUsername.TabIndex = 2;
            this.textBoxUsername.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxOutput.Location = new System.Drawing.Point(13, 128);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.ReadOnly = true;
            this.richTextBoxOutput.Size = new System.Drawing.Size(775, 310);
            this.richTextBoxOutput.TabIndex = 1;
            this.richTextBoxOutput.Text = "";
            // 
            // textBoxYdmUser
            // 
            this.textBoxYdmUser.Location = new System.Drawing.Point(391, 66);
            this.textBoxYdmUser.Name = "textBoxYdmUser";
            this.textBoxYdmUser.Size = new System.Drawing.Size(100, 21);
            this.textBoxYdmUser.TabIndex = 10;
            // 
            // textBoxYdmPassword
            // 
            this.textBoxYdmPassword.Location = new System.Drawing.Point(497, 66);
            this.textBoxYdmPassword.Name = "textBoxYdmPassword";
            this.textBoxYdmPassword.PasswordChar = '*';
            this.textBoxYdmPassword.Size = new System.Drawing.Size(100, 21);
            this.textBoxYdmPassword.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(332, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "云打码：";
            // 
            // MainFormView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.richTextBoxOutput);
            this.Controls.Add(this.groupBoxSet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainFormView";
            this.ShowIcon = false;
            this.Text = "微博抽奖机";
            this.Load += new System.EventHandler(this.MainFormView_Load);
            this.groupBoxSet.ResumeLayout(false);
            this.groupBoxSet.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHeader)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSet;
        private System.Windows.Forms.RichTextBox richTextBoxOutput;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.PictureBox pictureBoxHeader;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxInterval;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxYdmPassword;
        private System.Windows.Forms.TextBox textBoxYdmUser;
    }
}


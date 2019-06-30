﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeiboLotteryMachine.Win.Properties;

namespace WeiboLotteryMachine.Win
{
    public partial class MainFormView : Form
    {
        Model.User User { get; set; }
        Timer ForwardTimer = new Timer() { Interval = 10000 }; //默认10分钟
        Timer UpdateCookieTimer = new Timer() { Interval = 3600000 };   //1小时
        public MainFormView()
        {
            InitializeComponent();
        }

        private void MainFormView_Load(object sender, EventArgs e)
        {
            this.Text = "微博抽奖机 v" + SoftwareInfo.Version;
            this.ForwardTimer.Tick += ForwardTimer_Tick;
            this.UpdateCookieTimer.Tick += UpdateCookieTimer_Tick;

            BLL.ForwardDb.InitDataBase();

            //加载记忆账号
            if (!String.IsNullOrEmpty(Settings.Default.UserName) &&
                !String.IsNullOrEmpty(Settings.Default.Password))
            {
                this.textBoxUsername.Text = Settings.Default.UserName;
                this.textBoxPassword.Text = Settings.Default.Password;
            }
            if (!String.IsNullOrEmpty(Settings.Default.YunUserName) &&
                !String.IsNullOrEmpty(Settings.Default.YunPassword))
            {
                this.textBoxYdmUser.Text = Settings.Default.YunUserName;
                this.textBoxYdmPassword.Text = Settings.Default.YunPassword;
            }
        }

        #region [更新cookie]
        int updateCount = 0;
        private void UpdateCookieTimer_Tick(object sender, EventArgs e)
        {
            updateCount++;
            if (updateCount >= 20)
            {
                this.UpdateCookie();
                updateCount = 0;
            }
        }

        private void UpdateCookie()
        {
            Model.User user = BLL.Login.PrepareLogin(this.textBoxUsername.Text, this.textBoxPassword.Text);
            string result = BLL.Login.StartLogin(user);
            if (result.Equals("0"))
            {
                this.User = user;
                this.WriteOutputMessage("更新cookies成功");
            }
            else if (result.Equals("2070") || result.Equals("4096") || result.Equals("4049"))
            {
                if (this.textBoxUsername.Text.Equals("") || this.textBoxPassword.Text.Equals(""))
                {
                    this.WriteOutputMessage("云打码数据为空，无法更新cookies");
                }
                else
                {
                    //TODO 云打码更新cookies
                }
            }
        }
        #endregion

        #region [运行相关]
        List<Model.LotteryWeibo> Weibos = new List<Model.LotteryWeibo>();
        private void ForwardTimer_Tick(object sender, EventArgs e)
        {
            if (Weibos.Count <= 0)
            {
                this.Weibos = BLL.Lottery.GetLotteryList();
            }

            bool isForward = this.LotteryWeibo();

            if (!isForward)
            {
                Weibos = BLL.Lottery.GetLotteryList();
                this.LotteryWeibo();
            }
        }

        private bool LotteryWeibo()
        {
            if (Weibos.Count != 0)
            {
                foreach (Model.LotteryWeibo lotteryWeibo in Weibos)
                {
                    //防止重复转发、多人关注要求
                    if (lotteryWeibo.LinkedUsers.Count > 0 ||
                        BLL.ForwardDb.IsForwarded(lotteryWeibo.Mid))
                    {
                        continue;
                    }
                    //点赞
                    BLL.Lottery.Like(this.User.Cookies, lotteryWeibo.Mid);
                    //关注
                    BLL.Lottery.Follow(this.User.Cookies, lotteryWeibo.OwnerUser.Uid, lotteryWeibo.OwnerUser.NickName);
                    //转发
                    BLL.Lottery.Forward(this.User.Cookies, lotteryWeibo.Mid, this.User.Uid);

                    //记录数据
                    this.WriteOutputMessage("转发成功，被转用户：@" + lotteryWeibo.OwnerUser.NickName);
                    BLL.ForwardDb.InsertMid(lotteryWeibo.Mid);
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region [信息输出]
        /// <summary>
        /// 向状态窗口输出一组信息
        /// </summary>
        /// <param name="messages"></param>
        private void WriteOutputMessages(string[] messages)
        {
            string timeStr = DateTime.Now.ToString();

            this.richTextBoxOutput.Text += timeStr + ":" + System.Environment.NewLine;

            foreach (string str in messages)
            {
                this.richTextBoxOutput.AppendText(str + Environment.NewLine);
            }
            this.richTextBoxOutput.ScrollToCaret();
        }
        /// <summary>
        /// 向状态窗口输出一条信息
        /// </summary>
        /// <param name="message"></param>
        private void WriteOutputMessage(string message)
        {
            this.WriteOutputMessages(new string[] { message });
        }
        #endregion

        #region [窗口事件]
        //登录成功
        private void LoginSuccessful(Model.User user)
        {
            this.User = user;

            this.pictureBoxHeader.Image = user.HeaderPicture;
            this.groupBoxSet.Text = user.NickName;
            this.WriteOutputMessage("登录成功");
            this.buttonLogin.Enabled = false;
            this.buttonStop.Enabled = false;
            this.buttonStart.Enabled = true;
            this.textBoxInterval.Enabled = true;

            this.textBoxUsername.Enabled = false;
            this.textBoxPassword.Enabled = false;

            this.UpdateCookieTimer.Enabled = true;

            //记住账号
            Settings.Default.UserName = this.textBoxUsername.Text;
            Settings.Default.Password = this.textBoxPassword.Text;
            Settings.Default.Save();
        }

        //登录并开始转发
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            Model.User user = BLL.Login.PrepareLogin(this.textBoxUsername.Text, this.textBoxPassword.Text);
            string result = BLL.Login.StartLogin(user);
            if (result.Equals("0"))
            {
                if (user.NickName == null)
                {
                    this.WriteOutputMessage("账号信息获取失败");
                }
                else if (user.NickName.IndexOf("请先验证身份") > -1)
                {
                    this.WriteOutputMessage("账号被锁，需解锁后才能继续登录");
                    MessageBox.Show("账号被锁，需解锁后才能继续登录","提示");
                }
                else
                {
                    this.LoginSuccessful(user);
                }
            }
            else if (result.Equals("2070") || result.Equals("4096") || result.Equals("4049"))
            {
                do
                {
                    CheckCodeView checkCodeView = new CheckCodeView(BLL.Login.GetCodeImage(user));
                    checkCodeView.ShowDialog();
                    if (checkCodeView.Code.Equals(""))
                    {
                        this.WriteOutputMessage("退出登录");
                        return;
                    }
                    else
                    {
                        result = BLL.Login.StartLogin(user, checkCodeView.Code);
                    }
                } while (result.Equals(""));
                    this.LoginSuccessful(user);
            }
            else if (result.Equals("101&"))
            {
                //密码错误
                this.WriteOutputMessage("登录失败，账号或密码错误");
                MessageBox.Show("账号或密码错误！", "提示");
            }
            else
            {
                this.WriteOutputMessage("登录失败，未知错误信息");
                MessageBox.Show("未知错误！请关闭登录保护后重试！", "提示");
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (!this.textBoxPassword.Text.Equals("") && !this.textBoxUsername.Text.Equals(""))
            {
                this.buttonLogin.Enabled = true;
            }
        }

        //停止转发
        private void buttonStop_Click(object sender, EventArgs e)
        {
            this.buttonStart.Enabled = true;
            this.buttonStop.Enabled = false;
            this.textBoxInterval.Enabled = true;

            this.ForwardTimer.Enabled = false;

            this.textBoxYdmUser.Enabled = true;
            this.textBoxYdmPassword.Enabled = true;
        }

        //开始转发
        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.buttonStart.Enabled = false;
            this.buttonStop.Enabled = true;
            this.textBoxInterval.Enabled = false;

            this.ForwardTimer.Interval = Convert.ToInt32(this.textBoxInterval.Text) * 60000;
            this.ForwardTimer.Enabled = true;

            //记住云打码账号
            Settings.Default.YunUserName = this.textBoxYdmUser.Text;
            Settings.Default.YunPassword = this.textBoxYdmPassword.Text;
            Settings.Default.Save();
            this.textBoxYdmUser.Enabled = false;
            this.textBoxYdmPassword.Enabled = false;
        }
        #endregion
    }
}

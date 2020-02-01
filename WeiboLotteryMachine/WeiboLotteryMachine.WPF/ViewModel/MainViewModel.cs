using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WeiboLotteryMachine.WPF.View;

namespace WeiboLotteryMachine.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private Model.User User;
        private Timer forwardTimer;
        private List<Model.LotteryWeibo> Weibos = new List<Model.LotteryWeibo>();

        public ICommand StartCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }

        #region [Properties]
        private string nickName;
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName
        {
            get 
            {
                if (String.IsNullOrWhiteSpace(nickName))
                {
                    return "未登录";
                }
                return nickName; 
            }
            set 
            {
                if (value.Equals(nickName))
                {
                    return;
                }
                nickName = value;
                this.RaisePropertyChanged("NickName", value);
            }
        }

        private int interval = 30;
        /// <summary>
        /// 发布时间间隔
        /// </summary>
        public int Interval
        {
            get { return interval; }
            set 
            { 
                interval = value;
                this.RaisePropertyChanged("Interval", value);
            }
        }

        private BitmapImage avatar;
        /// <summary>
        /// 头像
        /// </summary>
        public BitmapImage Avatar
        {
            get 
            { 
                return this.avatar; 
            }
            set 
            { 
                this.avatar = value;
                this.RaisePropertyChanged("Avatar", value);
            }
        }

        private bool canLogin = false;
        /// <summary>
        /// 是否可以登录
        /// </summary>
        public bool CanLogin
        {
            get { return canLogin; }
            set 
            { 
                canLogin = value;
                this.RaisePropertyChanged("CanLogin", value);
            }
        }

        private bool isLogin;
        /// <summary>
        /// 账号是否登录成功
        /// </summary>
        public bool IsLogin
        {
            get { return isLogin; }
            set 
            { 
                isLogin = value;
                this.RaisePropertyChanged("IsLogin", value);
            }
        }

        private string userName;
        /// <summary>
        /// 登录账号
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set 
            { 
                userName = value;
                this.RaisePropertyChanged("UserName", value);
                if (!String.IsNullOrWhiteSpace(this.userName) && !String.IsNullOrWhiteSpace(this.password))
                {
                    CanLogin = true;
                }
            }
        }

        private string password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return password; }
            set 
            { 
                password = value;
                this.RaisePropertyChanged("Password", value);
                if (!String.IsNullOrWhiteSpace(this.userName) && !String.IsNullOrWhiteSpace(this.password))
                {
                    CanLogin = true;
                }
            }
        }

        private string ydmUserName;
        /// <summary>
        /// 云打码登录账号
        /// </summary>
        public string YdmUserName
        {
            get { return ydmUserName; }
            set 
            { 
                ydmUserName = value;
                this.RaisePropertyChanged("YdmUserName", value);
            }
        }

        private string ydmPassword;
        /// <summary>
        /// 云打码登录密码
        /// </summary>
        public string YdmPassword
        {
            get { return ydmPassword; }
            set 
            { 
                ydmPassword = value;
                this.RaisePropertyChanged("YdmPassword", value);
            }
        }

        private string outPut;
        /// <summary>
        /// 运行信息输出
        /// </summary>
        public string OutPut
        {
            get { return outPut; }
            set 
            { 
                outPut = value;
                this.RaisePropertyChanged("OutPut", value);
            }
        }

        private string startStatus = "开始转发";
        /// <summary>
        /// 开始按钮状态文本
        /// </summary>
        public string StartStatus
        {
            get { return startStatus; }
            set
            {
                startStatus = value;
                this.RaisePropertyChanged(nameof(StartStatus), value);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.StartCommand = new RelayCommand(() => ExecuteStartCommand());
            this.LoginCommand = new RelayCommand(() => ExecuteLoginCommand());

            forwardTimer = new Timer(this.forwardCallback, null, Timeout.Infinite, this.Interval * 1000 * 60);
        }

        private void ExecuteStartCommand()
        {
            if (this.StartStatus.Equals("开始转发"))
            {
                this.StartStatus = "停止转发";
                this.WriteOutputMessage("开始运行");
                this.forwardTimer.Change(0, this.Interval * 1000 * 60);
            }
            else
            {
                this.StartStatus = "开始转发";
                this.WriteOutputMessage("停止运行");
                this.forwardTimer.Change(Timeout.Infinite, this.Interval * 1000 * 60);
            }
        }

        private void ExecuteLoginCommand()
        {
            Model.User user = BLL.Login.PrepareLogin(this.UserName, this.Password);
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
                    MessageBox.Show("账号被锁，需解锁后才能继续登录", "提示");
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
                    CheckCodeView checkCodeView = new CheckCodeView();
                    ((CheckCodeViewModel)checkCodeView.DataContext).CheckCode = BLL.Login.GetCodeBitmapImage(user);
                    checkCodeView.ShowDialog();
                    if (String.IsNullOrEmpty(((CheckCodeViewModel)checkCodeView.DataContext).CodeString))
                    {
                        this.WriteOutputMessage("退出登录");
                        return;
                    }
                    else
                    {
                        result = BLL.Login.StartLogin(user, ((CheckCodeViewModel)checkCodeView.DataContext).CodeString);
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

        private void WriteOutputMessage(string message)
        {
            this.OutPut += DateTime.Now.ToString("MM-dd hh:mm:ss");
            this.OutPut += "\n";
            this.OutPut += message;
            this.OutPut += "\n";
        }

        private void LoginSuccessful(Model.User user)
        {
            this.User = user;
            this.IsLogin = true;
            this.Avatar = new BitmapImage(new Uri(user.AvatarUrl));
            this.NickName = user.NickName;
            this.WriteOutputMessage("登录成功");
        }

        private void forwardCallback(object state)
        {
            if (Weibos.Count <= 1)
            {
                this.Weibos = BLL.Lottery.GetLotteryList(this.User.Cookies);
            }
            this.LotteryWeibo();
        }

        private bool LotteryWeibo()
        {
            if (Weibos.Count != 0)
            {
                foreach (Model.LotteryWeibo lotteryWeibo in Weibos)
                {
                    //防止重复转发
                    if (BLL.ForwardDb.IsForwarded(lotteryWeibo.Mid))
                    {
                        continue;
                    }
                    //点赞
                    BLL.Lottery.Like(this.User.Cookies, lotteryWeibo.Mid);
                    //关注
                    BLL.Lottery.Follow(this.User.Cookies, lotteryWeibo.OwnerUser.Uid, lotteryWeibo.OwnerUser.NickName);
                    //转发
                    BLL.Lottery.Forward(this.User.Cookies, lotteryWeibo.Mid, this.User.Uid);
                    //关注其他用户
                    foreach (var user in lotteryWeibo.LinkedUsers)
                    {
                        BLL.Lottery.Follow(this.User.Cookies, user.Uid, user.NickName);
                        this.WriteOutputMessage($"已关注@{user.NickName}");
                    }

                    //记录数据
                    this.WriteOutputMessage("转发成功，被转用户：@" + lotteryWeibo.OwnerUser.NickName);
                    BLL.ForwardDb.InsertMid(lotteryWeibo.Mid);
                    this.Weibos.Remove(lotteryWeibo);
                    return true;
                }
            }
            return false;
        }
    }
}
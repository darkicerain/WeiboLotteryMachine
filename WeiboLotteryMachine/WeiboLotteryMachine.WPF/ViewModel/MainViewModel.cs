using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Drawing;
using System.Windows.Input;

namespace WeiboLotteryMachine.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand StartCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }

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

        private Image avatar;
        /// <summary>
        /// 头像
        /// </summary>
        public Image Avatar
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

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.StartCommand = new RelayCommand(() => ExecuteStartCommand());
            this.LoginCommand = new RelayCommand(() => ExecuteLoginCommand());
        }

        private void ExecuteStartCommand()
        {
            if (this.StartStatus.Equals("开始转发"))
            {
                this.StartStatus = "停止转发";
                this.setOutPut("开始运行");
            }
            else
            {
                this.StartStatus = "开始转发";
                this.setOutPut("停止运行");
            }
        }

        private void ExecuteLoginCommand()
        {
            this.IsLogin = true;
        }

        private void setOutPut(string message)
        {
            this.OutPut += DateTime.Now.ToString("MM-dd hh:mm:ss");
            this.OutPut += "\n";
            this.OutPut += message;
            this.OutPut += "\n";
        }
    }
}
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
        public ICommand StopCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            this.StartCommand = new RelayCommand(() => ExecuteStartCommand());
            this.StartCommand = new RelayCommand(() => ExecuteStopCommand());
            this.LoginCommand = new RelayCommand(() => ExecuteLoginCommand());
        }

        private string nickName;
        /// <summary>
        /// êÇ³Æ
        /// </summary>
        public string NickName
        {
            get 
            {
                if (String.IsNullOrWhiteSpace(nickName))
                {
                    return "Î´µÇÂ¼";
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
        /// ·¢²¼Ê±¼ä¼ä¸ô
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
        /// Í·Ïñ
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
        /// ÊÇ·ñ¿ÉÒÔµÇÂ¼
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
        /// ÕËºÅÊÇ·ñµÇÂ¼³É¹¦
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
        /// µÇÂ¼ÕËºÅ
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
        /// ÃÜÂë
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
        /// ÔÆ´òÂëµÇÂ¼ÕËºÅ
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
        /// ÔÆ´òÂëµÇÂ¼ÃÜÂë
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
        /// ÔËÐÐÐÅÏ¢Êä³ö
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


        private void ExecuteStartCommand()
        {
            
        }
        private void ExecuteStopCommand()
        {
            
        }

        private void ExecuteLoginCommand()
        {
            
        }
    }
}
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
        /// �ǳ�
        /// </summary>
        public string NickName
        {
            get 
            {
                if (String.IsNullOrWhiteSpace(nickName))
                {
                    return "δ��¼";
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
        /// ����ʱ����
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
        /// ͷ��
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
        /// �Ƿ���Ե�¼
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
        /// �˺��Ƿ��¼�ɹ�
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
        /// ��¼�˺�
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
        /// ����
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
        /// �ƴ����¼�˺�
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
        /// �ƴ����¼����
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
        /// ������Ϣ���
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

        private string startStatus = "��ʼת��";
        /// <summary>
        /// ��ʼ��ť״̬�ı�
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
            if (this.StartStatus.Equals("��ʼת��"))
            {
                this.StartStatus = "ֹͣת��";
                this.setOutPut("��ʼ����");
            }
            else
            {
                this.StartStatus = "��ʼת��";
                this.setOutPut("ֹͣ����");
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
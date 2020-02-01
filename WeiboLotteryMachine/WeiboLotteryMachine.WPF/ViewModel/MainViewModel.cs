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

        private BitmapImage avatar;
        /// <summary>
        /// ͷ��
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
            if (this.StartStatus.Equals("��ʼת��"))
            {
                this.StartStatus = "ֹͣת��";
                this.WriteOutputMessage("��ʼ����");
                this.forwardTimer.Change(0, this.Interval * 1000 * 60);
            }
            else
            {
                this.StartStatus = "��ʼת��";
                this.WriteOutputMessage("ֹͣ����");
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
                    this.WriteOutputMessage("�˺���Ϣ��ȡʧ��");
                }
                else if (user.NickName.IndexOf("������֤���") > -1)
                {
                    this.WriteOutputMessage("�˺ű��������������ܼ�����¼");
                    MessageBox.Show("�˺ű��������������ܼ�����¼", "��ʾ");
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
                        this.WriteOutputMessage("�˳���¼");
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
                //�������
                this.WriteOutputMessage("��¼ʧ�ܣ��˺Ż��������");
                MessageBox.Show("�˺Ż��������", "��ʾ");
            }
            else
            {
                this.WriteOutputMessage("��¼ʧ�ܣ�δ֪������Ϣ");
                MessageBox.Show("δ֪������رյ�¼���������ԣ�", "��ʾ");
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
            this.WriteOutputMessage("��¼�ɹ�");
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
                    //��ֹ�ظ�ת��
                    if (BLL.ForwardDb.IsForwarded(lotteryWeibo.Mid))
                    {
                        continue;
                    }
                    //����
                    BLL.Lottery.Like(this.User.Cookies, lotteryWeibo.Mid);
                    //��ע
                    BLL.Lottery.Follow(this.User.Cookies, lotteryWeibo.OwnerUser.Uid, lotteryWeibo.OwnerUser.NickName);
                    //ת��
                    BLL.Lottery.Forward(this.User.Cookies, lotteryWeibo.Mid, this.User.Uid);
                    //��ע�����û�
                    foreach (var user in lotteryWeibo.LinkedUsers)
                    {
                        BLL.Lottery.Follow(this.User.Cookies, user.Uid, user.NickName);
                        this.WriteOutputMessage($"�ѹ�ע@{user.NickName}");
                    }

                    //��¼����
                    this.WriteOutputMessage("ת���ɹ�����ת�û���@" + lotteryWeibo.OwnerUser.NickName);
                    BLL.ForwardDb.InsertMid(lotteryWeibo.Mid);
                    this.Weibos.Remove(lotteryWeibo);
                    return true;
                }
            }
            return false;
        }
    }
}
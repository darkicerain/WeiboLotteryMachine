using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WeiboLotteryMachine.WPF.ViewModel
{
    public class CheckCodeViewModel: ViewModelBase
    {
        #region [Properties]
        private BitmapImage checkCode;
        /// <summary>
        /// 验证码图片
        /// </summary>
        public BitmapImage CheckCode
        {
            get { return checkCode; }
            set
            {
                checkCode = value;
                this.RaisePropertyChanged(nameof(CheckCode), value);
            }
        }

        private string codeString;
        /// <summary>
        /// 验证码输入内容
        /// </summary>
        public string CodeString
        {
            get { return codeString; }
            set
            {
                codeString = value;
                this.RaisePropertyChanged(nameof(CodeString), value);
                this.OkEnabled = !String.IsNullOrWhiteSpace(this.CodeString);
            }
        }

        private bool okEnabled;
        /// <summary>
        /// 确认按钮使能
        /// </summary>
        public bool OkEnabled
        {
            get { return okEnabled; }
            set
            {
                okEnabled = value;
                this.RaisePropertyChanged(nameof(OkEnabled), value);
            }
        }
        #endregion

        public ICommand OkCommand { get; private set; }

        public CheckCodeViewModel()
        {
            this.OkCommand = new RelayCommand(this.ExecuteOkCommand);
        }

        private void ExecuteOkCommand()
        {
            
        }
    }
}

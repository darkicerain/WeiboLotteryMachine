using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WeiboLotteryMachine.Win
{
    public partial class CheckCodeView : Form
    {
        public string Code { private set; get; } = "";
        
        public CheckCodeView(Image image)
        {
            InitializeComponent();
            this.pictureBox1.Image = image;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Code = this.textBoxCode.Text;
            this.Close();
        }
    }
}

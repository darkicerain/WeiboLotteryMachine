using HouseKeeper.DAL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WeiboLotteryMachine.DAL;

namespace WeiboLotteryMachine.BLL
{
    public class Login
    {
        /// <summary>
        /// 登陆准备
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Model.User PrepareLogin(string userName, string password)
        {
            Model.User User = new Model.User();
            User.UserName = userName;
            User.Password = password;
            User.LoginPara = new Model.LoginParameter();
            //加密用户名
            Encoding myEncoding = Encoding.GetEncoding("utf-8");
            byte[] suByte = myEncoding.GetBytes(HttpUtility.UrlEncode(userName));
            User.LoginPara.su = Convert.ToBase64String(suByte);

            return User;
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Image GetCodeImage(Model.User user)
        {
            //获取登录参数
            string url = "http://login.sina.com.cn/sso/prelogin.php?entry=weibo&callback=sinaSSOController.preloginCallBack&su="
            + user.LoginPara.su + "&rsakt=mod&checkpin=1&client=ssologin.js(v1.4.18)";
            string content = HttpHelper.Get(url);
            int pos;
            pos = content.IndexOf("servertime");
            user.LoginPara.servertime = content.Substring(pos + 12, 10);
            pos = content.IndexOf("pcid");
            user.LoginPara.pcid = content.Substring(pos + 7, 39);
            pos = content.IndexOf("nonce");
            user.LoginPara.nonce = content.Substring(pos + 8, 6);
            pos = content.IndexOf("showpin");
            user.LoginPara.showpin = content.Substring(pos + 9, 1);

            user.LoginPara.IsForcedPin = true;

            //获取验证码
            url = "http://login.sina.com.cn/cgi/pin.php?p=" + user.LoginPara.pcid;
            return HttpHelper.GetImage(url);
        }

        /// <summary>
        /// 正式开始登录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="door">验证码，如没有验证码则不用传</param>
        /// <returns></returns>
        public static string StartLogin(Model.User user, string door = null)
        {
            string securityPassword = GetSecurityPassword(user.Password, user.LoginPara.servertime, user.LoginPara.nonce, user.LoginPara.PUBKEY);
            if (securityPassword == null)
            {
                return "RSA加密失败";
            }

            string postData = "entry=weibo&gateway=1&from=&savestate=7&useticket=1&pagerefer=&vsnf=1&su=" + user.LoginPara.su
                + "&service=miniblog&servertime=" + user.LoginPara.servertime
                + "&nonce=" + user.LoginPara.nonce
                + "&pwencode=rsa2&rsakv=" + user.LoginPara.RSAKV + "&sp=" + securityPassword
                + "&sr=1366*768&encoding=UTF-8&prelt=104&url=http%3A%2F%2Fweibo.com%2Fajaxlogin.php%3Fframelogin%3D1%26callback%3Dparent.sinaSSOController.feedBackUrlCallBack&returntype=META";

            if (((user.LoginPara.showpin != null && user.LoginPara.showpin.Equals("1")) || user.LoginPara.IsForcedPin) && door != null)
            {
                postData += "&pcid=" + user.LoginPara.pcid + "&door=" + door;
            }

            string content = HttpHelper.Post("http://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.18)", postData);
            int pos = content.IndexOf("retcode=");
            string retcode = content.Substring(pos + 8, 1);

            if (retcode == "0")
            {
                //获取cookie
                user.Cookies = new CookieContainer();
                pos = content.IndexOf("location.replace");
                string url = content.Substring(pos + 18, 276);//285->276 fuck!! 
                content = HttpHelper.Get(url, user.Cookies, false);
                //获取头像、uid、昵称
                GetUidNickNameAndSoOn(user);
            }
            else
            {
                retcode = content.Substring(pos + 8, 4);
            }


            return retcode;
        }

        /// <summary>
        /// 获取加密后的密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="servertime"></param>
        /// <param name="nonce"></param>
        /// <param name="pubkey"></param>
        /// <returns></returns>
        private static string GetSecurityPassword(string pwd, string servertime, string nonce, string pubkey)
        {
            StreamReader sr = new StreamReader("sinaSSOEncoder"); //从文本中读取修改过的JS
            string js = sr.ReadToEnd();
            //自定义function进行加密
            js += "function getpass(pwd,servicetime,nonce,rsaPubkey){var RSAKey=new sinaSSOEncoder.RSAKey();RSAKey.setPublic(rsaPubkey,'10001');var password=RSAKey.encrypt([servicetime,nonce].join('\\t')+'\\n'+pwd);return password;}";
            ScriptEngine se = new ScriptEngine(ScriptLanguage.JavaScript);
            object obj = se.Run("getpass", new object[] { pwd, servertime, nonce, pubkey }, js);
            sr.Close();
            return obj.ToString();
        }

        /// <summary>
        /// 获取昵称、uid、头像
        /// </summary>
        /// <param name="user"></param>
        private static void GetUidNickNameAndSoOn(Model.User user)
        {
            try
            {
                var userHomePageTxt = HttpHelper.Get("https://weibo.com", user.Cookies, true);

                //获取用户uid
                int indexStart = userHomePageTxt.IndexOf("$CONFIG['uid']='") + "$CONFIG['uid']='".Length;
                userHomePageTxt = userHomePageTxt.Substring(indexStart);
                user.Uid = userHomePageTxt.Substring(0, userHomePageTxt.IndexOf("';"));
                //获取昵称
                indexStart = userHomePageTxt.IndexOf("$CONFIG['nick']='") + "$CONFIG['nick']='".Length;
                userHomePageTxt = userHomePageTxt.Substring(indexStart);
                user.NickName = userHomePageTxt.Substring(0, userHomePageTxt.IndexOf("';"));
                //获取头像
                indexStart = userHomePageTxt.IndexOf("$CONFIG['avatar_large']='") + "$CONFIG['avatar_large']='".Length;
                userHomePageTxt = userHomePageTxt.Substring(indexStart);
                string url = userHomePageTxt.Substring(0, userHomePageTxt.IndexOf("';"));
                WebClient wc = new WebClient();
                user.HeaderPicture = Image.FromStream(wc.OpenRead("http:" + url));
            }
            catch (Exception ex)
            {
                //获取失败
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media.Imaging;
using WeiboLotteryMachine.WPF.Common;

namespace WeiboLotteryMachine.WPF.Models
{
    public class Login
    {
        /// <summary>
        /// 登录用户信息
        /// </summary>
        public LoginUser User { get; private set; }

        public Login(string userName, string password)
        {
            User = new LoginUser();
            User.UserName = userName;
            User.Password = password;
            //加密用户名
            Encoding myEncoding = Encoding.GetEncoding("utf-8");
            byte[] suByte = myEncoding.GetBytes(HttpUtility.UrlEncode(userName));
            User.LoginPara.su = Convert.ToBase64String(suByte);
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public BitmapImage GetCodeBitmapImage()
        {
            //获取登录参数
            string url = "http://login.sina.com.cn/sso/prelogin.php?entry=weibo&callback=sinaSSOController.preloginCallBack&su="
            + User.LoginPara.su + "&rsakt=mod&checkpin=1&client=ssologin.js(v1.4.18)";
            string content = HttpHelper.Get(url);
            int pos;
            pos = content.IndexOf("servertime");
            User.LoginPara.servertime = content.Substring(pos + 12, 10);
            pos = content.IndexOf("pcid");
            User.LoginPara.pcid = content.Substring(pos + 7, 39);
            pos = content.IndexOf("nonce");
            User.LoginPara.nonce = content.Substring(pos + 8, 6);
            pos = content.IndexOf("showpin");
            User.LoginPara.showpin = content.Substring(pos + 9, 1);

            User.LoginPara.IsForcedPin = true;
            //获取验证码
            url = "http://login.sina.com.cn/cgi/pin.php?p=" + User.LoginPara.pcid;
            return new BitmapImage(new Uri(url));
        }

        /// <summary>
        /// 正式开始登录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="door">验证码，如没有验证码则不用传</param>
        /// <returns></returns>
        public string StartLogin(string door = null)
        {
            string securityPassword = GetSecurityPassword(User.Password, User.LoginPara.servertime, User.LoginPara.nonce, User.LoginPara.PUBKEY);
            if (securityPassword == null)
            {
                return "RSA加密失败";
            }

            string postData = "entry=weibo&gateway=1&from=&savestate=7&useticket=1&pagerefer=&vsnf=1&su=" + User.LoginPara.su
                + "&service=miniblog&servertime=" + User.LoginPara.servertime
                + "&nonce=" + User.LoginPara.nonce
                + "&pwencode=rsa2&rsakv=" + User.LoginPara.RSAKV + "&sp=" + securityPassword
                + "&sr=1366*768&encoding=UTF-8&prelt=104&url=http%3A%2F%2Fweibo.com%2Fajaxlogin.php%3Fframelogin%3D1%26callback%3Dparent.sinaSSOController.feedBackUrlCallBack&returntype=META";

            if (((User.LoginPara.showpin != null && User.LoginPara.showpin.Equals("1")) || User.LoginPara.IsForcedPin) && door != null)
            {
                postData += "&pcid=" + User.LoginPara.pcid + "&door=" + door;
            }

            string content = HttpHelper.Post("http://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.18)", postData);
            int pos = content.IndexOf("retcode=");
            string retcode = content.Substring(pos + 8, 1);

            if (retcode == "0")
            {
                //获取cookie
                User.Cookies = new CookieContainer();
                pos = content.IndexOf("location.replace");
                string url = content.Substring(pos + 18, 276);
                content = HttpHelper.Get(url, User.Cookies, false);
                //获取头像、uid、昵称
                GetUidNickNameAndSoOn();
            }
            else
            {
                retcode = content.Substring(pos + 8, 4);
            }
            return retcode;
        }

        /// <summary>
        /// 获取抽奖微博列表
        /// </summary>
        /// <returns></returns>
        public List<LotteryWeibo> GetLotteryList()
        {
            List<LotteryWeibo> lotteryWeibos = new List<LotteryWeibo>();
            string url = "https://s.weibo.com/weibo/%25E5%25BE%25AE%25E5%258D%259A%25E6%258A%25BD%25E5%25A5%2596%25E5%25B9%25B3%25E5%258F%25B0?topnav=1&wvr=6&b=1";
            string result = HttpHelper.Get(url, this.User.Cookies , true);

            if (!result.Equals(""))
            {
                string regexStr = "<!--card-wrap-->(.|\n)*?<!--/card-wrap-->";
                MatchCollection matchCollections = Regex.Matches(result, regexStr);
                foreach (Match match in matchCollections)
                {
                    LotteryWeibo lotteryWeibo = new LotteryWeibo();
                    //获取mid
                    regexStr = "mid=\\\"[0-9]*";
                    Match mid = Regex.Match(match.Value, regexStr);
                    lotteryWeibo.Mid = mid.Value.Replace("mid=\"", "");
                    //获取owner
                    lotteryWeibo.OwnerUser = new LotteryUser();
                    regexStr = "nick-name=\\\"(.)*?\\\"";
                    Match name = Regex.Match(match.Value, regexStr);
                    lotteryWeibo.OwnerUser.NickName = name.Value.Replace("\"", "").Replace("nick-name=", "");
                    regexStr = "uid=[0-9]*";
                    Match uid = Regex.Match(match.Value, regexStr);
                    lotteryWeibo.OwnerUser.Uid = uid.Value.Replace("uid=", "");
                    //获取关联账号列表
                    lotteryWeibo.LinkedUsers = new List<LotteryUser>();
                    regexStr = "<p class=\\\"txt\\\" node(.|\n)*?</p>";
                    Match mainBody = Regex.Match(match.Value, regexStr);
                    regexStr = "<a(.)*?</a>";
                    MatchCollection users = Regex.Matches(mainBody.Value, regexStr);
                    foreach (Match user in users)
                    {
                        if (user.Value.Contains("微博") && user.Value.Contains("抽奖") && user.Value.Contains("平台"))
                        {
                            continue;
                        }
                        LotteryUser lotteryUser = new LotteryUser();
                        regexStr = "@(.)*?</a>";
                        Match nick = Regex.Match(user.Value, regexStr);
                        lotteryUser.NickName = nick.Value.Replace("@", "").Replace("</a>", "");
                        if (String.IsNullOrEmpty(lotteryUser.NickName) || lotteryUser.NickName.Equals(lotteryWeibo.OwnerUser.NickName))
                        {
                            continue;
                        }
                        regexStr = "href=(.)*? target";
                        Match friendUrl = Regex.Match(user.Value, regexStr);
                        lotteryUser.Uid = this.GetUidFromUrl(this.User.Cookies, friendUrl.Value.Replace("href=\"", "").Replace("\" target", ""));
                        if (!String.IsNullOrWhiteSpace(lotteryUser.NickName) &&
                            !String.IsNullOrWhiteSpace(lotteryUser.Uid))
                        {
                            lotteryWeibo.LinkedUsers.Add(lotteryUser);
                        }
                    }
                    lotteryWeibos.Add(lotteryWeibo);
                }
            }
            return lotteryWeibos;
        }

        public bool UpdateCookies()
        {


            return false;
        }

        /// <summary>
        /// 点赞微博
        /// </summary>
        /// <param name="mid">被点赞微博mid</param>
        public void Like(string mid)
        {
            string data = String.Format("location=page_100505_home&version=mini&qid=heart&mid={0}&loc=profile&cuslike=1&floating=0&_t=0", mid);
            string url = @"https://weibo.com/aj/v6/like/add?ajwvr=6&__rnd=" + TimeHelper.GetTimeStamp();
            HttpHelper.SendDataByPost(url, this.User.Cookies, data);
        }

        /// <summary>
        /// 关注用户
        /// </summary>
        /// <param name="uid">uid</param>
        /// <param name="nickName">用户昵称</param>
        public void Follow(string uid, string nickName)
        {
            string data = String.Format("uid={0}&objectid=&f=1&extra=&refer_sort=&refer_flag=1005050001_&location=page_100505_home&oid={0}&wforce=1&nogroup=1&fnick={1}&refer_lflag=1005050005_&refer_from=profile_headerv6&template=7&special_focus=1&isrecommend=1&is_special=0&redirect_url=%252Fp%252F1005056676557674%252Fmyfollow%253Fgid%253D4279893657022870%2523place&_t=0", uid, nickName);
            string url = @"https://weibo.com/aj/f/followed?ajwvr=6&__rnd=" + TimeHelper.GetTimeStamp();

            HttpHelper.SendDataByPost(url, this.User.Cookies, data);
        }

        /// <summary>
        /// 转发微博
        /// </summary>
        /// <param name="mid">被转发微博mid</param>
        /// <param name="comment">评论内容</param>
        public void Forward(string mid, string comment = "")
        {
            string data = String.Format("pic_src=&pic_id=&appkey=&mid={0}&style_type=1&mark=&reason={1}&from_plugin=0&location=v6_content_home&pdetail=&module=&page_module_id=&refer_sort=&is_comment_base=1&rank=0&rankid=&group_source=group_all&rid=3_0_8_2669529346570161066_0_0_0&isReEdit=false&_t=0", mid, comment);
            string url = String.Format(@"https://weibo.com/aj/v6/mblog/forward?ajwvr=6&domain={0}&__rnd={1}", this.User.Uid, TimeHelper.GetTimeStamp());
            HttpHelper.SendDataByPost(url, this.User.Cookies, data);
        }

        /// <summary>
        /// 评论微博
        /// </summary>
        /// <param name="mid">被评论微博mid</param>
        /// <param name="commmentUid">被评论用户uid</param>
        /// <param name="message">评论内容</param>
        public void Comment(string mid, string commentUid, string message)
        {
            string data = $"act=post&mid={mid}&uid={this.User.Uid}&forward=0&isroot=0&content={message}&location=page_100505_home&module=scommlist&group_source=&pdetail=100505{commentUid}&_t=0";
            string url = "https://weibo.com/aj/v6/comment/add?ajwvr=6&__rnd=" + TimeHelper.GetTimeStamp();
            HttpHelper.SendDataByPost(url, this.User.Cookies, data);
        }


        /// <summary>
        /// 获取加密后的密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="servertime"></param>
        /// <param name="nonce"></param>
        /// <param name="pubkey"></param>
        /// <returns></returns>
        private string GetSecurityPassword(string pwd, string servertime, string nonce, string pubkey)
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
        private void GetUidNickNameAndSoOn()
        {
            try
            {
                var userHomePageTxt = HttpHelper.Get("https://weibo.com", User.Cookies, true);

                //获取用户uid
                int indexStart = userHomePageTxt.IndexOf("$CONFIG['uid']='") + "$CONFIG['uid']='".Length;
                userHomePageTxt = userHomePageTxt.Substring(indexStart);
                User.Uid = userHomePageTxt.Substring(0, userHomePageTxt.IndexOf("';"));
                //获取昵称
                indexStart = userHomePageTxt.IndexOf("$CONFIG['nick']='") + "$CONFIG['nick']='".Length;
                userHomePageTxt = userHomePageTxt.Substring(indexStart);
                User.NickName = userHomePageTxt.Substring(0, userHomePageTxt.IndexOf("';"));
                //获取头像
                indexStart = userHomePageTxt.IndexOf("$CONFIG['avatar_large']='") + "$CONFIG['avatar_large']='".Length;
                userHomePageTxt = userHomePageTxt.Substring(indexStart);
                string url = userHomePageTxt.Substring(0, userHomePageTxt.IndexOf("';"));
                WebClient wc = new WebClient();
                User.HeaderPicture = Image.FromStream(wc.OpenRead(url));
                User.AvatarUrl = url;
            }
            catch (Exception ex)
            {
                //获取失败
            }
        }

        /// <summary>
        /// 根据url获取用户uid
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetUidFromUrl(CookieContainer cookie, string url)
        {
            string result = HttpHelper.Get("http:" + url, cookie, true);
            if (String.IsNullOrEmpty(result))
            {
                return "";
            }

            int indexStart = result.IndexOf("$CONFIG['oid']='") + "$CONFIG['oid']='".Length;
            result = result.Substring(indexStart);
            return result.Substring(0, result.IndexOf("';"));
        }
    }
}

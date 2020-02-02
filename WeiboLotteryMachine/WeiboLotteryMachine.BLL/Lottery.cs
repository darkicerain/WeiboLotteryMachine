using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using WeiboLotteryMachine.DAL;

namespace WeiboLotteryMachine.BLL
{
    public class Lottery
    {
        /// <summary>
        /// 获取抽奖微博列表
        /// </summary>
        /// <returns></returns>
        public static List<Model.LotteryWeibo> GetLotteryList(CookieContainer cookie)
        {
            List<Model.LotteryWeibo> lotteryWeibos = new List<Model.LotteryWeibo>(); 
            string url = "https://s.weibo.com/weibo/%25E5%25BE%25AE%25E5%258D%259A%25E6%258A%25BD%25E5%25A5%2596%25E5%25B9%25B3%25E5%258F%25B0?topnav=1&wvr=6&b=1";
            string result = DAL.HttpHelper.Get(url, cookie, true);

            if (!result.Equals(""))
            {
                string regexStr = "<!--card-wrap-->(.|\n)*?<!--/card-wrap-->";
                MatchCollection matchCollections = Regex.Matches(result, regexStr);
                foreach (Match match in matchCollections)
                {
                    Model.LotteryWeibo lotteryWeibo = new Model.LotteryWeibo();
                    //获取mid
                    regexStr = "mid=\\\"[0-9]*";
                    Match mid = Regex.Match(match.Value, regexStr);
                    lotteryWeibo.Mid = mid.Value.Replace("mid=\"","");
                    //获取owner
                    lotteryWeibo.OwnerUser = new Model.LotteryUser();
                    regexStr = "nick-name=\\\"(.)*?\\\"";
                    Match name = Regex.Match(match.Value, regexStr);
                    lotteryWeibo.OwnerUser.NickName = name.Value.Replace("\"", "").Replace("nick-name=", "");
                    regexStr = "uid=[0-9]*";
                    Match uid = Regex.Match(match.Value, regexStr);
                    lotteryWeibo.OwnerUser.Uid = uid.Value.Replace("uid=", "");
                    //获取关联账号列表
                    lotteryWeibo.LinkedUsers = new List<Model.LotteryUser>();
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
                        Model.LotteryUser lotteryUser = new Model.LotteryUser();
                        regexStr = "@(.)*?</a>";
                        Match nick = Regex.Match(user.Value, regexStr);
                        lotteryUser.NickName = nick.Value.Replace("@", "").Replace("</a>", "");
                        if (String.IsNullOrEmpty(lotteryUser.NickName) || lotteryUser.NickName.Equals(lotteryWeibo.OwnerUser.NickName))
                        {
                            continue;
                        }
                        regexStr = "href=(.)*? target";
                        Match friendUrl = Regex.Match(user.Value, regexStr);
                        lotteryUser.Uid = GetUidFromUrl(cookie, friendUrl.Value.Replace("href=\"", "").Replace("\" target", ""));
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

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="mid">单条微博mid</param>
        public static void Like(CookieContainer cookie, string mid)
        {
            string data = String.Format("location=page_100505_home&version=mini&qid=heart&mid={0}&loc=profile&cuslike=1&floating=0&_t=0", mid);
            string url = @"https://weibo.com/aj/v6/like/add?ajwvr=6&__rnd=" + GetTimeStamp();
            HttpHelper.SendDataByPost(url, cookie, data);
        }

        /// <summary>
        /// 关注
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="uid">被关注用户uid</param>
        /// <param name="nickName">被关注用户昵称</param>
        public static void Follow(CookieContainer cookie, string uid, string nickName)
        {
            string data = String.Format("uid={0}&objectid=&f=1&extra=&refer_sort=&refer_flag=1005050001_&location=page_100505_home&oid={0}&wforce=1&nogroup=1&fnick={1}&refer_lflag=1005050005_&refer_from=profile_headerv6&template=7&special_focus=1&isrecommend=1&is_special=0&redirect_url=%252Fp%252F1005056676557674%252Fmyfollow%253Fgid%253D4279893657022870%2523place&_t=0", uid, nickName);
            string url = @"https://weibo.com/aj/f/followed?ajwvr=6&__rnd=" + GetTimeStamp();

            HttpHelper.SendDataByPost(url, cookie, data);
        }

        /// <summary>
        /// 转发
        /// </summary>
        public static void Forward(CookieContainer cookie ,string mid,string uid, string comment = "")
        {
            string data = String.Format("pic_src=&pic_id=&appkey=&mid={0}&style_type=1&mark=&reason={1}&from_plugin=0&location=v6_content_home&pdetail=&module=&page_module_id=&refer_sort=&is_comment_base=1&rank=0&rankid=&group_source=group_all&rid=3_0_8_2669529346570161066_0_0_0&isReEdit=false&_t=0",mid,comment);
            string url = String.Format(@"https://weibo.com/aj/v6/mblog/forward?ajwvr=6&domain={0}&__rnd={1}", uid, GetTimeStamp());
            HttpHelper.SendDataByPost(url, cookie, data);
        }

        /// <summary>
        /// 评论微博
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="mid">微博id</param>
        /// <param name="uid">评论账号uid（自己）</param>
        /// <param name="commmentUid">被评论用户uid</param>
        /// <param name="message">评论内容</param>
        public static void Comment(CookieContainer cookie, string mid, string uid, string commmentUid, string message)
        {
            string data = String.Format("act=post&mid={0}&uid={1}&forward=0&isroot=0&content={2}&location=page_100505_home&module=scommlist&group_source=&pdetail=100505{3}&_t=0", mid, uid, message, commmentUid);
            string url = "https://weibo.com/aj/v6/comment/add?ajwvr=6&__rnd=" + GetTimeStamp();
            string s = HttpHelper.SendDataByPost(url, cookie, data);
        }

        #region [私有方法]
        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        private static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        /// <summary>
        /// 根据url获取用户uid
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetUidFromUrl(CookieContainer cookie, string url)
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
        #endregion
    }
}

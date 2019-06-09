using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using WeiboLotteryMachine.DAL;

namespace WeiboLotteryMachine.Model
{
    public class LotteryWeibo
    {
        /// <summary>
        /// 单条微博mid
        /// </summary>
        public string Mid { get; set; }

        /// <summary>
        /// 微博发布者
        /// </summary>
        public LotteryUser OwnerUser { get; set; }

        /// <summary>
        /// 条件关联者列表
        /// </summary>
        public List<LotteryUser> LinkedUsers { get; set; }
    }

    public class LotteryUser
    {
        /// <summary>
        /// 用户uid
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
    }
}

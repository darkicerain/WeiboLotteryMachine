using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiboLotteryMachine.WPF.Models
{
    /// <summary>
    /// 抽奖微博信息
    /// </summary>
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

    /// <summary>
    /// 微博用户信息
    /// </summary>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiboLotteryMachine.BLL
{
    public class ForwardDb
    {
        /// <summary>
        /// 初始化转发数据库
        /// </summary>
        public static void InitDataBase()
        {
            DAL.ForwardDb.CreateDataBase();
            DAL.ForwardDb.CreateForwardTable();
        }

        /// <summary>
        /// 是否存在于数据库中
        /// </summary>
        /// <param name="mid">微博mid</param>
        /// <returns></returns>
        public static bool IsForwarded(string mid)
        {
            return DAL.ForwardDb.QueryForwardMid(mid);
        }

        /// <summary>
        /// 插入一条新mid数据
        /// </summary>
        /// <param name="mid">微博mid</param>
        public static void InsertMid(string mid)
        {
            DAL.ForwardDb.InsertForwardMid( mid);
        }
    }
}

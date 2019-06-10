using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace WeiboLotteryMachine.DAL
{
    public class ForwardDb
    {
        private static string path = Environment.CurrentDirectory + "\\DateBases\\";
        private static string fileName = "Forward.db";

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        private static SQLiteConnection DataBaseConnection()
        {
            return new SQLiteConnection("data source = " + path + fileName);
        }

        /// <summary>
        /// 创建数据库文件
        /// </summary>
        public static void CreateDataBase()
        {
            if (!File.Exists(path + fileName))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                File.Create(path + fileName).Close();
            }
        }

        /// <summary>
        /// 创建转发微博列表
        /// </summary>
        public static void CreateForwardTable()
        {
            using (SQLiteConnection connection = DataBaseConnection())
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = connection;

                //判断table是否已经存在
                command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE TYPE = 'table' AND NAME = 'forward'";
                command.ExecuteNonQuery();
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                int count = reader.GetInt32(0);
                reader.Close();
                if (count == 0)
                {
                    //转发微博表
                    command.CommandText = "Create Table forward (mid varchar(200),date varchar(200))";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 向数据库中插入mid
        /// </summary>
        /// <param name="mid"></param>
        public static void InsertForwardMid(string mid)
        {
            using (SQLiteConnection connection = DataBaseConnection())
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = connection;

                command.CommandText = String.Format("INSERT INTO forward (mid,date) values ('{0}','{1}')", mid, DateTime.Now.Date.ToString());
                command.ExecuteNonQuery();
            }
        }

        public static bool QueryForwardMid(string mid)
        {
            using (SQLiteConnection connection = DataBaseConnection())
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand();
                command.Connection = connection;

                command.CommandText = String.Format("SELECT COUNT(*) FROM forward WHERE mid = '{0}'", mid);
                command.ExecuteNonQuery();
                SQLiteDataReader reader = command.ExecuteReader();
                reader.Read();
                int count = reader.GetInt32(0);
                reader.Close();
                return count == 0 ? false : true;
            }
        }
    }
}

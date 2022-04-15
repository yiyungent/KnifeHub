using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQStatPlugin.Models
{
    /// <summary>
    /// 某人发消息
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 数据是自增需要加上 IsIdentity 
        /// 数据库是主键需要加上 IsPrimaryKey 
        /// 注意：要完全和数据库一致2个属性
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public string QQName { get; set; }

        public string QQUin { get; set; }

        public string Content { get; set; }

        /// <summary>
        /// 若私聊则为 null
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 若私聊则为 null
        /// </summary>
        public string GroupUin { get; set; }

        public long CreateTime { get; set; }
    }
}

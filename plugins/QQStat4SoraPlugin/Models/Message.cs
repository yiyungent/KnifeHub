using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQStat4SoraPlugin.Models
{
    /// <summary>
    /// 某人发消息
    /// </summary>
    public class Message
    {
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

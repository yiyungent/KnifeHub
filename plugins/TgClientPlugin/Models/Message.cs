using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgClientPlugin.Models
{
    /// <summary>
    /// 消息
    /// </summary>
    public class Message
    {
       public int Id { get; set; }

        public string UName { get; set; }

        public string UId { get; set; }

        public string Content { get; set; }

        /// <summary>
        /// 若私聊则为 null
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 若私聊则为 null
        /// </summary>
        public string GroupId { get; set; }

        public long CreateTime { get; set; }
    }
}

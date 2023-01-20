using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramPlugin.Models
{
    /// <summary>
    /// 某人发消息
    /// </summary>
    public class QABox
    {
        public int Id { get; set; }

        /// <summary>
        /// 对应QQ群 群号
        /// </summary>
        public string QQGroup { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public long CreateTime { get; set; }

        public long UpdateTime { get; set; }
    }
}

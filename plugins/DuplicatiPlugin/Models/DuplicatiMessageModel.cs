using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicatiPlugin.Models
{
    public class DuplicatiMessageModel
    {
        /// <summary>
        /// 始终为 Duplicati
        /// </summary>
        public string Name { get; set; }

        public string OperationName { get; set; }

        public string BackupName { get; set; }

        /// <summary>
        /// Success
        /// Warning
        /// Fatal
        /// </summary>
        public string ParsedResult { get; set; }

        public string LocalPath { get; set; }

        public string RemoteUrl { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicatiPlugin.RequestModel
{
    public class DuplicatiRequestModel
    {
        /// <summary>
        /// cloudcone_wwwroot_nextcloud_to_mega
        /// </summary>
        public string BackupName { get; set; }

        public long DeletedFiles { get; set; }

        public long DeletedFolders { get; set; }

        public long ModifiedFiles { get; set; }

        public long ExaminedFiles { get; set; }

        public long OpenedFiles { get; set; }

        public long AddedFiles { get; set; }

        public long SizeOfModifiedFiles { get; set; }

        public long SizeOfAddedFiles { get; set; }

        public long SizeOfExaminedFiles { get; set; }

        public long SizeOfOpenedFiles { get; set; }

        public long NotProcessedFiles { get; set; }

        public long AddedFolders { get; set; }

        public long TooLargeFiles { get; set; }

        public long FilesWithError { get; set; }

        public long ModifiedFolders { get; set; }

        public long ModifiedSymlinks { get; set; }

        public long AddedSymlinks { get; set; }

        public long DeletedSymlinks { get; set; }

        public bool PartialBackup { get; set; }

        public bool Dryrun { get; set; }

        /// <summary>
        /// Backup
        /// </summary>
        public string MainOperation { get; set; }

        /// <summary>
        /// Success 等
        /// </summary>
        public string ParsedResult { get; set; }

        public string Version { get; set; }

        public string EndTime { get; set; }

        public string BeginTime { get; set; }

        public string Duration { get; set; }

        public long MessagesActualLength { get; set; }

        public long WarningsActualLength { get; set; }

        public long ErrorsActualLength { get; set; }

        public string[] LimitedMessages { get; set; }

        public string[] LimitedWarnings { get; set; }

        public string[] LimitedErrors { get; set; }

    }
}

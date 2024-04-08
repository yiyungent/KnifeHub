using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace MemosPlus
{
    public class SettingsModel : PluginSettingsModel
    {
        public long SecondsPeriod { get; set; }

        public MemosModel Memos { get; set; }

        public class MemosModel 
        {
            public string BaseUrl { get; set; }

            public string OpenId { get; set; }

            public string MemosSession { get; set; }
        }

        public GitHubModel GitHub { get; set; }

        public class GitHubModel
        {
            public string AccessToken { get; set; }

            public string RepoOwner { get; set; }

            public string RepoName { get; set; }

            public string RepoBranch { get; set; }

            public string RepoTargetDirPath { get; set; }

            /// <summary>
            /// 默认: memo-{{date}}.md
            /// eg: memo-2022-10-12-22-12-23.md
            /// </summary>
            /// <value></value>
            public string MemoFileName { get; set; }
        }

        public BackupModel Backup { get; set; }

        public class BackupModel 
        {
            public bool EnableBackupToGitHub { get;set; }

            public AllowedExecuteModel AllowedExecute { get; set; }

            public class AllowedExecuteModel
            {
                public int TimeHourFrom { get; set; }

                public int TimeHourTo { get; set; }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace HangfirePlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public string AppName { get; set; }

        public string ApiKey { get; set; }

        public BackupModel Backup { get; set; }

        public class BackupModel
        {
            public long SecondsPeriod { get; set; }

            public bool EnabledBackupToGitHub { get; set; }
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
            /// 默认: google-tasks.md
            /// 可用: google-tasks-{{date}}.md
            /// eg: google-tasks-2022-10-12-22-12-23.md
            /// </summary>
            /// <value></value>
            public string FileName { get; set; }
        }
    }
}

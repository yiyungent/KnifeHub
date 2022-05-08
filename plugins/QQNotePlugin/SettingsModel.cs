using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace QQNotePlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public List<string> AllowFriends { get; set; }


        public GitHubModel GitHub { get; set; }

        public class GitHubModel
        {
            public string AccessToken { get; set; }

            public string RepoOwner { get; set; }

            public string RepoName { get; set; }

            public string RepoBranch { get; set; }

            public string RepoTargetFilePath { get; set; }

        }
    }
}

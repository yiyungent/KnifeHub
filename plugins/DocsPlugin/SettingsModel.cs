using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace DocsPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public List<DocsGitHubSourceItemModel> DocsGitHubSources { get; set; }

        public List<DocsWebSourceItemModel> DocsWebSources { get; set; }

        public string Secret { get; set; }


        public SettingsModel()
        {
            this.DocsGitHubSources = new List<DocsGitHubSourceItemModel>();
            this.DocsWebSources = new List<DocsWebSourceItemModel>();
        }

        public sealed class DocsWebSourceItemModel
        {
            public string SourceKey { get; set; }

            public string Url { get; set; }
        }

        public sealed class DocsGitHubSourceItemModel
        {
            public string SourceKey { get; set; }

            public string RepoOwner { get; set; }

            public string RepoName { get; set; }

            public string RepoBranch { get; set; }

            public string RepoTargetDirPath { get; set; }

            public GitHubModel GitHub { get; set; }

            public sealed class GitHubModel
            {
                public string AccessToken { get; set; }
            }
        }


    }
}

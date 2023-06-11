using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace PluginStore
{
    public class SettingsModel : PluginSettingsModel
    {
        public SourceModel Source { get; set; }

        public sealed class SourceModel
        {
            public GitHubModel GitHub { get; set; }

            public sealed class GitHubModel
            {
                public string SearchTerm { get; set; }
            }
        }

    }
}

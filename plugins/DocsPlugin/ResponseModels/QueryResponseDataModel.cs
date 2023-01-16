using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPlugin.ResponseModels
{
    public class QueryResponseDataModel
    {
        public List<GitHubItemModel> GitHubList { get; set; }

        public sealed class GitHubItemModel
        {
            public string Content { get; set; }

            public string Url { get; set; }
        }
    }
}

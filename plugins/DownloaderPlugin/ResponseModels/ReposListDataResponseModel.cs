using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginStore.ResponseModels
{
    public class ReposListDataResponseModel
    {
        public int Page { get; set; }

        public int PerPage { get; set; }

        public int TotalCount { get; set; }

        public List<ReposItemModel> Repos { get; set; }

        public sealed class ReposItemModel
        {
            public string FullName { get; set; }
            public string OwnerName { get; set; }
            public string Name { get; set; }
            public string HtmlUrl { get; set; }
            public int StarCount { get; set; }
            public string Description { get; set; }
            public string LicenseName { get; set; }
            public string LicenseUrl { get; set; }
            public string UpdatedAt { get; set; }
        }
    }
}

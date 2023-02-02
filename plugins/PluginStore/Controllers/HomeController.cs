using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Downloader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using PluginCore;
using PluginStore.ResponseModels;

namespace PluginStore.Controllers
{
    /// <summary>
    /// 其实也可以不写这个, 直接访问 Plugins/PluginStore/index.html
    /// 
    /// 下面的方法, 是去掉 index.html
    /// 
    /// 若 wwwroot 下有其它需要访问的文件, 如何 css, js, 而你又不想每次新增 action 指定返回, 则 Route 必须 Plugins/{PluginId},
    /// 这样访问 Plugins/PluginStore/css/main.css 就会访问到你插件下的 wwwroot/css/main.css
    /// </summary>
    [Route($"Plugins/{nameof(PluginStore)}")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        #region Propertities
        public SettingsModel Settings
        {
            get
            {
                SettingsModel settings = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(PluginStore));

                return settings;
            }
        }
        #endregion

        #region Actions
        public async Task<ActionResult> Get()
        {
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(PluginStore)), "index.html");

            return PhysicalFile(indexFilePath, "text/html");
        }

        public async Task<BaseResponseModel<ReposListDataResponseModel>> ReposList(int page = 1, int perPage = 10)
        {
            BaseResponseModel<ReposListDataResponseModel> responseModel = new BaseResponseModel<ReposListDataResponseModel>();
            responseModel.Code = 1;
            responseModel.Message = "Success";

            // 1. GitHub 搜索 标识为 PluginCore 插件的仓库
            try
            {
                var github = new GitHubClient(new ProductHeaderValue(nameof(PluginStore)));
                var request = new SearchRepositoriesRequest(this.Settings.Source.GitHub.SearchTerm)
                {
                    // or go all out and search the readme, name or description?
                    In = new[] { InQualifier.Readme, InQualifier.Description, InQualifier.Name },

                    // sort by the number of stars
                    SortField = RepoSearchSort.Stars,

                    // how about changing that sort direction?
                    Order = SortDirection.Descending,

                    Page = page,
                    PerPage = perPage
                };
                var result = await github.Search.SearchRepo(request);
                responseModel.Data = new ReposListDataResponseModel
                {
                    Page = page,
                    PerPage = perPage,
                    TotalCount = result.TotalCount,
                    Repos = new List<ReposListDataResponseModel.ReposItemModel>()
                };
                foreach (var item in result.Items)
                {
                    string fullName = item.FullName;
                    string name = item.Name;
                    string ownerName = item.Owner.Name;
                    string url = item.HtmlUrl;
                    int starCount = item.StargazersCount;
                    string desc = item.Description;
                    string licenseName = item.License.Name;
                    string licenseUrl = item.License.Url;
                    var updated = item.UpdatedAt;
                    // 由于一个仓库可能不止提供一个插件, Releases 中也不止一个插件, 因此到这里还不能取插件具体信息
                    responseModel.Data.Repos.Add(new ReposListDataResponseModel.ReposItemModel
                    {
                        FullName = item.FullName,
                        Name = item.Name,
                        OwnerName = item.Owner.Name,
                        HtmlUrl = item.HtmlUrl,
                        StarCount = item.StargazersCount,
                        Description = item.Description,
                        LicenseName = item.License.Name,
                        LicenseUrl = item.License.Url,
                        UpdatedAt = item.UpdatedAt.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "Failure";
                Console.WriteLine(ex.ToString());

            }

            return responseModel;
        }


        public async Task<ActionResult> Releases(string owner, string name)
        {
            var github = new GitHubClient(new ProductHeaderValue(nameof(PluginStore)));
            // releases 中可能有多个不同插件, 1个插件的不同版本, 甚至可能 其中有不是插件的包
            var releases = await github.Repository.Release.GetAll(owner: owner, name: name);
            // TODO: 暂时干脆直接全部展示为列表, 后面+匹配与排列算法
            // 尝试从 Release.Name / Tag.Name 中解析 插件名, 版本
            // assets 排除非 zip , 尝试从文件名中解析 插件名, 版本, 目标 .NET 框架

            foreach (var release in releases)
            {
                foreach (var asset in release.Assets)
                {

                }
            }

            List<(string FileName, string DownloadUrl)> assets = new List<(string FileName, string DownloadUrl)>();

            // 2. 搜索 Releases
            // 3. 遍历 Releases, 遍历 Assets

            return Ok();
        }

        public async Task<ActionResult> DownloadZip(string downloadUrl)
        {
            var downloadOpt = new DownloadConfiguration()
            {
                ChunkCount = 8, // file parts to download, default value is 1
                ParallelDownload = true // download parts of file as parallel or not. Default value is false
            };
            var downloader = new DownloadService(downloadOpt);
            downloader.DownloadStarted += Downloader_DownloadStarted;
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;
            string filePath = @"Your_Path\fileName.zip";

            // Define the cancellation token.
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            await downloader.DownloadFileTaskAsync(address: downloadUrl, fileName: filePath, cancellationToken: cancellationToken.Token);
            //cancellationToken.Cancel();
            

            return Ok();
        }


        //[Route(nameof(Download))]
        //[Authorize("PluginCore.Admin")]
        //public async Task<ActionResult> Download()
        //{
        //    string dbFilePath = "";  // DbContext.DbFilePath;
        //    var fileStream = System.IO.File.OpenRead(dbFilePath);
        //    //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

        //    return File(fileStream: fileStream, contentType: "application/x-sqlite3", fileDownloadName: $"{nameof(BackupPlugin)}.sqlite", enableRangeProcessing: true);
        //}

        #endregion

        #region Download
        [NonAction]
        private void Downloader_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        [NonAction]
        private void Downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

        }

        [NonAction]
        private void Downloader_DownloadStarted(object sender, DownloadStartedEventArgs e)
        {

        }
        #endregion
    }
}

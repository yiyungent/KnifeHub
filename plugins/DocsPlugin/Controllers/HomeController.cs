using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore;
using PluginCore.Interfaces;
using DocsPlugin.Utils;
using DocsPlugin.ResponseModels;
using Octokit;

namespace DocsPlugin.Controllers
{
    /// <summary>
    /// 其实也可以不写这个, 直接访问 Plugins/ZhiDaoPlugin/index.html
    /// 
    /// 下面的方法, 是去掉 index.html
    /// 
    /// 若 wwwroot 下有其它需要访问的文件, 如何 css, js, 而你又不想每次新增 action 指定返回, 则 Route 必须 Plugins/{PluginId},
    /// 这样访问 Plugins/HelloWorldPlugin/css/main.css 就会访问到你插件下的 wwwroot/css/main.css
    /// </summary>
    [Route($"api/Plugins/{nameof(DocsPlugin)}")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;

        private readonly bool _debug;

        #endregion

        #region Ctor
        public HomeController(IPluginFinder pluginFinder)
        {
            _pluginFinder = pluginFinder;
            string debugStr = EnvUtil.GetEnv("DEBUG");
            if (!string.IsNullOrEmpty(debugStr) && bool.TryParse(debugStr, out bool debug))
            {
                _debug = debug;
            }
            else
            {
                _debug = false;
            }
        }
        #endregion

        [Route($"/Plugins/{nameof(DocsPlugin)}")]
        [HttpGet]
        [Authorize("PluginCore.Admin")]
        public async Task<ActionResult> Get()
        {
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(DocsPlugin)), "index.html");

            return PhysicalFile(indexFilePath, "text/html");
        }

        [Route(nameof(Query))]
        [HttpGet, HttpPost]
        public async Task<BaseResponseModel<QueryResponseDataModel>> Query(string q, string sourceKey = "", string secret = "")
        {
            BaseResponseModel<QueryResponseDataModel> responseModel = new BaseResponseModel<QueryResponseDataModel>();
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(DocsPlugin));
            if (!string.IsNullOrEmpty(secret) && secret != settingsModel.Secret)
            {
                responseModel.Code = -1;
                responseModel.Message = "Secret 不正确";
                return responseModel;
            }
            if (!string.IsNullOrEmpty(sourceKey))
            {
                settingsModel.DocsGitHubSources = settingsModel.DocsGitHubSources.Where(m => m.SourceKey == sourceKey).ToList();
                settingsModel.DocsWebSources = settingsModel.DocsWebSources.Where(m => m.SourceKey == sourceKey).ToList();
            }
            responseModel.Data = new QueryResponseDataModel();
            responseModel.Data.GitHubList = new List<QueryResponseDataModel.GitHubItemModel>();

            #region GitHub
            foreach (var githubSourceItem in settingsModel.DocsGitHubSources)
            {
                GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(nameof(DocsPlugin)));
                gitHubClient.Credentials = new Credentials(githubSourceItem.GitHub.AccessToken);

                #region 搜索
                try
                {
                    #region Issues
                    var issues = await gitHubClient.Issue.GetAllForRepository(owner: githubSourceItem.RepoOwner, name: githubSourceItem.RepoName);
                    issues = (from m in issues
                              where m.Title.ToLower().Contains(q.ToLower().Trim())
                              || m.Body.ToLower().Contains(q.ToLower().Trim())
                              select m).ToList();
                    foreach (var issueItem in issues)
                    {
                        responseModel.Data.GitHubList.Add(new QueryResponseDataModel.GitHubItemModel
                        {
                            Content = issueItem.Body,
                            Url = issueItem.Url
                        });
                    }
                    #endregion

                    #region File
                    //await gitHubClient.Repository.Content.GetAllContentsByRef(owner: githubSourceItem.RepoOwner, name: githubSourceItem.RepoName,
                    //     path: githubSourceItem.RepoTargetDirPath, reference: githubSourceItem.RepoBranch);
                    #endregion

                    responseModel.Message = "成功";
                }
                catch (Exception ex)
                {
                    LogUtil.Exception(ex);
                }
                #endregion

            }
            #endregion

            return responseModel;
        }

        [Route(nameof(Download))]
        [Authorize("PluginCore.Admin")]
        public async Task<ActionResult> Download()
        {
            string dbFilePath = DbContext.DbFilePath;
            var fileStream = System.IO.File.OpenRead(dbFilePath);
            //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            return File(fileStream: fileStream, contentType: "application/x-sqlite3", fileDownloadName: $"{nameof(DocsPlugin)}.sqlite", enableRangeProcessing: true);
        }


    }
}

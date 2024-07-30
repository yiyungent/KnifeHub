using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using MemosPlus.Utils;
using System.Text;
using Octokit;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PluginCore;
using Scriban;
using Microsoft.Extensions.Logging;

namespace MemosPlus
{
    public class MemosPlus : BasePlugin, IWidgetPlugin, IStartupXPlugin, IStartupPlugin, ITimeJobPlugin
    {
        #region Fields
        private readonly ILogger<MemosPlus> _logger;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        #endregion

        #region Props
        public long SecondsPeriod
        {
            get
            {
                var settings = PluginSettingsModelFactory.Create<SettingsModel>(nameof(MemosPlus));

                return settings.SecondsPeriod;
            }
        }

        public static bool AllowedExecute
        {
            get
            {
                bool isAllowed = false;
                var settings = PluginSettingsModelFactory.Create<SettingsModel>(nameof(MemosPlus));
                DateTime currentTime = DateTime.Now;
                if (settings?.Backup?.AllowedExecute != null)
                {
                    // 凌晨1点到5点的操作
                    isAllowed = (currentTime.Hour >= (settings?.Backup?.AllowedExecute?.TimeHourFrom ?? 1))
                                &&
                                (currentTime.Hour < (settings?.Backup?.AllowedExecute?.TimeHourTo ?? 5));
                }
                else
                {
                    isAllowed = true;
                }

                return isAllowed;
            }
        }
        #endregion

        public MemosPlus(ILogger<MemosPlus> logger, IServiceScopeFactory serviceScopeFactory)
        {
            this._logger = logger;
            this._serviceScopeFactory = serviceScopeFactory;
        }

        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(MemosPlus)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(MemosPlus)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        #region 定时任务
        public async Task ExecuteAsync()
        {
            try
            {
                var settings = PluginSettingsModelFactory.Create<SettingsModel>(nameof(MemosPlus));

                #region 备份到 GitHub
                // 备份到 GitHub
                if (settings.Backup.EnableBackupToGitHub && AllowedExecute)
                {
                    this._logger.LogWarning($"开始执行定时任务: 备份到 GitHub");

                    await using (var scope = this._serviceScopeFactory.CreateAsyncScope())
                    {
                        var httpXUtil = scope.ServiceProvider.GetRequiredService<HttpXUtil>();
                        MemosUtil memosUtil = new MemosUtil(settings.Memos.BaseUrl, httpXUtil);
                        int offset = 0;
                        bool isErrorMemosApi = false;
                        bool isErrorMemosApiResource = false;
                        var list = new List<MemoItemModel>();
                        try
                        {
                            list = await memosUtil.List(memosSession: settings.Memos.MemosSession, offset: 0, limit: 20);
                        }
                        catch (System.Exception ex)
                        {
                            // 只要 memosUtil.List 出现一次 异常, 就标记放弃删除 GitHub 文件, 防止因为没有记录 而误删除 GitHub 文件
                            isErrorMemosApi = true;
                            System.Console.WriteLine(ex.ToString());
                        }
                        GitHubUtil gitHubUtil = new GitHubUtil();
                        settings.GitHub.RepoTargetDirPath = settings.GitHub.RepoTargetDirPath.Trim().TrimEnd('/');

                        // 解析模版
                        string githubTemplateFilePath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(MemosPlus), "templates", "github.md");
                        string githubTemplateContent = File.ReadAllText(githubTemplateFilePath, System.Text.Encoding.UTF8);
                        // https://github.com/scriban/scriban
                        var githubTemplate = Template.Parse(githubTemplateContent);
                        List<string> memoFilePaths = new List<string>();
                        while (list != null && list.Count >= 1 && AllowedExecute)
                        {
                            foreach (Utils.MemoItemModel item in list)
                            {
                                try
                                {
                                    // 纯文本
                                    string memoFileName = settings.GitHub.MemoFileName.Replace("{{date}}", item.createdTs.ToDateTime10().ToString("yyyy-MM-dd-HH-mm-ss"));
                                    string repoTargetFilePath = $"{settings.GitHub.RepoTargetDirPath}/{item.creatorName}/{memoFileName}";
                                    // 注意: 先放进来, 防止后面报错导致没有放进去, 从而最终导致没有记录而误删
                                    memoFilePaths.Add(repoTargetFilePath);
                                    string createTimeStr = item.createdTs.ToDateTime10().ToString("yyyy-MM-dd HH:mm:ss");
                                    string updateTimeStr = item.updatedTs.ToDateTime10().ToString("yyyy-MM-dd HH:mm:ss");

                                    // 资源文件
                                    StringBuilder resourceFileMdSb = new StringBuilder();
                                    foreach (var resourceItem in item.resourceList)
                                    {
                                        string memoResourceFileName = $"image-{resourceItem.id}{Path.GetExtension(resourceItem.filename)}";
                                        string repoTargetResourceFilePath = $"{settings.GitHub.RepoTargetDirPath}/{item.creatorName}/{Path.GetFileNameWithoutExtension(memoFileName)}/{memoResourceFileName}";
                                        try
                                        {
                                            var resourceFile = await memosUtil.Resource(resourceId: resourceItem.id.ToString(), fileName: resourceItem.filename, memosSession: settings.Memos.MemosSession);
                                            if (resourceFile != null && resourceFile.Length > 0)
                                            {
                                                gitHubUtil.UpdateFile(
                                                    repoOwner: settings.GitHub.RepoOwner,
                                                    repoName: settings.GitHub.RepoName,
                                                    repoBranch: settings.GitHub.RepoBranch,
                                                    repoTargetFilePath: repoTargetResourceFilePath,
                                                    fileContent: resourceFile,
                                                    accessToken: settings.GitHub.AccessToken
                                                );
                                                // resourceFileMdSb.AppendLine($"![{memoResourceFileName}]({Path.GetFileNameWithoutExtension(memoFileName)}/{memoResourceFileName})   ");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            System.Console.WriteLine(ex.ToString());
                                            isErrorMemosApiResource = true;
                                        }

                                        // 注意: 对于资源文件的引用来说，无论资源文件是否成功获取, 它都是存在的，其在 md 文件中的引用字符串不会变
                                        resourceFileMdSb.AppendLine($"![{memoResourceFileName}]({Path.GetFileNameWithoutExtension(memoFileName)}/{memoResourceFileName})   ");

                                        // 注意: 资源文件路径也要放入其中
                                        memoFilePaths.Add(repoTargetResourceFilePath);
                                    }
                                    // if (!isErrorMemosApiResource)
                                    // {

                                    // }
                                    #region md 文件内容
                                    // 其实总体来看 md 文件内容不会因为 具体的资源文件API成功与否改变内容，因此不需要关联资源 API Error

                                    // 注意: By default, Properties and methods of .NET objects are automatically exposed with lowercase and `_` names. 
                                    // It means that a property like `MyMethodIsNice` will be exposed as `my_method_is_nice`.
                                    // https://github.com/scriban/scriban
                                    string githubRenderResult = githubTemplate.Render(new
                                    {
                                        Memo = item,
                                        Date = createTimeStr,
                                        Updated = updateTimeStr,
                                        Public = item.visibility != "PRIVATE",
                                        ResourceList = resourceFileMdSb.ToString()
                                    });
                                    // githubRenderResult += resourceFileMdSb.ToString();

                                    gitHubUtil.UpdateFile(
                                        repoOwner: settings.GitHub.RepoOwner,
                                        repoName: settings.GitHub.RepoName,
                                        repoBranch: settings.GitHub.RepoBranch,
                                        repoTargetFilePath: repoTargetFilePath,
                                        fileContent: githubRenderResult,
                                        accessToken: settings.GitHub.AccessToken
                                    );
                                    #endregion
                                }
                                catch (System.Exception ex)
                                {
                                    System.Console.WriteLine(ex.ToString());
                                }
                            }
                            offset = offset + list.Count;
                            try
                            {
                                list = await memosUtil.List(memosSession: settings.Memos.MemosSession, offset: offset, limit: 20);
                            }
                            catch (System.Exception ex)
                            {
                                isErrorMemosApi = true;
                                System.Console.WriteLine(ex.ToString());
                            }
                        }

                        #region 清理不存在的文件
                        if (!isErrorMemosApi && !isErrorMemosApiResource && AllowedExecute)
                        {
                            // 清理不存在的文件: 对于存放 memos 的文件夹, 清理 memos 中已删除的对应文件
                            try
                            {
                                List<string> githubExistFilePaths = gitHubUtil.Files(
                                    repoOwner: settings.GitHub.RepoOwner,
                                    repoName: settings.GitHub.RepoName,
                                    repoBranch: settings.GitHub.RepoBranch,
                                    repoTargetDirPath: $"{settings.GitHub.RepoTargetDirPath}/",
                                    accessToken: settings.GitHub.AccessToken);
                                // 对于 GitHub 存在 但 memos 并没有此对应文件, 就需要删除
                                List<string> deletedFilePaths = githubExistFilePaths.Where(m => !memoFilePaths.Contains(m)).ToList();
                                foreach (var deletedFilePath in deletedFilePaths)
                                {
                                    try
                                    {
                                        gitHubUtil.Delete(
                                            repoOwner: settings.GitHub.RepoOwner,
                                            repoName: settings.GitHub.RepoName,
                                            repoBranch: settings.GitHub.RepoBranch,
                                            repoTargetFilePath: deletedFilePath,
                                            accessToken: settings.GitHub.AccessToken
                                        );
                                    }
                                    catch (System.Exception ex)
                                    {
                                        System.Console.WriteLine($"删除失败: {deletedFilePath}");
                                        System.Console.WriteLine(ex.ToString());
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                System.Console.WriteLine(ex.ToString());
                            }
                        }
                        #endregion
                    }

                    this._logger.LogWarning($"完成执行定时任务: 备份到 GitHub");
                }
                #endregion

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"执行定时任务失败: {ex.ToString()}");
            }

            await Task.CompletedTask;
        }
        #endregion

        public async Task<string> Widget(string widgetKey, params string[] extraPars)
        {
            string rtnStr = null;
            if (widgetKey == "memos")
            {
                if (extraPars != null && extraPars.Length >= 1)
                {
                    Console.WriteLine(string.Join(",", extraPars));
                    string memosVersion = extraPars[0];
                    string memosPart = "";
                    if (extraPars.Length >= 2)
                    {
                        memosPart = extraPars[1];
                        switch (memosPart)
                        {
                            case "banner-wrapper":
                                // banner-wrapper
                                rtnStr = @"<script>
                                    console.log(""测试"");
                                    </script>";
                                break;
                            default:
                                break;
                        }

                    }

                }
            }

            return await Task.FromResult(rtnStr);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<Middlewares.CorsMiddleware>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<HttpXUtil>();
        }

        void IStartupPlugin.ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<HttpXUtil>();
        }

        public int ConfigureServicesOrder => 0;

        public int ConfigureOrder => 0;

    }
}

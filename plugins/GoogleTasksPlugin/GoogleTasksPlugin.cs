using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using GoogleTasksPlugin.Utils;
using System.Text;
using Octokit;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PluginCore;
using Scriban;
using Microsoft.Extensions.Logging;

namespace GoogleTasksPlugin
{
    public class GoogleTasksPlugin : BasePlugin, ITimeJobPlugin
    {
        #region Fields
        private readonly ILogger<GoogleTasksPlugin> _logger;
        #endregion

        #region Props
        public long SecondsPeriod
        {
            get
            {
                return 1;
            }
        }

        public DateTime GoogleTasksBackupToGitHubLastExecute { get; set; }
        #endregion

        #region Ctor
        public GoogleTasksPlugin(ILogger<GoogleTasksPlugin> loggerGoogleTasksPlugin, ILogger<GitHubUtil> loggerGitHubUtil)
        {
            _logger = loggerGoogleTasksPlugin;
            GitHubUtil.Logger = loggerGitHubUtil;
        }
        #endregion

        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(GoogleTasksPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(GoogleTasksPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        #region 定时任务
        public async Task ExecuteAsync()
        {
            try
            {
                var settings = PluginSettingsModelFactory.Create<SettingsModel>(nameof(GoogleTasksPlugin));

                #region Google Tasks 备份到 GitHub
                {
                    try
                    {
                        if (settings.Backup.EnabledBackupToGitHub && DateTime.Now - GoogleTasksBackupToGitHubLastExecute > TimeSpan.FromSeconds(settings.Backup.SecondsPeriod))
                        {
                            GoogleTasksBackupToGitHubLastExecute = DateTime.Now;

                            #region 备份到 GitHub

                            // 开始备份任务
                            GitHubUtil gitHubUtil = new GitHubUtil();
                            settings.GitHub.RepoTargetDirPath = settings.GitHub.RepoTargetDirPath.Trim().TrimEnd('/');

                            // 解析模版
                            string githubTemplateFilePath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(GoogleTasksPlugin), "templates", "github.md");
                            string githubTemplateContent = File.ReadAllText(githubTemplateFilePath, System.Text.Encoding.UTF8);
                            // https://github.com/scriban/scriban
                            var githubTemplate = Template.Parse(githubTemplateContent);

                            var googleTasksModel = GoogleTasksUtil.Tasks(appName: settings.AppName, apiKey: settings.ApiKey);

                            string githubRenderResult = githubTemplate.Render(googleTasksModel);
                            string repoTargetFilePath = $"{settings.GitHub.RepoTargetDirPath}/{settings.GitHub.FileName}";
                            gitHubUtil.UpdateFile(
                                repoOwner: settings.GitHub.RepoOwner,
                                repoName: settings.GitHub.RepoName,
                                repoBranch: settings.GitHub.RepoBranch,
                                repoTargetFilePath: repoTargetFilePath,
                                fileContent: githubRenderResult,
                                accessToken: settings.GitHub.AccessToken
                            );

                            #endregion

                            Console.WriteLine($"执行定时任务成功: Google Tasks-备份记录");
                            _logger.LogInformation($"执行定时任务成功: Google Tasks-备份记录");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine($"执行定时任务失败: Google Tasks-备份记录: {ex.ToString()}");
                        _logger.LogError(ex, $"执行定时任务失败: Google Tasks-备份记录:");
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行定时任务失败: {ex.ToString()}");
                _logger.LogError(ex, $"执行定时任务失败:");
            }

            await Task.CompletedTask;
        }
        #endregion

    }
}

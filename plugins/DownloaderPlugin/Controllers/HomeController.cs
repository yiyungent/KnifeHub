using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Downloader;
using DownloaderPlugin;
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

        // public async Task<ActionResult> DownloadFile(string downloadUrl, string filePath)
        // {
        //     var downloadOpt = new DownloadConfiguration()
        //     {
        //         ChunkCount = 8, // file parts to download, default value is 1
        //         ParallelDownload = true // download parts of file as parallel or not. Default value is false
        //     };
        //     var downloader = new DownloadService(downloadOpt);
        //     downloader.DownloadStarted += Downloader_DownloadStarted;
        //     downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
        //     downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;

        //     // Define the cancellation token.
        //     CancellationTokenSource cancellationToken = new CancellationTokenSource();
        //     await downloader.DownloadFileTaskAsync(address: downloadUrl, fileName: filePath, cancellationToken: cancellationToken.Token);
        //     //cancellationToken.Cancel();

        //     // 保存起来
        //     DownloaderStore.Downloaders.Add(new DownloaderStore.DownloaderItemModel
        //     {
        //         DownloadService = downloader,
        //         DownloadConfiguration = downloadOpt,
        //         CancellationTokenSource = cancellationToken
        //     });

        //     return Ok();
        // }

        #endregion

        #region NonActions

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

        #endregion
    }
}

using Downloader;

namespace DownloaderPlugin;

/// <summary>
/// https://github.com/aria2/aria2
/// https://aria2.github.io
/// </summary> 
public class Aria2Util
{
    static Aria2Util()
    {

    }

    public static void Init()
    {
        string filePath = "";
        if (!File.Exists(filePath))
        {
            string fileName = "";
            


            string downloadUrl = $"https://github.com/aria2/aria2/releases/download/release-1.36.0/{fileName}";
            var downloadOpt = new DownloadConfiguration()
            {
                ChunkCount = 8, // file parts to download, default value is 1
                ParallelDownload = true // download parts of file as parallel or not. Default value is false
            };
            var downloader = new DownloadService(downloadOpt);

            // downloader.DownloadStarted += Downloader_DownloadStarted;
            // downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            // downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;

            // Define the cancellation token.
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            downloader.DownloadFileTaskAsync(address: downloadUrl, fileName: filePath, cancellationToken: cancellationToken.Token);
        }
    }
}

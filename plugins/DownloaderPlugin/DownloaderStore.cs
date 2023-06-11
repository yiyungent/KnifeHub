using Downloader;
namespace DownloaderPlugin;

public class DownloaderStore
{
    public static List<DownloaderItemModel> Downloaders { get; set; }

    static DownloaderStore()
    {
        Downloaders = new List<DownloaderItemModel>();
    }

    public class DownloaderItemModel
    {
        public DownloadService DownloadService { get; set; }
        public DownloadConfiguration DownloadConfiguration { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore;

namespace ZhiDaoPlugin.Controllers
{
    /// <summary>
    /// 其实也可以不写这个, 直接访问 Plugins/ZhiDaoPlugin/index.html
    /// 
    /// 下面的方法, 是去掉 index.html
    /// 
    /// 若 wwwroot 下有其它需要访问的文件, 如何 css, js, 而你又不想每次新增 action 指定返回, 则 Route 必须 Plugins/{PluginId},
    /// 这样访问 Plugins/HelloWorldPlugin/css/main.css 就会访问到你插件下的 wwwroot/css/main.css
    /// </summary>
    [Route($"Plugins/{nameof(ZhiDaoPlugin)}")]
    public class HomeController : Controller
    {
        public async Task<ActionResult> Get()
        {
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(ZhiDaoPlugin)), "index.html");

            return PhysicalFile(indexFilePath, "text/html");
        }

        [Route(nameof(Download))]
        [Authorize("PluginCore.Admin")]
        public async Task<ActionResult> Download()
        {
            string dbFilePath = DbContext.DbFilePath;
            var fileStream = System.IO.File.OpenRead(dbFilePath);
            //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            return File(fileStream: fileStream, contentType: "application/x-sqlite3", fileDownloadName: $"{nameof(ZhiDaoPlugin)}.sqlite", enableRangeProcessing: true);
        }
    }
}

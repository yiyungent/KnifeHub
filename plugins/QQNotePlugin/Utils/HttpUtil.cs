using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QQNotePlugin.Utils
{
    public class HttpUtil
    {
        #region HTTP下载文件
        /// <summary>
        /// HTTP 下载文件
        /// </summary>
        public static byte[] HttpDownloadFile(string url)
        {
            // 设置参数
            HttpClient client = new HttpClient();
            
            byte[] rtn = null;
            try
            {
                rtn = client.GetByteArrayAsync(url).Result;
            }
            catch (Exception ex)
            {

            }

            return rtn;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TgClientPlugin.Utils
{
    public class HttpUtil
    {
        #region Http Get
        /// <summary>
        /// HTTP Get请求
        /// </summary>
        /// <param name="url">请求目标URL</param>
        /// <param name="isPost"></param>
        /// <param name="referer"></param>
        /// <param name="cookies"></param>
        /// <param name="ua"></param>
        /// <returns>返回请求回复字符串</returns>
        public static string HttpGet(string url, StringBuilder responseHeadersSb = null, string[] headers = null, WebProxy proxy = null)
        {
            string rtResult = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.KeepAlive = false;

                if (headers != null)
                {
                    foreach (string header in headers)
                    {
                        string[] temp = header.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                        if (temp[0].Equals("Referer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.Referer = temp[1];
                        }
                        else if (temp[0].Equals("User-Agent", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.UserAgent = temp[1];
                        }
                        else if (temp[0].Equals("Accept", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.Accept = temp[1];
                        }
                        else if (temp[0].Equals("Connection", StringComparison.InvariantCultureIgnoreCase) && temp[1].Equals("keep-alive", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.KeepAlive = true;
                        }
                        else if (temp[0].Equals("Connection", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.KeepAlive = false;
                        }
                        else if (temp[0].Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.ContentType = temp[1];
                        }
                        else
                        {
                            request.Headers.Add(header);
                        }
                    }
                }
                if (proxy != null)
                {
                    request.Proxy = proxy;
                }
                request.Timeout = 10000;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (responseHeadersSb != null)
                {
                    foreach (string name in response.Headers.AllKeys)
                    {
                        responseHeadersSb.AppendLine(name + ": " + response.Headers[name]);
                    }
                }
                Stream responseStream = response.GetResponseStream();
                //如果http头中接受gzip的话，这里就要判断是否为有压缩，有的话，直接解压缩即可 
                if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"].ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                using (StreamReader sReader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
                {
                    rtResult = sReader.ReadToEnd();
                }
                responseStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rtResult;
        }
        #endregion

        #region Http Post
        public static string HttpPost(string url, string postDataStr = "", StringBuilder responseHeadersSb = null, string[] headers = null, WebProxy proxy = null)
        {
            string rtResult = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.KeepAlive = false;
                if (headers != null)
                {
                    foreach (string header in headers)
                    {
                        string[] temp = header.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                        if (temp[0].Equals("Referer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.Referer = temp[1];
                        }
                        else if (temp[0].Equals("User-Agent", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.UserAgent = temp[1];
                        }
                        else if (temp[0].Equals("Accept", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.Accept = temp[1];
                        }
                        else if (temp[0].Equals("Connection", StringComparison.InvariantCultureIgnoreCase) && temp[1].Equals("keep-alive", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.KeepAlive = true;
                        }
                        else if (temp[0].Equals("Connection", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.KeepAlive = false;
                        }
                        else if (temp[0].Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                        {
                            request.ContentType = temp[1];
                        }
                        else
                        {
                            request.Headers.Add(header);
                        }
                    }
                }
                if (proxy != null)
                {
                    request.Proxy = proxy;
                }
                request.Timeout = 10000;
                byte[] postBytes = Encoding.UTF8.GetBytes(postDataStr);
                request.ContentLength = postBytes.Length;
                // 写 content-body 一定要在属性设置之后
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (responseHeadersSb != null)
                {
                    foreach (string name in response.Headers.AllKeys)
                    {
                        responseHeadersSb.AppendLine(name + ": " + response.Headers[name]);
                    }
                }
                Stream responseStream = response.GetResponseStream();
                //如果http头中接受gzip的话，这里就要判断是否为有压缩，有的话，直接解压缩即可  
                if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"].ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                using (StreamReader sReader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
                {
                    rtResult = sReader.ReadToEnd();
                }
                responseStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return rtResult;
        }
        #endregion

        #region HTTP下载文件
        /// <summary>
        /// HTTP 下载文件
        /// </summary>
        public static string HttpDownloadFile(string url, string filePath)
        {
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            //创建本地文件写入流
            Stream stream = new FileStream(filePath, FileMode.Create);
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
            }
            stream.Close();
            responseStream.Close();

            return filePath;
        }
        #endregion


        #region HTTP上传文件
        public static async Task<string> UploadFileAsync(string url, string filePath, Dictionary<string, string> formDataPairs = null, string cookieValue = null)
        {
            string fileName = Path.GetFileName(filePath);
            using (var client = new HttpClient())
            {
                //using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.Ticks.ToString("x")))
                using (var content = new MultipartFormDataContent())
                {
                    // TODO: 一口气读出，一口气发送
                    var upfilebytes = File.ReadAllBytes(filePath);
                    var ms = new MemoryStream(upfilebytes);

                    if (formDataPairs != null)
                    {
                        foreach (var item in formDataPairs)
                        {
                            // 添加字符串参数，参数名: item.Key, 参数值: item.Value
                            content.Add(new StringContent(item.Value), item.Key);
                        }
                    }

                    // 添加文件参数，参数名为file，文件名为 fileName
                    content.Add(new StreamContent(ms), "file", fileName);

                    if (cookieValue != null)
                    {
                        // Cookie: JSESSIONID=1A51768683B2FBAEB5B066992A229A13
                        content.Headers.Add("Cookie", cookieValue);
                    }


                    using (var httpResponseMessage = await client.PostAsync(url, content))
                    {
                        var responseContent = "";
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                        }
                        return responseContent;
                    }
                }
            }
        }
        #endregion


        public static async Task<(string response, string cookie)> LoginSessionAsync(string url, string userName, string password)
        {
            (string response, string cookie) rtn = (null, null);
            // 通过设置handler.UseCookies=true(默认为true)，默认的会自己带上cookies (请求登录后，获取 Set-Cookie)
            var handler = new HttpClientHandler() { UseCookies = true };
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=900");
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("userName", userName),
                    new KeyValuePair<string, string>("password", password),
                });

                using (var httpResponseMessage = await client.PostAsync(url, content))
                {
                    var responseContent = "";
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        // Set-Cookie: JSESSIONID=F839F04109B851F5C86F6657C756A484; Path=/; HttpOnly
                        // 获取 Set-Cookie
                        if (httpResponseMessage.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> cookies))
                        {
                            rtn.cookie = cookies?.Where(m => m.StartsWith("JSESSIONID="))?.FirstOrDefault();
                            // TODO: 不确定, 末尾有没有 ;
                            rtn.cookie = rtn.cookie.Substring("JSESSIONID=".Length);
                        }


                        responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                        rtn.response = responseContent;
                    }

                    return rtn;
                }
            };
        }

        public static async Task<(string response, string token)> LoginBearerTokenAsync(string url, string userName, string password)
        {
            (string response, string token) rtn = (null, null);
            // 通过设置handler.UseCookies=true(默认为true)，默认的会自己带上cookies (请求登录后，获取 Set-Cookie)
            var handler = new HttpClientHandler() { UseCookies = true };
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=900");
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("userName", userName),
                    new KeyValuePair<string, string>("password", password),
                });

                using (var httpResponseMessage = await client.PostAsync(url, content))
                {
                    var responseContent = "";
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        // Authorization: Bearer xxxxxx
                        // 获取 token
                        if (httpResponseMessage.Headers.TryGetValues("Authorization", out IEnumerable<string> authorizationItems))
                        {
                            rtn.token = authorizationItems.First();
                            rtn.token = rtn.token.Substring("Bearer ".Length);
                        }

                        responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                        rtn.response = responseContent;
                    }

                    return rtn;
                }
            };
        }

    }
}

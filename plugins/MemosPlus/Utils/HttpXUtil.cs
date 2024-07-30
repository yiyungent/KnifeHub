using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemosPlus.Utils
{
    public class HttpXUtil
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpXUtil> _logger;

        public HttpXUtil(IHttpClientFactory httpClientFactory, ILogger<HttpXUtil> logger)
        {
            this._httpClientFactory = httpClientFactory;
            this._logger = logger;
        }

        #region 辅助
        public class Header
        {
            /// <summary>
            /// Content-Type
            /// </summary>
            public class ContentType
            {
                public const string Json = "application/json; charset=UTF-8";
                public const string FormUrlEncoded = "application/x-www-form-urlencoded; charset=UTF-8";
            }

            /// <summary>
            /// User-Agent
            /// </summary>
            public const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
        }

        public static TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(2);

        #endregion

        #region HTTP下载文件
        /// <summary>
        /// HTTP 下载文件
        /// </summary>
        public async Task<byte[]> HttpGetFileAsync(string url, string cookie = null)
        {
            // 设置参数
            HttpClient client = this._httpClientFactory.CreateClient();
            if (!string.IsNullOrEmpty(cookie))
            {
                client.DefaultRequestHeaders.Add("cookie", cookie);
            }

            byte[] rtn = null;
            try
            {
                rtn = await client.GetByteArrayAsync(url);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"HttpXUtil.HttpGetFileAsync({url}, {cookie})");
            }

            return rtn;
        }
        #endregion

        #region Http Get

        public async Task<(string Res, HttpStatusCode StatusCode)> HttpGetAsync(string url, Dictionary<string, string> headers = null, StringBuilder responseHeadersSb = null)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            (string Res, HttpStatusCode StatusCode) rtn = (null, HttpStatusCode.OK);

            try
            {
                var client = _httpClientFactory.CreateClient();
                {
                    //client.Timeout = Timeout.InfiniteTimeSpan;
                    client.Timeout = Timeout;

                    if (headers != null)
                    {
                        foreach (KeyValuePair<string, string> header in headers)
                        {
                            client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                        }
                    }

                    var response = await client.GetAsync(url);

                    if (responseHeadersSb != null)
                    {
                        foreach (var header in response.Headers)
                        {
                            responseHeadersSb.AppendLine(header.Key + ": " + string.Join(", ", header.Value));
                        }
                    }

                    if (response.Content.Headers?.ContentEncoding?.ToString()?.ToLower()?.Contains("gzip") ?? false)
                    {
                        var responseStream = await response.Content.ReadAsStreamAsync();
                        using (Stream decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                        using (var reader = new StreamReader(decompressedStream))
                        {
                            rtn.Res = await reader.ReadToEndAsync();
                        }
                    }
                    else
                    {
                        rtn.Res = await response.Content.ReadAsStringAsync();
                    }

                    rtn.StatusCode = response.StatusCode;
                }
            }
            catch (Exception)
            {
                throw;
            }

            stopwatch.Stop();
            this._logger.LogWarning($"HttpXUtil.HttpGetAsync【{stopwatch.ElapsedMilliseconds}ms】{url}");

            return rtn;
        }

        #endregion

        #region Http Post
        
        public async Task<(string Res, HttpStatusCode StatusCode)> HttpPostAsync(string url, string postDataStr = "", Dictionary<string, string> headers = null, StringBuilder responseHeadersSb = null)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            (string Res, HttpStatusCode StatusCode) rtn = (null, HttpStatusCode.OK);

            try
            {
                var client = _httpClientFactory.CreateClient();
                {
                    //client.Timeout = Timeout.InfiniteTimeSpan;
                    client.Timeout = Timeout;

                    var httpContent = new StringContent(postDataStr, Encoding.UTF8);

                    if (headers != null)
                    {
                        foreach (KeyValuePair<string, string> header in headers)
                        {
                            // fixed: format error: { "Content-Type", "application/x-www-form-urlencoded; charset=UTF-8" }
                            if (header.Key.Equals("Content-Type", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // `HttpClient` 在发送请求时，会默认忽略 `DefaultRequestHeaders` 中的某些标头信息，例如 `Content-Type`。原因是 HTTP 协议规定这些头只能用在实际的内容中（即 `HttpContent Headers`），而不是HTTP请求头部。
                                // 因此，要设置 `Content-Type`，你需要在你的 `HttpContent`（比如 `StringContent`, `FormUrlEncodedContent`, `MultipartFormDataContent` 等）中进行设置。
                                if (MediaTypeHeaderValue.TryParse(header.Value, out var mediaType))
                                {
                                    httpContent.Headers.ContentType = mediaType;
                                }
                            }
                            else
                            {
                                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                            }
                            //client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                        }
                    }

                    var response = await client.PostAsync(url, httpContent);

                    if (responseHeadersSb != null)
                    {
                        foreach (var header in response.Headers)
                        {
                            responseHeadersSb.AppendLine(header.Key + ": " + string.Join(", ", header.Value));
                        }
                    }

                    if (response.Content.Headers?.ContentEncoding?.ToString()?.ToLower()?.Contains("gzip") ?? false)
                    {
                        var responseStream = await response.Content.ReadAsStreamAsync();
                        using (Stream decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                        using (StreamReader reader = new StreamReader(decompressedStream, Encoding.UTF8))
                        {
                            rtn.Res = await reader.ReadToEndAsync();
                        }
                    }
                    else
                    {
                        rtn.Res = await response.Content.ReadAsStringAsync();
                    }

                    rtn.StatusCode = response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            stopwatch.Stop();
            this._logger.LogWarning($"HttpXUtil.HttpPostAsync【{stopwatch.ElapsedMilliseconds}ms】{url}");

            return rtn;
        }

        #endregion

        #region HTTP下载文件
        /// <summary>
        /// HTTP 下载文件
        /// </summary>
        public string HttpDownloadFile(string url, string filePath)
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
        public async Task<string> UploadFileAsync(string url, string filePath, Dictionary<string, string> formDataPairs = null, string cookieValue = null)
        {
            string fileName = Path.GetFileName(filePath);
            var client = _httpClientFactory.CreateClient();
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

        #region 其它
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
        #endregion

    }
}

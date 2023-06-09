using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PluginCore.Interfaces;
using PluginCore.IPlugins;
using EleCho.GoCqHttpSdk.Model;
using EleCho.GoCqHttpSdk.Post;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post.Model;
using EleCho.GoCqHttpSdk.Utils;
using System.Security.Cryptography;
using System.Text.Json;
using System.Net;

namespace EleChoPlugin.Middlewares
{
    /// <summary>
    /// TODO: 目的: 反向 HTTP 与 ASP.NET Core Kestrel 共用一个端口
    /// </summary>
    public class GoCqHttpMiddleware
    {
        #region Fields
        private readonly RequestDelegate _next;
        readonly string? secret;
        HMACSHA1? sha1;
        CqPostPipeline postPipeline;
        #endregion

        #region Ctor
        public GoCqHttpMiddleware(RequestDelegate next)
        {
            _next = next;
            if (secret != null)
            {
                byte[] tokenBin = Encoding.UTF8.GetBytes(secret);
                sha1 = new HMACSHA1(tokenBin);
            }
            postPipeline = new CqPostPipeline();
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="pluginFinder">测试，是否运行时添加的Middleware，是否可以依赖注入</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext, IPluginFinder pluginFinder)
        {
            bool isMatch = false;

            isMatch = httpContext.Request.Path.Value.StartsWith($"/{nameof(EleChoPlugin)}", StringComparison.CurrentCultureIgnoreCase);

            if (isMatch)
            {
                string configId = httpContext.Request.Path.Value.Substring(httpContext.Request.Path.Value.IndexOf($"{nameof(EleChoPlugin)}-") + $"{nameof(EleChoPlugin)}-".Length).Trim('/');
                // go-cqhttp: 反向 HTTP
                using MemoryStream ms = new MemoryStream();
                httpContext.Request.Body.CopyTo(ms);
                byte[] data = ms.ToArray();
                if (Verify(httpContext.Request.Headers["X-Signature"], data))
                {
                    // string json = GlobalConfig.TextEncoding.GetString(data);
                    // CqWsDataModel? wsDataModel = JsonSerializer.Deserialize<CqWsDataModel>(json, JsonHelper.Options);
                    // if (wsDataModel is CqPostModel postModel)
                    // {
                    //     CqPostContext? postContext = CqPostContext.FromModel(postModel);

                    //     // 从存储中取出目标 Session 
                    //     var bot = EleChoBotStore.Bots.FirstOrDefault(m => m.ConfigId == configId);
                    //     if (bot != null && bot.CqHttpSession != null) {
                    //         postContext?.SetSession(bot.CqHttpSession);

                    //         if (postContext is CqPostContext)
                    //         {
                    //             await postPipeline.ExecuteAsync(postContext);

                    //             if (postContext.QuickOperationModel is object quickActionModel)
                    //                 JsonSerializer.Serialize(httpContext.Response.Body, quickActionModel, quickActionModel.GetType(), JsonHelper.Options);

                    //             httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    //             // context.Response.Close();
                    //             // continue;
                    //         }
                    //     }
                    //}

                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    // context.Response.Close();
                }
                else
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    // context.Response.Close();
                }

            }

            // Call the next delegate/middleware in the pipeline
            await _next(httpContext);
        }

        private bool Verify(string? signature, byte[] data)
        {
            if (signature == null)
                return sha1 == null;
            if (sha1 == null)
                return false;

            if (signature.StartsWith("SHA1=", StringComparison.OrdinalIgnoreCase))
                signature = signature.Substring(5);

            byte[] hash = sha1.ComputeHash(data);
            string realSignature = string.Join(null, hash.Select(bt => Convert.ToString(bt, 16).PadLeft(2, '0')));
            return signature == realSignature;
        }
        #endregion
    }
}

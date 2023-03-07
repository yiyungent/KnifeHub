using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PluginCore.Interfaces;
using PluginCore.IPlugins;

namespace MemosPlus.Middlewares
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// 在 <see cref="PluginApplicationBuilder"/> Build 时, 将会 new Middleware(), 最终将所有 Middleware 包装为一个 <see cref="RequestDelegate"/>
        /// </summary>
        /// <param name="next"></param>
        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="pluginFinder">测试，是否运行时添加的Middleware，是否可以依赖注入</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext, IPluginFinder pluginFinder)
        {
            bool isMatch = false;

            isMatch = httpContext.Request.Path.Value.StartsWith("/api/plugincore/PluginWidget/Widget", StringComparison.CurrentCultureIgnoreCase);

            if (isMatch)
            {
                httpContext.Response.Headers.Add("access-control-allow-origin", "*");
            }
            
            // Call the next delegate/middleware in the pipeline
            await _next(httpContext);
        }

    }
}

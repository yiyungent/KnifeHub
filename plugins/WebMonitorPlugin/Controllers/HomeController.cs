using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebMonitorPlugin.Infrastructure;
using WebMonitorPlugin.Models;
using WebMonitorPlugin.RequestModels;
using WebMonitorPlugin.ResponseModels;

namespace WebMonitorPlugin.Controllers
{
    [ApiController]
    [Authorize("PluginCore.Admin")]
    [Route($"plugins/{nameof(WebMonitorPlugin)}")]
    public class HomeController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string pluginWwwroot = PluginPathProvider.PluginWwwRootDir(nameof(WebMonitorPlugin));
            string pluginTemplates = System.IO.Path.Combine(pluginWwwroot, "templates");
            string layoutStr = System.IO.File.ReadAllText(System.IO.Path.Combine(pluginTemplates, "layoutMain.html"), System.Text.Encoding.UTF8);
            string targetStr = System.IO.File.ReadAllText(System.IO.Path.Combine(pluginTemplates, "index.html"), System.Text.Encoding.UTF8);
            string rtnStr = layoutStr.Replace("{{Content}}", targetStr);


            StringBuilder sb = new StringBuilder();
            var taskList = TaskManager.Tasks();
            foreach (var task in taskList)
            {
                // http://www.jsons.cn/htmltojsp/
                sb.AppendFormat($"<tr data-tr-id=\"{task.Name}\">");
                sb.AppendFormat($"	<td>{task.Name}</td>");
                sb.AppendFormat("	<td>");
                sb.AppendFormat($"		<span class=\"label label-info\">{task.WindowWidth}</span>");
                sb.AppendFormat("	</td>");
                sb.AppendFormat("	<td>");
                sb.AppendFormat($"		<span class=\"label label-info\">{task.WindowHeight}</span>");
                sb.AppendFormat("	</td>");
                sb.AppendFormat("	<td>");
                sb.AppendFormat($"		<span class=\"label label-info\">{task.ForceWait}</span>");
                sb.AppendFormat("	</td>");
                sb.AppendFormat("	<td>");
                sb.AppendFormat($"		<span class=\"label label-info\">{(task.Enable ? "true" : "false")}</span>");
                sb.AppendFormat("	</td>");
                sb.AppendFormat("	<td>");
                sb.AppendFormat("		<div class=\"btn-group\" role=\"group\" data-pjax>");
                sb.AppendFormat($"			<a class=\"btn btn-warning\" href=\"/plugins/{nameof(WebMonitorPlugin)}/Edit?id={task.Name}\">编辑</a>");
                sb.AppendFormat($"			<button class=\"btn btn-danger\" type=\"button\" onclick=\"deleteOp('{task.Name}')\">删除</button>");
                sb.AppendFormat("		</div>");
                sb.AppendFormat("	</td>");
                sb.AppendFormat("</tr>");
                sb.AppendFormat("");
            }
            rtnStr = rtnStr.Replace("{{Table.Trs}}", sb.ToString());

            return await Task.FromResult(Content(rtnStr, "text/html; charset=utf-8"));
        }

        [Route(nameof(Create))]
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            string pluginWwwroot = PluginPathProvider.PluginWwwRootDir(nameof(WebMonitorPlugin));
            string pluginTemplates = System.IO.Path.Combine(pluginWwwroot, "templates");
            string layoutStr = System.IO.File.ReadAllText(System.IO.Path.Combine(pluginTemplates, "layoutMain.html"), System.Text.Encoding.UTF8);
            string targetStr = System.IO.File.ReadAllText(System.IO.Path.Combine(pluginTemplates, "create.html"), System.Text.Encoding.UTF8);
            string rtnStr = layoutStr.Replace("{{Content}}", targetStr);

            return await Task.FromResult(Content(rtnStr, "text/html; charset=utf-8"));
        }

        [Route(nameof(Create))]
        [HttpPost]
        public async Task<BaseResponseModel> Create([FromBody] TaskCreateRequestModel requestModel)
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {
                TaskManager.AddTask(new Models.TaskModel
                {
                    Name = requestModel.Name,
                    Url = requestModel.Url,
                    Message = requestModel.Message,
                    ForceWait = requestModel.ForceWait,
                    WindowWidth = requestModel.WindowWidth,
                    WindowHeight = requestModel.WindowHeight,
                    JsCondition = requestModel.JsCondition,
                    ForceWaitAfterJsConditionExecute = requestModel.ForceWaitAfterJsConditionExecute,
                    Cookies = ConvertToCookieList(requestModel.Cookies),
                    Enable = requestModel.Enable
                });

                responseModel.Code = 1;
                responseModel.Message = "成功";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                responseModel.Code = -1;
                responseModel.Message = "失败";
            }

            return await Task.FromResult(responseModel);
        }

        [Route(nameof(Edit))]
        [HttpGet]
        public async Task<ActionResult> Edit([FromQuery] string id)
        {
            string pluginWwwroot = PluginPathProvider.PluginWwwRootDir(nameof(WebMonitorPlugin));
            string pluginTemplates = System.IO.Path.Combine(pluginWwwroot, "templates");
            string layoutStr = System.IO.File.ReadAllText(System.IO.Path.Combine(pluginTemplates, "layoutMain.html"), System.Text.Encoding.UTF8);
            string targetStr = System.IO.File.ReadAllText(System.IO.Path.Combine(pluginTemplates, "edit.html"), System.Text.Encoding.UTF8);
            string rtnStr = layoutStr.Replace("{{Content}}", targetStr);
            TaskModel task = TaskManager.Task(id);
            rtnStr = rtnStr.Replace("{{Name}}", task.Name)
                            .Replace("{{Url}}", task.Url)
                            .Replace("{{Message}}", task.Message)
                            .Replace("{{ForceWait}}", task.ForceWait.ToString())
                            .Replace("{{WindowWidth}}", task.WindowWidth.ToString())
                            .Replace("{{WindowHeight}}", task.WindowHeight.ToString())
                            .Replace("{{ForceWaitAfterJsConditionExecute}}", task.ForceWaitAfterJsConditionExecute.ToString())
                            .Replace("{{Cookies}}", ConvertToCookiesStr(task.Cookies))
                            .Replace("{{Enable}}", task.Enable ? "true" : "false");

            return await Task.FromResult(Content(rtnStr, "text/html; charset=utf-8"));
        }

        [Route(nameof(JsCondition))]
        [HttpGet]
        public async Task<ActionResult> JsCondition([FromQuery] string id)
        {
            TaskModel task = TaskManager.Task(id);
            string jsCondition = task.JsCondition;

            return await Task.FromResult(Content(jsCondition, "text/javascript"));
        }



        [Route(nameof(Edit))]
        [HttpPost]
        public async Task<BaseResponseModel> Edit([FromBody] TaskEditRequestModel requestModel)
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {
                TaskManager.AddTask(new Models.TaskModel
                {
                    Name = requestModel.Name,
                    Url = requestModel.Url,
                    Message = requestModel.Message,
                    ForceWait = requestModel.ForceWait,
                    WindowWidth = requestModel.WindowWidth,
                    WindowHeight = requestModel.WindowHeight,
                    JsCondition = requestModel.JsCondition,
                    ForceWaitAfterJsConditionExecute = requestModel.ForceWaitAfterJsConditionExecute,
                    Cookies = ConvertToCookieList(requestModel.Cookies),
                    Enable = requestModel.Enable
                });

                responseModel.Code = 1;
                responseModel.Message = "成功";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                responseModel.Code = -1;
                responseModel.Message = "失败";
            }

            return await Task.FromResult(responseModel);
        }

        [Route(nameof(Delete))]
        [HttpPost]
        public async Task<BaseResponseModel> Delete([FromForm] string id)
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {
                TaskManager.RemoveTask(id);

                responseModel.Code = 1;
                responseModel.Message = "成功";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                responseModel.Code = -1;
                responseModel.Message = "失败";
            }

            return await Task.FromResult(responseModel);
        }


        private List<TaskModel.CookieModel> ConvertToCookieList(string cookiesStr)
        {
            string[] cookieStrArray = cookiesStr.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var cookieList = new List<TaskModel.CookieModel>();
            foreach (var cookieStr in cookieStrArray)
            {
                // cookieStr: name=xxx;value=xxx;domain=.bilibili.com;path=/
                string[] cookiePairs = cookieStr.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var cookieModel = new TaskModel.CookieModel();
                foreach (var cookiePair in cookiePairs)
                {
                    // cookiePair: name=xxx
                    string[] keyValue = cookiePair.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    switch (keyValue[0])
                    {
                        case "name":
                            cookieModel.Name = keyValue[1];
                            break;
                        case "value":
                            cookieModel.Value = keyValue[1];
                            break;
                        case "domain":
                            cookieModel.Domain = keyValue[1];
                            break;
                        case "path":
                            cookieModel.Path = keyValue[1];
                            break;
                        default:
                            break;
                    }
                }
                cookieList.Add(cookieModel);
            }

            return cookieList;
        }


        private string ConvertToCookiesStr(List<TaskModel.CookieModel> cookieModels)
        {
            StringBuilder cookiesStrSb = new StringBuilder();
            foreach (var cookie in cookieModels)
            {
                cookiesStrSb.AppendLine($"name={cookie.Name};value={cookie.Value};domain={cookie.Domain};path={cookie.Path}");
            }

            return cookiesStrSb.ToString();
        }

    }
}

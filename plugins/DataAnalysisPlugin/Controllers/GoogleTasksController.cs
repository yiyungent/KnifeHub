// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAnalysisPlugin.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Tasks.v1.Data;
using Google.Apis.Tasks.v1;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task = Google.Apis.Tasks.v1.Data.Task;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Web;
using Octokit;
using System.Security.Policy;
using System.Net;

namespace DataAnalysisPlugin.Controllers
{
    /// <summary>
    /// 记账鸭
    /// </summary>
    [ApiController]
    [Route($"api/Plugins/{(nameof(DataAnalysisPlugin))}/[controller]/[action]")]
    [Authorize("PluginCore.Admin")]
    public class GoogleTasksController : ControllerBase
    {
        #region Actions
        [HttpPost]
        public async Task<BaseResponseModel> Test()
        {
            BaseResponseModel responseModel = new BaseResponseModel();

            try
            {
                #region 测试
                UserCredential credential;
                // Copy & Paste from Google Docs
                //using (var stream =
                //    new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                //{
                //    string credPath = System.Environment.GetFolderPath(
                //        System.Environment.SpecialFolder.Personal);
                //    credPath = Path.Combine(credPath, ".credentials/tasks-dotnet-quickstart.json");

                //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                //        GoogleClientSecrets.Load(stream).Secrets,
                //        Scopes,
                //        "user",
                //        CancellationToken.None,
                //        new FileDataStore(credPath, true)).Result;
                //    Console.WriteLine("Credential file saved to: " + credPath);
                //}

                // Create Google Tasks API service.
                var service = new TasksService(new BaseClientService.Initializer()
                {
                    //HttpClientInitializer = credential,
                    //ApplicationName = ApplicationName,
                });

                // Define parameters of request.
                TasklistsResource.ListRequest listRequest = service.Tasklists.List();
                // Fetch all task lists
                IList<TaskList> taskList = listRequest.Execute().Items;

                Task task = new Task { Title = "New Task" };
                task.Notes = "Please complete me";
                task.Due = "2010-10-15T12:00:00.000Z";
                task.Title = "Test";

                // careful no verification that taskList[0] exists
                //var response = service.Tasks.Insert(task, taskLists[0].Id).Execute();
                //Console.WriteLine(response.Title);
                #endregion

                responseModel.Code = 1;
                responseModel.Message = "success";
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "failure";
                responseModel.Data = ex.ToString();
            }

            return responseModel;
        }

        #region 登录
        [HttpGet]
        public IActionResult Authorize()
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "YOUR_CLIENT_ID",
                    ClientSecret = "YOUR_CLIENT_SECRET"
                },
                Scopes = new[] {
                    "openid", "email", "profile",
                    TasksService.Scope.TasksReadonly,
                }, // 设置授权范围
                DataStore = new MemoryDataStore(). // 若要在内存中存储令牌，使用MemoryDataStore
            });

            var redirectUri = Url.Action("AuthorizeCallback", "GoogleAuth", null, Request.Scheme);

            var authorizationUrl = flow.CreateAuthorizationCodeRequest(redirectUri).Build();

            return Redirect(authorizationUrl.AbsoluteUri);
        }

        [HttpGet]
        public async Task<IActionResult> AuthorizeCallback(string code)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "YOUR_CLIENT_ID",
                    ClientSecret = "YOUR_CLIENT_SECRET"
                },
                Scopes = new[] { "openid", "email", "profile" }, // 设置授权范围
                DataStore = new MemoryDataStore() // 若要在内存中存储令牌，使用MemoryDataStore
            });

            var tokenResponse = await flow.ExchangeCodeForTokenAsync("user", code, Url.Action("AuthorizeCallback", "GoogleAuth", null, Request.Scheme), CancellationToken.None);

            // 使用tokenResponse访问Google API
            UserCredential credential = new UserCredential(flow, "userId", tokenResponse);
            var service = new TasksService(new BaseClientService.Initializer
            {
                ApplicationName = "Tasks API Sample",
                HttpClientInitializer = credential,
            });


            return Content("授权完成");
        }
        #endregion

        #endregion
    }
}

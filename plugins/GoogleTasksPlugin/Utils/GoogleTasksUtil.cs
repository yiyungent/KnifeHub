using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Util.Store;
using PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleTasksPlugin.Utils
{
    //public class CustomDataStore : IDataStore
    //{
    //    public Task ClearAsync()
    //    {
    //        return Task.CompletedTask;
    //    }

    //    public Task DeleteAsync<T>(string key)
    //    {
    //        return Task.CompletedTask;
    //    }

    //    public Task<T> GetAsync<T>(string key)
    //    {
    //        return Task.CompletedTask;
    //    }

    //    public Task StoreAsync<T>(string key, T value)
    //    {
    //        return Task.CompletedTask;
    //    }
    //}

    public class GoogleTasksUtil
    {
        public static GoogleTasksModel Tasks(string appName, string oAuthClientId, string oAuthClientSecret, string tokenJsonStr)
        {
            var rtnModel = new GoogleTasksModel();
            rtnModel.Items = new List<GoogleTasksModel.TaskListItemModel>();

            var scopes = new string[] { "openid", "email", "profile", TasksService.Scope.TasksReadonly, };
            string fileDataStoreDirPath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(GoogleTasksPlugin), "FileDataStore");
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = oAuthClientId,
                    ClientSecret = oAuthClientSecret,
                },
                Scopes = scopes, // 设置授权范围
                //DataStore = new CustomDataStore(),
                DataStore = new FileDataStore(fileDataStoreDirPath),
            });
            TokenResponse tokenResponseJsonModel = JsonUtil.JsonStr2Obj<TokenResponse>(tokenJsonStr);
            var tasksService = new TasksService(new BaseClientService.Initializer
            {
                ApplicationName = appName,
                //ApiKey = apiKey,
                HttpClientInitializer = new UserCredential(flow: flow, userId: "userId", token: tokenResponseJsonModel),
            });

            Dictionary<string, object> dataModel = new Dictionary<string, object>();
            var taskListResult = tasksService.Tasklists.List().Execute();
            if (taskListResult.Items != null)
            {
                foreach (Google.Apis.Tasks.v1.Data.TaskList taskList in taskListResult.Items)
                {
                    string taskListKey = $"{taskList.Title} - {taskList.Id}";
                    var taskListItem = new GoogleTasksModel.TaskListItemModel
                    {
                        Id = taskList.Id,
                        Title = taskList.Title,
                        Tasks = new List<GoogleTasksModel.TaskListItemModel.TaskItemModel>(),
                    };
                    var tasksResult = tasksService.Tasks.List(taskList.Id).Execute();
                    var tasks = tasksResult.Items;
                    foreach (var task in tasks)
                    {
                        taskListItem.Tasks.Add(new GoogleTasksModel.TaskListItemModel.TaskItemModel
                        {
                            Id = task.Id,
                            Title = task.Title,
                            ParentId = task.Parent,
                            Updated = DateTime.Parse(task.Updated),
                            Status = task.Status,
                            Notes = task.Notes,
                            Position = task.Position,
                        });
                    }
                    foreach (var item in taskListItem.Tasks)
                    {
                        var parentTemp = taskListItem.Tasks.FirstOrDefault(m => m.Id == item.ParentId);
                        if (parentTemp != null)
                        {
                            item.Parent = parentTemp;
                        }
                    }

                    rtnModel.Items.Add(taskListItem);
                }
            }

            return rtnModel;
        }

        public class GoogleTasksModel
        {
            public List<TaskListItemModel> Items { get; set; }

            public class TaskListItemModel
            {
                public string Id;

                public string Title { get; set; }

                public List<TaskItemModel> Tasks { get; set; }

                public class TaskItemModel
                {
                    public string Id;

                    public string Status;

                    public string Position { get; set; }

                    public string Title { get; set; }

                    public string Notes { get; set; }

                    public DateTime Updated { get; set; }

                    public string ParentId { get; set; }

                    public TaskItemModel Parent { get; set; }
                }
            }
        }
    }
}

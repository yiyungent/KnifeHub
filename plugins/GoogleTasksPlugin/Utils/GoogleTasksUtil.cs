using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleTasksPlugin.Utils
{
    public class GoogleTasksUtil
    {
        public static GoogleTasksModel Tasks(string appName, string apiKey)
        {
            var rtnModel = new GoogleTasksModel();
            rtnModel.Items = new List<GoogleTasksModel.TaskListItemModel>();

            var tasksService = new TasksService(new BaseClientService.Initializer
            {
                ApplicationName = appName,
                ApiKey = apiKey,
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

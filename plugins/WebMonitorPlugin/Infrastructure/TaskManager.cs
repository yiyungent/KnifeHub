using PluginCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebMonitorPlugin.Models;

namespace WebMonitorPlugin.Infrastructure
{
    public class TaskManager
    {
        public static List<TaskModel> Tasks()
        {
            List<TaskModel> tasks = new List<TaskModel>();
            // 读取文件
            string pluginDir = System.IO.Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(WebMonitorPlugin));
            string tasksDir = System.IO.Path.Combine(pluginDir, "Tasks");
            if (System.IO.Directory.Exists(tasksDir))
            {
                var taskDirArray = System.IO.Directory.GetDirectories(tasksDir);
                if (taskDirArray != null && taskDirArray.Length > 0)
                {
                    foreach (string taskDir in taskDirArray)
                    {
                        string taskFilePath = System.IO.Path.Combine(taskDir, "task.json");
                        string taskJsonStr = System.IO.File.ReadAllText(taskFilePath, Encoding.UTF8);
                        TaskModel task = Utils.JsonUtil.JsonStr2Obj<TaskModel>(taskJsonStr);
                        // 注意: 没有 JsCondition
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }

        public static TaskModel Task(string taskName)
        {
            string pluginDir = System.IO.Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(WebMonitorPlugin));
            string tasksDir = System.IO.Path.Combine(pluginDir, "Tasks");
            string taskDir = System.IO.Path.Combine(tasksDir, taskName);
            string taskFilePath = System.IO.Path.Combine(taskDir, "task.json");
            string taskJsonStr = System.IO.File.ReadAllText(taskFilePath, Encoding.UTF8);
            TaskModel task = Utils.JsonUtil.JsonStr2Obj<TaskModel>(taskJsonStr);
            string jsConditionFilePath = System.IO.Path.Combine(taskDir, "jsCondition.js");
            task.JsCondition = System.IO.File.ReadAllText(jsConditionFilePath, Encoding.UTF8);

            return task;
        }



        public static bool ExistTaskName(string taskName)
        {
            string pluginDir = System.IO.Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(WebMonitorPlugin));
            string tasksDir = System.IO.Path.Combine(pluginDir, "Tasks");

            return System.IO.Directory.Exists(System.IO.Path.Combine(tasksDir, taskName));
        }

        /// <summary>
        /// 存在 相同 Name , 则覆盖
        /// </summary>
        /// <param name="task"></param>
        public static void AddTask(TaskModel task)
        {
            string pluginDir = System.IO.Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(WebMonitorPlugin));
            string tasksDir = System.IO.Path.Combine(pluginDir, "Tasks");

            if (!System.IO.Directory.Exists(tasksDir))
            {
                System.IO.Directory.CreateDirectory(tasksDir);
            }
            string taskDir = System.IO.Path.Combine(tasksDir, task.Name);
            if (!System.IO.Directory.Exists(taskDir))
            {
                System.IO.Directory.CreateDirectory(taskDir);
            }

            // jsCondition.js
            string jsConditionFilePath = System.IO.Path.Combine(taskDir, "jsCondition.js");
            System.IO.File.WriteAllText(jsConditionFilePath, task.JsCondition, Encoding.UTF8);

            // task.json
            task.JsCondition = "";

            string taskJsonStr = Utils.JsonUtil.Obj2JsonStr(task);
            taskJsonStr = Utils.JsonUtil.ConvertJsonString(taskJsonStr);
            string taskFilePath = System.IO.Path.Combine(taskDir, "task.json");
            System.IO.File.WriteAllText(taskFilePath, taskJsonStr, Encoding.UTF8);


        }

        public static void RemoveTask(string taskName)
        {
            string pluginDir = System.IO.Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(WebMonitorPlugin));
            string tasksDir = System.IO.Path.Combine(pluginDir, "Tasks");
            string taskDir = System.IO.Path.Combine(tasksDir, taskName);
            if (System.IO.Directory.Exists(taskDir))
            {
                System.IO.Directory.Delete(taskDir, true);
            }
        }
    }
}

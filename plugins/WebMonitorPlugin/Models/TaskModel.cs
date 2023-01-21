using System;
using System.Collections.Generic;
using System.Text;

namespace WebMonitorPlugin.Models
{
    public class TaskModel
    {
        public string Name { get; set; }
        public string JsCondition { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        /// <summary>
        /// 强制等待 在 请求 Url 后
        /// </summary>
        public int ForceWait { get; set; }
        /// <summary>
        /// 强制等待 在 JsCondition 执行后
        /// </summary>
        public int ForceWaitAfterJsConditionExecute { get; set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public List<CookieModel> Cookies { get; set; }

        public List<StorageItemModel> Storage { get; set; }

        public bool Enable { get; set; }

        public class StorageItemModel
        {
            public string Key { get; set; }

            public string Value { get; set; }
        }

        public class CookieModel
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public string Domain { get; set; }

            public string Path { get; set; }

        }
    }
}

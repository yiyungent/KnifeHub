using System;
using System.Collections.Generic;
using System.Text;

namespace WebMonitorPlugin.RequestModels
{
    public class TaskEditRequestModel
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

        public string Cookies { get; set; }

        public bool Enable { get; set; }
    }
}

using QQStatPlugin.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using QQStatPlugin.ResponseModels.Calendar;
using System.Linq;
using QQStatPlugin;
using QQStatPlugin.Utils;
using PluginCore;
using ResponseModels.StackedArea;

namespace QQStatPlugin.Controllers
{
    [Route("api/Plugins/QQStatPlugin/{controller}/{action}")]
    public class StackedAreaController : ControllerBase
    {
        /// <summary>
        /// 效验权限
        /// </summary>
        public static DateTime CreateTime { get; set; }


        #region Actions

        [Route("/Plugins/QQStatPlugin/StackedArea")]
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] string memeberUin = "", [FromQuery] string groupUin = "")
        {
            if (CreateTime.AddHours(1) < DateTime.Now)
            {
                // 1小时后过期
                return NotFound();
            }

            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(QQStatPlugin)), "stacked-area.html");
            string htmlStr = System.IO.File.ReadAllText(path: indexFilePath, Encoding.UTF8);
            htmlStr = htmlStr.Replace("{{memeberUin}}", memeberUin).Replace("{{groupUin}}", groupUin);

            return await Task.FromResult(Content(htmlStr, "text/html"));
        }

        [HttpGet]
        public async Task<BaseResponseModel> EChartsOption([FromQuery] string memeberUin = "", [FromQuery] string groupUin = "")
        {
            Console.WriteLine($"{nameof(StackedAreaController)}.{nameof(EChartsOption)} {nameof(memeberUin)}:{memeberUin} {nameof(groupUin)}:{groupUin}");

            BaseResponseModel responseModel = new BaseResponseModel();

            // 1:00 + 1 = 2:00
            // 1:30 false
            // 2:00 < 1:30 false
            if (CreateTime.AddHours(1) < DateTime.Now)
            {
                // 1小时后过期
                return responseModel;
            }


            StackedAreaEChartsOptionResponseDataModel chartOption = new StackedAreaEChartsOptionResponseDataModel();

            try
            {
                var messageList = DbContext.QueryAllMessage();

                Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
                if (!string.IsNullOrEmpty(memeberUin) && uint.TryParse(memeberUin, out uint mUin))
                {
                    // 个人
                    messageList = messageList.Where(m => m.QQUin == memeberUin).ToList();
                }
                else if (!string.IsNullOrEmpty(groupUin) && uint.TryParse(groupUin, out uint gUin))
                {
                    // 某群
                    messageList = messageList.Where(m => m.GroupUin == groupUin).ToList();
                }
                else
                {
                    // 全部

                }
                // 只要前10 发言最多的人
                #region 只要 top10
                //Dictionary<string, int> tempDic = new Dictionary<string, int>();
                //foreach (var item in messageList)
                //{
                //    if (tempDic.ContainsKey(item.QQUin))
                //    {
                //        tempDic[item.QQUin] += 1;
                //    }
                //    else
                //    {
                //        tempDic.Add(item.QQUin, 1);
                //    }
                //}
                //List<string> top10QQUinList = new List<string>();
                //top10QQUinList = tempDic.OrderByDescending(m => m.Value).Take(10).Select(m => m.Key).ToList();
                //messageList = messageList.Where(m => top10QQUinList.Contains(m.QQUin)).ToList();
                // top10 发言字数最多
                var tempMesageList = messageList.GroupBy(m => m.QQUin).OrderByDescending(m => m.Sum(a => a.Content.Length)).Take(5).ToList();
                Console.WriteLine("tempMesageList.Count: " + tempMesageList.Count.ToString());
                List<string> top5QQUinList = tempMesageList.Select(m => m.Key).ToList();
                Console.WriteLine($"top5QQUinList: {string.Join(",", top5QQUinList)}");
                messageList = messageList.Where(m => top5QQUinList.Contains(m.QQUin)).ToList();
                #endregion


                #region EChartsOption 初始化
                chartOption.title = new StackedAreaEChartsOptionResponseDataModel.Title();
                chartOption.title.text = "堆叠区域图";
                chartOption.tooltip = new StackedAreaEChartsOptionResponseDataModel.Tooltip();
                chartOption.tooltip.trigger = "axis";
                chartOption.tooltip.axisPointer = new StackedAreaEChartsOptionResponseDataModel.Axispointer();
                chartOption.tooltip.axisPointer.type = "cross";
                chartOption.tooltip.axisPointer.label = new StackedAreaEChartsOptionResponseDataModel.Label();
                chartOption.tooltip.axisPointer.label.backgroundColor = "#6a7985";
                chartOption.legend = new StackedAreaEChartsOptionResponseDataModel.Legend();
                chartOption.toolbox = new StackedAreaEChartsOptionResponseDataModel.Toolbox();
                chartOption.toolbox.feature = new StackedAreaEChartsOptionResponseDataModel.Feature();
                chartOption.toolbox.feature.saveAsImage = new StackedAreaEChartsOptionResponseDataModel.Saveasimage();
                chartOption.grid = new StackedAreaEChartsOptionResponseDataModel.Grid();
                chartOption.grid.left = "3%";
                chartOption.grid.right = "4%";
                chartOption.grid.bottom = "3%";
                chartOption.grid.containLabel = true;
                chartOption.xAxis = new List<StackedAreaEChartsOptionResponseDataModel.Xaxi>();
                #endregion

                // 应该说这里最多10个
                chartOption.legend.data = messageList.Select(m => m.QQName).Distinct().ToList();

                IList<string> recordDays = messageList.OrderBy(m => m.Id).Select(m => m.CreateTime.ToDateTime13().ToString("yyyy-MM-dd HH"))
                    .Distinct().ToList();
                chartOption.xAxis.Add(new StackedAreaEChartsOptionResponseDataModel.Xaxi()
                {
                    type = "category",
                    boundaryGap = false,
                    data = recordDays
                });
                chartOption.yAxis = new List<StackedAreaEChartsOptionResponseDataModel.Yaxi>();
                chartOption.yAxis.Add(new StackedAreaEChartsOptionResponseDataModel.Yaxi()
                {
                    type = "value"
                });
                chartOption.series = new List<StackedAreaEChartsOptionResponseDataModel.Series>();
                // types: 时间
                IList<string> types = chartOption.legend.data;
                foreach (var type in types)
                {
                    var item = new StackedAreaEChartsOptionResponseDataModel.Series();
                    item.name = type;
                    item.type = "line";
                    item.stack = "字数";
                    item.areaStyle = new StackedAreaEChartsOptionResponseDataModel.Areastyle();
                    item.emphasis = new StackedAreaEChartsOptionResponseDataModel.Emphasis();
                    item.emphasis.focus = "series";
                    item.data = new List<double>();
                    foreach (string day in recordDays)
                    {
                        // TODO: 注意: 这里用 QQName 区分
                        long symbolCount = messageList
                            .Where(m => m.QQName == type && m.CreateTime.ToDateTime13().ToString("yyyy-MM-dd HH") == day)
                            ?.Select(m => m.Content.Length)?.Sum() ?? 0;
                        item.data.Add(symbolCount);
                    }

                    chartOption.series.Add(item);
                }


                responseModel.Code = 1;
                responseModel.Message = "成功";
                responseModel.Data = chartOption;
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "失败";
                responseModel.Data = chartOption;
            }

            return await Task.FromResult(responseModel);
        }

        #endregion
    }
}

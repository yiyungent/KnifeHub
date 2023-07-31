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
using Konata.Core.Message.Model;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace QQStatPlugin.Controllers
{
    [Route("api/Plugins/QQStatPlugin/{controller}/{action}")]
    public class StackedAreaController : ControllerBase
    {
        public static TempDataModel TempData { get; set; } = new TempDataModel();

        public class TempDataModel
        {
            /// <summary>
            /// 效验权限
            /// </summary>
            public DateTime CreateTime { get; set; }

            public string GroupUin { get; set; }

            public string MemeberUin { get; set; }
        }


        #region Actions

        [Route("/Plugins/QQStatPlugin/StackedArea")]
        [HttpGet]
        public async Task<ActionResult> Get(/*[FromQuery] string groupUin = "", [FromQuery] string memeberUin = ""*/)
        {
            string memeberUin = TempData.MemeberUin;
            string groupUin = TempData.GroupUin;
            Console.WriteLine($"/Plugins/QQStatPlugin/StackedArea {TempData.GroupUin}-{TempData.MemeberUin}: {TempData.CreateTime}");
            if (TempData.CreateTime.AddHours(1) < DateTime.Now)
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
        public async Task<BaseResponseModel> EChartsOption(/*[FromQuery] string memeberUin = "", [FromQuery] string groupUin = ""*/)
        {
            string memeberUin = TempData.MemeberUin;
            string groupUin = TempData.GroupUin;
            Console.WriteLine($"{nameof(StackedAreaController)}.{nameof(EChartsOption)} {nameof(memeberUin)}:{memeberUin} {nameof(groupUin)}:{groupUin}");

            BaseResponseModel responseModel = new BaseResponseModel();

            // 1:00 + 1 = 2:00
            // 1:30 false
            // 2:00 < 1:30 false
            if (TempData.CreateTime.AddHours(1) < DateTime.Now)
            {
                // 1小时后过期
                return responseModel;
            }


            StackedAreaEChartsOptionResponseDataModel chartOption = new StackedAreaEChartsOptionResponseDataModel();

            try
            {
                var messageList = DbContext.QueryAllMessage();

                #region 过滤
                if (!string.IsNullOrEmpty(groupUin) && uint.TryParse(groupUin, out uint gUin))
                {
                    // 某群
                    messageList = messageList.Where(m => m.GroupUin == groupUin).ToList();
                    if (!string.IsNullOrEmpty(memeberUin) && uint.TryParse(memeberUin, out uint mUin))
                    {
                        // 某人
                        messageList = messageList.Where(m => m.QQUin == memeberUin).ToList();
                    }
                }
                #endregion


                Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();


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
                var tempMesageList = messageList.GroupBy(m => m.QQUin).OrderByDescending(m => m.Sum(a => ExtractHanzi2(a.Content).Length)).Take(5).ToList();
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
                var tempQQNameAndUin = messageList.Select(m => (uin: m.QQUin, name: m.QQName)).Distinct(new QQCompare()).ToList();
                chartOption.legend.data = tempQQNameAndUin.Select(m => $"{m.name}").ToList();

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
                // types: 昵称
                IList<string> names = chartOption.legend.data;
                int i = 0;
                foreach (var name in names)
                {
                    var item = new StackedAreaEChartsOptionResponseDataModel.Series();
                    item.name = name;
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
                            .Where(m =>
                                m.QQUin == tempQQNameAndUin[i].uin
                                && m.QQName == name
                                && m.CreateTime.ToDateTime13().ToString("yyyy-MM-dd HH") == day
                            )
                            ?.Select(m => ExtractHanzi2(m.Content).Length)?.Sum() ?? 0;
                        item.data.Add(symbolCount);
                    }
                    i++;

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
                Console.WriteLine("堆叠图失败: ");
                Console.WriteLine(ex.ToString());
                responseModel.Data = chartOption;
            }

            return await Task.FromResult(responseModel);
        }

        #endregion


        //private static string ConvertTime(this long time)
        //{
        //    string rtnStr = "";
        //    DateTime dateTime = time.ToDateTime13();
        //    rtnStr = dateTime.ToString("yyyy-MM-dd HH");

        //    return rtnStr;
        //}


        public class QQCompare : IEqualityComparer<(string uin, string name)>
        {
            public bool Equals((string uin, string name) x, (string uin, string name) y)
            {
                return x.uin == y.uin;
            }

            public int GetHashCode([DisallowNull] (string uin, string name) obj)
            {
                return obj.GetHashCode();
            }
        }

        /// <summary>
        /// 提取字符串中的汉字
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static string ExtractHanzi2(string st)
        {
            if (string.IsNullOrEmpty(st))
            {
                return "";
            }
            string hanziString = "";
            foreach (var ch in st)
            {
                // if ((uint)ch - 0x4e00) <= 0x9fbb - 0x4e00)
                if (Regex.IsMatch(ch.ToString(), @"[\u4e00-\u9fbb]+"))
                {
                    hanziString += ch.ToString();
                }
            }
            return hanziString;
        }

    }
}

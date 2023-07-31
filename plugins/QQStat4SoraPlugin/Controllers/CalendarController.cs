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
using System.Text.RegularExpressions;

namespace QQStatPlugin.Controllers
{
    [Route("api/Plugins/QQStatPlugin/{controller}/{action}")]
    public class CalendarController : ControllerBase
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

        [Route("/Plugins/QQStatPlugin/Calendar")]
        [HttpGet]
        public async Task<ActionResult> Get(/*[FromQuery] string groupUin = "", [FromQuery] string memeberUin = ""*/)
        {
            string memeberUin = TempData.MemeberUin;
            string groupUin = TempData.GroupUin;
            if (TempData.CreateTime.AddHours(1) < DateTime.Now)
            {
                // 1小时后过期
                return NotFound();
            }

            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(QQStatPlugin)), "calendar.html");
            string htmlStr = System.IO.File.ReadAllText(path: indexFilePath, Encoding.UTF8);
            htmlStr = htmlStr.Replace("{{memeberUin}}", memeberUin).Replace("{{groupUin}}", groupUin);

            return await Task.FromResult(Content(htmlStr, "text/html"));
        }

        [HttpGet]
        public async Task<BaseResponseModel> EChartsOption(/*[FromQuery] string memeberUin = "", [FromQuery] string groupUin = ""*/)
        {
            string memeberUin = TempData.MemeberUin;
            string groupUin = TempData.GroupUin;
            Console.WriteLine($"{nameof(CalendarController)}.{nameof(EChartsOption)} {nameof(memeberUin)}:{memeberUin} {nameof(groupUin)}:{groupUin}");

            BaseResponseModel responseModel = new BaseResponseModel();

            // 1:00 + 1 = 2:00
            // 1:30 false
            // 2:00 < 1:30 false
            if (TempData.CreateTime.AddHours(1) < DateTime.Now)
            {
                // 1小时后过期
                return responseModel;
            }

            CalendarEChartsOptionResponseDataModel dataModel = new CalendarEChartsOptionResponseDataModel();

            #region CalendarEChartsOption 初始化
            dataModel.tooltip = new CalendarEChartsOptionResponseDataModel.TooltipModel()
            {
                position = "top"
            };
            dataModel.visualMap = new CalendarEChartsOptionResponseDataModel.VisualmapModel()
            {
                min = 0,
                max = 1000,
                calculable = true,
                orient = "horizontal",
                left = "center",
                top = "top",
                inRange = new CalendarEChartsOptionResponseDataModel.VisualmapModel.inRangeModel()
                {
                    color = new List<string>() { "#f0f0f0", "#dcf064", "#d2e650", "#bed228", "#5ab40a" }, // 格子的颜色
                    colorAlpha = 0.9
                }
            };
            dataModel.calendar = new List<CalendarEChartsOptionResponseDataModel.CalendarModel>();

            #endregion

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

                foreach (var item in messageList)
                {
                    if (keyValuePairs.ContainsKey(item.CreateTime.ToDateTime13().ToString("yyyy-MM-dd")))
                    {
                        //keyValuePairs[item.CreateTime.ToDateTime13().ToString("yyyy-MM-dd")] += item.Content?.Length ?? 0;
                        keyValuePairs[item.CreateTime.ToDateTime13().ToString("yyyy-MM-dd")] += ExtractHanzi2(item.Content).Length;

                    }
                    else
                    {
                        //keyValuePairs.Add(item.CreateTime.ToDateTime13().ToString("yyyy-MM-dd"), item.Content?.Length ?? 0);
                        keyValuePairs.Add(item.CreateTime.ToDateTime13().ToString("yyyy-MM-dd"), ExtractHanzi2(item.Content).Length);
                    }
                }


                // 排序 转换
                List<List<string>> calendarList = keyValuePairs
                    .Select(keyValue => (key: DateTime.Parse(keyValue.Key), value: keyValue.Value)
                     ).OrderBy(m => m.key)
                     .Select(m => new List<string>() { m.key.ToString("yyyy-MM-dd"), m.value.ToString() }).ToList();

                dataModel.visualMap.max = calendarList.Select(m => Convert.ToInt32(m[1])).Max();

                // 不同年份划分开
                List<List<List<string>>> list = new List<List<List<string>>>();
                int calendarIndex = -1; // 注意: 从 -1 开始
                List<int> yearList = new List<int>();
                dataModel.series = new List<CalendarEChartsOptionResponseDataModel.SeriesModel>();
                foreach (var item in calendarList)
                {
                    //int currentYear = DateTime.Parse(item[0]).Year;
                    int currentYear = Convert.ToInt32(item[0].Substring(0, 4));
                    if (!yearList.Contains(currentYear))
                    {
                        calendarIndex++;
                        var serie = new CalendarEChartsOptionResponseDataModel.SeriesModel();
                        serie.type = "heatmap";
                        serie.coordinateSystem = "calendar";
                        serie.calendarIndex = calendarIndex;
                        serie.data = new List<List<string>>();

                        serie.data.Add(item);

                        yearList.Add(currentYear);
                        dataModel.series.Add(serie);
                    }
                    else
                    {
                        dataModel.series[calendarIndex].data.Add(item);
                    }

                }

                int index = 0;
                foreach (var year in yearList)
                {
                    dataModel.calendar.Add(new CalendarEChartsOptionResponseDataModel.CalendarModel
                    {
                        range = year.ToString(),
                        cellSize = new List<string> { "auto", "20" },
                        top = 80 + 180 * index
                    });
                    index++;
                }

                responseModel.Code = 1;
                responseModel.Message = "成功";
                responseModel.Data = dataModel;
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "失败";
                responseModel.Data = dataModel;
            }

            return await Task.FromResult(responseModel);
        }

        #endregion

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
                if (Regex.IsMatch(ch.ToString(), @"[\u4e00-\u9fbb]+"))
                {
                    hanziString += ch.ToString();
                }
            }
            return hanziString;
        }
    }
}

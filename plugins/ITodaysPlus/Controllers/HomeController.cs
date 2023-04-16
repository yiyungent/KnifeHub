using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using ITodaysPlus.Models;
using ITodaysPlus.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PluginCore;

namespace ITodaysPlus.Controllers
{
    /// <summary>
    /// 其实也可以不写这个, 直接访问 Plugins/ITodaysPlus/index.html
    /// 
    /// 下面的方法, 是去掉 index.html
    /// 
    /// 若 wwwroot 下有其它需要访问的文件, 如何 css, js, 而你又不想每次新增 action 指定返回, 则 Route 必须 Plugins/{PluginId},
    /// 这样访问 Plugins/ITodaysPlus/css/main.css 就会访问到你插件下的 wwwroot/css/main.css
    /// </summary>
    [Route($"Plugins/{(nameof(ITodaysPlus))}")]
    public class HomeController : Controller
    {
        #region Actions
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(ITodaysPlus)), "index.html");

            return PhysicalFile(indexFilePath, "text/html");
        }

        [HttpPost, Route(nameof(ITodaysOne))]
        public async Task<ActionResult<BaseResponseModel>> ITodaysOne([FromForm] IFormFile file)
        {
            BaseResponseModel responseModel = null;

            #region 效验
            if (file == null)
            {
                responseModel.Code = -1;
                responseModel.Message = "上传的文件不能为空";
                return responseModel;
            }
            // 文件后缀
            string fileExtension = Path.GetExtension(file.FileName);// 获取文件格式，拓展名
            if (fileExtension != ".xls")
            {
                responseModel.Code = -1;
                responseModel.Message = "只能上传 xls 格式文件";
                return responseModel;
            }
            // 判断文件大小
            var fileSize = file.Length;
            if (fileSize > 1024 * 1024 * 5) // 5M
            {
                responseModel.Code = -1;
                responseModel.Message = "上传的文件不能大于5MB";
                return responseModel;
            }
            #endregion

            try
            {
                // 返回数据转换后的 json url
                string viewModel = string.Empty;

                // 1. 在 Stream 中处理 xls
                #region 在 Stream 中处理 xls
                Stream stream = file.OpenReadStream();
                DataTable dt = NPOIUtil.ImportExcelForITodays(stream, fileExtension);
                int totalCount = dt.Rows.Count, importSuccessCount = 0;
                IList<ITodaysModel> listModel = new List<ITodaysModel>();
                foreach (DataRow row in dt.Rows)
                {
                    long num = Convert.ToInt64(row["序号"]?.ToString());
                    string target = row["目标"]?.ToString();
                    string type = row["类型"]?.ToString();
                    DateTime startTime = Convert.ToDateTime(row["开始时间"]?.ToString());
                    DateTime endTime = Convert.ToDateTime(row["结束时间"]?.ToString());
                    long spendSecond = Convert.ToInt64(row["共花费(秒)"]?.ToString());
                    string remark = row["备注"]?.ToString();
                    var model = new ITodaysModel()
                    {
                        Num = num,
                        Target = target,
                        Type = type,
                        StartTime = startTime,
                        EndTime = endTime,
                        SpendSecond = spendSecond,
                        Remark = remark
                    };

                    listModel.Add(model);
                }
                #endregion

                // 2. 转换为json
                #region 转换为json
                ITodaysOneChartOption chartOption = new ITodaysOneChartOption();
                chartOption.title = new ITodaysOneChartOption.Title();
                chartOption.title.text = "堆叠区域图";
                chartOption.tooltip = new ITodaysOneChartOption.Tooltip();
                chartOption.tooltip.trigger = "axis";
                chartOption.tooltip.axisPointer = new ITodaysOneChartOption.Axispointer();
                chartOption.tooltip.axisPointer.type = "cross";
                chartOption.tooltip.axisPointer.label = new ITodaysOneChartOption.Label();
                chartOption.tooltip.axisPointer.label.backgroundColor = "#6a7985";
                chartOption.legend = new ITodaysOneChartOption.Legend();
                chartOption.legend.data = listModel.Select(m => m.Type).Distinct().ToList();
                chartOption.toolbox = new ITodaysOneChartOption.Toolbox();
                chartOption.toolbox.feature = new ITodaysOneChartOption.Feature();
                chartOption.toolbox.feature.saveAsImage = new ITodaysOneChartOption.Saveasimage();
                chartOption.grid = new ITodaysOneChartOption.Grid();
                chartOption.grid.left = "3%";
                chartOption.grid.right = "4%";
                chartOption.grid.bottom = "3%";
                chartOption.grid.containLabel = true;
                chartOption.xAxis = new List<ITodaysOneChartOption.Xaxi>();
                IList<string> recordDays = listModel.OrderBy(m => m.Num).Select(m => m.StartTime.ToString("yyyy-MM-dd"))
                    .Distinct().ToList();
                chartOption.xAxis.Add(new ITodaysOneChartOption.Xaxi()
                {
                    type = "category",
                    boundaryGap = false,
                    data = recordDays
                });
                chartOption.yAxis = new List<ITodaysOneChartOption.Yaxi>();
                chartOption.yAxis.Add(new ITodaysOneChartOption.Yaxi()
                {
                    type = "value"
                });
                chartOption.series = new List<ITodaysOneChartOption.Series>();
                IList<string> types = chartOption.legend.data;
                foreach (var type in types)
                {
                    var item = new ITodaysOneChartOption.Series();
                    item.name = type;
                    item.type = "line";
                    item.stack = "共花费(秒)";
                    item.areaStyle = new ITodaysOneChartOption.Areastyle();
                    item.emphasis = new ITodaysOneChartOption.Emphasis();
                    item.emphasis.focus = "series";
                    item.data = new List<double>();
                    foreach (string day in recordDays)
                    {
                        long daySecond = listModel.Where(m => m.Type == type && m.StartTime.ToString("yyyy-MM-dd") == day)?.Select(m => m.SpendSecond)?.Sum() ?? 0;
                        double dayHour = Math.Round((double)daySecond / 60 / 60, 1);
                        item.data.Add(dayHour);
                    }

                    chartOption.series.Add(item);
                }

                string jsonStr = JsonUtil.Obj2JsonStr(chartOption);
                #endregion

                // 3. 保存 json 到本地
                #region 保存 json 到本地
                DateTime now = DateTime.Now;
                string dirPath = Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(ITodaysPlus)), "Upload", now.Year.ToString(), now.Month.ToString(), now.Day.ToString());
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                string saveJsonFileName = Guid.NewGuid() + ".json";
                string jsonfilePath = Path.Combine(dirPath, saveJsonFileName);
                System.IO.File.WriteAllText(jsonfilePath, jsonStr, System.Text.Encoding.UTF8);
                #endregion

                // 4. 返回json文件的 url 链接 (相对地址)
                viewModel = $"/plugins/{nameof(ITodaysPlus)}/Upload/{now.Year.ToString()}/{now.Month.ToString()}/{now.Day.ToString()}/{saveJsonFileName}";

                responseModel = new BaseResponseModel
                {
                    Code = 1,
                    Message = "成功",
                    Data = viewModel
                };
            }
            catch (Exception ex)
            {
                responseModel = new BaseResponseModel
                {
                    Code = -1,
                    Message = "失败 " + ex.Message + " " + ex.InnerException?.Message
                };
            }

            return responseModel;
        }


        #endregion

    }
}

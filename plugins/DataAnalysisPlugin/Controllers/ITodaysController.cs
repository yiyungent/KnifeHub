// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using DataAnalysisPlugin.Models;
using DataAnalysisPlugin.Models.ITodays;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAnalysisPlugin.Utils;
using Microsoft.AspNetCore.Authorization;
using DataAnalysisPlugin.Models.ECharts;
using System.Data;

namespace DataAnalysisPlugin.Controllers
{
    [ApiController]
    [Route($"api/Plugins/{(nameof(DataAnalysisPlugin))}/[controller]/[action]")]
    [Authorize("PluginCore.Admin")]
    public class ITodaysController : ControllerBase
    {
        #region Actions

        [HttpPost]
        public async Task<BaseResponseModel> Upload(IFormFile file, [FromQuery] ChartTypeEnum chartType = ChartTypeEnum.ECharts_StackedAreaChart)
        {
            BaseResponseModel responseModel = new BaseResponseModel();

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
            if (fileSize > 1024 * 1024 * 30) // 30M
            {
                responseModel.Code = -1;
                responseModel.Message = "上传的文件不能大于 30 MB";
                return responseModel;
            }
            #endregion

            try
            {
                #region xls -> RecordModel
                Stream stream = file.OpenReadStream();
                DataTable dt = NPOIUtil.ImportExcelForITodays(stream, fileExtension);
                int totalCount = dt.Rows.Count, importSuccessCount = 0;
                IList<RecordModel> recordModels = new List<RecordModel>();
                foreach (DataRow row in dt.Rows)
                {
                    long num = Convert.ToInt64(row["序号"]?.ToString());
                    string target = row["目标"]?.ToString();
                    string type = row["类型"]?.ToString();
                    DateTime startTime = Convert.ToDateTime(row["开始时间"]?.ToString());
                    DateTime endTime = Convert.ToDateTime(row["结束时间"]?.ToString());
                    long spendSecond = Convert.ToInt64(row["共花费(秒)"]?.ToString());
                    string remark = row["备注"]?.ToString();
                    var model = new RecordModel()
                    {
                        Num = num,
                        Target = target,
                        Type = type,
                        StartTime = startTime,
                        EndTime = endTime,
                        SpendSecond = spendSecond,
                        Remark = remark
                    };

                    recordModels.Add(model);
                }
                #endregion

                #region RecordModel -> EChartOption
                StackedAreaChartOptionModel chartOption = new();
                chartOption.title = new StackedAreaChartOptionModel.Title();
                chartOption.title.text = "堆叠面积图";
                chartOption.tooltip = new StackedAreaChartOptionModel.Tooltip();
                chartOption.tooltip.trigger = "axis";
                chartOption.tooltip.axisPointer = new StackedAreaChartOptionModel.Axispointer();
                chartOption.tooltip.axisPointer.type = "cross";
                chartOption.tooltip.axisPointer.label = new StackedAreaChartOptionModel.Label();
                chartOption.tooltip.axisPointer.label.backgroundColor = "#6a7985";
                chartOption.legend = new StackedAreaChartOptionModel.Legend();
                chartOption.legend.data = recordModels.Select(m => m.Type).Distinct().ToList();
                chartOption.toolbox = new StackedAreaChartOptionModel.Toolbox();
                chartOption.toolbox.feature = new StackedAreaChartOptionModel.Feature();
                chartOption.toolbox.feature.saveAsImage = new StackedAreaChartOptionModel.Saveasimage();
                chartOption.grid = new StackedAreaChartOptionModel.Grid();
                chartOption.grid.left = "3%";
                chartOption.grid.right = "4%";
                chartOption.grid.bottom = "3%";
                chartOption.grid.containLabel = true;
                chartOption.xAxis = new List<StackedAreaChartOptionModel.Xaxi>();
                IList<string> recordDays = recordModels.OrderBy(m => m.Num).Select(m => m.StartTime.ToString("yyyy-MM-dd"))
                    .Distinct().ToList();
                chartOption.xAxis.Add(new StackedAreaChartOptionModel.Xaxi()
                {
                    type = "category",
                    boundaryGap = false,
                    data = recordDays
                });
                chartOption.yAxis = new List<StackedAreaChartOptionModel.Yaxi>();
                chartOption.yAxis.Add(new StackedAreaChartOptionModel.Yaxi()
                {
                    type = "value"
                });
                chartOption.series = new List<StackedAreaChartOptionModel.Series>();
                IList<string> types = chartOption.legend.data;
                foreach (var type in types)
                {
                    var item = new StackedAreaChartOptionModel.Series();
                    item.name = type;
                    item.type = "line";
                    item.stack = "共花费(小时)";
                    item.areaStyle = new StackedAreaChartOptionModel.Areastyle();
                    item.emphasis = new StackedAreaChartOptionModel.Emphasis();
                    item.emphasis.focus = "series";
                    item.data = new List<double>();
                    foreach (string day in recordDays)
                    {
                        long daySecond = recordModels.Where(m => m.Type == type && m.StartTime.ToString("yyyy-MM-dd") == day)?.Select(m => m.SpendSecond)?.Sum() ?? 0;
                        double dayHour = Math.Round((double)((double)daySecond / 60 / 60), 2);
                        item.data.Add(dayHour);
                    }

                    chartOption.series.Add(item);
                }

                string chartOptionJsonStr = JsonUtil.Obj2JsonStr(chartOption);
                #endregion

                responseModel.Code = 1;
                responseModel.Message = "success";
                responseModel.Data = chartOption;
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "failure";
                responseModel.Data = ex.ToString();
            }

            return responseModel;
        }

        #endregion
    }
}

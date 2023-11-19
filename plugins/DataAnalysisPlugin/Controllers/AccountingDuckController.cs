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
using DataAnalysisPlugin.Models.AccountingDuck;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAnalysisPlugin.Utils;
using Microsoft.AspNetCore.Authorization;
using DataAnalysisPlugin.Models.ECharts;
using System.Data;
using MiniExcelLibs;

namespace DataAnalysisPlugin.Controllers
{
    /// <summary>
    /// 记账鸭
    /// </summary>
    [ApiController]
    [Route($"api/Plugins/{(nameof(DataAnalysisPlugin))}/[controller]/[action]")]
    [Authorize("PluginCore.Admin")]
    public class AccountingDuckController : ControllerBase
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
            if (fileExtension != ".xlsx")
            {
                responseModel.Code = -1;
                responseModel.Message = "只能上传 xlsx 格式文件";
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
                #region XlsxModel
                Stream stream = file.OpenReadStream();
                var rows = stream.Query<XlsxModel>();
                #endregion

                #region XlsxModel -> RecordModel
                IList<RecordModel> recordModels = new List<RecordModel>();
                foreach (var row in rows)
                {
                    // 过滤: 支出/收入
                    if (row.AccountingType?.Trim() == "支出" || row.AccountingType?.Trim() == "收入")
                    {
                        var model = new RecordModel()
                        {
                            Num = row.Num,
                            Type = row.Category,
                            DateTime = DateTime.Parse($"{row.Date} {row.Time}"),
                            Money = row.Money,
                            Remark = row.Remark,
                        };
                        recordModels.Add(model);
                    }
                }
                #endregion

                #region RecordModel -> EChartOption
                StackedAreaChartOptionModel chartOption = new();
                chartOption.title = new StackedAreaChartOptionModel.Title();
                chartOption.title.text = "支出/收入-堆叠面积图";
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
                IList<string> recordDays = recordModels.OrderBy(m => m.Num).Select(m => m.DateTime.ToString("yyyy-MM-dd"))
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
                    item.stack = "金额";
                    item.areaStyle = new StackedAreaChartOptionModel.Areastyle();
                    item.emphasis = new StackedAreaChartOptionModel.Emphasis();
                    item.emphasis.focus = "series";
                    item.data = new List<double>();
                    foreach (string day in recordDays)
                    {
                        double dayMoney = recordModels.Where(m => m.Type == type && m.DateTime.ToString("yyyy-MM-dd") == day)?.Select(m => m.Money)?.Sum() ?? 0;
                        item.data.Add(dayMoney);
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

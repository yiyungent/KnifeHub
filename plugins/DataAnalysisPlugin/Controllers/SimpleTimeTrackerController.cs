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
using DataAnalysisPlugin.Models.SimpleTimeTracker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAnalysisPlugin.Utils;
using Microsoft.AspNetCore.Authorization;

namespace DataAnalysisPlugin.Controllers
{
    [ApiController]
    [Route($"Plugins/{(nameof(DataAnalysisPlugin))}/[controller]/[action]")]
    [Authorize("PluginCore.Admin")]
    public class SimpleTimeTrackerController : ControllerBase
    {
        #region Actions

        [HttpPost]
        public async Task<BaseResponseModel> Upload([FromForm] IFormFile file)
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
            if (fileSize > 1024 * 1024 * 5) // 5M
            {
                responseModel.Code = -1;
                responseModel.Message = "上传的文件不能大于5MB";
                return responseModel;
            }
            #endregion

            try
            {
                #region CsvModel
                List<CsvModel> csvModels = new();
                Stream stream = file.OpenReadStream();
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csvModels = csv.GetRecords<CsvModel>().ToList();
                }
                #endregion

                #region RecordModel
                List<RecordModel> recordModels = new();
                for (int i = 0; i < recordModels.Count; i++)
                {
                    recordModels.Add(new RecordModel
                    {
                        Num = (i + 1),
                        Type = csvModels[i].ActivityName,
                        Remark = csvModels[i].Comment,
                        StartTime = DateTime.Parse(csvModels[i].TimeStarted),
                        EndTime = DateTime.Parse(csvModels[i].TimeEnded),
                        SpendSecond = (long)(DateTime.Parse(csvModels[i].TimeEnded) - DateTime.Parse(csvModels[i].TimeStarted)).TotalSeconds,
                    });
                }
                #endregion

                #region EChartOption
                RecordEChartOptionModel chartOption = new();
                chartOption.title = new RecordEChartOptionModel.Title();
                chartOption.title.text = "堆叠区域图";
                chartOption.tooltip = new RecordEChartOptionModel.Tooltip();
                chartOption.tooltip.trigger = "axis";
                chartOption.tooltip.axisPointer = new RecordEChartOptionModel.Axispointer();
                chartOption.tooltip.axisPointer.type = "cross";
                chartOption.tooltip.axisPointer.label = new RecordEChartOptionModel.Label();
                chartOption.tooltip.axisPointer.label.backgroundColor = "#6a7985";
                chartOption.legend = new RecordEChartOptionModel.Legend();
                chartOption.legend.data = recordModels.Select(m => m.Type).Distinct().ToList();
                chartOption.toolbox = new RecordEChartOptionModel.Toolbox();
                chartOption.toolbox.feature = new RecordEChartOptionModel.Feature();
                chartOption.toolbox.feature.saveAsImage = new RecordEChartOptionModel.Saveasimage();
                chartOption.grid = new RecordEChartOptionModel.Grid();
                chartOption.grid.left = "3%";
                chartOption.grid.right = "4%";
                chartOption.grid.bottom = "3%";
                chartOption.grid.containLabel = true;
                chartOption.xAxis = new List<RecordEChartOptionModel.Xaxi>();
                IList<string> recordDays = recordModels.OrderBy(m => m.Num).Select(m => m.StartTime.ToString("yyyy-MM-dd"))
                    .Distinct().ToList();
                chartOption.xAxis.Add(new RecordEChartOptionModel.Xaxi()
                {
                    type = "category",
                    boundaryGap = false,
                    data = recordDays
                });
                chartOption.yAxis = new List<RecordEChartOptionModel.Yaxi>();
                chartOption.yAxis.Add(new RecordEChartOptionModel.Yaxi()
                {
                    type = "value"
                });
                chartOption.series = new List<RecordEChartOptionModel.Series>();
                IList<string> types = chartOption.legend.data;
                foreach (var type in types)
                {
                    var item = new RecordEChartOptionModel.Series();
                    item.name = type;
                    item.type = "line";
                    item.stack = "共花费(秒)";
                    item.areaStyle = new RecordEChartOptionModel.Areastyle();
                    item.emphasis = new RecordEChartOptionModel.Emphasis();
                    item.emphasis.focus = "series";
                    item.data = new List<double>();
                    foreach (string day in recordDays)
                    {
                        long daySecond = recordModels.Where(m => m.Type == type && m.StartTime.ToString("yyyy-MM-dd") == day)?.Select(m => m.SpendSecond)?.Sum() ?? 0;
                        double dayHour = Math.Round((double)daySecond / 60 / 60, 1);
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

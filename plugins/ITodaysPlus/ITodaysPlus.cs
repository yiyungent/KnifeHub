using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using ITodaysPlus.Utils;
using System.Text;
using Octokit;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PluginCore;
using Scriban;
using System.Text.Json;

namespace ITodaysPlus
{
    public class ITodaysPlus : BasePlugin, ITimeJobPlugin
    {

        #region Props
        public long SecondsPeriod
        {
            get
            {
                return 1;
            }
        }

        public DateTime ITodaysBackupLastExecute { get; set; }
        #endregion

        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(ITodaysPlus)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(ITodaysPlus)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        #region 定时任务
        public async Task ExecuteAsync()
        {
            try
            {
                var settings = PluginSettingsModelFactory.Create<SettingsModel>(nameof(ITodaysPlus));

                #region 爱今天-备份记录
                try
                {
                    if (DateTime.Now - ITodaysBackupLastExecute > TimeSpan.FromSeconds(settings.Backup.SecondsPeriod))
                    {
                        ITodaysBackupLastExecute = DateTime.Now;

                        #region 备份到 本地
                        // 备份到 本地
                        if (settings.Backup.EnabledBackupToLocal)
                        {
                            ITodaysUtil iTodaysUtil = new ITodaysUtil();
                            var jsonOptions = new JsonSerializerOptions
                            {
                                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                WriteIndented = true
                            };
                            try
                            {
                                var loginResModel = iTodaysUtil.Login(settings.UserName, settings.Password);
                                if (loginResModel.status == "1")
                                {
                                    // 登录成功
                                    string folderPath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(ITodaysPlus), "Backups");
                                    if (!Directory.Exists(folderPath))
                                    {
                                        Directory.CreateDirectory(folderPath);
                                    }
                                    string loginResFilePath = Path.Combine(folderPath, "login.json");
                                    await File.WriteAllTextAsync(path: loginResFilePath, System.Text.Json.JsonSerializer.Serialize(loginResModel, jsonOptions), Encoding.UTF8);

                                    #region 备份日期记录
                                    if (!string.IsNullOrEmpty(settings.Backup?.DateRecordStartTime)
                                        &&
                                        DateTime.TryParse(settings.Backup.DateRecordStartTime, out DateTime startTime)
                                        &&
                                        !string.IsNullOrEmpty(settings.Backup?.DateRecordEndTime)
                                        &&
                                        DateTime.TryParse(settings.Backup.DateRecordEndTime, out DateTime endTime))
                                    {
                                        var folderDir = new DirectoryInfo(folderPath);
                                        while (startTime <= endTime)
                                        {
                                            string dateRecordFileName = $"DateRecord-{startTime.ToString("yyyy-MM-dd")}.json";
                                            string dateRecordFilePath = Path.Combine(folderPath, dateRecordFileName);
                                            Console.WriteLine($"{dateRecordFileName} - 尝试");

                                            var dateRecordFileNames = folderDir.GetFiles("DateRecord-*.json")?.Select(m => m.Name)?.ToArray();
                                            if (dateRecordFileNames != null && !dateRecordFileNames.Contains(dateRecordFileName))
                                            {
                                                // 不存在此记录才 新增保存
                                                try
                                                {
                                                    var dateRecordResModel = iTodaysUtil.GetDateRecord(loginResModel.token, startTime);
                                                    if (dateRecordResModel.ResponseModel != null
                                                        && dateRecordResModel.ResponseModel.status == 1
                                                        && dateRecordResModel.DataModel != null
                                                        && dateRecordResModel.DataModel.items.Length >= 1)
                                                    {
                                                        // 按日期/每一天 保存, 而不按每一条记录保存
                                                        await File.WriteAllTextAsync(path: dateRecordFilePath,
                                                            System.Text.Json.JsonSerializer.Serialize(dateRecordResModel.DataModel, jsonOptions),
                                                            Encoding.UTF8);
                                                        Console.WriteLine($"{dateRecordFileName} - 新增保存");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine(ex.ToString());
                                                }

                                                // 避免频繁
                                                Thread.Sleep(2000);
                                            }

                                            startTime = startTime.AddDays(1);
                                        }
                                    }
                                    #endregion
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"爱今天-备份记录: {ex.ToString()}");
                            }
                        }
                        #endregion

                        Console.WriteLine($"执行定时任务成功: 爱今天-备份记录");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"执行定时任务失败: 爱今天-备份记录: {ex.ToString()}");
                }
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行定时任务失败: {ex.ToString()}");
            }

            await Task.CompletedTask;
        }
        #endregion

    }
}

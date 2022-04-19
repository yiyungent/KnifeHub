using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using BackupPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using PluginCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.IO;

namespace BackupPlugin
{
    public class BackupPlugin : BasePlugin, ITimeJobPlugin
    {
        /// <summary>
        /// 43200 = 12 小时
        /// </summary>
        public long SecondsPeriod
        {
            get
            {
                try
                {
                    // 注意: 当 json 中的 SecondsPeriod 为 字符串时, 这么获取会导致报错, 然后整个程序都会退出
                    var settings = PluginSettingsModelFactory.Create<SettingsModel>(nameof(BackupPlugin));
                    return settings.SecondsPeriod;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("设置里的 SecondsPeriod 出错");

                    return 180;
                }

            }
        }

        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(BackupPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(BackupPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        #region 定时任务
        public async Task ExecuteAsync()
        {
            try
            {
                SettingsModel settings = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(BackupPlugin));

                #region Telegram
                if (settings.Telegram != null && settings.Telegram.Enable)
                {
                    string chatId = settings.Telegram.ChatId;
                    var botClient = new TelegramBotClient(settings.Telegram.Token);

                    Console.WriteLine($"Telegram.ChatId: {chatId}");
                    Console.WriteLine($"Telegram.Token: {settings.Telegram.Token}");

                    bool isSuccess = BackupFiles(out string fileName, out string zipFilePath);
                    System.IO.Stream fileStream = null;
                    if (isSuccess)
                    {
                        #region 发送文件给 Tg
                        try
                        {
                            using (fileStream = System.IO.File.OpenRead(zipFilePath))
                            {
                                await botClient.SendDocumentAsync(
                                        chatId: new ChatId(long.Parse(chatId))
                                        , document: new Telegram.Bot.Types.InputFiles.InputOnlineFile(content: fileStream, fileName: fileName)
                                         );
                            }
                        }
                        catch (Exception ex)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: new ChatId(long.Parse(chatId))
                                , text: "发送 备份文件 失败");
                            await botClient.SendTextMessageAsync(
                                chatId: new ChatId(long.Parse(chatId))
                                , text: ex.ToString());
                        }
                        #endregion
                    }
                    else
                    {
                        Console.WriteLine("压缩 备份文件 失败");
                        await botClient.SendTextMessageAsync(
                        chatId: new ChatId(long.Parse(chatId))
                        , text: "压缩 备份文件 失败");
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行定时 备份 任务失败: \r\n {ex.ToString()}");
            }

            await Task.CompletedTask;
        }
        #endregion


        public bool BackupFiles(out string fileName, out string zipFilePath)
        {
            bool isSuccess = false;
            List<string> sourceListTemp = new List<string>()
            {
                PluginPathProvider.PluginsRootPath(),
                PluginPathProvider.PluginsWwwRootDir(),
                System.IO.Path.Combine(Directory.GetCurrentDirectory(), "App_Data")
            };
            List<string> sourceList = new List<string>();
            foreach (var item in sourceListTemp)
            {
                if (Directory.Exists(item))
                {
                    sourceList.Add(item);
                }
                else
                {
                    Console.WriteLine("文件夹不存在:");
                    Console.WriteLine(item);
                }
            }
            string backupsDirPath = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
            if (!Directory.Exists(backupsDirPath))
            {
                Directory.CreateDirectory(backupsDirPath);
            }
            fileName = $"{nameof(BackupPlugin)}-{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.zip";
            zipFilePath = Path.Combine(backupsDirPath, fileName);
            Console.WriteLine("备份目标文件路径: ");
            Console.WriteLine(zipFilePath);
            try
            {
                isSuccess = Utils.ZipUtil.CompressFile(sourceList: sourceList, zipFilePath: zipFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("压缩文件出错:");
                Console.WriteLine(ex.ToString());
            }

            return isSuccess;
        }


    }
}

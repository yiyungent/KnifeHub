using Konata.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonataPlugin
{
    public static class KonataBotStore
    {
        public static Bot Bot { get; set; }



        #region 废弃
        //public static class StoreUtil
        //{
        //    public static T Get<T>(string fileName, T defaultObj)
        //    {
        //        T rtnObj = defaultObj;
        //        string configDir = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
        //        if (!Directory.Exists(configDir))
        //        {
        //            Directory.CreateDirectory(configDir);
        //        }
        //        string filePath = Path.Combine(configDir, fileName);
        //        if (!File.Exists(filePath))
        //        {
        //            string jsonStr = Utils.JsonUtil.Obj2JsonStr(rtnObj);
        //            File.WriteAllText(path: filePath, contents: jsonStr, encoding: System.Text.Encoding.UTF8);
        //        }
        //        else
        //        {
        //            string jsonStr = File.ReadAllText(path: filePath, encoding: System.Text.Encoding.UTF8);
        //            rtnObj = Utils.JsonUtil.JsonStr2Obj<T>(jsonStr);
        //        }

        //        return rtnObj;
        //    }

        //    public static void Set<T>(T obj, string fileName)
        //    {
        //        string configDir = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
        //        if (!Directory.Exists(configDir))
        //        {
        //            Directory.CreateDirectory(configDir);
        //        }
        //        string filePath = Path.Combine(configDir, fileName);
        //        string jsonStr = Utils.JsonUtil.Obj2JsonStr(obj);
        //        File.WriteAllText(path: filePath, contents: jsonStr, encoding: System.Text.Encoding.UTF8);
        //    }

        //}

        //public static BotConfig BotConfig
        //{
        //    get
        //    {
        //        BotConfig botConfig = BotConfig.Default();
        //        botConfig = StoreUtil.Get<BotConfig>("config.json", botConfig);

        //        return botConfig;
        //    }
        //    set
        //    {
        //        BotConfig botConfig = value;
        //        StoreUtil.Set<BotConfig>(botConfig, "config.json");
        //    }
        //}

        //public static BotDevice BotDevice
        //{
        //    get
        //    {
        //        BotDevice botDevice = BotDevice.Default();
        //        botDevice = StoreUtil.Get<BotDevice>("device.json", botDevice);

        //        return botDevice;
        //    }
        //    set
        //    {
        //        BotDevice botDevice = value;
        //        StoreUtil.Set<BotDevice>(botDevice, "device.json");
        //    }
        //}

        //public static BotKeyStore BotKeyStore
        //{
        //    get
        //    {
        //        SettingsModel settingsModel = Utils.SettingsUtil.Get();
        //        string account = settingsModel.QQ;
        //        string password = settingsModel.Password;
        //        BotKeyStore botKeyStore = new BotKeyStore(uin: account, password: password);
        //        botKeyStore = StoreUtil.Get<BotKeyStore>("keystore.json", botKeyStore);

        //        return botKeyStore;
        //    }
        //    set
        //    {
        //        BotKeyStore botKeyStore = value;
        //        StoreUtil.Set<BotKeyStore>(botKeyStore, "keystore.json");
        //    }
        //} 
        #endregion
    }
}

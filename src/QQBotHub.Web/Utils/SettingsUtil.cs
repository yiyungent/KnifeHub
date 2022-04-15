using QQBotHub.Sdk.Utils;

namespace QQBotHub.Web.Utils
{
    public class SettingsUtil
    {

        public static void EnsureExist()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
            if (!File.Exists(filePath))
            {
                Set(new SettingsModel());
            }
        }

        public static SettingsModel Get()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
            if (!File.Exists(filePath))
            {
                SettingsModel jsonModel = new SettingsModel();
                string jsonStr = JsonUtil.Obj2JsonStr(jsonModel);
                File.WriteAllText(path: filePath, contents: jsonStr, encoding: System.Text.Encoding.UTF8);

                return jsonModel;
            }
            else
            {
                string jsonStr = File.ReadAllText(path: filePath, encoding: System.Text.Encoding.UTF8);
                SettingsModel jsonModel = JsonUtil.JsonStr2Obj<SettingsModel>(jsonStr);

                return jsonModel;
            }
        }

        public static void Set(SettingsModel settingsModel)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
            string jsonStr = JsonUtil.Obj2JsonStr(settingsModel);
            File.WriteAllText(path: filePath, contents: jsonStr, encoding: System.Text.Encoding.UTF8);
        }
    }
}

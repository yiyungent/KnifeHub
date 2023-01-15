

using System.IO;

namespace KnifeHub.Sdk.Utils
{
    public class SettingsUtil
    {
        public static void EnsureExist<T>(string pluginId)
            where T : PluginCore.Models.PluginSettingsModel, new()
        {
            string pluginDir = Path.Combine(PluginCore.PluginPathProvider.PluginsRootPath(), pluginId);
            string filePath = Path.Combine(pluginDir, "settings.json");
            if (!File.Exists(filePath))
            {
                Set(pluginId, new T());
            }
        }

        public static T Get<T>(string pluginId)
            where T : PluginCore.Models.PluginSettingsModel, new()
        {
            string pluginDir = Path.Combine(PluginCore.PluginPathProvider.PluginsRootPath(), pluginId);
            string filePath = Path.Combine(pluginDir, "settings.json");
            if (!File.Exists(filePath))
            {
                T jsonModel = new T();
                string jsonStr = JsonUtil.Obj2JsonStr(jsonModel);
                File.WriteAllText(path: filePath, contents: jsonStr, encoding: System.Text.Encoding.UTF8);

                return jsonModel;
            }
            else
            {
                string jsonStr = File.ReadAllText(path: filePath, encoding: System.Text.Encoding.UTF8);
                T jsonModel = JsonUtil.JsonStr2Obj<T>(jsonStr);

                return jsonModel;
            }
        }

        public static void Set<T>(string pluginId, T settingsModel)
            where T : PluginCore.Models.PluginSettingsModel, new()
        {
            string pluginDir = Path.Combine(PluginCore.PluginPathProvider.PluginsRootPath(), pluginId);
            string filePath = Path.Combine(pluginDir, "settings.json");
            string jsonStr = JsonUtil.Obj2JsonStr(settingsModel);
            File.WriteAllText(path: filePath, contents: jsonStr, encoding: System.Text.Encoding.UTF8);
        }
    }
}



using System.IO;

namespace KonataPlugin.Utils
{
    public class SettingsUtil
    {
        public static void EnsureExist(string pluginId)
        {
            string pluginDir = Path.Combine(PluginCore.PluginPathProvider.PluginsRootPath(), pluginId);
            string filePath = Path.Combine(pluginDir, "settings.json");
            if (!File.Exists(filePath))
            {
                Set(pluginId, new SettingsModel());
            }
        }

        public static SettingsModel Get(string pluginId)
        {
            string pluginDir = Path.Combine(PluginCore.PluginPathProvider.PluginsRootPath(), pluginId);
            string filePath = Path.Combine(pluginDir, "settings.json");
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

        public static void Set(string pluginId, SettingsModel settingsModel)
        {
            string pluginDir = Path.Combine(PluginCore.PluginPathProvider.PluginsRootPath(), pluginId);
            string filePath = Path.Combine(pluginDir, "settings.json");
            string jsonStr = JsonUtil.Obj2JsonStr(settingsModel);
            File.WriteAllText(path: filePath, contents: jsonStr, encoding: System.Text.Encoding.UTF8);
        }
    }
}

using EleCho.GoCqHttpSdk;
using EleChoPlugin;
using PluginCore.IPlugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCore.IPlugins
{
    public interface IEleChoPlugin : IPlugin
    {
        public List<CqPostPlugin> UseCqPostPlugins(EleChoBotStore.BotItemModel bot);
    }
}

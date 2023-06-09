using Sora.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoraPlugin
{
    public static class SoraBotStore
    {
        public static BotItemModel Bot { get; set; }

        static SoraBotStore()
        {
            Bot = new BotItemModel();
        }

        public sealed class BotItemModel
        {
            public ISoraService SoraService { get; set; }
        }
    }
}

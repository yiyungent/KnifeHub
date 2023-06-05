using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using AnkiPlus.Utils;
using System.Text;
using Octokit;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PluginCore;
using Scriban;
using System.Text.Json;

namespace AnkiPlus
{
    /// <summary>
    /// AnkiConnect: https://github.com/FooSoft/anki-connect
    /// </summary>
    public class AnkiPlus : BasePlugin
    {
        #region Props
        
        #endregion

        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(AnkiPlus)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(AnkiPlus)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

    }
}

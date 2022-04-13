using System;
using System.Collections.Generic;
using System.Text;
using QQHelloWorldPlugin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PluginCore;

namespace QQHelloWorldPlugin.Controllers
{
    [Route("api/plugins/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {

        public ActionResult Get()
        {
            SettingsModel settingsModel = PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQHelloWorldPlugin));
            string str = $"Hello PluginCore ! {settingsModel.Hello}";

            return Ok(str);
        }

    }
}

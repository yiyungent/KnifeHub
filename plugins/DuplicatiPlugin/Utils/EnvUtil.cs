using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicatiPlugin.Utils
{
    public class EnvUtil
    {
        public static string GetEnv(string name)
        {
            // 注意: 当没有这个环境变量时, 不会报错, 而是返回 null
            return Environment.GetEnvironmentVariable(name);
        }

    }
}

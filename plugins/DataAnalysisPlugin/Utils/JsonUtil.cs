// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAnalysisPlugin.Utils
{
    public class JsonUtil
    {

        #region JsonStr2Obj
        public static T JsonStr2Obj<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
        #endregion

        #region Obj2JsonStr
        public static string Obj2JsonStr(object jsonObj)
        {
            return JsonConvert.SerializeObject(jsonObj);
        }
        #endregion

    }
}

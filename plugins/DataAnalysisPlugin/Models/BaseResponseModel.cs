// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysisPlugin.Models
{
    public class BaseResponseModel<T>
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }

    public class BaseResponseModel : BaseResponseModel<object>
    { }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysisPlugin.Models.SimpleTimeTracker
{
    public class RecordModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public long Num { get; set; }

        /// <summary>
        /// 类型/分类
        /// </summary>
        public string Type { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public long SpendSecond { get; set; }

        public string Remark { get; set; }
    }
}

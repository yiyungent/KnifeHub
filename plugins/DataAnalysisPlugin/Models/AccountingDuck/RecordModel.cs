// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysisPlugin.Models.AccountingDuck
{
    public class RecordModel
    {
        /// <summary>
        /// 序号
        /// </summary>
        public long Num { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 记账日期+记账时间
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}

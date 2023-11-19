// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniExcelLibs.Attributes;

namespace DataAnalysisPlugin.Models.AccountingDuck
{
    /// <summary>
    /// 序号
    /// 记账日期
    /// 记账时间	
    /// 分类	
    /// 记账类型	
    /// 金额	
    /// 流出账户	
    /// 流入账户	
    /// 备注
    /// </summary>
    public class XlsxModel
    {
        [ExcelColumnName("序号")]
        public int Num { get; set; }

        [ExcelColumnName("记账日期")]
        public string Date { get; set; }

        [ExcelColumnName("记账时间")]
        public string Time { get; set; }

        [ExcelColumnName("分类")]
        public string Category { get; set; }

        /// <summary>
        /// 记账类型
        /// </summary>
        /// <remarks>
        /// 支出
        /// 收入
        /// 转账
        /// </remarks>
        [ExcelColumnName("记账类型")]
        public string AccountingType { get; set; }

        [ExcelColumnName("金额")]
        public double Money { get; set; }

        /// <summary>
        /// 流出账户
        /// </summary>
        [ExcelColumnName("流出账户")]
        public string OutAccount { get; set; }

        /// <summary>
        /// 流入账户
        /// </summary>
        [ExcelColumnName("流入账户")]
        public string InAccount { get; set; }

        [ExcelColumnName("备注")]
        public string Remark { get; set; }
    }
}

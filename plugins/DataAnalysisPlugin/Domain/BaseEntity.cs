// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAnalysisPlugin.Domain
{
    public class BaseEntity
    {
        [Key]
        public long Id { get; set; }

        //[DataType(DataType.Date)] // https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.datatypeattribute?view=net-7.0
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 13位 JavaScript 毫秒 时间戳
        /// </summary>
        public long CreateTimeStamp { get; set; }

        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 13位 JavaScript 毫秒 时间戳
        /// </summary>
        public long UpdateTimeStamp { get; set; }
    }
}

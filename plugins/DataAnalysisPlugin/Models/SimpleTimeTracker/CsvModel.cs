// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace DataAnalysisPlugin.Models.SimpleTimeTracker
{
    /// <summary>
    /// Simple Time Tracker 
    /// v1.29
    /// </summary>
    public class CsvModel
    {
        /// <summary>
        /// 睡眠
        /// </summary>
        [Name("activity name")]
        public string ActivityName { get; set; }

        /// <summary>
        /// 2022-06-19 13:02:31
        /// </summary>
        [Name("time started")]
        public string TimeStarted { get; set; }

        /// <summary>
        /// 2022-06-19 13:09:47
        /// </summary>
        [Name("time ended")]
        public string TimeEnded { get; set; }

        /// <summary>
        /// 趴在桌子睡
        /// </summary>
        [Name("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Name("categories")]
        public string Categories { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Name("record tags")]
        public string RecordTags { get; set; }

        /// <summary>
        /// 0:6:14
        /// </summary>
        [Name("duration")]
        public string Duration { get; set; }

        /// <summary>
        /// 5
        /// </summary>
        [Name("duration minutes")]
        public long DurationMinutes { get; set; }
    }
}

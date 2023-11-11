// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAnalysisPlugin.Domain.Entities.SimpleTimeTracker
{
    /// <summary>
    /// Simple Time Tracker 
    /// Android App v1.29
    /// </summary>
    public class SimpleTimeTrackerCsvEntity : BaseEntity
    {
        /// <summary>
        /// 睡眠
        /// </summary>
        [StringLength(1000)]
        [Column(TypeName = "text")]
        public string ActivityName { get; set; }

        /// <summary>
        /// 2022-06-19 13:02:31
        /// </summary>
        public DateTime TimeStarted { get; set; }

        public long TimeStartedStamp { get; set; }

        /// <summary>
        /// 2022-06-19 13:09:47
        /// </summary>
        public DateTime TimeEnded { get; set; }

        public long TimeEndedStamp { get; set; }

        /// <summary>
        /// 趴在桌子睡
        /// </summary>
        [StringLength(1000)]
        [Column(TypeName = "text")]
        public string Comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(1000)]
        [Column(TypeName = "text")]
        public string Categories { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [StringLength(1000)]
        [Column(TypeName = "text")]
        public string RecordTags { get; set; }

        /// <summary>
        /// 0:6:14
        /// </summary>
        [StringLength(30)]
        public string Duration { get; set; }

        /// <summary>
        /// 5
        /// </summary>
        public long DurationMinutes { get; set; }
    }
}

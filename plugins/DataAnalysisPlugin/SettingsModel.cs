using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace DataAnalysisPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        #region Simple Time Tracker
        public SimpleTimeTrackerModel SimpleTimeTracker { get; set; }

        public class SimpleTimeTrackerModel
        {

        }
        #endregion

        #region 爱今天
        public ITodaysModel ITodays { get; set; }

        public class ITodaysModel
        {
            public string UserName { get; set; }

            public string Password { get; set; }

            public BackupModel Backup { get; set; }

            public class BackupModel
            {
                public long SecondsPeriod { get; set; }

                public bool EnabledBackupToLocal { get; set; }

                /// <summary>
                /// yyyy-MM-dd
                /// </summary>
                public string DateRecordStartTime { get; set; }

                /// <summary>
                /// yyyy-MM-dd
                /// </summary>
                public string DateRecordEndTime { get; set; }
            }
        }
        #endregion
    }
}

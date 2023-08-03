using System;
using System.Collections.Generic;
using System.Text;

namespace QQStat4SoraPlugin.ResponseModels.Calendar
{
    public class CalendarEChartsOptionResponseDataModel
    {
        public TooltipModel tooltip { get; set; }
        public VisualmapModel visualMap { get; set; }
        public List<CalendarModel> calendar { get; set; }
        public List<SeriesModel> series { get; set; }

        public class TooltipModel
        {
            public string position { get; set; }
        }

        public class VisualmapModel
        {
            public int min { get; set; }
            public int max { get; set; }
            public bool calculable { get; set; }
            public string orient { get; set; }
            public string left { get; set; }
            public string top { get; set; }

            public inRangeModel inRange { get; set; }

            public class inRangeModel
            {
                public List<string> color { get; set; }

                public double colorAlpha { get; set; }
            }
        }

        public class CalendarModel
        {
            public string range { get; set; }
            public List<string> cellSize { get; set; }
            public int top { get; set; }
            public int right { get; set; }
        }

        public class SeriesModel
        {
            public string type { get; set; }
            public string coordinateSystem { get; set; }
            public int calendarIndex { get; set; }
            public List<List<string>> data { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace AnkiPlus
{
    public class SettingsModel : PluginSettingsModel
    {
        public AnkiConnectModel AnkiConnect { get; set; }

        public class AnkiConnectModel
        {
            public string BaseUrl { get; set; }

            public int Version { get; set; }

            public NoteModel Note { get; set; }

            public class NoteModel
            {
                public string DeckName { get; set; }

                public string ModelName { get; set; }

                public FieldsModel Fields { get; set; }

                public class FieldsModel
                {
                    public string Front { get; set; } = nameof(Front);

                    public string Back { get; set; } = nameof(Back);
                }
            }
        }
    }
}

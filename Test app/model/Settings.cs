using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koda_radio.model
{
    public class Settings
    {
        public static FileInfo file = new FileInfo(Path.Combine(Environment.SpecialFolder.MyDocuments.ToString(), @"Koda Creations\Koda Radio\Settings.json").ToString());

        private bool _hideOnMinimise = false; // Hide to toolbar instead of minimising th main window
        private bool _hideOnClose = false; // Hide to toolbar instead of closing th main window

        private int _automaticRefreshInterval = 15; // Minute interval the player restarts the playback to prevent lag and unintended stops; 0 = off




        public bool HideOnMinimise { get => _hideOnMinimise; set => _hideOnMinimise = value; }
        public bool HideOnClose { get => _hideOnClose; set => _hideOnClose = value; }
        public int AutomaticRefreshInterval { get => _automaticRefreshInterval; set => _automaticRefreshInterval = value; }

        public Settings()
        {
            if (file.Exists)
            {

            }
            else
            {
                file.Create();
            }
            JObject o1 = JObject.Parse(File.ReadAllText(Path.Combine(file.FullName, file.Name)));
            
        }

    }
}

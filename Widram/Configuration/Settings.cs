using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Widram.Configuration
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Settings
    {
        private static CrudSettings crudSettings = new CrudSettings();


        [JsonIgnore]
        private static Settings instance { get; set; }

        [JsonIgnore]
        public static Settings SetInstance
        {
            set { instance = value; }
        }

        [JsonIgnore]
        public static Settings Instance
        {
            get
            {
                return instance ?? (instance = crudSettings.Read());
            }
        }

        public void Write()
        {
            crudSettings.Write(Instance);
        }


        [JsonIgnore]
        public bool ProVersion { get { return false; }}

        [JsonProperty]
        public int TextSizeDigits { get; set; }
        [JsonProperty]
        public string TypefaceDigits { get; set; }
        [JsonProperty]
        public string ColorTextDigits { get; set; }
        [JsonProperty]
        public string CanvasColor { get; set; }
        [JsonProperty]
        public float Abscissa { get; set; }
        [JsonProperty]
        public float Ordinate { get; set; }
        [JsonProperty]
        public string CanvasMaterial { get; set; }
        [JsonProperty]
        public int UpdateTimer { get; set; }
        [JsonProperty]
        public bool LayoutNotificationEmpty { get; set; }
        [JsonProperty]
        public bool IsLogErrorStorage { get; set; }
        [JsonProperty]
        public bool IsShowRelativeRAM { get; set; }
        [JsonProperty]
        public bool IsShowFreeRam { get; set; }
        [JsonProperty]
        public bool IsSendCrashes { get; set; }
        [JsonProperty]
        public bool IsRamSelectedNotification { get; set; }
        [JsonProperty]
        public bool IsCpuSelectedNotification { get; set; }
        [JsonProperty]
        public bool IsShowAbsoluteRAM { get; set; }
        [JsonProperty]
        public bool IsShowFreeCpu { get; set; }
        [JsonProperty]
        public bool IsAutoStartWhenRebootOS { get; set; }

    

       public Settings() { }
    }
}
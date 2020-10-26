using Android.App;
using Android.Content;
using Google.Android.Material.BottomNavigation;
using Microsoft.AppCenter.Crashes;

namespace Widram.Helpers
{
    [LogRuntime("Info.txt", "Widram")]
    public class Info
    {
        private Context context;

        public static readonly string AppCenterId;
        public static readonly string FileNameSettings;
        public static readonly string ApplicationName;
        public static readonly string Name;
        public static readonly string Description;
        public static readonly string Chanlelid;
        public static readonly int NotificationId;
        public static readonly string IntentActionStartService;
        public static readonly string IntentActionStopService;
        public static readonly string IntentActionRestartServiceRam;
        public static readonly string IntentActionUpdateSettings;
        public static readonly string ActionMainActivity;
        public static readonly string ServiceStartedKey;
        public static readonly string ServiceBoundedKey;
        public static readonly string BroadcastMessengeKey;
        public static readonly string IntentActionAlarm;
  
        static Info()
        {
            try
            {
                Context context = Android.App.Application.Context;

                AppCenterId = context.Resources.GetString(Resource.String.AppCenterId);
                FileNameSettings = context.Resources.GetString(Resource.String.name_settings_file);
                ApplicationName = context.Resources.GetString(Resource.String.app_name);
                Name = context.Resources.GetString(Resource.String.ChannelNameRam);
                Description = context.Resources.GetString(Resource.String.ChannelDescriptionRam);
                Chanlelid = context.Resources.GetString(Resource.String.ChannelIDRam);
                NotificationId = context.Resources.GetInteger(Resource.Integer.IntegerNotificationRamID);
                IntentActionAlarm = context.Resources.GetString(Resource.String.IntentActionAlarm);
                IntentActionUpdateSettings = context.Resources.GetString(Resource.String.IntentActionUpdateSettings);
                IntentActionStartService = context.Resources.GetString(Resource.String.IntentActionStartServiceRam);
                IntentActionStopService = context.Resources.GetString(Resource.String.IntentActionStopServiceRam);
                IntentActionRestartServiceRam = context.Resources.GetString(Resource.String.IntentActionRestartServiceRam);
                ActionMainActivity = context.Resources.GetString(Resource.String.ActionMainActivity);
                ServiceStartedKey = context.Resources.GetString(Resource.String.ServiceStartedKey);
                ServiceBoundedKey = context.Resources.GetString(Resource.String.ServiceBoundedKey);
                BroadcastMessengeKey = context.Resources.GetString(Resource.String.BroadcastMessengeKey);
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(Info), ex);
                #endregion
            }
            finally { }
        }
    }
}
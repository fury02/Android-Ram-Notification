using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Widram.Helpers;
using Widram.Service;

using Android.Content;
using Java.IO;
using Microsoft.AppCenter.Crashes;

namespace Widram.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "com.arbuz.widram.alarmreceiver" })]
    [LogRuntime("AlarmReceiver.txt", "Widram")]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {          
            if (Info.IntentActionAlarm == intent.Action)
            {
                try
                {
                    if (!HearServices.IsServiceRunning(typeof(NotifyService), context))
                    {
                        Intent serviceIntent;

                        serviceIntent = new Intent(context, typeof(NotifyService));
                        serviceIntent.SetAction(Info.IntentActionStartService);

                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                        {
                            context.StartForegroundService(serviceIntent);
                        }
                        else
                        {
                            context.StartService(serviceIntent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    #region Logging
                    LogRuntimeAttribute.InLogFiles(typeof(AlarmReceiver), ex);
                    #endregion
                }
                finally  { }
            }
        }
    }
}
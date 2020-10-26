using Widram.Helpers;
using Widram.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;

namespace Widram.Receivers
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { "com.arbuz.widram.notifyreceiver" })]
    [LogRuntime("NotifyReceiver.txt", "Widram")]
    public class NotifyReceiver : BroadcastReceiver
    {
        Intent serviceIntentStart;
        Intent serviceIntentStop;

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (serviceIntentStop == null)
                {
                    serviceIntentStop = new Intent(context, typeof(NotifyService));
                    serviceIntentStop.SetAction(Info.IntentActionStopService);
                }

                if (serviceIntentStart == null)
                {
                    serviceIntentStart = new Intent(context, typeof(NotifyService));
                    serviceIntentStart.SetAction(Info.IntentActionStartService);
                }

                #region Actions
                #region Action Start
                if (Info.IntentActionStartService == intent.Action)
                {
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                    {
                        context.StartForegroundService(serviceIntentStart);
                    }
                    else
                    {
                        context.StartService(serviceIntentStart);
                    }
                }
                #endregion
                #region Action Stop
                if (Info.IntentActionStopService == intent.Action)
                {
                    context.StopService(serviceIntentStop);
                }
                #endregion
                #region Action Restart
                if (Info.IntentActionRestartServiceRam == intent.Action)
                {
                    context.StopService(serviceIntentStop);

                    if (!HearServices.IsServiceRunning(typeof(NotifyService), context))
                    {
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                        {
                            context.StartForegroundService(serviceIntentStart);
                        }
                        else
                        {
                            context.StartService(serviceIntentStart);
                        }
                    }
                }
                #endregion
                #region Action Update
                if (Info.IntentActionUpdateSettings == intent.Action)
                {
                    if (HearServices.IsServiceRunning(typeof(NotifyService), context))
                    {
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                        {
                            context.StartForegroundService(serviceIntentStart);
                        }
                        else
                        {
                            context.StartService(serviceIntentStart);
                        }
                    }
                }
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(NotifyReceiver), ex);
                #endregion
            }
            finally { }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Java.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using Widram.Helpers;
using Widram.Service;
using Widram.Configuration;
using Microsoft.AppCenter.Crashes;

namespace Widram.Receivers
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted })]
    [LogRuntime("LoadingReceiver.txt", "Widram")]
    class LoadingReceiver : BroadcastReceiver
    {
        private Settings settings;
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionBootCompleted)
            {
                try
                {
                    settings = Settings.Instance;    

                    #region Action starting OS       
                    Intent serviceIntent;

                    serviceIntent = new Intent(context, typeof(NotifyService));
                    serviceIntent.SetAction(Info.IntentActionStartService);

                    if(settings.IsAutoStartWhenRebootOS == true)
                    {
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                        {
                            context.StartForegroundService(serviceIntent);
                        }
                        else
                        {
                            context.StartService(serviceIntent);
                        }
                    }
                       
                    #endregion
                }
                catch (Exception ex)
                {
                    #region Logging
                    LogRuntimeAttribute.InLogFiles(typeof(LoadingReceiver), ex);
                    #endregion
                }
                finally { }
            }
        }
    }
}

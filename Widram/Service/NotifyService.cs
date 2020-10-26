using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.IO;
using Java.Lang;
using Microsoft.AppCenter.Crashes;
using Widram.Configuration;
using Widram.Helpers;
using Widram.Model;
using Widram.Paiting;
using Widram.Receivers;

namespace Widram.Service
{
    [Service(IsolatedProcess = false, Exported = false, Name = "widram.notifyservice")]
    [LogRuntime("NotifyService.txt", "Widram")]
    public class NotifyService : Android.App.Service
    {
        private Context context;

        private PartLoad partLoad;
        private Settings settings;

        private DrawableIconDigits<long> drawableIconDigit;
        private DrawableIcon<string> drawableIcon;

        private AlarmManager alarmManager;
        private PendingIntent pendingIntentAlarm;

        private Handler handler;
        private Action action;


        #region layout_notification

        RemoteViews remoteViewsEmpty;
        RemoteViews remoteViews;

        LayoutInflater layoutInflater;

        View view;

        TextView textView1;
        TextView textView2;
        TextView textView3;
        TextView textView4;
        TextView textView5;
        TextView textView6;

        #endregion

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            try
            {
                context = Android.App.Application.Context;
                partLoad = new PartLoad();
                settings = Settings.Instance;

                #region Layout notification
                LayoutNotificationResourceInit();
                #endregion

                #region Notify Receivers 
                NotifyReceiver notifyReceiver = new NotifyReceiver();

                IntentFilter notifyIntentFilter = new IntentFilter("com.arbuz.widram.notifyreceiver");
                notifyIntentFilter.AddAction(Info.IntentActionStopService);
                notifyIntentFilter.AddAction(Info.IntentActionRestartServiceRam);
                notifyIntentFilter.AddAction(Info.IntentActionUpdateSettings);

                context.RegisterReceiver(notifyReceiver, notifyIntentFilter);
                #endregion

                #region  Loading Receivers 
                LoadingReceiver loadingReceiver = new LoadingReceiver();
                IntentFilter loadingIntentFilter = new IntentFilter(Android.Content.Intent.ActionBootCompleted);
                context.RegisterReceiver(loadingReceiver, loadingIntentFilter);
                #endregion

                #region  Alarm Receivers 
                AlarmReceiver alarmReceiver = new AlarmReceiver();

                IntentFilter alarmIntentFilter = new IntentFilter("com.arbuz.widram.alarmreceiver");
                alarmIntentFilter.AddAction(Info.IntentActionAlarm);

                context.RegisterReceiver(alarmReceiver, alarmIntentFilter);

                Intent intentAlarm = new Intent(context, typeof(AlarmReceiver));
                intentAlarm.SetAction(Info.IntentActionAlarm);

                pendingIntentAlarm = PendingIntent.GetBroadcast(context, 0, intentAlarm, PendingIntentFlags.UpdateCurrent);

                alarmManager = (AlarmManager)context.GetSystemService(AlarmService);
                alarmManager.SetRepeating(AlarmType.ElapsedRealtime, 1000, 1000, pendingIntentAlarm); //API 19 Timer not working for setup number
                #endregion

                #region Main
                handler = new Handler();
                action = new Action(async () => await OnWork());
                partLoad.RamUpdated += Updated;
                #endregion
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(NotifyService), ex);
                #endregion
            }

            finally { }
        }

        private void Updated(object sender, PartLoad e)
        {
            Icon icon = GenerateIcon(e);
            RegisterNotification(icon, e);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                RegisterNotification();
                handler.PostDelayed(action, 1);

                return StartCommandResult.Sticky;
            }
            catch (System.Exception ex)
            {
                #region Attempt restart
                AttemptRestart();
                #endregion

                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(NotifyService), ex);
                #endregion

                return StartCommandResult.Sticky;
            }

            finally { }
        }

        public override void OnDestroy()
        {
            try
            {
                base.OnDestroy();

                alarmManager.Cancel(pendingIntentAlarm);
                handler.RemoveCallbacks(action);
                RemoveNotification();

                StopSelf();
                partLoad.Stop();
                partLoad.RamUpdated -= Updated;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(NotifyService), ex);
                #endregion
            }
            finally { }
        }

        private async Task OnWork()
        {
            settings = Settings.Instance;
            bool serviceRun = HearServices.IsServiceRunning(typeof(NotifyService), context);

            if (serviceRun)
            {
                partLoad.Start(settings.UpdateTimer);
            }
        }

        private void RegisterNotification(Icon icon = null, PartLoad e = null)
        {
            try
            {
                using (NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService))
                {
                    Notification.Builder notificationBuilder;

                    settings = Settings.Instance;

                    string freeRam = System.String.Empty;
                    string loadRam = System.String.Empty;
                    string totalRam = System.String.Empty;
                    string usageCpu = System.String.Empty;
                    string freeCpu = System.String.Empty;
                    string cpuCores = System.String.Empty;

                    #region Layout notification
                    if (view == null
                        || textView1 == null || textView2 == null || textView3 == null || textView4 == null || textView5 == null || textView6 == null || remoteViewsEmpty == null || remoteViews == null)
                    {
                        LayoutNotificationResourceInit();
                    }
                    #endregion

                    #region Notification Channel (Support ANDROID VERSION  <  Oreo)
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        NotificationChannel channel = new NotificationChannel(Info.Chanlelid, Info.Name, NotificationImportance.Default)
                        {
                            Description = Info.Description
                        };

                        notificationManager.CreateNotificationChannel(channel);
                        notificationBuilder = new Notification.Builder(this, Info.Chanlelid);
                    }
                    else
                    {
                        notificationBuilder = new Notification.Builder(this);
                    }
                    #endregion



                    if (icon == null)
                    {
                        drawableIcon = new DrawableIcon<string>();
                        icon = drawableIcon.GenerateIcon(System.String.Empty);
                    }

                    if (e != null)
                    {
                        freeRam = System.String.Concat(new string[] { "Load", ":", " ", e.UsedRamMB.ToString(), "Mb" }); ;
                        loadRam = System.String.Concat(new string[] { "Free", ":", " ", e.AvailableRamMB.ToString(), "Mb" });
                        totalRam = System.String.Concat(new string[] { "Total", ":", " ", e.TotalRamMB.ToString(), "Mb" });
                        usageCpu = System.String.Concat(new string[] { "Load", ":", " ", e.CpuUsage.ToString(), " ", "%" });
                        freeCpu = System.String.Concat(new string[] { "Free", ":", " ", e.CpuFree.ToString(), " ", "%" });
                        cpuCores = System.String.Concat(new string[] { "Cpu", ":", " ", e.CpuCores.ToString(), " ", "cores" });
                    }

                    if (settings.LayoutNotificationEmpty == false)
                    {
                        #region Empty 

                        Notification notificationEmpty = notificationBuilder
                            .SetCustomContentView(remoteViewsEmpty)
                            .SetSmallIcon(icon)
                            .SetOngoing(true)
                            .SetAutoCancel(false)
                            .Build();

                        #region Start Service (Support ANDROID VERSION  <  Oreo)
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                        {
                            StartForeground(Info.NotificationId, notificationEmpty);
                        }
                        else
                        {
                            notificationManager.Notify(Info.NotificationId, notificationEmpty);
                        }
                        #endregion

                        #endregion
                    }
                    else
                    {
                        #region Filling

                        remoteViews.SetTextViewText(textView1.Id, loadRam);
                        remoteViews.SetTextViewText(textView2.Id, freeRam);
                        remoteViews.SetTextViewText(textView3.Id, totalRam);
                        remoteViews.SetTextViewText(textView4.Id, usageCpu);
                        remoteViews.SetTextViewText(textView5.Id, freeCpu);
                        remoteViews.SetTextViewText(textView6.Id, cpuCores);

                        Notification notification = notificationBuilder
                            .SetCustomContentView(remoteViews)
                            .SetSmallIcon(icon)
                            .SetOngoing(true)
                            .SetAutoCancel(false)
                            .Build();

                        #region Start Service (Support ANDROID VERSION  <  Oreo)
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                        {
                            StartForeground(Info.NotificationId, notification);
                        }
                        else
                        {
                            notificationManager.Notify(Info.NotificationId, notification);
                        }
                        #endregion

                        #endregion
                    }
                }
            }
            catch (System.Exception ex)
            {
                #region Attempt restart
                AttemptRestart();
                #endregion

                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(NotifyService), ex);
                #endregion

            }

            finally { }
        }

        private void RemoveNotification()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    StopForeground(StopForegroundFlags.Remove);
                }
                using (NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService))
                {
                    notificationManager.Cancel(Info.NotificationId);
                }
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(NotifyService), ex);
                #endregion

            }
            finally { }
        }

        private Icon GenerateIcon(PartLoad e)
        {
            Icon icon;

            try
            {
                settings = Settings.Instance;

                drawableIconDigit = new DrawableIconDigits<long>();

                long ram = 0;

                if (settings.IsRamSelectedNotification == true)
                {
                    if (settings.IsShowFreeRam == true)
                    {
                        if (settings.IsShowRelativeRAM == true)
                        {
                            ram = (long)e.PercentAvailable;
                        }
                        if (settings.IsShowAbsoluteRAM == true)
                        {
                            ram = (long)e.AvailableRamGB;
                        }
                    }
                    else
                    {
                        if (settings.IsShowRelativeRAM == true)
                        {
                            ram = (long)e.PercentUsed;
                        }
                        if (settings.IsShowAbsoluteRAM == true)
                        {
                            ram = (long)e.UsedRamGB;
                        }
                    }
                }
                if (settings.IsCpuSelectedNotification == true)
                {
                    if (settings.IsShowFreeCpu == true)
                    {
                        ram = (long)e.CpuFree;
                    }
                    else
                    {
                        ram = (long)e.CpuUsage;
                    }
                }

                icon = drawableIconDigit.GenerateIcon(ram);

                return icon;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(NotifyService), ex);
                #endregion

                icon = drawableIconDigit.GenerateIcon(0);

                return icon;
            }
            finally { }
        }

        private void LayoutNotificationResourceInit()
        {
            try
            {
                remoteViewsEmpty = new RemoteViews(context.PackageName, Resource.Layout.layout_notification_empty);
                remoteViews = new RemoteViews(context.PackageName, Resource.Layout.layout_notification);

                layoutInflater = LayoutInflater.FromContext(context);
                view = layoutInflater.Inflate(remoteViews.LayoutId, null);

                textView1 = view.FindViewById<TextView>(Resource.Id.textView1);
                textView2 = view.FindViewById<TextView>(Resource.Id.textView2);
                textView3 = view.FindViewById<TextView>(Resource.Id.textView3);
                textView4 = view.FindViewById<TextView>(Resource.Id.textView4);
                textView5 = view.FindViewById<TextView>(Resource.Id.textView5);
                textView6 = view.FindViewById<TextView>(Resource.Id.textView6);
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(NotifyService), ex);
                #endregion

            }
            finally { }
        }

        #region Attempt restart
        private void AttemptRestart()
        {
           
            try
            {
                PendingIntent restartServicePendingIntent;
                Intent startIntent = new Intent(context, typeof(NotifyReceiver));
                startIntent.SetAction(Info.IntentActionRestartServiceRam);
                restartServicePendingIntent = PendingIntent.GetBroadcast(context, 0, startIntent, 0);
                restartServicePendingIntent.Send();
            }
            catch (System.Exception exIntent)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(NotifyService), exIntent);
                #endregion
            }
            finally { }
        }
        #endregion
    }
}
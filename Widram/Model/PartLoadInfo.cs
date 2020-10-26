using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using Android.SE.Omapi;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Java.IO;
using Java.Lang;
using Javax.Crypto.Spec;

using Widram.Helpers;
using Widram.Model;

using static Android.App.ActivityManager;
using Microsoft.AppCenter.Crashes;

namespace Widram.Model
{
    [LogRuntime("PartLoadInfo.txt", "Widram")]
    internal class PartLoadInfo : EventArgs
    {
        private Context context;

        private MemoryInfo memoryInfo;

        private ActivityManager activityManager;
        private readonly long? Percent;

        private const long bytesMB = 1048576;
        private const long bytesGB = 1073741824;

        #region Property

        //Bytes

        internal readonly long? TotalRamBytes;
        internal long? AvailableRamBytes { get; set; }
        internal long? UsedRamBytes { get; set; }

        internal long? TotalRamMB { get { return TotalRamBytes / bytesMB; } }
        internal long? AvailableRamMB { get { return AvailableRamBytes / bytesMB; } }
        internal long? UsedRamMB { get { return UsedRamBytes / bytesMB; } }

        internal long? TotalRamGB { get { return TotalRamBytes / bytesGB; } }
        internal long? AvailableRamGB { get { return AvailableRamBytes / bytesGB; } }
        internal long? UsedRamGB { get { return UsedRamBytes / bytesGB; } }

        //Percent Used - Available

        internal long? PercentAvailable { get { return AvailableRamBytes / Percent; } }
        internal long? PercentUsed { get { return UsedRamBytes / Percent; } }

        //Cpu

        internal double CpuUsage { get; set; }
        internal double CpuFree { get; set; }
        internal int CpuCores { get; set; }
        #endregion

        public PartLoadInfo()
        {
            UpdateContext();

            TotalRamBytes = memoryInfo?.TotalMem;
            Percent = TotalRamBytes / 100;
        }

        protected virtual void UpdateInfo()
        {
            UpdateContext();
        }

        internal async void UpdateContext()
        {
            await UpdateMemoryInfo();
            await UpdateCpuInfo();
        }

        private async Task UpdateMemoryInfo()
        {
            try
            {
                context = Android.App.Application.Context;

                memoryInfo = new MemoryInfo();

                activityManager = context?.GetSystemService(Context.ActivityService) as ActivityManager;

                activityManager?.GetMemoryInfo(memoryInfo);

                //Update
                AvailableRamBytes = memoryInfo?.AvailMem;
                UsedRamBytes = TotalRamBytes - AvailableRamBytes;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(PartLoadInfo), ex);
                #endregion
            }
            finally { }
        }

        private async Task UpdateCpuInfo()
        {
            try
            {
                double averageUsage = 0;
                double averageFree = 0;

                averageUsage = await AverageCpuUsage();
                averageFree = 100 - averageUsage;

                //Update
                CpuUsage = System.Math.Round(averageUsage, 1);
                CpuFree = System.Math.Round(averageFree, 1);
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(PartLoadInfo), ex);
                #endregion
            }
            finally { }
        }

        internal async Task<double> AverageCpuUsage()
        {
            try
            {
                string coresPercent = System.String.Empty;

                double countCorePercent = 0;
                double averageUsage = 0;             
                double core = 0;

                CpuCores = System.Environment.ProcessorCount;
                int coresCpu = CpuCores - 1;

                while (coresCpu > 0)
                {
                    using (RandomAccessFile scalingCurFreq = new RandomAccessFile($"/sys/devices/system/cpu/cpu{coresCpu}/cpufreq/scaling_cur_freq", "r"))
                    {
                        using (RandomAccessFile cpuInfoMaxFreq = new RandomAccessFile($"/sys/devices/system/cpu/cpu{coresCpu}/cpufreq/cpuinfo_max_freq", "r"))
                        {
                            string curfreg = await scalingCurFreq.ReadLineAsync();
                            string maxfreg = await cpuInfoMaxFreq.ReadLineAsync();

                            double currentFreq = double.Parse(curfreg) / 1000;
                            double maxFreq = double.Parse(maxfreg) / 1000;

                            core = currentFreq * 100 / maxFreq;

                            cpuInfoMaxFreq.Close();
                        }

                        scalingCurFreq.Close();
                    }

                    countCorePercent += core;

                    coresPercent += $"core number {coresCpu}: {core}%\n";

                    coresCpu--;
                }

                averageUsage = countCorePercent / 100 * System.Environment.ProcessorCount;

                return averageUsage;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(PartLoadInfo), ex);
                #endregion

                return 0;
            }
            finally { }
        }
    }
}
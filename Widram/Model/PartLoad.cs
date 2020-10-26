using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using Widram.Helpers;

namespace Widram.Model
{
    [LogRuntime("PartLoad.txt", "Widram")]
    internal class PartLoad : PartLoadInfo
    {
        public event EventHandler<PartLoad> RamUpdated;

        public System.Threading.Timer workTimer;
        public int timeMilliseconds = 3000;

        public PartLoad() { }

        internal void Start(int timeUserMilliseconds)
        {
            try
            {
                workTimer = new System.Threading.Timer((x) => UpdateInfo(), null, 0, timeUserMilliseconds);
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(PartLoad), ex);
                #endregion
            }
            finally { }
        }

        internal void Stop() => workTimer?.Dispose();

        protected override void UpdateInfo()
        {
            try
            {
                base.UpdateInfo();

                if (RamUpdated?.Target != null)
                {
                    RamUpdated(this, this);
                }
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(PartLoad), ex);
                #endregion
            }
            finally { }           
        }
    }
}
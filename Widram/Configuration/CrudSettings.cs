using Widram.Service;
using Widram.Paiting;
using Widram.Helpers;

using System;

using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using Widram.Model;
using Widram.Receivers;
using Org.Apache.Http.Conn;
using Java.IO;
using Java.Util;
using Android.Content.Res;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AppCenter.Crashes;

namespace Widram.Configuration
{
    [LogRuntime("CrudSettings.txt", "Widram")]
    internal class CrudSettings
    {
        private readonly string personalPath;
        private readonly string dataDir;
        private readonly AssetManager assetManager;

        public CrudSettings() 
        {
            try
            {
                personalPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                dataDir = String.Concat(new string[] { personalPath, "/", Info.FileNameSettings });
                assetManager = Android.App.Application.Context.Assets;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(CrudSettings), ex);
                #endregion

            }
            finally { }
        }

        internal void Write(Settings settings)
        {
            try
            {
                string jsonWrite = JsonConvert.SerializeObject(settings);

                using (StreamWriter streamWriter = new StreamWriter(dataDir))
                {
                    streamWriter.WriteLine(jsonWrite);

                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(CrudSettings), ex);
                #endregion

            }
            finally { }
 
        }

        internal Settings Read()
        {
            Settings settings;

            try
            {
                Java.IO.File file = new Java.IO.File(dataDir);

                if (file.Exists())
                {
                    settings = ReadSpecialFolder();
                }
                else
                {
                    settings = ReadDefault();
                }

                file.Dispose();

                return settings;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(CrudSettings), ex);
                #endregion


                return null;
            }
            finally { }  
        }

        internal bool Delete()
        {
            try
            {
                bool isDeleteFile = false;

                Java.IO.File file = new Java.IO.File(dataDir);
                isDeleteFile = file.Delete();

                file.Dispose();

                return isDeleteFile;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(CrudSettings), ex);
                #endregion

                return false;
            }
            finally { }          
        }
 
        /// <summary>
        /// Saved Parameters in a Special Application Folder
        /// </summary>
        /// <returns>Settings</returns>
        private Settings ReadSpecialFolder()
        {
            Settings settings;

            try
            {
                using (StreamReader streamReader = new StreamReader(dataDir))
                {
                    string jsonRead = streamReader.ReadToEnd();
                    settings = JsonConvert.DeserializeObject<Settings>(jsonRead);

                    streamReader.Close();
                    streamReader.Dispose();
                }

                return settings;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(CrudSettings), ex);
                #endregion
     
                return null;
            }
            finally { }     
        }

        /// <summary>
        /// Settings Application Default
        /// </summary>
        /// <returns>Settings</returns>
        public Settings ReadDefault()
        {
            Settings settings;

            try
            {
                using (Stream stream = assetManager.Open(Info.FileNameSettings))
                {
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        string jsonRead = streamReader.ReadToEnd();
                        settings = JsonConvert.DeserializeObject<Settings>(jsonRead);

                        streamReader.Close();
                        streamReader.Dispose();
                    }

                    stream.Close();
                    stream.Dispose();
                }

                return settings;
            }
            catch (System.Exception ex)
            {
                #region Logging
                LogRuntimeAttribute.InLogFiles(typeof(CrudSettings), ex);
                #endregion

                return null;
            }
            finally { }       
        }
    }
}
using System;
using System.Reflection;
using Java.IO;
using Microsoft.AppCenter.Crashes;
using Widram.Configuration;
using Xamarin.Essentials;

namespace Widram.Helpers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LogRuntimeAttribute : Attribute
    {
        private static Settings settings;

        public string FolderName { get; set; }
        public string FileName { get; set; }
        public string AbsolutePathStorage { get; set; }
        public string PathFolder { get; set; }
        public string PathFile { get; set; }

        public LogRuntimeAttribute(string FileName, string FolderName)
        {
            settings = Settings.Instance;

            this.FileName = FileName;
            this.FolderName = FolderName;

            AbsolutePathStorage = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            PathFolder = String.Concat(new string[] { AbsolutePathStorage, "/", FolderName });
            PathFile = String.Concat(new string[] { PathFolder, "/", FileName });
        }

        public static async void InLogFiles(Type type, Exception exception)
        {
            string pathCurrentFolder = String.Empty;
            string pathCurrentFile = String.Empty;

            settings = Settings.Instance;

            try
            {
                if (settings.IsLogErrorStorage == true)
                {
                    TypeInfo typeInfo = type.GetTypeInfo();

                    TypeAttributes typeAttributes = typeInfo.Attributes;
                    object[] attributes = typeInfo.GetCustomAttributes(true);

                    if (attributes != null)
                    {
                        foreach (var item in attributes)
                        {
                            if (item.ToString() == (typeof(LogRuntimeAttribute)).FullName)
                            {
                                LogRuntimeAttribute logRun = (LogRuntimeAttribute)item;

                                pathCurrentFolder = logRun.PathFolder;
                                pathCurrentFile = logRun.PathFile;

                                break;
                            }
                        }
                    }


                    PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

                    if (status == PermissionStatus.Granted)
                    {
                        if (pathCurrentFile != String.Empty)
                        {
                            Java.IO.File file = new Java.IO.File(pathCurrentFolder);

                            file.Mkdir();

                            file = new Java.IO.File(pathCurrentFile);
                            file.CreateNewFile();

                            FileWriter writer = new FileWriter(file);

                            writer.Write(exception.ToString());
                            writer.Flush();


                            file.Dispose();
                            writer.Close();
                            writer.Dispose();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
            }
            finally { }
        }
    }
}
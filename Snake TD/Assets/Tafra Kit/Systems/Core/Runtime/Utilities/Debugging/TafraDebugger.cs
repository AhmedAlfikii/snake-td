//This class is disabled because it's already added to TafraKit dll. It's only here for reference.
//The reason it's in a DLL instead of simply being added as a file is to prevent Unity's console from directing logs towards it,
//and instead direct it towards the script that called the debugger.
#if false
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public static class TafraDebugger
    {
        public enum LogType
        { 
            Verbose = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }
        public enum LogLevel
        { 
            None = 100,
            Verbose = 0,
            Info = 1,
            Warning = 2,
            Error = 3
        }

        //private static TafraDebuggerSettings settings;
        private static StringBuilder sb;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            //settings = TafraSettings.GetSettings<TafraDebuggerSettings>();
            sb = new StringBuilder();
        }

        public static void Log(string logger, string msg, LogType logType, Object context = null)
        {
            //if (settings != null && (int)logType < (int)settings.LogLevel)
            //    return;

            string colorTag = "";

            switch (logType)
            {
                case LogType.Verbose:
                    break;
                case LogType.Info:
                    break;
                case LogType.Warning:
                    colorTag = "<color=yellow>";
                    break;
                case LogType.Error:
                    colorTag = "<color=red>";
                    break;
            }
            string colorTagEnd = string.IsNullOrEmpty(colorTag) ? "" : "</color>";
            
            sb.Clear();

            //if (!settings.DisableColorCoding)
                sb.Append("<color=#ff313f>");

            sb.Append("<b>Tafra Kit</b>");

            //if (!settings.DisableColorCoding)
                sb.Append("</color>");
             
            sb.Append(" - ");

            //if (!settings.DisableColorCoding)
                sb.Append("<color=white>");

            sb.Append("<b>");

            sb.Append(logger);

            sb.Append("</b>");

            //if(!settings.DisableColorCoding)
                sb.Append("</color>");

            sb.Append(" - ");

            //if(!settings.DisableColorCoding)
                sb.Append(colorTag);

            sb.Append(msg);

            //if (!settings.DisableColorCoding)
                sb.Append(colorTagEnd);


            UnityEngine.Debug.Log(sb.ToString(), context);
        }
    }
}
#endif
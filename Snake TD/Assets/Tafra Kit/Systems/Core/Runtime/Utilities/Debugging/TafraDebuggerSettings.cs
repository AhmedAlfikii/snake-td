using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class TafraDebuggerSettings : SettingsModule
    {
        public TafraDebugger.LogLevel BuildLogLevel = TafraDebugger.LogLevel.Error;
        public TafraDebugger.LogLevel EditorLogLevel = TafraDebugger.LogLevel.Verbose;
        public bool DisableColorCoding = false;
        public override int Priority => 500;

        public override string Name => "General/Debugger";

        public override string Description => "Control the level of logs inside Tafra Kit (or any logs done through TafraDebugger).";

        /// <summary>
        /// Since the debugger is in a dll, it can't fetch the settings itself, so we're using the settings class to apply it's configurations to the debugger.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            TafraDebuggerSettings settings = TafraSettings.GetSettings<TafraDebuggerSettings>();

            #if UNITY_EDITOR
            TafraDebugger.SetLogLevel(settings.EditorLogLevel);
            TafraDebugger.SetTagsState(true);
            #else
            TafraDebugger.SetLogLevel(settings.BuildLogLevel);
            TafraDebugger.SetTagsState(false);
            #endif

            TafraDebugger.SetColorsState(!settings.DisableColorCoding);
        }
    }
}
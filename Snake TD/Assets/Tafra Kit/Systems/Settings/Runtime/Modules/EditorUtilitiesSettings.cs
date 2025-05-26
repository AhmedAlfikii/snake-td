using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;

namespace TafraKit
{
    public class EditorUtilitiesSettings : SettingsModule
    {
        [Tooltip("Should the PlayerPrefs be cleared eachtime the game is played in the editor?")]
        public bool ClearPrefsOnPlay;
        [Tooltip("Should the addressables group be built each time the game is being built?")]
        public bool BuildAddressablesOnBuild;

        [Header("Xcode")]
        public string[] SKAdNetworkItems = new string[] { };

        public override int Priority => 80;

        public override string Name => "Editor/Editor Utilities";

        public override string Description => "Various editor utilities that help with small tasks.";
    }
}
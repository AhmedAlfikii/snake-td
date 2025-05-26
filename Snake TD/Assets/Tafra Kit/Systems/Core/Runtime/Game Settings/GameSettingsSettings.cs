using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZUI;
using TafraKit.Internal;

namespace TafraKit
{
    public class GameSettingsSettings : SettingsModule
    {
        public bool Enabled = true;
        public GameSettingsPopup Popup;
        public bool PauseOnOpen = true;

        public override int Priority => 20;

        public override string Name => "General/Game Settings";

        public override string Description => "Allow players to control your game's settings.";
    }
}
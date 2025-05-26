using System;
using UnityEngine;

namespace TafraKit
{
    public class BasicProgressManagerSettings : SettingsModule
    {
        [Tooltip("Should progression be enabled?")]
        public bool Enabled = false;
        [Tooltip("The index of the first level's scenes in build index.")]
        public int Level1IndexInBuild = 1;
        [Tooltip("Should the game loop back to the first level if the last one was finished?")]
        public bool LoopLevels = true;
        [Tooltip("By default, the loop starts at level 1, check this if you want to adjust the loop start level (the level that the loop will go back to once it's finished).")]
        public bool ChangeLoopStartLevel;
        [Tooltip("The the level to start the loop at (note that this is not an index, it starts at level 1).")]
        public int LoopStartLevel = 1;
        [Tooltip("If the levels loop, should the player be snapped to the first new level whenever a new level is added?")]
        public bool SnapPlayerToNewLatestLevelIfLooped = true;
        [Tooltip("Should level 1 be marked as \"Opened\" the first time the game starts?")]
        public bool OpenLevel1ByDefault = true;
        [Tooltip("Mark this as false if you don't want to automatically load the current level once the splash scene is opened in case you need to initialize other systems/sdks that takes time, or if you want to handle level loading logic on your own.")]
        public bool AutoLoadAtSplash = true;
        [Tooltip("Should the next level be saved once the current level is completed? (so that if players closed the game during win screen, they would start at the next level when they open again).")]
        public bool IncreaseAtWinScreen = true;
        [Tooltip("(Editor only) Should the saved level be loaded regardless of the opened scene.")]
        public bool EditorForceLoadLatestLevel = false;
        [Tooltip("(Editor only) Enables level win/fail/replay shortcuts (Win: LeftControl+W. Fail: LeftControl+F. Replay: LeftControl+R.")]
        public bool EditorEnableShortcuts = true;

        public override string Name => "Progression/Basic Progress Manager";
        public override int Priority => 500;
        public override string Description => "Level based progression that enables you to loop levels.";
    }
}
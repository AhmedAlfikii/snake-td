using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal
{
    public class ProgressManagerSettings : SettingsModule
    {
        [SerializeField] private List<GameLevelBase> levels = new List<GameLevelBase>();

        [Header("Tracking")]
        [SerializeField] private ScriptableFloat runsCount;
        [SerializeField] private ScriptableFloat clearsCount;

        [Header("Editor")]
        [Tooltip("The index of the first level scene in the build settings. Will be used to recognize if the played scene in the editor is a level or not.")]
        [SerializeField] private int levelScenesStartBuildIndex = 2;
        [Tooltip("This level will be considered the open level if a level scene was directly played in editor.")]
        [SerializeField] private GameLevel editorLevel;

        public List<GameLevelBase> Levels => levels;
        public int LevelScenesStartBuildIndex => levelScenesStartBuildIndex;
        public GameLevel EditorLevel => editorLevel;
        public ScriptableFloat RunsCount => runsCount;
        public ScriptableFloat ClearsCount => clearsCount;

        public override string Name => "Progression/Progress Manager";
        public override string Description => "Handle how the game moves from one level to another.";
    }
}
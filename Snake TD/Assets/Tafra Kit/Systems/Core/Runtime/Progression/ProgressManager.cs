using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal;
using TafraKit.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TafraKit
{
    public static class ProgressManager
    {
        private static ProgressManagerSettings settings;
        private static GameLevel openedLevel;
        private static GameLevelsGroup openedLevelsGroup;
        private static Dictionary<GameLevel, GameLevelsGroup> groupByLevel = new Dictionary<GameLevel, GameLevelsGroup>();
        private static Dictionary<string, GameLevel> levelById = new Dictionary<string, GameLevel>();
        /// <summary>
        /// Contains the number of the level based on its position in its levels group. If it's an independent level, its number will always be depending on its position in the main levels list.
        /// </summary>
        private static Dictionary<GameLevel, int> levelNumberByLevel = new Dictionary<GameLevel, int>();
        private static HashSet<GameLevel> levelsHashSet = new HashSet<GameLevel>();
        private static LevelEndHandler activeLevelEndHandler;
        private static List<GameLevelsGroup> levelGroups = new List<GameLevelsGroup>();
        private static int runsCount;
        private static ScriptableFloat runsCountSF;
        private static ScriptableFloat clearsCountSF;

        private static UnityEvent<GameLevel, int> onLevelAboutToOpen = new UnityEvent<GameLevel, int>();
        private static UnityEvent<GameLevel, int> onLevelOpened = new UnityEvent<GameLevel, int>();
        private static UnityEvent<GameLevel, int> onLevelFreshOpen = new UnityEvent<GameLevel, int>();
        private static UnityEvent<GameLevel, int, LevelEndState> onLevelAboutToConclude = new UnityEvent<GameLevel, int, LevelEndState>();
        private static UnityEvent<GameLevel, int, LevelEndState> onLevelConcluded = new UnityEvent<GameLevel, int, LevelEndState>();

        private const string openedLevelIdSaveKey = "PROGRESS_MANAGER_OPENED_LEVEL_ID";
        private const string openedGroupIdSaveKey = "PROGRESS_MANAGER_OPENED_LEVEL_GROUP_ID";
        private const string runsCountSaveKey = "PROGRESS_MANAGER_RUNS_COUNT";

        public static bool IsInLevel => openedLevel != null;
        public static GameLevel OpenedLevel => openedLevel;
        public static bool HasSavedLevel {
            get
            {
                return !string.IsNullOrEmpty(TafraSaveSystem.LoadString(openedLevelIdSaveKey));
            }
        }
        public static List<GameLevelBase> Levels => settings.Levels;
        public static List<GameLevelsGroup> LevelGroups => levelGroups;
        public static int RunsCount => runsCount;
        /// <summary>
        /// Gets called before the level is loaded.
        /// </summary>
        public static UnityEvent<GameLevel, int> OnLevelAboutToOpen => onLevelAboutToOpen;
        public static UnityEvent<GameLevel, int> OnLevelOpened => onLevelOpened;
        public static UnityEvent<GameLevel, int> OnLevelFreshOpen => onLevelFreshOpen;
        /// <summary>
        /// Gets fired when a level is about to be won, lost or abandoned.
        /// </summary>
        public static UnityEvent<GameLevel, int, LevelEndState> OnLevelAboutToConclude => onLevelAboutToConclude;
        /// <summary>
        /// Gets fired when a level is won, lost or abandoned. Things reset by here, so if you need to access things before they reset, listen to OnLevelAboutToConclude event.
        /// </summary>
        public static UnityEvent<GameLevel, int, LevelEndState> OnLevelConcluded => onLevelConcluded;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<ProgressManagerSettings>();

            if (settings == null)
                return;

            runsCount = PlayerPrefs.GetInt(runsCountSaveKey);
           
            runsCountSF = settings.RunsCount;
            clearsCountSF = settings.ClearsCount;

            if(runsCountSF != null)
                runsCountSF.Set(runsCount);

            int savedClears = 0;

            List<GameLevelBase> levels = settings.Levels;
            for (int i = 0; i < levels.Count; i++)
            {
                var levelBase = levels[i];

                if (levelBase == null) 
                    continue;

                if (levelBase is GameLevel level)
                {
                    levelById.Add(level.ID, level);
                    levelsHashSet.Add(level);
                    levelNumberByLevel.Add(level, i + 1);
                }
                else if (levelBase is GameLevelsGroup levelGroup)
                { 
                    List<GameLevel> groupLevels = levelGroup.Levels;

                    for (int j = 0; j < groupLevels.Count; j++)
                    {
                        var groupLevel = groupLevels[j];

                        groupByLevel.Add(groupLevel, levelGroup);
                        levelById.Add(groupLevel.ID, groupLevel);
                        levelsHashSet.Add(groupLevel);
                        levelNumberByLevel.Add(groupLevel, j + 1);
                    }

                    levelGroups.Add(levelGroup);

                    savedClears += levelGroup.GetTotalClears();
                }
            }

            if (clearsCountSF != null)
                clearsCountSF.Set(savedClears);

            #if UNITY_EDITOR
            int openedSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if(openedSceneIndex >= settings.LevelScenesStartBuildIndex)
            { 
                openedLevel = settings.EditorLevel;

                bool freshStart = !TafraSaveSystem.HasKey(openedLevelIdSaveKey);

                GeneralCoroutinePlayer.StartCoroutine(FireLevelOpenedEventDelayed(openedLevel, 1, freshStart));
            }
            #endif
        }

        private static void ConcludeLevel(LevelEndState state)
        {
            int levelNumber;

            if(clearsCountSF != null && state == LevelEndState.Win)
                clearsCountSF.Add(1);

            levelNumberByLevel.TryGetValue(openedLevel, out levelNumber);

            if(activeLevelEndHandler != null)
                activeLevelEndHandler.LevelConcluded();

            onLevelConcluded?.Invoke(openedLevel, levelNumber, state);

            openedLevel = null;
            openedLevelsGroup = null;

            TafraSaveSystem.DeleteKey(openedLevelIdSaveKey);
            TafraSaveSystem.DeleteKey(openedGroupIdSaveKey);
        }

        public static void StartLevel(GameLevel level)
        {
            if (!levelsHashSet.Contains(level))
            {
                TafraDebugger.Log("Progress Manager", $"The level you're attempting to start ({level}) was not added to the Progress Manager Settings " +
                    $"as an independent level or part of a group. Please add it.", TafraDebugger.LogType.Error, level);
                return;
            }

            openedLevel = level;
            GameLevelsGroup group;

            groupByLevel.TryGetValue(level, out group);

            openedLevelsGroup = group;

            bool freshStart = !TafraSaveSystem.HasKey(openedLevelIdSaveKey);

            int levelNumber;
            
            levelNumberByLevel.TryGetValue(level, out levelNumber);

            onLevelAboutToOpen?.Invoke(level, levelNumber);

            TafraSaveSystem.SaveString(openedLevelIdSaveKey, level.ID);
                
            TafraSaveSystem.SaveString(openedGroupIdSaveKey, group != null ? group.ID : "");

            TafraSceneManager.LoadScene(level.SceneName, onLoaded: () =>
            {
                if(freshStart)
                {
                    runsCount++;

                    PlayerPrefs.SetInt(runsCountSaveKey, runsCount);

                    if(runsCountSF != null)
                        runsCountSF.Set(runsCount);
                }

                GeneralCoroutinePlayer.StartCoroutine(FireLevelOpenedEventDelayed(level, levelNumber, freshStart));
            });
        }
        public static bool StartSavedLevel()
        {
            string savedLevelId = TafraSaveSystem.LoadString(openedLevelIdSaveKey);

            if (string.IsNullOrEmpty(savedLevelId))
            {
                TafraDebugger.Log("Progress Manager", "There's no saved level, can't start one.", TafraDebugger.LogType.Error);
                return false;
            }

            if (levelById.TryGetValue(savedLevelId, out var level))
            {
                StartLevel(level);
                return true;
            }
            else
            {
                TafraDebugger.Log("Progress Manager", "Found a saved level Id, but couldn't find the level in the levels list, make sure it's assigned in the settings.", TafraDebugger.LogType.Error);
                return false;
            }
        }
        public static void WinOpenedLevel()
        {
            if (openedLevel == null)
            {
                TafraDebugger.Log("Progress Manager", "There's no opened level. Can't win.", TafraDebugger.LogType.Error);
                return;
            }

            openedLevel.ClearsCount++;

            int levelNumber;

            levelNumberByLevel.TryGetValue(openedLevel, out levelNumber);

            if(activeLevelEndHandler != null)
            {
                activeLevelEndHandler.LevelQuit(openedLevel, levelNumber, (shouldContinue) =>
                {
                    if(shouldContinue)
                    {
                        onLevelAboutToConclude?.Invoke(openedLevel, levelNumber, LevelEndState.Win);

                        ConcludeLevel(LevelEndState.Win);
                    }
                });
            }
            else
            {
                onLevelAboutToConclude?.Invoke(openedLevel, levelNumber, LevelEndState.Win);

                ConcludeLevel(LevelEndState.Win);
            }
        }
        public static void FailOpenedLevel()
        {
            if (openedLevel == null)
            {
                TafraDebugger.Log("Progress Manager", "There's no opened level. Can't fail.", TafraDebugger.LogType.Error);
                return;
            }

            int levelNumber;

            levelNumberByLevel.TryGetValue(openedLevel, out levelNumber);

            if(activeLevelEndHandler != null)
            {
                activeLevelEndHandler.LevelFailed(openedLevel, levelNumber, (shouldContinue) => 
                {
                    if(shouldContinue)
                    {
                        onLevelAboutToConclude?.Invoke(openedLevel, levelNumber, LevelEndState.Fail);

                        ConcludeLevel(LevelEndState.Fail);
                    }
                });
            }
            else
            {
                onLevelAboutToConclude?.Invoke(openedLevel, levelNumber, LevelEndState.Fail);

                ConcludeLevel(LevelEndState.Fail);
            }
        }
        public static void QuitOpenedLevel()
        {
            if (openedLevel == null)
            {
                TafraDebugger.Log("Progress Manager", "There's no opened level. Can't quit.", TafraDebugger.LogType.Error);
                return;
            }

            int levelNumber;

            levelNumberByLevel.TryGetValue(openedLevel, out levelNumber);

            if(activeLevelEndHandler != null)
            {
                activeLevelEndHandler.LevelQuit(openedLevel, levelNumber, (shouldContinue) =>
                {
                    if(shouldContinue)
                    {
                        onLevelAboutToConclude?.Invoke(openedLevel, levelNumber, LevelEndState.Quit);

                        ConcludeLevel(LevelEndState.Quit);
                    }
                });
            }
            else
            {
                onLevelAboutToConclude?.Invoke(openedLevel, levelNumber, LevelEndState.Quit);

                ConcludeLevel(LevelEndState.Quit);
            }
        }
        public static void SetLevelEndHandler(LevelEndHandler handler)
        { 
            activeLevelEndHandler = handler;
        }

        private static IEnumerator FireLevelOpenedEventDelayed(GameLevel level, int levelNumber, bool freshStart)
        {
            yield return null;

            onLevelOpened?.Invoke(level, levelNumber);

            if(freshStart)
                onLevelFreshOpen?.Invoke(level, levelNumber);
        }
    }
}
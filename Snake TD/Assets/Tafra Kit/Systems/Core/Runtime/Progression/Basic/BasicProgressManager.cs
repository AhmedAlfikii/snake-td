using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TafraKit.SceneManagement;

namespace TafraKit
{
    public static class BasicProgressManager
    {
        /// <summary>
        /// Fires whenever a level is started.
        /// </summary>
        public static UnityEvent OnLevelStarted = new UnityEvent();
        /// <summary>
        /// Fires when the current level is completed.
        /// </summary>
        public static UnityEvent OnLevelCompleted = new UnityEvent();
        public static UnityEvent OnNextLevelUnlocked = new UnityEvent();
        /// <summary>
        /// Fires when the current level is failed.
        /// </summary>
        public static UnityEvent OnLevelFailed = new UnityEvent();
        /// <summary>
        /// Fires when the current level is replayed.
        /// </summary>
        public static UnityEvent OnLevelReplayed = new UnityEvent();
        /// <summary>
        /// Fires when the player quits the current level.
        /// </summary>
        public static UnityEvent OnLevelQuit = new UnityEvent();
        /// <summary>
        /// Fires when the last level in the game is completed (if the levels are not set to loop).
        /// </summary>
        public static UnityEvent OnGameCompleted = new UnityEvent();

        private static BasicProgressManagerSettings settings;
        private static int totalLevelsCount;
        private static int loopingLevels;
        private static int latestLevelNumber = 1;
        private static int openedLevelNumber = -1;
        private static int lastOpenedLevelNumber = -1;

        /// <summary>
        /// The number of the latest level the player reached.
        /// </summary>
        public static int LatestLevelNumber
        {
            get 
            { 
                return latestLevelNumber;
            }
            private set
            {
                latestLevelNumber = value;
                PlayerPrefs.SetInt("TAFRAKIT_PROGRESSION_CUR_LEVEL_NUMBER", latestLevelNumber);
            }
        }

        /// <summary>
        /// The number of the opened level if any.
        /// </summary>
        public static int OpenedLevelNumber
        {
            get 
            { 
                return openedLevelNumber;
            }
            set
            {
                openedLevelNumber = value;
                PlayerPrefs.SetInt("TAFRAKIT_PROGRESSION_OPENED_LEVEL_NUMBER", openedLevelNumber);
            }
        }
        /// <summary>
        /// The number of the last opened level if any (use this in home screens or win/lose screens if you want to know what the opened level was since it gets reset on win/fail).
        /// </summary>
        public static int LastOpenedLevelNumber
        {
            get 
            { 
                return lastOpenedLevelNumber;
            }
            set
            {
                lastOpenedLevelNumber = value;
                PlayerPrefs.SetInt("TAFRAKIT_PROGRESSION_LAST_OPENED_LEVEL_NUMBER", lastOpenedLevelNumber);
            }
        }
        /// <summary>
        /// The number of levels in game
        /// </summary>
        public static int TotalLevelsCount
        {
            get
            {
                return totalLevelsCount;
            }
        }

        #region Initialization
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<BasicProgressManagerSettings>();

            if (!settings) return;

            if (settings.Enabled)
            {
                totalLevelsCount = SceneManager.sceneCountInBuildSettings - settings.Level1IndexInBuild;

                loopingLevels = settings.ChangeLoopStartLevel? (totalLevelsCount - settings.LoopStartLevel) + 1 : totalLevelsCount;

                int openedSceneIndex = SceneManager.GetActiveScene().buildIndex;

                latestLevelNumber = PlayerPrefs.GetInt("TAFRAKIT_PROGRESSION_CUR_LEVEL_NUMBER", 1);
                openedLevelNumber = PlayerPrefs.GetInt("TAFRAKIT_PROGRESSION_OPENED_LEVEL_NUMBER", settings.OpenLevel1ByDefault? 1 : -1);
                lastOpenedLevelNumber = PlayerPrefs.GetInt("TAFRAKIT_PROGRESSION_LAST_OPENED_LEVEL_NUMBER", 1);

                int latestLevelIndexInBuild = CalculateLevelBuildIndex(LatestLevelNumber);

                int previousTotalLevels = PlayerPrefs.GetInt("TAFRAKIT_PROGERSSION_TOTAL_LEVELS", totalLevelsCount);
                bool newLevelsAdded = totalLevelsCount > previousTotalLevels;
                bool previouslyFinishedLevels = LatestLevelNumber > previousTotalLevels;

                if (settings.SnapPlayerToNewLatestLevelIfLooped && newLevelsAdded && previouslyFinishedLevels)
                {
                    int newLevelNumber = previousTotalLevels + 1;

                    LatestLevelNumber = OpenedLevelNumber = LastOpenedLevelNumber = newLevelNumber;
                }

                if (settings.EditorForceLoadLatestLevel && openedSceneIndex != latestLevelIndexInBuild && openedSceneIndex >= settings.Level1IndexInBuild)
                    TafraSceneManager.LoadScene(latestLevelIndexInBuild);
                else if (openedSceneIndex >= settings.Level1IndexInBuild)
                {
                    //In case the first opened scene is a level scene (opened from the editor most likely), then make the opened scene the current one.
                    
                    OpenedLevelNumber = (openedSceneIndex - settings.Level1IndexInBuild) + 1;
                    LastOpenedLevelNumber = OpenedLevelNumber;
                    LatestLevelNumber = OpenedLevelNumber;
                }

                PlayerPrefs.SetInt("TAFRAKIT_PROGERSSION_TOTAL_LEVELS", totalLevelsCount);

                #if UNITY_EDITOR
                if (settings.EditorEnableShortcuts)
                {
                    GameObject pcu = new GameObject("Progress Control Utility", typeof(BasicProgressControllerUtility));
                    GameObject.DontDestroyOnLoad(pcu);
                }
                #endif

                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }
        #endregion

        #region Callbacks
        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name == "Splash" && settings.AutoLoadAtSplash)
                LoadLevel(LastOpenedLevelNumber);
            else if (scene.buildIndex >= settings.Level1IndexInBuild)
                OnLevelStarted?.Invoke();
        }
        #endregion

        #region Private Functions
        private static int CalculateLevelBuildIndex(int levelNumber)
        {
            int indexInBuild;
            if (levelNumber <= totalLevelsCount)
                indexInBuild = settings.Level1IndexInBuild + levelNumber - 1;
            else
            {
                indexInBuild = settings.Level1IndexInBuild + (levelNumber - totalLevelsCount - 1) % loopingLevels;

                if (settings.ChangeLoopStartLevel)
                    indexInBuild += settings.LoopStartLevel - 1;
            }

            return indexInBuild;
        }
        private static int GetNextLevelNumber(int nextFor)
        {
            int levelNumber = nextFor;
            if (settings.LoopLevels)
                levelNumber++;
            else
            {
                if (levelNumber < totalLevelsCount)
                    levelNumber++;
            }
            return levelNumber;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Invokes OnLevelCompleted or OnGameCompleted events so that level win or game complete screens could show up, and saves the next level as the current level in the PlayerPrefs if set to do so in the settings.
        /// </summary>
        public static void CompleteCurrentLevel()
        {
            if (OpenedLevelNumber < 0)
            {
                TafraDebugger.Log("Progress Manager", "Can't complete current level because no level is opened.", TafraDebugger.LogType.Warning);
                return;
            }

            int tempLatestLevelNumber = LatestLevelNumber;
            int newLevelNumber = GetNextLevelNumber(LastOpenedLevelNumber);
            if (newLevelNumber > tempLatestLevelNumber)
            {
                //Save next level.
                if (settings.IncreaseAtWinScreen)
                {
                    //If the won level is the latest level, then the next level is now the lateset level.
                    if (LastOpenedLevelNumber == LatestLevelNumber)
                        LatestLevelNumber = newLevelNumber;

                    LastOpenedLevelNumber = newLevelNumber;
                }   
            }
            else
            {
                //No more levels, game completed.
                OnGameCompleted?.Invoke();
            }

            OnLevelCompleted?.Invoke();
            ResetOpenedLevelNumber();
        }
        public static void UnlockNextLevel()
        {
            int tempLatestLevelNumber = LatestLevelNumber;
            
            int newLevelNumber = GetNextLevelNumber(LastOpenedLevelNumber);

            if (newLevelNumber > tempLatestLevelNumber)
            {
                //Save next level.
                //If the won level is the latest level, then the next level is now the latest leve.
                //if (LastOpenedLevelNumber == LatestLevelNumber)
                
                LatestLevelNumber = newLevelNumber;

                LastOpenedLevelNumber = newLevelNumber;
                
                OnNextLevelUnlocked?.Invoke();
            }

            ResetOpenedLevelNumber();
        }
        /// <summary>
        /// Invokes the OnLevelFailed event.
        /// </summary>
        public static void FailCurrentLevel()
        {
            if (OpenedLevelNumber < 0)
            {
                TafraDebugger.Log("Progress Manager", "Can't fail current level because no level is opened.", TafraDebugger.LogType.Warning);
                return;
            }

            OnLevelFailed?.Invoke();

            ResetOpenedLevelNumber();
        }

        /// <summary>
        /// Loads a level.
        /// </summary>
        public static void LoadLevel(int levelNumber)
        {
            TafraSceneManager.LoadScene(CalculateLevelBuildIndex(levelNumber));

            OpenedLevelNumber = levelNumber;
            LastOpenedLevelNumber = levelNumber;
        }
        /// <summary>
        /// Meant to be called from the level win screen. Loads the next level and saves it as the latest level.
        /// </summary>
        public static void LoadNextLevel()
        {
            if (!settings.IncreaseAtWinScreen)
            {
                //If the won level is the latest level, then the next level is now the lateset level.
                if (LastOpenedLevelNumber == LatestLevelNumber)
                    LatestLevelNumber = GetNextLevelNumber(LastOpenedLevelNumber);

                LoadLevel(GetNextLevelNumber(LastOpenedLevelNumber));
            }
            else
                LoadLevel(LastOpenedLevelNumber);
        }

        /// <summary>
        /// Reloads the latest level the player reached.
        /// </summary>
        public static void ReplayCurrentLevel()
        {
            if (!IsCurrentSceneALevel())
            {
                TafraDebugger.Log("Progress Manager", "Can't replay current level because no level is opened.", TafraDebugger.LogType.Warning);
                return;
            }

            //Recalculate the last opened level since it gets assinged the next level number in case of level won...
            //...(which will result in playing the next level if replay was requested on win screen).
            //LastOpenedLevelNumber = 1 + (SceneManager.GetActiveScene().buildIndex - settings.Level1IndexInBuild);
            OpenedLevelNumber = LastOpenedLevelNumber;

            OnLevelReplayed?.Invoke();

            TafraSceneManager.LoadScene(CalculateLevelBuildIndex(LastOpenedLevelNumber));
        }

        /// <summary>
        /// Quits the current level (fires the level quit event).
        /// </summary>
        public static void QuitCurrentLevel()
        {
            OnLevelQuit?.Invoke(); 
        }

        /// <summary>
        /// Load the home scene and resets the opened level number.
        /// </summary>
        public static void GoToHome()
        {
            TafraSceneManager.LoadScene(1);
            ResetOpenedLevelNumber();
        }
        /// <summary>
        /// Clear saved opened level number (this automatically happens when the level is won or lost, it doesn't happen in other cases like...
        /// ...where the player closes the game during a level, or quits a level through a pause screen, etc...
        /// ...so you have to manually reset it if any of those cases happened. You might want to use this on next game launch to load that level, or simply clear it if you don't want to load it.
        /// </summary>
        public static void ResetOpenedLevelNumber()
        {
            OpenedLevelNumber = -1;
        }

        /// <summary>
        /// Returns the build index of the latest level the player reached.
        /// </summary>
        /// <returns></returns>
        public static int GetLatestLevelIndexInBuild()
        {
            return CalculateLevelBuildIndex(LatestLevelNumber);
        }

        /// <summary>
        /// Returns the build index of the opened level if any.
        /// </summary>
        /// <returns></returns>
        public static int GetOpenedLevelIndexInBuild()
        {
            if (OpenedLevelNumber == -1) return -1;

            return CalculateLevelBuildIndex(OpenedLevelNumber);
        }

        /// <summary>
        /// Returns the build index of the last opened level if any (use this in win/lose screens if you want to know what the opened level was since it gets reset on win/fail).
        /// </summary>
        /// <returns></returns>
        public static int GetLastOpenedLevelIndexInBuild()
        {
            if (LastOpenedLevelNumber == -1) return -1;

            return CalculateLevelBuildIndex(LastOpenedLevelNumber);
        }

        public static int GetFirstLevelIndexInBuild()
        {
            return settings.Level1IndexInBuild;
        }

        public static bool IsCurrentSceneALevel()
        {
            int activeSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

            return activeSceneBuildIndex >= settings.Level1IndexInBuild && activeSceneBuildIndex < settings.Level1IndexInBuild + totalLevelsCount;
        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal
{
    public class LevelEndHandler : MonoBehaviour, IResettable
    {
        [Tooltip("Modules will be processed one by one in order.")]
        [SerializeReferenceListContainer("modules", true, "Module", "Modules")]
        [SerializeField] private LevelEndModulesContainer levelEndModules;

        private List<LevelEndModule> winEndModules = new List<LevelEndModule>();
        private List<LevelEndModule> failEndModules = new List<LevelEndModule>();
        private List<LevelEndModule> quitEndModules = new List<LevelEndModule>();

        private bool interruptLevelWin;
        private bool interruptLevelFail;
        private bool interruptLevelQuit;

        private void Awake()
        {
            ProgressManager.SetLevelEndHandler(this);

            for (int i = 0; i < levelEndModules.Modules.Count; i++)
            {
                var module = levelEndModules.Modules[i];

                if (module.OnWin)
                    winEndModules.Add(module);
                else if (module.OnFail)
                    failEndModules.Add(module);
                else if (module.OnQuit)
                    quitEndModules.Add(module);

                module.Initialize(this);
            }
        }

        private IEnumerator ProcessModulesList(List<LevelEndModule> modules, Action onEnd)
        {
            for(int i = 0; i < modules.Count; i++)
            {
                var module = modules[i];

                bool completed = false;

                module.Start(() => {
                    completed = true;
                });

                while(!completed)
                    yield return null;
            }

            onEnd?.Invoke();
        }

        public void LevelWon(GameLevel level, int levelNumber, Action<bool> onEnd)
        {
            interruptLevelWin = false;

            StartCoroutine(ProcessModulesList(winEndModules, () => 
            {
                onEnd?.Invoke(!interruptLevelWin);
            }));
        }
        public void LevelFailed(GameLevel level, int levelNumber, Action<bool> onEnd)
        {
            interruptLevelFail = false;
           
            StartCoroutine(ProcessModulesList(failEndModules, () =>
            {
                onEnd?.Invoke(!interruptLevelFail);
            }));
        }
        public void LevelQuit(GameLevel level, int levelNumber, Action<bool> onEnd)
        {
            interruptLevelQuit = false;
         
            StartCoroutine(ProcessModulesList(quitEndModules, () =>
            {
                onEnd?.Invoke(!interruptLevelQuit);
            }));
        }
        public void LevelConcluded()
        {
            ResetSavedData();
        }
        public void InterruptLevelWin()
        {
            interruptLevelWin = true;
        }
        public void InterruptLevelFail()
        {
            interruptLevelFail = true;
        }
        public void InterruptLevelQuit()
        {
            interruptLevelQuit = true;
        }

        public void ResetSavedData()
        {
            for (int i = 0; i < levelEndModules.Modules.Count; i++)
            {
                levelEndModules.Modules[i].ResetSavedData();
            }
        }
    }
}
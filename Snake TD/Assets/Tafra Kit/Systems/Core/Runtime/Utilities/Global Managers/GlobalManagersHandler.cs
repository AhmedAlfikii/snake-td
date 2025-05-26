using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal
{
    public static class GlobalManagersHandler
    {
        private static GlobalManagersHandlerSettings settings;
        private static Dictionary<string, GlobalManager> spawnedManagersById = new Dictionary<string, GlobalManager>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<GlobalManagersHandlerSettings>();

            if(settings == null)
                return;

            var managers = settings.Managers;

            if(managers.Count == 0)
                return;

            for (int i = 0; i < managers.Count; i++)
            {
                var manager = managers[i];

                GlobalManager prefab = manager.Load();

                string managerId = prefab.ID;

                if(string.IsNullOrEmpty(managerId))
                {
                    TafraDebugger.Log("Global Managers Handler", $"Manager ids can't be null ({prefab.name}).", TafraDebugger.LogType.Error, prefab);
                    continue;
                }

                if(spawnedManagersById.ContainsKey(managerId))
                {
                    TafraDebugger.Log("Global Managers Handler", $"A manager with the same id ({managerId}) already exists in the list, manager ids should be unique.", TafraDebugger.LogType.Error, prefab);
                    continue;
                }

                //Spawn the manager, and it will register itself.
                GameObject.Instantiate(prefab);
            }
        }

        public static bool RegisterManager(GlobalManager manager)
        {
            bool registered = spawnedManagersById.TryAdd(manager.ID, manager);

            if(!registered)
                return false;

            GameObject.DontDestroyOnLoad(manager.gameObject);

            return true;
        }
    }
}
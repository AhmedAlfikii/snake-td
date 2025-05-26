using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TafraKit
{
    public static class TafraPooler
    {
        private static Dictionary<int, object> localPools = new Dictionary<int, object>();
        private static Dictionary<int, object> globalPools = new Dictionary<int, object>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            localPools.Clear();
        }

        /// <summary>
        /// Creates a pool using the given unit, if the pool was created before, it returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolID"></param>
        /// <param name="unit"></param>
        /// <param name="isLocal">Should this pool be bound to the scene (most pools shoul be)? If not, the objects spawned in the pool will not be destroyed on scene load, use with care.</param>
        public static DynamicPool<T> CreateOrGetPool<T>(int poolID, T unit, bool isLocal)
        {
            if (!isLocal)
                Debug.LogError("GLOBAL POOLS ARE NOT SUPPORTED YET!");

            var poolsToLookIn = isLocal ? localPools : globalPools;

            if (!poolsToLookIn.TryGetValue(poolID, out object existingPool))
            {
                DynamicPool<T> newPool = new DynamicPool<T>();

                T sampleUnit = unit;

                if (unit is GameObject)
                {
                    GameObject mainUnitGO = unit as GameObject;

                    mainUnitGO.SetActive(false);
                    GameObject unitGO = GameObject.Instantiate(mainUnitGO);
                    mainUnitGO.SetActive(true);

                    sampleUnit = (T)Convert.ChangeType(unitGO, typeof(T));
                }
                else if (unit is Component)
                {
                    Component compnent = unit as Component;
                    GameObject mainUnitGO = compnent.gameObject;

                    mainUnitGO.SetActive(false);
                    GameObject unitGO = GameObject.Instantiate(compnent.gameObject);
                    mainUnitGO.SetActive(true);

                    sampleUnit = unitGO.GetComponent<T>();
                }

                newPool.AddUnit(sampleUnit);
                newPool.Initialize();

                poolsToLookIn.Add(poolID, newPool);

                return newPool;
            }
            else
                return existingPool as DynamicPool<T>;
        }

        /// <summary>
        /// Returns a pool from the local or the global pools depending on where the give pool was constructed 
        /// (use GetLocalPool or GetGlobalPool if you know what kind of pool you want to save the checks).
        /// </summary>
        /// <param name="poolID"></param>
        /// <returns></returns>
        public static DynamicPool<T> GetPool<T>(int poolID)
        {
            if (localPools.TryGetValue(poolID, out var localPool))
                return localPool as DynamicPool<T>;
            else if (globalPools.TryGetValue(poolID, out var globalPool))
                return globalPool as DynamicPool<T>;

            return null;
        }
        /// <summary>
        /// Returns a pool the local pools. Use this if you know the pool you're looking for is in the local pool.
        /// </summary>
        /// <param name="poolID"></param>
        /// <returns></returns>
        public static DynamicPool<T> GetLocalPool<T>(int poolID)
        {
            if (localPools.TryGetValue(poolID, out var localPool))
                return localPool as DynamicPool<T>;

            return null;
        }
        /// <summary>
        /// Returns a pool the global pools. Use this if you know the pool you're looking for is in the global pool.
        /// </summary>
        /// <param name="poolID"></param>
        /// <returns></returns>
        public static DynamicPool<T> GetGlobalPool<T>(int poolID)
        {
            if (globalPools.TryGetValue(poolID, out var globalPool))
                return globalPool as DynamicPool<T>;

            return null;
        }

        public static void DestroyPool<T>(int poolID, bool dontDestroyTakenUnits = false)
        {
            if (localPools.TryGetValue(poolID, out var localPool))
            {
                (localPool as DynamicPool<T>).Uninitialize(dontDestroyTakenUnits);
                localPools.Remove(poolID);
            }
            else if (globalPools.TryGetValue(poolID, out var globalPool))
            {
                (globalPool as DynamicPool<T>).Uninitialize(dontDestroyTakenUnits);
                globalPools.Remove(poolID);
            }
        }
        public static void DestroyLocalPool<T>(int poolID, bool dontDestroyTakenUnits = false)
        {
            if (localPools.TryGetValue(poolID, out var localPool))
            {
                (localPool as DynamicPool<T>).Uninitialize(dontDestroyTakenUnits);
                localPools.Remove(poolID);
            }
        }
        public static void DestroyGlobalPool<T>(int poolID, bool dontDestroyTakenUnits = false)
        {
            if (globalPools.TryGetValue(poolID, out var globalPool))
            {
                (globalPool as DynamicPool<T>).Uninitialize(dontDestroyTakenUnits);
                globalPools.Remove(poolID);
            }
        }
    }
}
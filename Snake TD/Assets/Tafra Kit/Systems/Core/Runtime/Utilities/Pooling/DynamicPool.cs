using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    /// <summary>
    /// A dynamic pool of gameobjects or components (that are attached to gameobjects).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DynamicPool<T>
    {

        [SerializeField] private List<T> pool = new List<T>();

        [Space()]

        [Tooltip("OPTIONAL: the transform that is the parent of the pool units. (New units will be instantiated under the holder)")]
        [SerializeField] private Transform poolHolder;
        [Tooltip("Should the pool expand (create new units) if a unit was requested and no free units were found?")]
        [SerializeField] private bool flexibleSize = true;
        [Tooltip("The maximum number of units the pool can expand to (0 means infinite)")]
        [SerializeField] private int sizeLimit = 0;
        [Tooltip("Should the units be deactivated while they're inside the pool?")]
        [SerializeField] private bool deactivateUnitsInPool = true;

        public UnityEvent<T> OnNewUnitCreated = new UnityEvent<T>();

        public bool IsInitialized => isInitialized;
        /// <summary>
        /// The parent transform of the pool's units (can be null).
        /// </summary>
        public Transform Holder { get => poolHolder; set => poolHolder = value; }

        private bool isInitialized;
        private int curPoolMaxSize;
        private List<T> takenUnits = new List<T>();             //A list of the units that are taken (unavailble).
        private GameObject sampleUnit;
        private T sampleCustomClassUnit;
        private bool isTypeDetermined;
        private bool isGameObject;
        private bool isComponent;
        private bool isGeneralObject;

        public void Construct(List<T> pool, Transform poolHolder, bool flexibleSize = true, int sizeLimit = 0, bool deactivateUnitsInPool = true)
        {
            this.pool = pool;
            this.poolHolder = poolHolder;
            this.flexibleSize = flexibleSize;
            this.sizeLimit = sizeLimit;
            this.deactivateUnitsInPool = deactivateUnitsInPool;
        }

        public void Initialize()
        {
            if (isInitialized)
            {
                TafraDebugger.Log("Dynamic Pool", "The pool is alreayd initialized, no need to do it again.", TafraDebugger.LogType.Info);
                return;
            }

            #region Type Check
            if (!isTypeDetermined)
            {
                Type type = typeof(T);

                if (type == typeof(GameObject))
                {
                    isGameObject = true;
                    isComponent = false;
                    isGeneralObject = false;
                }
                else if (type == typeof(Component) || type.IsSubclassOf(typeof(Component)))
                {
                    isGameObject = false;
                    isComponent = true;
                    isGeneralObject = false;
                }
                else
                {
                    isGameObject = false;
                    isComponent = false;
                    isGeneralObject = true;
                }

                isTypeDetermined = true;
            }
            #endregion

            #region Error Checks
            if (pool.Count == 0)
            {
                TafraDebugger.Log("Dynamic Pool", "The pool is empty. Make sure there's at least one unit inside the pool before initializing it.", TafraDebugger.LogType.Verbose);
                return;
            }
            #endregion

            #region Deactivate Unit GameObjects If Required
            if (deactivateUnitsInPool && !isGeneralObject)
            {
                for (int i = 0; i < pool.Count; i++)
                {
                    GameObject go = GetUnitGameObject(pool[i]);

                    if (go != null && go.activeSelf)
                        go.SetActive(false);
                }
            }
            #endregion

            #region Create Sample Unit If Flexible Size
            if (flexibleSize)
            {
                if(!isGeneralObject)
                {
                    GameObject unitGO = GetUnitGameObject(pool[0]);

                    sampleUnit = GameObject.Instantiate(unitGO, poolHolder);
                }
                else
                {
                    sampleCustomClassUnit = (T)Activator.CreateInstance(pool[0].GetType());
                }
            }
            #endregion

            curPoolMaxSize = pool.Count;

            isInitialized = true;
        }
        /// <summary>
        /// Destroy all the units inside the pool and outside of it, including the sample unit, and reset the pool.
        /// </summary>
        public void Uninitialize(bool dontDestroyTakenUnits = false)
        {
            if (!isInitialized)
                return;

            for (int i = 0; i < pool.Count; i++)
            {
                if (isGameObject)
                    GameObject.Destroy(pool[i] as GameObject);
                else if (isComponent)
                {
                    Component compnent = pool[i] as Component;
                    if (compnent != null)
                        GameObject.Destroy(compnent.gameObject);
                }
            }
            if (!dontDestroyTakenUnits)
            {
                for (int i = 0; i < takenUnits.Count; i++)
                {
                    if (isGameObject)
                        GameObject.Destroy(takenUnits[i] as GameObject);
                    else if (isComponent)
                    {
                        Component compnent = takenUnits[i] as Component;
                        if (compnent != null)
                            GameObject.Destroy(compnent.gameObject);
                    }
                }
            }

            pool.Clear();
            takenUnits.Clear();
            
            if (sampleUnit)
            {
                UnityEngine.Object.Destroy(sampleUnit);
                sampleUnit = null;
            }

            if(sampleCustomClassUnit != null)
                sampleCustomClassUnit = default;

            isInitialized = false;
        }

        /// <summary>
        /// Returns a unit from the pool if available. And marks that unit as unavailable for later requests, until released.
        /// </summary>
        /// <param name="newParent">New transform to make the unit a child of.</param>
        /// <param name="activateUnit">Should the unit's gameobject be activated if it's not?</param>
        /// <returns></returns>
        public T RequestUnit(Transform newParent = null, bool activateUnit = true, bool forceUnparent = false)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Dynamic Pool", "Th pool isn't initialized, can't request a unit.", TafraDebugger.LogType.Error);
                return default;
            }

            if (pool.Count > 0)
            {
                T unit = pool[0];

                pool.RemoveAt(0);

                takenUnits.Add(unit);

                if (!isGeneralObject)
                {
                    GameObject unitGO = GetUnitGameObject(unit);

                    if (newParent != null || forceUnparent)
                        unitGO.transform.SetParent(newParent, false);

                    if (activateUnit && !unitGO.gameObject.activeSelf)
                        unitGO.gameObject.SetActive(true);
                }

                return unit;
            }
            else if (flexibleSize && (sizeLimit == 0 || curPoolMaxSize < sizeLimit))
            {
                ExpandPool(1);

                return RequestUnit(newParent, activateUnit, forceUnparent);
            }
            else
            {
                TafraDebugger.Log("Dynamic Pool", "Th pool doesn't contain any available units.", TafraDebugger.LogType.Info);
                return default;
            }
        }

        /// <summary>
        /// Returns a list of units from the pool if available. And marks those units as unavailable for later requests, until released.
        /// </summary>
        /// <param name="unitsNeeded">The needed number of units.</param>
        /// <param name="newParent">New transform to make the units a child of.</param>
        /// <param name="activateUnit">Should the units' gameobjects be activated if they're not?</param>
        /// <returns></returns>
        public List<T> RequestUnits(int unitsNeeded, Transform newParent = null, bool activateUnit = true)
        {
            int diff = unitsNeeded - pool.Count;

            if (diff <= 0)
            {
                List<T> units = new List<T>();

                for (int i = 0; i < unitsNeeded; i++)
                {
                    T unit = pool[0];

                    pool.RemoveAt(0);

                    takenUnits.Add(unit);

                    if (!isGeneralObject)
                    {
                        GameObject unitGO = GetUnitGameObject(unit);

                        if (newParent != null)
                            unitGO.transform.SetParent(newParent, false);

                        if (activateUnit && !unitGO.gameObject.activeSelf)
                            unitGO.gameObject.SetActive(true);
                    }

                    units.Add(unit);
                }

                return units;
            }
            else if (flexibleSize)
            {
                if (sizeLimit == 0)
                    ExpandPool(diff);
                else
                {
                    int maxUnitsToAdd = sizeLimit - curPoolMaxSize;

                    if (maxUnitsToAdd <= 0)
                        return new List<T>();

                    if (unitsNeeded > maxUnitsToAdd + pool.Count)
                    {
                        unitsNeeded = maxUnitsToAdd + pool.Count;
                        diff = unitsNeeded - curPoolMaxSize;
                    }

                    if (diff > 0)
                        ExpandPool(diff);
                }

                return RequestUnits(unitsNeeded, newParent, activateUnit);
            }
            else
            {
                TafraDebugger.Log("Dynamic Pool", "Th pool doesn't contain any available units.", TafraDebugger.LogType.Verbose);
                return new List<T>();
            }
        }

        /// <summary>
        /// Release a preivously requested unit so that it can be requested again.
        /// </summary>
        /// <param name="unit">The unit to release.</param>
        /// <param name="reparent">Should the unit be reparented to the pool's units holder?</param>
        public bool ReleaseUnit(T unit, bool reparent = true)
        {
            int unitIndex = takenUnits.IndexOf(unit);

            if(unitIndex > -1)
            {
                pool.Add(takenUnits[unitIndex]);

                takenUnits.RemoveAt(unitIndex);

                if (!isGeneralObject)
                {
                    if(reparent || deactivateUnitsInPool)
                    {
                        GameObject unitGO = GetUnitGameObject(unit);

                        if(deactivateUnitsInPool && unitGO.activeSelf)
                            unitGO.SetActive(false);

                        if (reparent)
                            unitGO.transform.SetParent(poolHolder, false);
                    }
                }
                return true;
            }
            else
            {
                TafraDebugger.Log("Dynamic Pool", "The unit you're trying to release is not in the taken units list.", TafraDebugger.LogType.Verbose);
                return false;
            }
        }

        /// <summary>
        /// Release preivously requested units so that they can be requested again.
        /// </summary>
        /// <param name="units">The unit to release.</param>
        /// <param name="reparent">Should the unit be reparented to the pool's units holder?</param>
        public void ReleaseUnits(List<T> units, bool reparent = true)
        {
            for (int i = 0; i < units.Count; i++)
            {
                int unitIndex = takenUnits.IndexOf(units[i]);

                if (unitIndex > -1)
                {
                    pool.Add(takenUnits[unitIndex]);

                    takenUnits.RemoveAt(unitIndex);

                    if (!isGeneralObject)
                    {
                        if (reparent || deactivateUnitsInPool)
                        {
                            GameObject unitGO = GetUnitGameObject(units[i]);

                            if (deactivateUnitsInPool && unitGO.activeSelf)
                                unitGO.SetActive(false);

                            if (reparent)
                                unitGO.transform.SetParent(poolHolder, false);
                        }
                    }
                }
                else
                    TafraDebugger.Log("Dynamic Pool", "On of the units you're trying to release is not in the taken units list.", TafraDebugger.LogType.Verbose);
            }
        }

        /// <summary>
        /// Returns a list of all the units inside the pool and outside of it without taking them from the pool.
        /// </summary>
        /// <returns></returns>
        public List<T> GetAllPoolUnits()
        {
            List<T> units = new List<T>();
            
            units.AddRange(pool);
            units.AddRange(takenUnits);

            return units;
        }

        /// <summary>
        /// Returns a list of all the units that are currently taken from the pool.
        /// </summary>
        /// <returns></returns>
        public List<T> GetAllTakenUnits()
        {
            return takenUnits;
        }
        /// <summary>
        /// Returns a list of all the units that are not yet taken from the pool.
        /// </summary>
        /// <returns></returns>
        public List<T> GetAllRemainingUnits()
        {
            return pool;
        }

        public GameObject GetSampleUnit()
        {
            return sampleUnit;
        }
        /// <summary>
        /// This is meant to be used for reference purpose only. Do not make adjustments to this unit.
        /// </summary>
        /// <returns></returns>
        public T GetUnitInstance()
        {
            if (pool.Count > 0)
                return pool[0];
            else if (takenUnits.Count > 0)
                return takenUnits[0];

            return default;
        }
        public void AddUnit(T unit)
        {
            pool.Add(unit);
        }
        /// <summary>
        /// Expands the pool by one unit.
        /// </summary>
        /// <param name="newUnits">The number of units to add to the pool.</param>
        private void ExpandPool(int newUnits)
        {
            for (int i = 0; i < newUnits; i++)
            {
                T unit = default;

                if (!isGeneralObject)
                {
                    GameObject newUnitGO = GameObject.Instantiate(sampleUnit, poolHolder);

                    if (isGameObject)
                        unit = (T)Convert.ChangeType(newUnitGO, typeof(T));
                    else if (isComponent)
                        unit = newUnitGO.GetComponent<T>();
                }
                else
                    unit = (T)Activator.CreateInstance(sampleCustomClassUnit.GetType());

                pool.Add(unit);

                OnNewUnitCreated?.Invoke(unit);
            }

            curPoolMaxSize += newUnits;
        }

        private GameObject GetUnitGameObject(T unit)
        {
            GameObject unitGO = null;

            if (!isGeneralObject)
            {
                if(isGameObject)
                    unitGO = (unit as GameObject);
                else if(isComponent)
                {
                    Component compnent = unit as Component;

                    if(compnent != null)
                        unitGO = compnent.gameObject;
                    else
                        Debug.LogError("A game breaking error should've occured here, let Ziad know.");
                }
            }

            return unitGO;
        }
    }
}
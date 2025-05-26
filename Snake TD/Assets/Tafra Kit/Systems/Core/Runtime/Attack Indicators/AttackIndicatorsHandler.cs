using System;
using System.Collections.Generic;
using TafraKit;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TafraKit
{
    public class AttackIndicatorsHandler : MonoBehaviour
    {
        #region Public Fields
        public static AttackIndicatorsHandler Instance;
        #endregion

        #region Private Serialized Fields
        [SerializeField] private List<AttackIndicator> attackIndicatorPrefabs = new List<AttackIndicator>();
        #endregion

        #region private Fields
        private readonly Dictionary<Type, DynamicPool<AttackIndicator>> poolByType = new Dictionary<Type, DynamicPool<AttackIndicator>>();
        private readonly Dictionary<Type, List<AttackIndicator>> spawnedUnitsByType = new Dictionary<Type, List<AttackIndicator>>();
        #endregion

        #region Monobehaviour Messages
        private void Awake()
        {
            Instance = this;

            for(int i = 0; i < attackIndicatorPrefabs.Count; i++)
            {
                DynamicPool<AttackIndicator> pool = new DynamicPool<AttackIndicator>();
                AttackIndicator unit = Instantiate(attackIndicatorPrefabs[i], transform);
                pool.Construct(new List<AttackIndicator>() { unit }, transform);
                pool.Initialize();
                poolByType.Add(unit.GetType(), pool);
                spawnedUnitsByType.Add(unit.GetType(), new List<AttackIndicator>());
            }
        }
        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnLoaded;
        }
        #endregion

        #region Public Functions
        public TAttackIndicator RequestAttackIndicator<TAttackIndicator>(AttackIndicatorData data) where TAttackIndicator : AttackIndicator
        {
            TAttackIndicator indicator = poolByType[typeof(TAttackIndicator)].RequestUnit() as TAttackIndicator;

            spawnedUnitsByType[indicator.GetType()].Add(indicator);

            indicator.Initialize(data);

            indicator.Show();

            return indicator;
        }
        public void ReleaseAttackIndicator(AttackIndicator indicator)
        {
            indicator.Hide(false, () =>
            {
                poolByType[indicator.GetType()].ReleaseUnit(indicator);

                spawnedUnitsByType[indicator.GetType()].Remove(indicator);
            });
        }
        #endregion

        #region Private Functions
        private void ReleaseAllIndicators()
        {
            for(int i = 0; i < attackIndicatorPrefabs.Count; i++)
            {
                if(!spawnedUnitsByType.TryGetValue(attackIndicatorPrefabs[i].GetType(), out List<AttackIndicator> indicators)) continue;

                for(int j = 0; j < indicators.Count; j++)
                    ReleaseAttackIndicator(indicators[j]);
            }
        }
        #endregion

        #region Callbacks
        private void OnSceneUnLoaded(Scene scene)
        {
            ReleaseAllIndicators();
        }
        #endregion
    }
}
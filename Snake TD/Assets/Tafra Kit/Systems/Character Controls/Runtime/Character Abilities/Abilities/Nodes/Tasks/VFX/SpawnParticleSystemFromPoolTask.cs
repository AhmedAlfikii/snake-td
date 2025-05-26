using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;
using System;
using Unity.VisualScripting;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/VFX/Spawn Particles - Pool"), GraphNodeName("Spawn Particles - Pool")]
    public class SpawnParticleSystemFromPoolTask : AbilityTaskNode
    {
        [SerializeField] private BlackboardObjectGetter particleSystem;
        [SerializeField] private BlackboardObjectSetter storageProperty;
        [SerializeField] private BlackboardDynamicPointGetter spawnPoint;
        [Tooltip("How long after a unit is spawned should it go back to the pool?")]
        [SerializeField] private float releaseAfter = 5;

        [NonSerialized] private DynamicPool<ParticleSystem> pool;
        [NonSerialized] private HashSet<IEnumerator> runningReleaseEnums = new HashSet<IEnumerator>();

        public SpawnParticleSystemFromPoolTask(SpawnParticleSystemFromPoolTask other) : base(other)
        {
            particleSystem = new BlackboardObjectGetter(other.particleSystem);
            storageProperty = new BlackboardObjectSetter(other.storageProperty);
            spawnPoint = new BlackboardDynamicPointGetter(other.spawnPoint);
            pool = other.pool;
        }
        public SpawnParticleSystemFromPoolTask()
        {

        }

        protected override void OnInitialize()
        {
            particleSystem.Initialize(ability.BlackboardCollection);
            storageProperty.Initialize(ability.BlackboardCollection);
            spawnPoint.Initialize(ability.BlackboardCollection);

            UnityEngine.Object psObject = particleSystem.Value;

            ParticleSystem psPrefab;
            if(psObject is ParticleSystem ps)
                psPrefab = ps;
            else
                psPrefab = psObject.GetComponent<ParticleSystem>();

            psPrefab.gameObject.SetActive(false);

            ParticleSystem psInstance = GameObject.Instantiate(psPrefab);

            psPrefab.gameObject.SetActive(true);

            pool = new DynamicPool<ParticleSystem>();
            pool.Construct(new List<ParticleSystem>(1) { psInstance }, null, true, 0, true);
            pool.Initialize();
        }
        protected override void OnTriggerBlackboardSet()
        {
            particleSystem.SetSecondaryBlackboard(triggerBlackboard);
            storageProperty.SetSecondaryBlackboard(triggerBlackboard);
            spawnPoint.SetSecondaryBlackboard(triggerBlackboard);
        }
        protected override BTNodeState OnUpdate()
        {
            ParticleSystem ps = pool.RequestUnit(activateUnit: false);

            ps.transform.position = spawnPoint.Value;
            ps.gameObject.SetActive(true);

            IEnumerator releaseAfter = null;

            releaseAfter = ReleaseAfterDelay(ps, () => 
            {
                runningReleaseEnums.Remove(releaseAfter);
            });

            runningReleaseEnums.Add(releaseAfter);

            characterAbilities.StartCoroutine(releaseAfter);

            return BTNodeState.Success;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            runningReleaseEnums.Clear();
            pool.Uninitialize();
        }
        protected IEnumerator ReleaseAfterDelay(ParticleSystem ps, Action onEnd)
        {
            yield return Yielders.GetWaitForSeconds(releaseAfter);

            pool.ReleaseUnit(ps);

            onEnd?.Invoke();
        }

        protected override BTNode CloneContent()
        {
            SpawnParticleSystemFromPoolTask clonedNode = new SpawnParticleSystemFromPoolTask(this);

            return clonedNode;
        }
    }
}
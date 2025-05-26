using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Healthies;
using TafraKit.Weaponry;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Combat/Projectiles/Straight Projectile"), GraphNodeName("Straight Projectile", "Projectile")]
    public class StraightProjectileTask : TaskNode
    {
        [SerializeField] private BlackboardObjectGetter projectile;
        [SerializeField] private BlackboardDynamicFloatGetter damage = new BlackboardDynamicFloatGetter(10);
        [Tooltip("Set the speed of the projectile, 0 means don't change it.")]
        [SerializeField] private BlackboardDynamicFloatGetter speed = new BlackboardDynamicFloatGetter();
        [SerializeField] private BlackboardDynamicPointGetter spawnPoint;
        [SerializeField] private BlackboardDynamicPointGetter aimPoint;

        private DynamicPool<Projectile> projectilesPool = new DynamicPool<Projectile>();

        protected override void OnInitialize()
        {
            projectile.Initialize(agent.BlackboardCollection);
            damage.Initialize(agent.BlackboardCollection);
            speed.Initialize(agent.BlackboardCollection);
            spawnPoint.Initialize(agent.BlackboardCollection);
            aimPoint.Initialize(agent.BlackboardCollection);

            UnityEngine.Object projectileValue = projectile.Value;

            if(projectileValue == null || projectileValue is not Projectile projectileObj)
            {
                TafraDebugger.Log("Straight Projectile Task", "The assigned projectile is null or not of type \"Projectile\".", TafraDebugger.LogType.Error);
                return;
            }

            projectileObj.gameObject.SetActive(false);
            Projectile sampleUnit = GameObject.Instantiate(projectileObj);
            projectileObj.gameObject.SetActive(true);

            projectilesPool.AddUnit(sampleUnit);

            projectilesPool.Initialize();
        }

        protected override void OnStart()
        {
            if(projectilesPool == null)
                return;

            StraightProjectile activeProjectile = projectilesPool.RequestUnit(activateUnit: false) as StraightProjectile;

            Vector3 spawnPos = spawnPoint.Value;
            Vector3 dir = aimPoint.Value - spawnPos;
            
            //Just to make sure it's not enabled in a different position to prevent the spawn particles (or trail) from playing elsewher.
            activeProjectile.transform.position = spawnPos;
            
            activeProjectile.gameObject.SetActive(true);
            
            float speedValue = speed.Value;
            if (speedValue > 0.001f)
                activeProjectile.SetProperties(speedValue);

            activeProjectile.Initialize(new HitInfo(Mathf.RoundToInt(damage.Value), false, false,false, agent, true), spawnPos, dir, null, projectilesPool);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Success;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            projectilesPool.Uninitialize();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using TafraKit.AI3;
using TafraKit.Healthies;
using TafraKit.Weaponry;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Combat/Projectiles/Oblique Projectile"), GraphNodeName("Oblique Projectile")]
    public class ObliquetProjectileTask : TaskNode
    {
        [SerializeField] private BlackboardObjectGetter obliqueProjectile;
        [SerializeField] private BlackboardDynamicPointGetter spawnPoint;
        [SerializeField] private BlackboardDynamicPointGetter landingPoint;
        [SerializeField] private BlackboardDynamicFloatGetter landingDamageDiameter = new BlackboardDynamicFloatGetter(4);
        [SerializeField] private BlackboardDynamicFloatGetter travelDuration = new BlackboardDynamicFloatGetter(0.75f);
        [SerializeField] private BlackboardDynamicFloatGetter damage = new BlackboardDynamicFloatGetter(10);
        [SerializeField] private BlackboardDynamicFloatGetter throwHeight = new BlackboardDynamicFloatGetter(10);
        [Tooltip("0 means land exactly where you should, above that means there's a cirlce of that radius around the target that you can land in any point inside it.")]
        [SerializeField] private BlackboardDynamicFloatGetter extraLandingRadius;

        private DynamicPool<Projectile> projectilesPool = new DynamicPool<Projectile>();

        protected override void OnInitialize()
        {
            obliqueProjectile.Initialize(agent.BlackboardCollection);
            spawnPoint.Initialize(agent.BlackboardCollection);
            landingPoint.Initialize(agent.BlackboardCollection);
            landingDamageDiameter.Initialize(agent.BlackboardCollection);
            travelDuration.Initialize(agent.BlackboardCollection);
            damage.Initialize(agent.BlackboardCollection);
            throwHeight.Initialize(agent.BlackboardCollection);
            extraLandingRadius.Initialize(agent.BlackboardCollection);

            UnityEngine.Object projectileValue = obliqueProjectile.Value;

            if(projectileValue == null || projectileValue is not ObliqueProjectile projectileObj)
            {
                TafraDebugger.Log("Straight Projectile Task", "The assigned projectile is null or not of type \"ObliqueProjectile\", make sure to assign the component itself not the game object.", TafraDebugger.LogType.Error);
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

            ObliqueProjectile activeProjectile = projectilesPool.RequestUnit(activateUnit: false) as ObliqueProjectile;

            Vector3 spawnPos = spawnPoint.Value;
            Vector3 dir = landingPoint.Value - spawnPos;

            //Just to make sure it's not enabled in a different position to prevent the spawn particles (or trail) from playing elsewher.
            activeProjectile.transform.position = spawnPos;

            activeProjectile.gameObject.SetActive(true);

            Vector3 landingPointValue = landingPoint.Value;
            Vector2 randomPointOnCircle = Random.insideUnitCircle.normalized;
            Vector3 offset = Random.Range(0, extraLandingRadius.Value) * new Vector3(randomPointOnCircle.x, 0, randomPointOnCircle.y);

            landingPointValue += offset;

            activeProjectile.SetProperties(landingPointValue, travelDuration.Value, throwHeight.Value, landingDamageDiameter.Value / 2f, true);
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
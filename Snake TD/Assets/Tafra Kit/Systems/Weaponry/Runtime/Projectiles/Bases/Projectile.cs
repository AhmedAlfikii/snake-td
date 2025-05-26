using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TafraKit.Healthies;

namespace TafraKit.Weaponry
{
    public abstract class Projectile : MonoBehaviour
    {
        #region Private Serialized Fields
        [Header("SFX")]
        [Tooltip("Impact audio clip that will be used on impact with an object that isn't present in the \"Impact SFX Based On ID\" list")]
        [SerializeField] protected SFXClip genericImpactSFX;
        [SerializeField] protected SFXByID[] impactSFXBasedOnID;
        #endregion

        #region Protected/Private Fields
        protected Healthy target;
        protected Healthy impactedTarget;
        protected Transform targetDamagePoint;
        protected Vector3 spawnPoint;
        protected DynamicPool<Projectile> pool;
        protected IEnumerator despawningEnum;
        protected HitInfo hitInfo;
        protected bool isActive;
        protected bool disposeOnDespawnWithoutPool = true;
        protected Dictionary<string, SFXClip> sfxBasedOnID = new Dictionary<string, SFXClip>();
        protected Action<Projectile> onImpactAction;
        protected bool canDie;
        protected bool canHitTarget;
        #endregion

        [HideInInspector] public UnityEvent OnDespawn = new UnityEvent();
        [HideInInspector] public UnityEvent<Healthy> OnImpact = new UnityEvent<Healthy>();
        [HideInInspector] public UnityEvent onWouldDie = new UnityEvent();
        [HideInInspector] protected UnityEvent onWouldHitTarget = new UnityEvent();
        [HideInInspector] protected UnityEvent onInitialized = new UnityEvent();

        public HitInfo HitInfo { get => hitInfo; set => hitInfo = value; }
        public Vector3 SpawnPoint => spawnPoint;
        public Healthy Target => target;
        public Healthy ImpactedTarget => impactedTarget;
        public DynamicPool<Projectile> Pool => pool;
        public UnityEvent OnInitialized => onInitialized;
        public UnityEvent OnWouldHitTarget => onWouldHitTarget;
        public UnityEvent OnWouldDie => onWouldDie;

        #region MonoBehaviour Messages
        protected virtual void Awake()
        {
            for (int i = 0; i < impactSFXBasedOnID.Length; i++)
            {
                sfxBasedOnID.Add(impactSFXBasedOnID[i].ID, impactSFXBasedOnID[i].Clip);
            }
        }
        protected virtual void OnDestroy()
        { 

        }
        #endregion

        #region Protected/Private Functions
        protected virtual void OnInitialize()
        {

        }
        protected virtual void OnBeforeDespawn()
        {

        }
        protected virtual void Despawn()
        {
            //Just to make sure that the active state is set to flase. Typically, it should be deactivated earlier.
            isActive = false;

            OnBeforeDespawn();

            OnDespawn?.Invoke();

            bool wentBackToPool = pool != null && pool.ReleaseUnit(this);

            if(disposeOnDespawnWithoutPool && !wentBackToPool)
            {
                Destroy(gameObject);
            }
        }
        protected virtual void DespawnAfter(float delay)
        {
            if (despawningEnum != null)
                StopCoroutine(despawningEnum);

            despawningEnum = DespawningAfter(delay);

            StartCoroutine(despawningEnum);
        }
        private IEnumerator DespawningAfter(float delay)
        {
            yield return Yielders.GetWaitForSeconds(delay);

            Despawn();
        }
        #endregion

        #region Public Functions
        public virtual void Initialize(HitInfo hitInfo, Vector3 spawnPoint, Vector3 targetDirection, Healthy target = null, DynamicPool<Projectile> myPool = null, Action<Projectile> onImpact = null)
        {
            this.hitInfo = hitInfo;
            this.target = target;
            this.spawnPoint = spawnPoint;
            this.onImpactAction = onImpact;
            
            impactedTarget = null;
            transform.rotation = Quaternion.LookRotation(targetDirection);
            transform.position = spawnPoint;

            if(!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (target)
                targetDamagePoint = target.TargetPoint;
            else
                targetDamagePoint = null;

            pool = myPool;

            isActive = true;

            OnInitialize();

            onInitialized?.Invoke();
        }
        public void Initialize(HitInfo hitInfo)
        {
            this.hitInfo = hitInfo;
        }

        /// <summary>
        /// Should this projectile get destroyed whenever it should despawn and there's no pool to put it back in.
        /// </summary>
        /// <param name="on"></param>
        public void SetDisposeOnDespawnWithoutPoolState(bool on)
        {
            disposeOnDespawnWithoutPool = on;
        }
        public void PreventDeath()
        {
            canDie = false;
        }
        public void PreventHittingTarget()
        {
            canHitTarget = false;
        }
        public virtual void UninitializeGlobalPools()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
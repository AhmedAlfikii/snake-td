using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Auto Attacking/Targets Detector")]
    public class TargetsDetectorModule : CharacterCombatModule
    {
        [Header("Detection")]
        [Tooltip("The radius of the sphere around the character that we'll be looking for targets inside.")]
        [SerializeField] private TafraFloat detectionRadius = new TafraFloat(15);
        [Tooltip("The height of the capsule that will make the detection, the character is at the center.")]
        [SerializeField] private TafraFloat detectionHeight = new TafraFloat(0);
        [Tooltip("The intervals between each detection. Lower values will result in faster detection but higher performance cost.")]
        [SerializeField] private float detectionIntervals = 0.2f;
        [Tooltip("The layers where we'll be looking for targets at.")]
        [SerializeField] private TafraLayerMask targetLayers;

        [Header("Obstacles")]
        [Tooltip("If enabled, targets in range will be excluded if an obstacle is blocking the line of sight of the character towards those targets.")]
        [SerializeField] private bool obstaclesBlockSight = false;
        [Tooltip("The layers of obstacles that will block our sight, preventing us from detecting targets (if \"Obstacles Block Sight\" is enabled).")]
        [SerializeField] private TafraLayerMask obstacleLayers;
        [Tooltip("Define how to treat obstacles that are triggers.")]
        [SerializeField] protected QueryTriggerInteraction queryTriggerObstacles = QueryTriggerInteraction.Ignore;

        [NonSerialized] private Transform transform;
        [NonSerialized] private Collider[] detectedColliders = new Collider[100];
        [NonSerialized] private int detectionsCount;
        [NonSerialized] private float nextDetectionTime = 0;
        [NonSerialized] private Comparer<Collider> detectablesComparer;
        /// <summary>
        /// The closest target to the character, regardless of whether or not the line of sight is clear.
        /// </summary>
        [NonSerialized] private Collider potentialTarget;
        /// <summary>
        /// If the line of sight between the character and the closest target is clear, the target will be assigned here.
        /// </summary>
        [NonSerialized] private Collider target;
        [NonSerialized] private List<Collider> sortedTargetsInRange = new List<Collider>();

        [NonSerialized] private UnityEvent<Collider> onFoundPotentialTarget  = new UnityEvent<Collider>();
        [NonSerialized] private UnityEvent<Collider> onFoundTarget  = new UnityEvent<Collider>();
        [NonSerialized] private UnityEvent onLostPotentialTarget  = new UnityEvent();
        [NonSerialized] private UnityEvent onLostTarget  = new UnityEvent();

        /// <summary>
        /// If the line of sight between the character and the closest target is clear, the target will be assigned here.
        /// </summary>
        public Collider Target => target;
        public UnityEvent<Collider> OnFoundPotentialTarget => onFoundPotentialTarget;
        public UnityEvent<Collider> OnFoundTarget => onFoundTarget;
        public UnityEvent OnLostPotentialTarget => onLostPotentialTarget;
        public UnityEvent OnLostTarget => onLostTarget;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => true;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            transform = characterCombat.transform;
            nextDetectionTime = 0;

            detectablesComparer = Comparer<Collider>.Create(delegate (Collider a, Collider b)
            {
                float aSqrDistance = (transform.position - a.transform.position).sqrMagnitude;
                float bSqrDistance = (transform.position - b.transform.position).sqrMagnitude;

                //Sort by distance to modularComponent
                return aSqrDistance.CompareTo(bSqrDistance);
            });
        }
        public override void LateUpdate()
        {
            //Check if we should make a detection now.
            if(Time.time > nextDetectionTime)
                Detect();
        }

        public void Detect()
        {
            nextDetectionTime = Time.time + detectionIntervals;

            float halfHeight = detectionHeight.Value / 2f;
            Vector3 point0 = transform.position + new Vector3(0, -halfHeight, 0);
            Vector3 point1 = transform.position + new Vector3(0, halfHeight, 0);

            detectionsCount = Physics.OverlapCapsuleNonAlloc(point0, point1, detectionRadius.Value, detectedColliders, targetLayers.Value);

            sortedTargetsInRange.Clear();

            if(detectionsCount == 0)
            {
                if(potentialTarget)
                {
                    potentialTarget = null;
                    onLostPotentialTarget?.Invoke();
                }
                if(target)
                {
                    target = null;
                    onLostTarget?.Invoke();
                }

                return;
            }

            for(int i = 0; i < detectionsCount; i++)
            {
                sortedTargetsInRange.Add(detectedColliders[i]);
            }

            sortedTargetsInRange.Sort(detectablesComparer);

            if(potentialTarget != sortedTargetsInRange[0])
            { 
                potentialTarget = sortedTargetsInRange[0];
                onFoundPotentialTarget?.Invoke(potentialTarget);
            }

            if(obstaclesBlockSight)
            {
                Debug.LogError("Implement this");
            }
            else
            {
                if(target != potentialTarget)
                {
                    target = potentialTarget;
                    onFoundTarget?.Invoke(target);
                }
            }
        }

        /// <summary>
        /// Returns the detected colliders list that is sorted by distance to detector.
        /// </summary>
        /// <param name="detectionsCount"></param>
        /// <returns></returns>
        public List<Collider> GetSortedDetectedColliders()
        {
            return sortedTargetsInRange;
        }
    }
}
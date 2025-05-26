using System.Collections;
using System.Collections.Generic;
using TafraKit.Healthies;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.Weaponry
{
    /// <summary>
    /// A projectile that will move in a straight path, will get destroyed if it hit any damgeable object or an obstacle in its path.
    /// </summary>
    public class StraightProjectile : SingleProjectile
    {
        [Tooltip("The speed of which the projectile would fly at.")]
        [SerializeField] protected float speed = 30f;
        [Tooltip("Deactivate after flying for this duration without hitting an object.")]
        [SerializeField] protected float despawnAfterFlyingDelay = 5f;
        [Tooltip("The distance backwards used for the initial detection (first frame after spawn) just in case we're inside a hittable collider")]
        [SerializeField] protected float initialDetectionBackDistance;

        [Header("Collision Detection")]
        [SerializeField] protected bool hitTriggers = true;
        [SerializeField] protected TafraLayerMask damageableLayers;
        [SerializeField] protected Bounds damageableHitBox;
        [SerializeField] protected TafraLayerMask obstacleLayers;
        [SerializeField] protected Bounds obstacleHitBox;

        protected QueryTriggerInteraction queryTrigger;
        protected Vector3 lastPosition;

        protected Vector3 damageableHitBoxSliceHalfExtents;
        protected float damageableHitBoxHalfLength;

        protected Vector3 obstacleHitBoxSliceHalfExtents;
        protected float obstacleHitBoxHalfLength;

        protected override void Awake()
        {
            base.Awake();

            queryTrigger = hitTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            float sliceThickness = 0.01f;

            damageableHitBoxHalfLength = damageableHitBox.extents.z - sliceThickness;

            Vector3 damageableHitBoxSlice = damageableHitBox.extents;
            damageableHitBoxSlice.z = sliceThickness;
            damageableHitBoxSliceHalfExtents = damageableHitBoxSlice;

            obstacleHitBoxHalfLength = obstacleHitBox.extents.z - sliceThickness;

            Vector3 obstacleHitBoxSlice = obstacleHitBox.extents;
            obstacleHitBoxSlice.z = sliceThickness;
            obstacleHitBoxSliceHalfExtents = obstacleHitBoxSlice;
        }

        protected virtual void Update()
        {
            if(!isActive)
                return;

            Vector3 movementDirection = transform.forward;

            transform.position += movementDirection * speed * Time.deltaTime;
           
            Quaternion castOrientation = transform.rotation;

            //Start at the back edge of the hit box wherever it was in the previous frame.
            Vector3 damageableCastStartPosition = lastPosition - movementDirection * damageableHitBoxHalfLength;
            Vector3 obstacleCastStartPosition = lastPosition - movementDirection * obstacleHitBoxHalfLength;
            //End at the front edge of the hit box in this frame, to cover everything in between.
            Vector3 damageableCastEndPosition = transform.position + movementDirection * damageableHitBoxHalfLength;
            Vector3 obstacleCastEndPosition = transform.position + movementDirection * obstacleHitBoxHalfLength;
            
            float damageableCastLength = (damageableCastEndPosition - damageableCastStartPosition).magnitude;
            float obstacleCastLength = (obstacleCastEndPosition - obstacleCastStartPosition).magnitude;

            RaycastHit damageableHit;
            RaycastHit obstacleHit;

            //Cast a box from beggining to end, and another one from end to begining to make sure to detect objects that have their backfaces in one of the detection directions.
            if(Physics.BoxCast(damageableCastStartPosition, damageableHitBoxSliceHalfExtents, movementDirection, out damageableHit, castOrientation, damageableCastLength, damageableLayers.Value, queryTrigger)
                || Physics.BoxCast(damageableCastEndPosition, damageableHitBoxSliceHalfExtents, -movementDirection, out damageableHit, castOrientation, damageableCastLength, damageableLayers.Value, queryTrigger))
            {
                TafraActor hitActor = ComponentProvider.GetComponent<TafraActor>(damageableHit.collider.gameObject);
                Healthy hitHealthy = null;

                if(hitActor != null)
                {
                    hitHealthy = hitActor.GetCachedComponent<Healthy>();

                    FoundTarget(hitHealthy, damageableHit.point, damageableHit.normal);
                }
            }
            else if(Physics.BoxCast(obstacleCastStartPosition, obstacleHitBoxSliceHalfExtents, movementDirection, out obstacleHit, castOrientation, obstacleCastLength, obstacleLayers.Value, queryTrigger)
                || Physics.BoxCast(obstacleCastEndPosition, obstacleHitBoxSliceHalfExtents, -movementDirection, out obstacleHit, castOrientation, obstacleCastLength, obstacleLayers.Value, queryTrigger))
            {
                FoundTarget(null, obstacleHit.point, obstacleHit.normal);
            }

            #if UNITY_EDITOR
            Debug.DrawLine(lastPosition, transform.position);
            #endif

            lastPosition = transform.position;
        }

        protected virtual void OnDrawGizmos/*Selected*/()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            float lossyScale = transform.lossyScale.x;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(damageableHitBox.center, damageableHitBox.size / lossyScale);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(obstacleHitBox.center, obstacleHitBox.size / lossyScale);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            lastPosition = transform.position - (transform.forward * initialDetectionBackDistance);

            DespawnAfter(despawnAfterFlyingDelay);
        }

        public void SetProperties(float speed)
        { 
            this.speed = speed;
        }
    }
}
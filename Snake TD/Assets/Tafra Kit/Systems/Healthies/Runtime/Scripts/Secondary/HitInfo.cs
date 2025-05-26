using UnityEngine;

namespace TafraKit.Healthies
{
    public struct HitInfo
    {
        public float originalDamage;
        public float damage;
        public bool isCritical;
        public bool isMissed;
        public Vector3 pointOfImpact;
        public Vector3 pointOfImpactNormal;
        public bool isRanged;
        public bool hitterIsPlayer;
        public TafraActor hitter;

        public HitInfo(float damage, TafraActor hitter = null, bool isRanged = false)
        {
            originalDamage = damage;
            this.damage = damage;
            isCritical = false;
            isMissed = false;
            pointOfImpact = Vector3.zero;
            pointOfImpactNormal = Vector3.zero;
            hitterIsPlayer = false;
            this.isRanged = isRanged;
            this.hitter = hitter;
        }
        public HitInfo(float damage, bool isCritical, bool isMissed, bool hitterIsPlayer = false, TafraActor hitter = null, bool isRanged = false)
        { 
            originalDamage = damage;
            this.damage = damage;
            this.isCritical = isCritical;
            this.isMissed = isMissed;
            this.hitterIsPlayer = hitterIsPlayer;
            this.hitter = hitter;
            pointOfImpact = Vector3.zero;
            pointOfImpactNormal = Vector3.zero;
            this.isRanged = isRanged;
        }
        public HitInfo(float damage, bool isCritical, bool isMissed, Vector3 pointOfImpact, Vector3 pointOfImpactNormal, bool hitterIsPlayer = false, TafraActor hitter = null, bool isRanged = false)
        { 
            originalDamage = damage;
            this.damage = damage;
            this.isCritical = isCritical;
            this.isMissed = isMissed;
            this.pointOfImpact = pointOfImpact;
            this.pointOfImpactNormal = pointOfImpactNormal;
            this.hitterIsPlayer = hitterIsPlayer;
            this.hitter = hitter;
            this.isRanged = isRanged;
        }
        public HitInfo(float damage, bool isCritical, bool isMissed, Vector3 pointOfImpact, Vector3 pointOfImpactNormal, bool isRanged, bool hitterIsPlayer = false, TafraActor hitter = null)
        { 
            originalDamage = damage;
            this.damage = damage;
            this.isCritical = isCritical;
            this.isMissed = isMissed;
            this.pointOfImpact = pointOfImpact;
            this.pointOfImpactNormal = pointOfImpactNormal;
            this.isRanged = isRanged;
            this.hitterIsPlayer = hitterIsPlayer;
            this.hitter = hitter;
        }
    }
}
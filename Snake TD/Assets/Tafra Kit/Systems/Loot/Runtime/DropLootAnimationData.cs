using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Loot
{
    [CreateAssetMenu(menuName = "Tafra Kit/Loot/Drop Loot Animation Data", fileName = "Drop Loot Animation Data")]
    public class DropLootAnimationData : ScriptableObject
    {
        #region Private Serialized Fields
        [SerializeField] private float preSpawnDelay;
        [SerializeField] private float throwForce = 5f;
        [SerializeField] private float throwRadius = 1.5f;
        [SerializeField] private float throwDuration = 0.8f;
        [SerializeField] private float spawnOffsetY;
        [SerializeField] private float betweenSpawnsDelay = 0.05f;
        [SerializeField] private AnimationCurve bounceCurve;
        [SerializeField] private float bouncePower = 0.5f;
        [SerializeField] private float bounceDuration = 0.5f;
        [SerializeField] private SFXClips dropSFX = new SFXClips();
        #endregion

        #region Public Properites
        public float PreSpawnDelay => preSpawnDelay;
        public float ThrowForce => throwForce;
        public float ThrowRadius => throwRadius;
        public float ThrowDuration => throwDuration;
        public float SpawnOffsetY => spawnOffsetY;
        public float BetweenSpawnsDelay => betweenSpawnsDelay;
        public AnimationCurve BounceCurve => bounceCurve;
        public float BouncePower => bouncePower;
        public float BounceDuration => bounceDuration;
        public SFXClips DropSFX => dropSFX;
        #endregion
    }
}
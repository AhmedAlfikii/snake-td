using System;
using System.Collections.Generic;
using TafraKit.Loot;
using TafraKit.RPG;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    [SearchMenuItem("Looting/Loot Collector")]
    public class LootCollectorModule : TafraActorModule, ILootCollector
    {
        [SerializeField] private Transform collectionPoint;
        [Tooltip("Should this collector only collect certain types of loot?")]
        [SerializeField] private bool limitLoot;
        [Tooltip("If loot is limited, what loot types should this collector be able to collect?")]
        [SerializeField] private LootData[] includedLoot;
        [Tooltip("The radius the detection sphere will have")]
        [SerializeField] private TafraFloat detectionRadius = new TafraFloat(3);
        [Tooltip("The intervals in seconds between each detection.")]
        [SerializeField] private float detectionFrequency = 0.2f;
        [Tooltip("How many loot can be detected in a single detection. (Sets the size of the fixed array on initialization)")]
        [SerializeField] private int singleDetectionCount = 15;
        [Tooltip("The loot layers to detect.")]
        [SerializeField] private LayerMask lootLayers;

        [NonSerialized] private Collider[] detectableLoot;
        [NonSerialized] private HashSet<LootData> includedLootHash = new HashSet<LootData>();
        [NonSerialized] private float offsetY;
        [NonSerialized] protected Transform collectPoint;
        [NonSerialized] protected Inventory inventory;
        [NonSerialized] protected Transform transform;

        [NonSerialized] private UnityEvent<LootData> onCollectedLoot = new UnityEvent<LootData>();

        public UnityEvent<LootData> OnCollectedLoot => onCollectedLoot;
        public Transform CollectionPoint => collectionPoint;
        public Inventory LootInventory => inventory;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => true;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            inventory = actor.GetCachedComponent<Inventory>();
            transform = actor.transform;

            detectableLoot = new Collider[singleDetectionCount];

            for(int i = 0; i < includedLoot.Length; i++)
            {
                includedLootHash.Add(includedLoot[i]);
            }
        }

        public override void FixedUpdate()
        {
            int lootInRangeCount = Physics.OverlapSphereNonAlloc(transform.position + new Vector3(0, offsetY, 0), detectionRadius.Value, detectableLoot, lootLayers);

            if(lootInRangeCount == 0) return;

            for(int i = 0; i < lootInRangeCount; i++)
            {
                Collider detectedLootCollider = detectableLoot[i];
                LootData lootData = LootHandler.GetLootData(detectedLootCollider);

                if(limitLoot && !includedLootHash.Contains(lootData)) return;

                LootHandler.CollectLoot(detectedLootCollider, this);
            }
        }

        public void OnLootCollected(LootData lootData)
        {
            onCollectedLoot?.Invoke(lootData);
        }
    }
}
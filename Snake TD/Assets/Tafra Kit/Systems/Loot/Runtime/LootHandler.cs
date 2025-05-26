using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace TafraKit.Loot
{
    //TODO: What happens when a new scene loads or the current scene reloads?
    //where does the spawned loot go to and what happens to the pools (their elements will be nulled if the units got destroyed with the scene)..
    public static class LootHandler
    {
        #region Classes
        public class SpawnedLootData
        {
            public LootData lootData;
            public IEnumerator droppingLootEnum;

            public SpawnedLootData(LootData lootData, IEnumerator droppingLootEnum)
            {
                this.lootData = lootData;
                this.droppingLootEnum = droppingLootEnum;
            }
        }
        #endregion

        public static UnityEvent<LootData, ILootCollector> OnLootCollected = new UnityEvent<LootData, ILootCollector>();
        public static UnityEvent<Collider, ILootCollector> OnLootColliderCollected = new UnityEvent<Collider, ILootCollector>();
        public static UnityEvent<Collider> OnLootColliderDropped = new UnityEvent<Collider>();

        private static int lootNavMeshAreaMask;

        private static Dictionary<LootData, DynamicPool<Collider>> lootColliderPools = new Dictionary<LootData, DynamicPool<Collider>>();
        private static Dictionary<Collider, SpawnedLootData> spawnedLootData = new Dictionary<Collider, SpawnedLootData>();
        private static HashSet<LootData> preventedLootRewarding = new HashSet<LootData>();
        private static List<Collider> tempAutoCollectedLoot = new List<Collider>();
        private static MonoBehaviour coroutinePlayer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            GameObject player = new GameObject("LootHandlerCoroutinePlayer");
            coroutinePlayer = player.AddComponent<EmptyMonoBehaviour>();
            GameObject.DontDestroyOnLoad(coroutinePlayer);

            lootNavMeshAreaMask = 1 << NavMesh.GetAreaFromName("Loot");

            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            ClearAllLootRelatedData();
        }

        private static void ClearAllLootRelatedData()
        {
            coroutinePlayer.StopAllCoroutines();

            lootColliderPools.Clear();
            spawnedLootData.Clear();
        }
        private static IEnumerator CollectingLoot(Collider loot, LootData data, Transform collectionPoint, EasingType easing, float collectionDuration, float delay, bool enableSFX = true, bool enableVFX = true, Action OnReached = null)
        {
            loot.enabled = false;

            if (delay > 0.001f)
                yield return Yielders.GetWaitForSeconds(delay);

            float duration = collectionDuration;

            float startTime = Time.time;

            Vector3 graphicStartScale = loot.transform.localScale;
            bool playedSFX = false;
            Vector3 currPos = loot.transform.position;

            while (Time.time - startTime < duration)
            {
                float t = (Time.time - startTime) / duration;

                Vector3 targetPos = collectionPoint.position;

                if (t > 0.9f)
                {
                    float scaleT = (t - 0.9f) * 10;
                    loot.transform.localScale = Vector3.LerpUnclamped(graphicStartScale, Vector3.zero, scaleT);
                }

                t = MotionEquations.GetEaseFloat(t, easing);

                Vector3 pos = Vector3.LerpUnclamped(currPos, targetPos, t);
                pos = new Vector3(pos.x, Mathf.Clamp(pos.y, currPos.y, targetPos.y), pos.z);
                loot.transform.position = pos;

                if (enableSFX)
                {
                    if (!playedSFX && t > 0.75f)
                    {
                        SFXPlayer.Play(data.CollectSFX);

                        playedSFX = true;
                    }
                }

                yield return null;
            }

            Vector3 endPos = collectionPoint.position;

            loot.transform.localScale = Vector3.zero;
            loot.transform.position = endPos;

            if (enableVFX)
            {
                //TODO: handle VFX.

                //GameObject collectVFX = globalPools.RequestGameObjectUnit(data.CollectVFXID);
                //if (collectVFX != null)
                //{
                //    collectVFX.transform.position = endPos;
                //    collectVFX.transform.SetParent(collectPoint);
                //}
            }

            OnReached?.Invoke();
        }
        private static IEnumerator DroppingLoot(Collider lootCollider, Transform spawnPoint, float delay, DropLootAnimationData animation)
        {
            yield return Yielders.GetWaitForSeconds(delay);

            Transform lootTransform = lootCollider.transform;

            lootTransform.gameObject.SetActive(true);

            Vector3 spawnPosition = spawnPoint.position;

            lootTransform.localScale = Vector3.one;

            if (animation != null)
            {
                Vector3 dropPoint = GetRandomNavMeshPoint(spawnPosition, animation.ThrowRadius, lootNavMeshAreaMask);

                Vector3 landingPoint = spawnPosition + (dropPoint - spawnPosition) * 0.8f;

                float startTime = Time.time;
                float duration = animation.ThrowDuration;

                Vector3 midPoint = spawnPosition + (landingPoint - spawnPosition) * 0.5f + new Vector3(0, animation.ThrowForce, 0);

                if(animation.DropSFX != null && animation.DropSFX.Clips != null)
                    SFXPlayer.Play(animation.DropSFX);

                while(Time.time - startTime < duration)
                {
                    float t = (Time.time - startTime) / duration;
                    Vector3 position = ZBezier.GetPointOnQuadraticCurve(t, spawnPosition, midPoint, landingPoint);
                    lootTransform.position = position;
                    yield return null;
                }

                startTime = Time.time;
                duration = animation.BounceDuration;

                lootCollider.enabled = true;

                while (Time.time - startTime < duration)
                {
                    float t = (Time.time - startTime) / duration;
                    float slideT = MotionEquations.EaseOut(t);
                    Vector3 currPos = Vector3.Lerp(landingPoint, dropPoint, slideT);
                    currPos += new Vector3(0, animation.BounceCurve.Evaluate(t) * animation.BouncePower, 0);
                    lootTransform.position = currPos;
                    yield return null;
                }
            }
            else
            {
                lootCollider.enabled = true;
            }

            OnLootColliderDropped?.Invoke(lootCollider);
            spawnedLootData[lootCollider].droppingLootEnum = null;
        }
        private static IEnumerator DroppingLoot(LootData lootData, Collider lootCollider, Vector3 spawnPoint, float delay, DropLootAnimationData animation)
        {
            yield return Yielders.GetWaitForSeconds(delay);

            if(lootData.DropSFX != null && lootData.DropSFX.Clips.Length > 0)
                SFXPlayer.Play(lootData.DropSFX);

            Transform lootTransform = lootCollider.transform;

            lootTransform.gameObject.SetActive(true);

            Vector3 spawnPosition = spawnPoint;

            lootTransform.localScale = Vector3.one;

            if (animation != null)
            {
                Vector3 dropPoint = GetRandomNavMeshPoint(spawnPosition, animation.ThrowRadius, lootNavMeshAreaMask);

                Vector3 landingPoint = spawnPosition + (dropPoint - spawnPosition) * 0.8f;

                float startTime = Time.time;
                float duration = animation.ThrowDuration;

                Vector3 midPoint = spawnPosition + (landingPoint - spawnPosition) * 0.5f + new Vector3(0, animation.ThrowForce, 0);

                while (Time.time - startTime < duration)
                {
                    float t = (Time.time - startTime) / duration;
                    Vector3 position = ZBezier.GetPointOnQuadraticCurve(t, spawnPosition, midPoint, landingPoint);
                    lootTransform.position = position;
                    yield return null;
                }

                startTime = Time.time;
                duration = animation.BounceDuration;

                lootCollider.enabled = true;

                while (Time.time - startTime < duration)
                {
                    float t = (Time.time - startTime) / duration;
                    float slideT = MotionEquations.EaseOut(t);
                    Vector3 currPos = Vector3.Lerp(landingPoint, dropPoint, slideT);
                    currPos += new Vector3(0, animation.BounceCurve.Evaluate(t) * animation.BouncePower, 0);
                    lootTransform.position = currPos;
                    yield return null;
                }
            }
            else
            {
                lootCollider.enabled = true;
            }

            OnLootColliderDropped?.Invoke(lootCollider);
            spawnedLootData[lootCollider].droppingLootEnum = null;
        }
        private static Vector3 GetRandomNavMeshPoint(Vector3 center, float radius, int areaMask)
        {
            Vector2 randomPoint = Random.insideUnitCircle;
            Vector3 finalPoint = center + new Vector3(randomPoint.x, 0, randomPoint.y) * radius;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(finalPoint, out hit, 5, areaMask))
                return hit.position;
            else
                return center;
        }

        public static Collider DropLoot(LootData loot, Transform spawnPoint, DropLootAnimationData animation)
        {
            DynamicPool<Collider> pool;

            //If no pool was created before for this loot data's prefab, then create one.
            if (!lootColliderPools.TryGetValue(loot, out pool))
            {
                GameObject sampleLootGO = GameObject.Instantiate(loot.LootPrefab);
                sampleLootGO.SetActive(false);

                Collider sampleLootCollider = sampleLootGO.GetComponent<Collider>();

                pool = new DynamicPool<Collider>();
                pool.Construct(new List<Collider>() { sampleLootCollider }, null, true);
                pool.Initialize();

                lootColliderPools.Add(loot, pool);
            }

            Collider lootCollider = pool.RequestUnit(null, false);
            lootCollider.enabled = false;

            lootCollider.transform.position = spawnPoint.position;

            IEnumerator droppingCoroutine = DroppingLoot(lootCollider, spawnPoint, 0f, animation);

            spawnedLootData.Add(lootCollider, new SpawnedLootData(loot, droppingCoroutine));

            coroutinePlayer.StartCoroutine(droppingCoroutine);

            return lootCollider;
        }
        public static Collider DropLoot(LootData loot, Vector3 spawnPoint, DropLootAnimationData animation)
        {
            DynamicPool<Collider> pool;

            //If no pool was created before for this loot data's prefab, then create one.
            if (!lootColliderPools.TryGetValue(loot, out pool))
            {
                GameObject sampleLootGO = GameObject.Instantiate(loot.LootPrefab);
                sampleLootGO.SetActive(false);

                Collider sampleLootCollider = sampleLootGO.GetComponent<Collider>();

                pool = new DynamicPool<Collider>();
                pool.Construct(new List<Collider>() { sampleLootCollider }, null, true);
                pool.Initialize();

                lootColliderPools.Add(loot, pool);
            }

            Collider lootCollider = pool.RequestUnit(null, false);
            lootCollider.enabled = false;

            lootCollider.transform.position = spawnPoint;

            IEnumerator droppingCoroutine = DroppingLoot(loot, lootCollider, spawnPoint, 0f, animation);

            spawnedLootData.Add(lootCollider, new SpawnedLootData(loot, droppingCoroutine));

            coroutinePlayer.StartCoroutine(droppingCoroutine);

            return lootCollider;
        }
        public static void DropLoot(List<LootData> loot, Transform spawnPoint, DropLootAnimationData animation)
        {
            for (int lootIndex = 0; lootIndex < loot.Count; lootIndex++)
            {
                LootData lootData = loot[lootIndex];
                DynamicPool<Collider> pool;

                //If no pool was created before for this loot data's prefab, then create one.
                if (!lootColliderPools.TryGetValue(lootData, out pool))
                {
                    GameObject sampleLootGO = GameObject.Instantiate(lootData.LootPrefab);
                    sampleLootGO.SetActive(false);
                    
                    Collider sampleLootCollider = sampleLootGO.GetComponent<Collider>();

                    pool = new DynamicPool<Collider>();
                    pool.Construct(new List<Collider>() { sampleLootCollider }, null, true);
                    pool.Initialize();

                    lootColliderPools.Add(lootData, pool);
                }

                Collider lootCollider = pool.RequestUnit(null, false);
                lootCollider.enabled = false;

                lootCollider.transform.position = spawnPoint.position;

                IEnumerator droppingCoroutine = DroppingLoot(lootCollider, spawnPoint,  animation ? animation.BetweenSpawnsDelay * lootIndex : 0f, animation);

                spawnedLootData.Add(lootCollider, new SpawnedLootData(lootData, droppingCoroutine));

                coroutinePlayer.StartCoroutine(droppingCoroutine);
            }
        }
        public static void DropLoot(List<LootData> loot, Vector3 spawnPoint, DropLootAnimationData animation)
        {
            for (int lootIndex = 0; lootIndex < loot.Count; lootIndex++)
            {
                LootData lootData = loot[lootIndex];
                DynamicPool<Collider> pool;

                //If no pool was created before for this loot data's prefab, then create one.
                if (!lootColliderPools.TryGetValue(lootData, out pool))
                {
                    GameObject sampleLootGO = GameObject.Instantiate(lootData.LootPrefab);
                    sampleLootGO.SetActive(false);

                    Collider sampleLootCollider = sampleLootGO.GetComponent<Collider>();

                    pool = new DynamicPool<Collider>();
                    pool.Construct(new List<Collider>() { sampleLootCollider }, null, true);
                    pool.Initialize();

                    lootColliderPools.Add(lootData, pool);
                }

                Collider lootCollider = pool.RequestUnit(null, false);
                lootCollider.enabled = false;

                lootCollider.transform.position = spawnPoint;

                IEnumerator droppingCoroutine = DroppingLoot(lootData, lootCollider, spawnPoint, animation ? animation.BetweenSpawnsDelay * lootIndex : 0f, animation);

                spawnedLootData.Add(lootCollider, new SpawnedLootData(lootData, droppingCoroutine));

                coroutinePlayer.StartCoroutine(droppingCoroutine);
            }
        }

        public static void CollectLoot(Collider lootCollider, ILootCollector collector)
        {
            if (!spawnedLootData.TryGetValue(lootCollider, out SpawnedLootData spawnedData))
                return;

            OnLootColliderCollected?.Invoke(lootCollider, collector);

            spawnedLootData.Remove(lootCollider);

            if (spawnedData.droppingLootEnum != null)
            {
                coroutinePlayer.StopCoroutine(spawnedData.droppingLootEnum);
                spawnedData.droppingLootEnum = null;
            }

            IEnumerator collectingEnum = CollectingLoot(lootCollider, spawnedData.lootData, collector.CollectionPoint,
                new EasingType(MotionType.EaseIn, new EasingEquationsParameters(new EasingEquationsParameters.EaseInOutParameters())),
                0.25f, 0, true, true,
                OnReached:() => 
                {
                    if (!preventedLootRewarding.Contains(spawnedData.lootData))
                        collector.CollectLoot(spawnedData.lootData);

                    OnLootCollected?.Invoke(spawnedData.lootData, collector);

                    lootColliderPools[spawnedData.lootData].ReleaseUnit(lootCollider);
                });

            coroutinePlayer.StartCoroutine(collectingEnum);
        }
        public static void CollectAllDroppedLoot(ILootCollector collector, float totalCollectionTime, bool activeLootOnly = true)
        {
            int index = -1;
            int totalLoot = spawnedLootData.Count;
            float delayBetweenLoot = Mathf.Clamp(totalCollectionTime / totalLoot, 0, 0.05f);

            foreach(KeyValuePair<Collider, SpawnedLootData> element in spawnedLootData)
            {
                if (element.Key == null || element.Value == null) 
                    continue;

                Collider lootCollider = element.Key;

                if(activeLootOnly && !lootCollider.enabled)
                    continue;

                SpawnedLootData spawnedData = element.Value;

                OnLootColliderCollected?.Invoke(lootCollider, collector);
                
                if (spawnedData.droppingLootEnum != null)
                {
                    coroutinePlayer.StopCoroutine(spawnedData.droppingLootEnum);
                    spawnedData.droppingLootEnum = null;
                }

                index++;

                IEnumerator collectingEnum = CollectingLoot(lootCollider, spawnedData.lootData, collector.CollectionPoint,
                    new EasingType(MotionType.EaseIn, new EasingEquationsParameters(new EasingEquationsParameters.EaseInOutParameters())),
                    totalCollectionTime, index * delayBetweenLoot, true, true,
                    OnReached: () =>
                    {
                        if(spawnedData != null)
                        {
                            if(collector != null && !preventedLootRewarding.Contains(spawnedData.lootData))
                                collector.CollectLoot(spawnedData.lootData);

                            OnLootCollected?.Invoke(spawnedData.lootData, collector);

                            lootColliderPools[spawnedData.lootData].ReleaseUnit(lootCollider);
                        }
                    });

                coroutinePlayer.StartCoroutine(collectingEnum);

                tempAutoCollectedLoot.Add(lootCollider);
            }

            for(int i = 0; i < tempAutoCollectedLoot.Count; i++)
            {
                spawnedLootData.Remove(tempAutoCollectedLoot[i]);
            }
            tempAutoCollectedLoot.Clear();
        }
        public static LootData GetLootData(Collider lootCollider)
        {
            if (!spawnedLootData.TryGetValue(lootCollider, out SpawnedLootData spawnedData))
                return null;

            return spawnedData.lootData;
        }
        public static void PreventLootRewarding(LootData lootData)
        {
            if (!preventedLootRewarding.Contains(lootData))
                preventedLootRewarding.Add(lootData);
        }
        public static void UnpreventLootRewarding(LootData lootData)
        {
            if (!preventedLootRewarding.Contains(lootData))
                preventedLootRewarding.Remove(lootData);
        }
    }
}
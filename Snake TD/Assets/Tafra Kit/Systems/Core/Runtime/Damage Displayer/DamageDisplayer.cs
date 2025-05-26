using TafraKit.Healthies;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace TafraKit
{
    public static class DamageDisplayer
    {
        private static bool isEnabled;
        private static float spawnRandomRange;
        private static readonly DynamicPool<DamageText> textPool = new DynamicPool<DamageText>();
        private static readonly DynamicPool<DamageText> criticalTextPool = new DynamicPool<DamageText>();
        private static readonly DynamicPool<DamageText> missTextPool = new DynamicPool<DamageText>();
        private static DamageDisplayerSettings settings;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<DamageDisplayerSettings>();

            if(!settings.Enabled)
                return;

            spawnRandomRange = settings.SpawnRandomRange;

            SceneManager.sceneLoaded += OnSceneLoaded;

            isEnabled = true;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            textPool.Uninitialize();
            criticalTextPool.Uninitialize();
            missTextPool.Uninitialize();
            
            DamageText damageTextPrefab = settings.DamageTextPrefab;            
            DamageText criticalDamageTextPrefab = settings.CriticalDamageTextPrefab;
            DamageText missDamageTextPrefab = settings.MissDamageTextPrefab;

            //Disable the prefab object so that it doesn't invoke Awake and OnEnable once instantiated.
            damageTextPrefab.gameObject.SetActive(false);
            criticalDamageTextPrefab.gameObject.SetActive(false);
            missDamageTextPrefab.gameObject.SetActive(false);

            DamageText sampleDamageTxtUnit = GameObject.Instantiate(damageTextPrefab.gameObject).GetComponent<DamageText>();           
            DamageText sampleCriticalDamageTxtUnit = GameObject.Instantiate(criticalDamageTextPrefab.gameObject).GetComponent<DamageText>();
            DamageText sampleMissDamageTxtUnit = GameObject.Instantiate(missDamageTextPrefab.gameObject).GetComponent<DamageText>();

            //Enable the prefab back again since we most likely want it enabled by default for ease of editing.
            damageTextPrefab.gameObject.SetActive(true);
            criticalDamageTextPrefab.gameObject.SetActive(true);
            missDamageTextPrefab.gameObject.SetActive(true);

            textPool.AddUnit(sampleDamageTxtUnit);
            textPool.Initialize();
            
            criticalTextPool.AddUnit(sampleCriticalDamageTxtUnit);
            criticalTextPool.Initialize();

            missTextPool.AddUnit(sampleMissDamageTxtUnit);
            missTextPool.Initialize();
        }

        public static void DisplayDamageOnTarget(ITargetPoint target, HitInfo hitInfo)
        {
            if(!isEnabled)
                return;

            DamageText damageText;

            if(hitInfo.isCritical)
                damageText = criticalTextPool.RequestUnit(activateUnit: false);
            else if(hitInfo.isMissed)
            {
                damageText = missTextPool.RequestUnit(activateUnit: false);
            }
            else
                damageText = textPool.RequestUnit(activateUnit: false);

            int intDamage = Mathf.RoundToInt(hitInfo.damage);

            if (!hitInfo.isMissed)
                damageText.SetText(intDamage.ToString());
            else
                damageText.SetText("Dodged");

            damageText.transform.position = target.TargetPoint.position + Random.insideUnitSphere * spawnRandomRange;

            damageText.gameObject.SetActive(true);
        }
    }
}
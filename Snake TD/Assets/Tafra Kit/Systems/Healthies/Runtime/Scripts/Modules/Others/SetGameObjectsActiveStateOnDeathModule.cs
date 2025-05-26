using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Others/Set GOs Active State On Death")]
    public class SetGameObjectsActiveStateOnDeathModule : HealthyModule
    {
        [SerializeField] private List<GameObject> gameObjects;

        [Header("Settings")]
        [SerializeField] private bool stateOnDeath;
        [SerializeField] private bool revertOnRevive = true;

        private bool stateWasSet;

        public override bool DisableOnDeath => false;
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnEnable()
        {
            if(stateWasSet && !healthy.IsDead)
                OnRevive();

            healthy.Events.OnDeath.AddListener(OnDeath);
            healthy.Events.OnRevive.AddListener(OnRevive);
        }
        protected override void OnDisable()
        {
            healthy.Events.OnDeath.RemoveListener(OnDeath);
            healthy.Events.OnRevive.RemoveListener(OnRevive);
        }

        private void OnDeath(Healthy healthy, HitInfo killHit)
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].SetActive(stateOnDeath);
            }
            stateWasSet = true;
        }
        private void OnRevive()
        {
            if(revertOnRevive)
            {
                for(int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].SetActive(!stateOnDeath);
                }
                stateWasSet = false;
            }
        }
    }
}
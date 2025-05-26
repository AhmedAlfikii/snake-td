using System.Collections;
using System.Collections.Generic;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit.AI3
{
    public class Enemy : AIAgent
    {
        [SerializeField] private bool isBoss;

        [Header("Stats")]
        [SerializeField] private ExternalStatsContainer externalStatsContainer;

        public bool IsBoss => isBoss;

        protected override void OnEnable()
        {
            base.OnEnable();

            if(externalStatsContainer)
                externalStatsContainer.AddDependant();
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            if(externalStatsContainer)
                externalStatsContainer.RemoveDependant();
        }
    }
}
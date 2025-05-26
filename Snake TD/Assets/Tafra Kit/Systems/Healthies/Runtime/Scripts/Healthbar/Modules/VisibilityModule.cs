using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using TafraKit.MotionFactory;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Visibility")]
    public class VisibilityModule : HealthBarModule
    {
        [SerializeField] private bool hideWhileDead = true;

        [Header("References")]
        [SerializeField] private VisibilityMotionController motionController;

        private ControlReceiver hideControllers;
        private bool nextSwitchIsInstant;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            hideControllers = new ControlReceiver(OnFirstHideControllerAdded, null, OnAllHideControllersCleared);
        }
        protected override void OnEnable()
        {
            if(healthBar.Healthy.IsInitialized)
                OnHealthyInitialized();
            else
                healthBar.Healthy.Events.OnInitialize.AddListener(OnHealthyInitialized);
        }
        protected override void OnDisable()
        {
            healthBar.Healthy.Events.OnDeath.RemoveListener(OnDeath);
            healthBar.Healthy.Events.OnRevive.RemoveListener(OnRevive);
        }

        private void OnFirstHideControllerAdded()
        {
            motionController.Hide(nextSwitchIsInstant);
        }
        private void OnAllHideControllersCleared()
        {
            if(hideWhileDead && healthBar.Healthy.IsDead)
                return;

            motionController.Show(nextSwitchIsInstant);
        }

        private void OnHealthyInitialized()
        {
            healthBar.Healthy.Events.OnInitialize.RemoveListener(OnHealthyInitialized);

            healthBar.Healthy.Events.OnDeath.AddListener(OnDeath);
            healthBar.Healthy.Events.OnRevive.AddListener(OnRevive);

            if(hideWhileDead && healthBar.Healthy.IsDead)
                OnDeath(healthBar.Healthy, new HitInfo());
            else if (!hideControllers.HasAnyController())
                motionController.Show(true);
        }
        private void OnDeath(Healthy healthy, HitInfo killerHit)
        {
            if(hideWhileDead)
                AddHider("dead");
        }
        private void OnRevive()
        {
            if(hideWhileDead)
                RemoveHider("dead");
        }

        /// <summary>
        /// Hide the health bar.
        /// </summary>
        /// <param name="hiderID"></param>
        /// <param name="instant"></param>
        public void AddHider(string hiderID, bool instant = false)
        {
            nextSwitchIsInstant = instant;

            hideControllers.AddController(hiderID);
        }
        /// <summary>
        /// No longer make this hider contribute to hiding the health bar.
        /// </summary>
        /// <param name="hiderID"></param>
        /// <param name="instant"></param>
        public void RemoveHider(string hiderID, bool instant = false)
        {
            nextSwitchIsInstant = instant;

            hideControllers.RemoveController(hiderID);
        }
    }
}
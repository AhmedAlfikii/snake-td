using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.CharacterControls
{
    public class AbilityUISlot : MonoBehaviour
    {
        [SerializeField] private Image icon;

        [Header("Cooldown")]
        [SerializeField] private GameObject cooldownContainer;
        [SerializeField] private GameObject cooldownWithoutChargesContainer;
        [SerializeField] private GameObject cooldownWithChargesContainer;
        [Tooltip("The cooldown indicator that will appear if no charges are available to use in the ability")]
        [SerializeField] private Image cooldownIndicatorWithoutCharges;
        [Tooltip("The cooldown indicator that will appear if at least 1 charge is available to use in the ability")]
        [SerializeField] private Image cooldownIndicatorWithCharges;
        [SerializeField] private TextMeshProUGUI cooldownText;

        [Header("Charges")]
        [SerializeField] private GameObject chargesContainer;
        [SerializeField] private TextMeshProUGUI chargesCounter;

        private Ability ability;
        private bool hasCooldown;
        private bool containsCharges;
        private bool cooldownActive;
        /// <summary>
        /// Becomes true if at least 1 charge is available to use in the ability.
        /// </summary>
        private bool hasUsableCharges;
        private Sprite normalIcon;
        private Sprite lockedIcon;

        private void Update()
        {
            if (cooldownActive)
            {
                float cooldown = ability.Cooldown;
                float remainingCooldown = ability.RemainingCooldown;

                if (hasUsableCharges)
                    cooldownIndicatorWithCharges.fillAmount = remainingCooldown / cooldown;
                else
                    cooldownIndicatorWithoutCharges.fillAmount = remainingCooldown / cooldown;

                cooldownText.text = remainingCooldown.ToString("N1");
            }
        }

        private void OnDestroy()
        {
            if (ability != null)
                UnhookFromAbility(ability);
        }
        public void SetAbility(Ability ability)
        {
            if (this.ability == ability) 
                return;

            if(this.ability != null)
                UnhookFromAbility(this.ability);

            this.ability = ability;


            hasCooldown = ability.HasCooldown;
            containsCharges = ability.TotalCharges > 1;

            UpdateCharges();

            if(ability.IsOnCooldown)
                StartCooldown();

            chargesContainer.gameObject.SetActive(containsCharges);
            cooldownContainer.gameObject.SetActive(ability.IsOnCooldown);

            HookToAbility(ability);
        }

        private void HookToAbility(Ability abilityToHookTo)
        {
            abilityToHookTo.LoadIcons();

            normalIcon = abilityToHookTo.GetLoadedIcon(1);
            //lockedIcon = abilityToHookTo.RequestLockedIcon();

            abilityToHookTo.OnPerformed.AddListener(OnAbilityPerformed);
            abilityToHookTo.OnStartedCooldown.AddListener(OnStartedCooldown);
            abilityToHookTo.OnFinishedCooldown.AddListener(OnFinishedCooldown);
        }

        private void UnhookFromAbility(Ability abilityToUnhookFrom)
        {
            normalIcon = null;
            lockedIcon = null;

            abilityToUnhookFrom.ReleaseIcons();
            //abilityToUnhookFrom.ReleaseLockedIcon();

            abilityToUnhookFrom.OnPerformed.RemoveListener(OnAbilityPerformed);
            abilityToUnhookFrom.OnStartedCooldown.RemoveListener(OnStartedCooldown);
            abilityToUnhookFrom.OnFinishedCooldown.RemoveListener(OnFinishedCooldown);
        }

        private void OnAbilityPerformed()
        {
            if(!containsCharges)
                return;

            UpdateCharges();
        }
        private void OnStartedCooldown()
        {
            StartCooldown();
        }
        private void OnFinishedCooldown()
        {
            EndCooldown();
        }

        private void StartCooldown()
        {
            cooldownActive = true;

            cooldownContainer.gameObject.SetActive(true);
        }
        private void EndCooldown()
        {
            cooldownActive = false;

            cooldownContainer.gameObject.SetActive(false);

                UpdateCharges();
        }
        private void UpdateCharges()
        {
            int remainingCharges = ability.RemainingCharges;
            
            if(containsCharges)
                chargesCounter.text = remainingCharges.ToString();

            hasUsableCharges = remainingCharges > 0;

            cooldownWithChargesContainer.SetActive(hasUsableCharges);
            cooldownWithoutChargesContainer.SetActive(!hasUsableCharges);

            icon.sprite = hasUsableCharges ? normalIcon : lockedIcon;
        }
    }
}
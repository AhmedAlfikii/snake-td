using UnityEngine;
using TafraKit.Roguelike;
using TMPro;
using UnityEngine.UI;
using System;
using TafraKit.UI;

namespace TafraKit.Internal.Roguelike
{
    public class PerkCard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private UISkinReceiver skinReceiver;

        private Perk displayedPerk;
        private ZButton myButton;
        private Action onClick;

        public Perk DisplayedPerk => displayedPerk;

        private void Awake()
        {
            myButton = GetComponent<ZButton>();
        }
        private void OnEnable()
        {
            if (myButton != null)
                myButton.onClick.AddListener(OnClick);
        }
        private void OnDisable()
        {
            if (myButton != null)
                myButton.onClick.RemoveListener(OnClick);

            if(displayedPerk != null)
            {
                displayedPerk.ReleaseIcons();
                displayedPerk = null;
            }
        }

        private void OnClick()
        {
            onClick?.Invoke();
        }

        public void Populate(Perk perk, bool displayAsOffer, Action onClick)
        {
            if (displayedPerk != null)
                displayedPerk.ReleaseIcons();

            displayedPerk = perk;
            
            this.onClick = onClick;

            perk.LoadIcons();
            
            if(displayAsOffer)
            {
                if (title != null)
                    title.text = perk.OfferDisplayName;

                if (description != null)
                    description.text = perk.OfferDescription;

                if (icon != null)
                    icon.sprite = perk.GetLoadedOfferIcon();

                if (skinReceiver != null)
                    skinReceiver.ApplySkin(perk.OfferSkin);
            }
            else
            {
                if (title != null)
                    title.text = perk.AppliedDisplayName;

                if (description != null)
                    description.text = perk.AppliedDescription;

                if (icon != null)
                    icon.sprite = perk.GetLoadedAppliedIcon();

                if (skinReceiver != null)
                    skinReceiver.ApplySkin(perk.AppliedSkin);
            }

            OnPopulate(perk, displayAsOffer);
        }

        protected virtual void OnPopulate(Perk perk, bool displayAsOffer) { }
    }
}
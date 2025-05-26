using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.Consumables
{
    public class ConsumableUIBar : ScriptableFloatUIBar
    {
        [SerializeField] private Image iconIMG;

        [Header("Properties")]

        [Tooltip("Should this bar automatically signal that it's visible/invisible to the \"Consumable Bar Fetcher\" whenever it's enabled/disabled.")]
        [SerializeField] private bool autoSignalVisiblity;
        [SerializeField] private int iconIndex;

        public Consumable Consumable
        {
            get 
            {
                return scriptableFloat as Consumable;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (autoSignalVisiblity)
                ConsumablesBarFetcher.SignalBarVisible(this);

            if (Consumable)
            {
                RefreshIcon();
            }
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            if (autoSignalVisiblity)
                ConsumablesBarFetcher.SignalBarInvisible(this);
        }

        public override void Populate(ScriptableFloat sf)
        {
            base.Populate(sf);

            if (Consumable)
                RefreshIcon();
        }
        private void RefreshIcon()
        {
            iconIMG.sprite = Consumable.GetIcon(iconIndex);
        }

        public Transform GetIconRT()
        {
            return iconIMG.transform;
        }

        private void OnConsumableIconsUpdated(Consumable consumable)
        {
            RefreshIcon();
        }
    }
}
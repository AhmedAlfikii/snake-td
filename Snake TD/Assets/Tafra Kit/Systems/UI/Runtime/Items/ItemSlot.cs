using TafraKit.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TafraKit.Consumables;
using TafraKit.RPG;

namespace TafraKit
{
    public class ItemSlot : MonoBehaviour
    {

        #region Private Serialized Fields
        [SerializeField] private ItemSlotSkin skin;
        [SerializeField] private Button infoBtn;
        [SerializeField] private GameObject infoIcon;

        [Header("Testing")]
        [SerializeField] private ScriptableObject testItem;
        #endregion

        #region Private Serialized Fields
        private string targetName;
        private string targetDescription;
        private RectTransform myRt;
        #endregion

        #region Monobehaviour Messages
        private void Awake()
        {
            myRt = GetComponent<RectTransform>();
        }
        private void OnEnable()
        {
            if (infoBtn != null) 
                infoBtn.onClick.AddListener(ShowInfo);
        }
        private void OnDisable()
        {
            if (infoBtn != null)
                infoBtn.onClick.RemoveListener(ShowInfo);
        }
        #endregion

        #region Private Functions
        public void ShowInfo()
        {
            InfoBubbleHandler.Show(myRt, Side.Top, targetDescription, targetName);
        }
        #endregion

        #region Public Functions
        public void Populate(ScriptableObject item, int quantityOverride = -1, bool isAvailable = true)
        {
            skin.Populate(item, quantityOverride, isAvailable);

            if(item is Consumable consumable)
            {
                targetName = consumable.DisplayName;
                targetDescription = consumable.Description;
                if(infoIcon != null)
                    infoIcon.gameObject.SetActive(true);
                if(infoBtn != null)
                    infoBtn.gameObject.SetActive(!string.IsNullOrEmpty(targetDescription));
            }
            else if(item is Equipment equipment)
            {
                targetName = equipment.DisplayName;
                targetDescription = equipment.Description;
                if (infoIcon != null)
                    infoIcon.gameObject.SetActive(false);
                if (infoBtn != null)
                    infoBtn.gameObject.SetActive(!string.IsNullOrEmpty(targetDescription));
            }
            else if(item is IdentifiableScriptableObject idSO)
            {
                targetName = idSO.DisplayName;
                targetDescription = idSO.Description;
                if(infoIcon != null)
                    infoIcon.gameObject.SetActive(true);
                if(infoBtn != null)
                    infoBtn.gameObject.SetActive(!string.IsNullOrEmpty(targetDescription));
            }
        }
        #endregion

        #region Context Functions
        [ContextMenu("Apply Test Item")]
        private void ApplyTestItem()
        {
            Populate(testItem);
        }
        [ContextMenu("Apply Test Item Unavailable")]
        private void ApplyTestItemUnavailable()
        {
            Populate(testItem, -1, false);
        }
        #endregion
    }
}

using UnityEngine;
using TMPro;
using TafraKit.UI;
using UnityEngine.Events;
using ZUI;

namespace TafraKit.Internal.UI
{
    public class GenericPopupObject : MonoBehaviour
    {
        [SerializeField] protected UIElementsGroup popupUIEG;
        [Space(20)]
        [SerializeField] protected TextMeshProUGUI titleTXT;
        [SerializeField] protected TextMeshProUGUI infoTXT;
        [SerializeField] protected ZButton confirmBtn;
        [SerializeField] protected TextMeshProUGUI confirmBtnTXT;
        [SerializeField] protected ZButton cancelBtn;
        [SerializeField] protected TextMeshProUGUI cancelBtnTXT;

        public virtual void Show(string title, string info, string confirmButtonText = "Ok", string cancelButtonText = "Cancel",
            UnityAction OnConfirmAction = null, UnityAction OnCancelAction = null)
        {
            //Clear prev
            confirmBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.RemoveAllListeners();

            //Sets
            titleTXT.text = title;
            infoTXT.text = info;

            confirmBtnTXT.text = confirmButtonText;
            confirmBtn.onClick.AddListener(Hide);
            if (OnConfirmAction != null)
                confirmBtn.onClick.AddListener(OnConfirmAction);

            cancelBtnTXT.text = cancelButtonText;
            cancelBtn.onClick.AddListener(Hide);
            if (OnCancelAction != null)
            {
                cancelBtn.onClick.AddListener(OnCancelAction);

                if(!cancelBtn.gameObject.activeSelf)
                    cancelBtn.gameObject.SetActive(true);
            }
            else
            { //No cancel action, hide button
                if (cancelBtn.gameObject.activeSelf)
                    cancelBtn.gameObject.SetActive(false);
            }

            popupUIEG.ChangeVisibility(true);
        }

        public virtual void Hide()
        {
            popupUIEG.ChangeVisibility(false);
        }
    }
}
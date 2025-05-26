using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TafraKit
{
    public class ScriptableFloatFormulaLinkUIBar : MonoBehaviour
    {
        [SerializeField] protected ScriptableFloatFormulaLink formulaLink;
        [SerializeField] protected TextMeshProUGUI amountTXT;
        [SerializeField] protected bool displayAsCompact = true;
        [SerializeField] private string maxValuePrefix = "<size=80%>";
        [SerializeField] private Slider progressSlider;

        protected ScriptableFloat level;
        protected ScriptableFloat experience;
        protected int displayedValue;
        protected bool autoUpdate = true;

        public bool AutoUpdate
        {
            set => autoUpdate = value;
        }
        public TextMeshProUGUI AmountTXT => amountTXT;
        public int DisplayedValue
        {
            get => displayedValue;
            set
            {
                displayedValue = value;
                UpdateAmountTxt();
            }
        }
        private void Awake()
        {
            level = formulaLink.Level;
            experience = formulaLink.Experience;
        }
        protected virtual void OnEnable()
        {
            experience.OnDisplayValueChange.AddListener(OnDisplayValueChange);
            
            if(autoUpdate)
                DisplayedValue = experience.DisplayValueInt;
        }
        protected virtual void OnDisable()
        {
            experience.OnDisplayValueChange.RemoveListener(OnDisplayValueChange);
        }
        protected virtual void Start()
        {
            if(autoUpdate)
                DisplayedValue = experience.DisplayValueInt;
        }
        protected virtual void OnDisplayValueChange(float value)
        {
            if(!autoUpdate)
                return;

            DisplayedValue = Mathf.RoundToInt(value);
        }
        protected virtual void UpdateAmountTxt()
        {
            float cap = formulaLink.GetRequiredExp();

            if(amountTXT != null)
            {
                amountTXT.text = displayAsCompact ? $"{displayedValue.ToCompactNumberString()}{maxValuePrefix} / {cap.ToCompactNumberString()}"
                    : $"{displayedValue}{maxValuePrefix} / {cap}";
            }

            if(progressSlider != null && cap > 0)
                progressSlider.value = displayedValue / cap;
        }
    }
}
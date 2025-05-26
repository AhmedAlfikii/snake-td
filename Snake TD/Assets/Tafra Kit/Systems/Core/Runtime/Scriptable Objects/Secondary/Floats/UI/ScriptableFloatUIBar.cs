using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

namespace TafraKit
{
    public class ScriptableFloatUIBar : MonoBehaviour
    {
        [SerializeField] protected ScriptableFloat scriptableFloat;
        [SerializeField] protected TextMeshProUGUI amountTXT;
        [SerializeField] protected bool displayAsCompact = true;
        [SerializeField] protected bool addPrefix;
        [SerializeField] protected string prefix;

        [Header("Cap")]
        [SerializeField] private bool displayCap;
        [SerializeField] private string capPrefix = "<size=80%>";
        [SerializeField] private Slider capProgressSlider;

        protected int displayedValue;
        protected StringBuilder sb = new StringBuilder();
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

        protected virtual void OnEnable()
        {
            scriptableFloat.OnDisplayValueChange.AddListener(OnDisplayValueChange);
            
            if(autoUpdate)
                DisplayedValue = scriptableFloat.DisplayValueInt;
        }
        protected virtual void OnDisable()
        {
            scriptableFloat.OnDisplayValueChange.RemoveListener(OnDisplayValueChange);
        }
        protected virtual void Start()
        {
            if(autoUpdate)
                DisplayedValue = scriptableFloat.DisplayValueInt;
        }
        protected virtual void OnDisplayValueChange(float value)
        {
            if(!autoUpdate)
                return;
            
            DisplayedValue = Mathf.RoundToInt(value);
        }
        protected virtual void UpdateAmountTxt()
        {
            sb.Clear();

            if(addPrefix)
                sb.Append(prefix);

            if(!displayCap)
            {
                if(displayAsCompact)
                    sb.Append(displayedValue.ToCompactNumberString());
                else
                    sb.Append(displayedValue.ToString());

                amountTXT.text = sb.ToString();
            }
            else
            {
                float cap = scriptableFloat.GetCap();

                if(displayAsCompact)
                {
                    sb.Append(displayedValue.ToCompactNumberString());
                    sb.Append(capPrefix);
                    sb.Append(" / ");
                    sb.Append(cap.ToCompactNumberString());
                }
                else
                {
                    sb.Append(displayedValue.ToString());
                    sb.Append(capPrefix);
                    sb.Append(" / ");
                    sb.Append(cap);
                }

                if(capProgressSlider != null)
                    capProgressSlider.value = displayedValue / cap;
            }
        }
        public virtual void Populate(ScriptableFloat sf)
        {
            if(scriptableFloat != null)
                scriptableFloat.OnDisplayValueChange.RemoveListener(OnDisplayValueChange);
            
            scriptableFloat = sf;
            
            scriptableFloat.OnDisplayValueChange.AddListener(OnDisplayValueChange);

            DisplayedValue = scriptableFloat.DisplayValueInt;
        }
    }
}
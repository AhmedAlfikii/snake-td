using System;
using System.Text;
using TafraKit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.RPG
{
    public class StatSlotUI : MonoBehaviour
    {
        [SerializeField] private bool initializeOnStart;
        [SerializeField] private Stat stat;
        [SerializeField] private bool statsContainerIsPlayer = true;
        [SerializeField] private bool updateOnEnable = true;
        [SerializeField] private bool autoUpdateOnChange = true;

        [Header("Display Properties")]
        [SerializeField] private IntRounding rounding;
        [SerializeField] private bool displayAsPercentage;
        [SerializeField] private int statIconIndex;
        [SerializeField] private int statDisplayNameIndex;
        [Tooltip("Should the stat value text color change if there's an extra value in the stat?")]
        [SerializeField] private bool displayExtraValueColor = true;
        [SerializeField] private Color extraValuePositiveColor = Color.green;
        [SerializeField] private Color extraValueNegativeColor = Color.red;

        [Header("UI")]
        [SerializeField] private Image statIcon;
        [SerializeField] private TextMeshProUGUI statName;
        [SerializeField] private TextMeshProUGUI statValue;

        private StatsContainer statsContainer;
        private Stat displayedStat;
        private ZButton myButton;
        private StringBuilder sb = new StringBuilder();
        private bool isInitialized;
        private Action onClick;

        public Stat Stat => stat;

        private void Awake()
        {
            myButton = GetComponent<ZButton>();
        }
        private void OnEnable()
        {
            if(stat == null)
                return;

            PopulateIdentification();

            if(statsContainer != null)
            {
                if(updateOnEnable)
                    UpdateStat();

                if(autoUpdateOnChange)
                    statsContainer.OnStatUpdated.AddListener(OnStatUpdated);
            }

            if(myButton)
                myButton.onClick.AddListener(OnButtonClick);
        }
        private void OnDisable()
        {
            if(stat == null)
                return;

            stat.ReleaseIcon(statIconIndex);

            if(statsContainer != null)
            {
                if(autoUpdateOnChange)
                    statsContainer.OnStatUpdated.RemoveListener(OnStatUpdated);
            }

            if(myButton)
                myButton.onClick.RemoveListener(OnButtonClick);
        }
        private void Start()
        {
            if(initializeOnStart && !isInitialized)
                Initialize();
        }

        private void OnStatUpdated(Stat stat, float newValue)
        {
            if(stat != this.stat)
                return;

            UpdateStat();
        }
        private void OnButtonClick()
        {
            onClick?.Invoke();
        }

        private void PopulateIdentification()
        {
            statIcon.sprite = stat.RequestIcon(statIconIndex);
            statName.text = stat.GetDisplayName(statDisplayNameIndex);
        }
        private void UpdateStat()
        {
            sb.Clear();

            float totalValue = statsContainer.GetStatTotalValue(stat);
            float extraValue = statsContainer.GetStatExtraValue(stat);

            if(displayAsPercentage)
                totalValue *= 100;

            switch(rounding)
            {
                case IntRounding.None:
                    sb.Append(totalValue);
                    break;
                case IntRounding.Round:
                    sb.Append(Mathf.RoundToInt(totalValue));
                    break;
                case IntRounding.RoundUp:
                    sb.Append(Mathf.CeilToInt(totalValue));
                    break;
                case IntRounding.RoundDown:
                    sb.Append(Mathf.FloorToInt(totalValue));
                    break;
            }

            if(displayAsPercentage)
                sb.Append('%');

            statValue.text = sb.ToString();

            if(displayExtraValueColor && (extraValue > 0 || extraValue < 0))
                statValue.color = extraValue > 0 ? extraValuePositiveColor : extraValueNegativeColor;
        }

        public void Initialize(Action onClick = null)
        {
            this.onClick = onClick;

            if(!isInitialized)
                return;

            if(stat == null)
                return;

            if(!statsContainerIsPlayer)
                Debug.LogError("This isn't handled. Handle it");

            if(statsContainerIsPlayer)
            {
                statsContainer = SceneReferences.Player.StatsContainer;
            }

            if(updateOnEnable)
                UpdateStat();

            if(autoUpdateOnChange)
                statsContainer.OnStatUpdated.AddListener(OnStatUpdated);

            isInitialized = true;
        }
        public void SetStat(Stat stat)
        { 
            this.stat = stat;

            if(statsContainerIsPlayer)
            {
                statsContainer = SceneReferences.Player.StatsContainer;
            }
        }
        public void SetRounding(IntRounding rounding)
        { 
            this.rounding = rounding;
        }
        public void SetDisplayAsPercentage(bool displayAsPercentage)
        { 
            this.displayAsPercentage = displayAsPercentage;
        }
    }
}
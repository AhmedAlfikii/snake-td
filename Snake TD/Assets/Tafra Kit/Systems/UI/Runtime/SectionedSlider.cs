using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(Slider))]
    public class SectionedSlider : MonoBehaviour
    {
        [System.Serializable]
        public struct Separator
        {
            [Range(0, 1)]
            public float Placement;
            [Range(0, 1)]
            public float ForcedVisualPlacement;
            [Range(0, 1)]
            public float Size;

            public Separator(float placement, float size, float forcedVisualPlacement = 0)
            {
                Placement = placement;
                ForcedVisualPlacement = forcedVisualPlacement;
                Size = size;
            }
        }
        [System.Serializable]
        public struct Section
        {
            public float SectionStart;
            public float SectionEnd;
            public float SectionSize;
            public float SectionVisualStart;
            public float SectionVisualEnd;
            public float SectionVisualSize;
        }
        [SerializeField] private bool initializeOnAwake = true;
        [SerializeField] private List<Separator> separators = new List<Separator>();
        [Tooltip("Should the separators be visually placed based on the visual placement? If false, they will be placed based on their actual placement.")]
        [SerializeField] private bool useVisualPlacement;

        [SerializeField, Range(0, 1)] private float value;
        
        private Slider slider;
        [SerializeField] private List<Section> sections = new List<Section>();
        //private float halfSeparatorSize;

        public bool UseVisualPlacement => useVisualPlacement;

        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                Set(value);
            }
        }

        private void Awake()
        {
            slider = GetComponent<Slider>();

            if (initializeOnAwake)
                Initialize();
        }

        #if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
                Set(value);
        }
        #endif
        private void Set(float input)
        {
            if(!slider)
                return;

            value = input;

            float remappedValue = input;

            for(int i = 0; i < sections.Count; i++)
            {
                Section section = sections[i];

                //If the input falls in this section.
                if(input >= section.SectionStart && input <= section.SectionEnd)
                {

                    remappedValue = section.SectionVisualStart + ((input - section.SectionStart) / section.SectionSize) * section.SectionVisualSize;

                    break;
                }
            }

            slider.value = remappedValue;
        }

        [ContextMenu("Initialize")]
        public void Initialize()
        {
            sections.Clear();
            for (int i = 0; i < separators.Count; i++)
            {
                Separator separator = separators[i];

                float halfSize = separator.Size / 2f;

                float sectionVisualSize = 0;
                float sectionStart = 0;
                float sectionVisualStart = 0;

                //If this is the first separator;
                if (i == 0)
                {
                    if (separator.Placement > halfSize)
                    {
                        if (!useVisualPlacement)
                            sectionVisualSize = separator.Placement - halfSize;
                        else
                            sectionVisualSize = separator.ForcedVisualPlacement - halfSize;
                    }
                }
                else
                {
                    Separator previousSeparator = separators[i - 1];
                    float previousSeparatorHalfSize = previousSeparator.Size / 2f;

                    if (separator.Placement > previousSeparator.Placement + previousSeparatorHalfSize)
                    {
                        sectionStart = previousSeparator.Placement;

                        sectionVisualStart = (useVisualPlacement? previousSeparator.ForcedVisualPlacement : sectionStart) + previousSeparatorHalfSize;

                        if (!useVisualPlacement)
                            sectionVisualSize = separator.Placement - halfSize - sectionVisualStart;
                        else
                            sectionVisualSize = separator.ForcedVisualPlacement - halfSize - sectionVisualStart;
                    }
                }

                //If the separator is stuck next to a separator before it or the begining of the slider and there's no space inbetween, then it won't form a section.
                if (sectionVisualSize <= 0)
                    continue;

                float sectionEnd = separator.Placement;
                float sectionVisualEnd = (useVisualPlacement? separator.ForcedVisualPlacement : separator.Placement) - halfSize;

                Section section = new Section();
                section.SectionStart = sectionStart;
                section.SectionEnd = sectionEnd;
                section.SectionSize = sectionEnd - sectionStart;
                section.SectionVisualSize = sectionVisualSize;
                section.SectionVisualStart = sectionVisualStart;
                section.SectionVisualEnd = sectionVisualEnd;
                sections.Add(section);
            }

            //Check if there's a viable section after the last separator
            if (separators.Count > 0)
            {
                Separator lastSeparator = separators[separators.Count - 1];

                float sectionVisualStart = lastSeparator.Placement + (lastSeparator.Size / 2f);

                if (sectionVisualStart < 1)
                {
                    Section section = new Section();

                    section.SectionStart = lastSeparator.Placement;
                    section.SectionEnd = section.SectionVisualEnd = 1;
                    section.SectionSize = 1 - lastSeparator.Placement;
                    section.SectionVisualStart = sectionVisualStart;
                    section.SectionVisualSize = 1 - sectionVisualStart;

                    sections.Add(section);
                }
            }
        }

        public void CreateSeparators(List<Separator> newSeparators)
        {
            separators.Clear();

            separators.AddRange(newSeparators);

            Initialize();
        }
        public void SetSeparatorPlacement(int separatorIndex, float placement)
        {
            separators[separatorIndex] = new Separator(placement, separators[separatorIndex].Size, separators[separatorIndex].ForcedVisualPlacement);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public class GameSectionsContainer : GameSection
    {
        [SerializeField] protected string id;
        [SerializeField] protected bool saveAndLoadActiveSections = true;
        [SerializeField] protected List<GameSection> sections;

        [SerializeField] protected UnityEvent<GameSection> onSubSectionStart;
        [SerializeField] protected UnityEvent<GameSection> onSubSectionComplete;

        protected int activeSectionIndex = -1;
        protected const string activeSectionPrefsKey = "ACTIVE_GAME_SECTION";

        public int ActiveSectionIndex
        {
            get => activeSectionIndex;
            private set
            {
                activeSectionIndex = value;

                if(saveAndLoadActiveSections)
                    PlayerPrefs.SetInt(ActiveSectionPrefsKey(), activeSectionIndex);
            }
        }
        public List<GameSection> Sections => sections;
        public UnityEvent<GameSection> OnSubSectionStart => onSubSectionStart;
        public UnityEvent<GameSection> OnSubSectionComplete => onSubSectionComplete;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            for(int i = 0; i < sections.Count; i++)
            {
                sections[i].Initialize();
            }
        }
        protected override void OnStarted()
        {
            base.OnStarted();

            int savedActiveSection = saveAndLoadActiveSections ? PlayerPrefs.GetInt(ActiveSectionPrefsKey()) : -1;

            bool loaded = savedActiveSection != -1;

            if(savedActiveSection < 0)
                savedActiveSection = 0;

            if(savedActiveSection < sections.Count)
            {
                StartSubSection(savedActiveSection);

                if(loaded)
                    sections[savedActiveSection].RaiseLoadedFlag();
            }
            else
                Complete();
        }
        private void OnActiveSectionComplete()
        {
            GameSection activeSection = sections[ActiveSectionIndex];

            activeSection.OnComplete.RemoveListener(OnActiveSectionComplete);

            onSubSectionComplete?.Invoke(activeSection);

            StartNextSectionOrComplete();
        }
        private string ActiveSectionPrefsKey()
        {
            if (!string.IsNullOrEmpty(id))
                return $"{id}_{activeSectionPrefsKey}";
            else
                return $"{gameObject.name}_{activeSectionPrefsKey}";
        }

        public void StartSubSection(int sectionIndex)
        {

            ActiveSectionIndex = sectionIndex;

            GameSection section = sections[sectionIndex];
            
            section.OnComplete.AddListener(OnActiveSectionComplete);
            section.StartSection();

            OnSubSectionStart?.Invoke(section);
        }
        public void StartNextSectionOrComplete()
        {
            ActiveSectionIndex++;

            bool nextSectionExist = ActiveSectionIndex < sections.Count;

            if(nextSectionExist)
                StartSubSection(ActiveSectionIndex);
            else
                Complete();
        }
        public override void ResetSavedData()
        {
            base.ResetSavedData();

            if(saveAndLoadActiveSections)
                PlayerPrefs.DeleteKey(ActiveSectionPrefsKey());

            for(int i = 0; i < sections.Count; i++)
            {
                sections[i].ResetSavedData();
            }
        }

        public GameSection GetActiveSubSection()
        {
            return sections[activeSectionIndex];
        }
        public override List<GameSection> GetSubSections()
        {
            return sections;
        }
    }
}
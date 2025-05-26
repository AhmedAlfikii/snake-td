using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    /// <summary>
    /// Holds a list of sections, and uses only the active ones in it.
    /// </summary>
    public class GameSectionsContainerActiveOnly : GameSectionsContainer
    {
        [SerializeField] private GameSection[] sectionsToProcess;
        [SerializeField] private GameSection[] tailingSections;

        protected override void OnInitialized()
        {
            for (int i = 0; i < sectionsToProcess.Length; i++)
            {
                if (sectionsToProcess[i].gameObject.activeInHierarchy)
                    sections.Add(sectionsToProcess[i]);
            }

            for(int i = 0; i < tailingSections.Length; i++)
            {
                if(tailingSections[i].gameObject.activeInHierarchy)
                    sections.Add(tailingSections[i]);
            }

            base.OnInitialized();
        }

        public void AddSection(GameSection section)
        {
            sections.Add(section);
        }
    }
}
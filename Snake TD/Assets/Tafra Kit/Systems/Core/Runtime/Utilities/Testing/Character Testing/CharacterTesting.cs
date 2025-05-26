using System.Collections.Generic;
using TafraKit.ModularSystem;
using UnityEngine;

namespace TafraKit
{
    public class CharacterTesting : InternallyModularComponent<CharacterTestingModule>
    {
        [SerializeReferenceListContainer("modules", false, "Module", "Modules")]
        [SerializeField] private CharacterTestingModulesContainer modulesContainer;

        protected override List<CharacterTestingModule> InternalModules => modulesContainer.Modules;

        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < modulesCount; i++)
            {
                allModules[i].Initialize(this);
            }
        }
    }
}
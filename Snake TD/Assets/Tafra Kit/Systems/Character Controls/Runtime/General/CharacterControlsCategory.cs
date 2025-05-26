using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [Serializable]
    public class CharacterControlsCategory
    {
        public TafraString Category;
        [Tooltip("ITafraPlayable objects that are this category's controls.")]
        public UnityEngine.Object[] TafraPlayableObjects;

        private List<ITafraPlayable> controlPlayables;

        public List<ITafraPlayable> ControlPlayables => controlPlayables;

        public void Initialize()
        {
            controlPlayables = new List<ITafraPlayable>();

            for(int i = 0; i < TafraPlayableObjects.Length; i++)
            {
                if(TafraPlayableObjects[i] == null)
                    continue;

                ITafraPlayable playable = ZHelper.ExtractClass<ITafraPlayable>(TafraPlayableObjects[i]);

                if (playable == null) 
                    continue;

                controlPlayables.Add(playable);
            }
        }
    }
}
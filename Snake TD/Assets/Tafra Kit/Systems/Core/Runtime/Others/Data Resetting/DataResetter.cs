using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class DataResetter : MonoBehaviour
    {
        [SerializeField] private List<Object> resettableObjects;

        private List<IResettable> resettables = new List<IResettable>();

        private void Awake()
        {
            for(int i = 0; i < resettableObjects.Count; i++)
            {
                IResettable resettable= ZHelper.ExtractClass<IResettable>(resettableObjects[i]);

                if(resettable != null)
                    resettables.Add(resettable);
            }
        }
        public void ResetData()
        {
            for(int i = 0; i < resettables.Count; i++)
            {
                resettables[i].ResetSavedData();
            }
        }
    }
}
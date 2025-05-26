using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.UI;

namespace TafraKit.UI
{
    [System.Serializable]
    [SearchMenuItem("Swappable GameObjects")]
    public class SwappableGameObjects : UISwappableValues
    {
        [SerializeField] protected List<GameObject> gameObjects;

        public List<GameObject> GameObjects => gameObjects;

        protected override void OnStateChange(int stateIndex)
        {
            if (gameObjects == null)
                return;

            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].SetActive(i == stateIndex);
            }
        }
    }
}
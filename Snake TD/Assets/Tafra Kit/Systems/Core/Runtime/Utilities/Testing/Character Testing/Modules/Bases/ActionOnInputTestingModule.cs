using TafraKit.ModularSystem;
using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public abstract class ActionOnInputTestingModule : CharacterTestingModule
    {
        [SerializeField] private KeyCode inputKey;
        [Tooltip("This key must be held down when the input key was pressed for this action to trigger.")]
        [SerializeField] private KeyCode secondaryKey;

        public override bool UseUpdate => true;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        public override void Update()
        {
            if(Input.GetKeyDown(inputKey) && (secondaryKey == KeyCode.None || Input.GetKey(secondaryKey)))
            {
                OnInputReceived();
            }
        }

        protected abstract void OnInputReceived();
    }
}
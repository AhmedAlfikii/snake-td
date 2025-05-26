using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [System.Serializable]
    public abstract class AbilitySystemObjectInitiailzer
    {
        [SerializeField] private string propertyName;

        protected int propertyNameHash;

        public void Apply(GraphBlackboard blackboard)
        {
            propertyNameHash = Animator.StringToHash(propertyName);

            var prop = blackboard.TryGetSystemObjectProperty(propertyNameHash, -1);

            prop.value = GetObject();

        }
        public abstract object GetObject();
    }
}
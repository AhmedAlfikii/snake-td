using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [System.Serializable]
    public abstract class AbilityUpgradeableProperty
    {
        [SerializeField] private string propertyName;

        protected int propertyNameHash;

        public void Initialize(GraphBlackboard blackboard)
        {
            propertyNameHash = Animator.StringToHash(propertyName);

            OnInitialize(blackboard);
        }
        public virtual void OnInitialize(GraphBlackboard blackboard) { }
        public abstract void UpdateValue(int level);
    }
}
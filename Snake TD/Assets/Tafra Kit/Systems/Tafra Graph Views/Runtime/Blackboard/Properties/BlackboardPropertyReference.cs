using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal.GraphViews;
using UnityEngine;

namespace TafraKit.GraphViews
{
    public class BlackboardPropertyReference
    {
        [Tooltip("The target blackboard that we should look for this property inside. " +
            "If both is selected, the internal blackboard will be checked first, if a property was found, it will be used. " +
            "The external blackboard in this case won't be checked unless the property wasn't found in the internal one..")]
        [SerializeField] protected TargetBlackboard targetBlackboard;
        [Tooltip("The name of the property inside the target blackboard.")]
        [SerializeField] protected string blackboardPropertyName;
        [SerializeField] protected int internalBlackboardPropertyID = -1;

        [NonSerialized] protected BlackboardCollection blackboardCollection;
        [NonSerialized] public SecondaryBlackboard secondaryBlackboard;
        [NonSerialized] protected int blackboardPropertyNameHash;

        public int PropertyNameHash => blackboardPropertyNameHash;
        [Tooltip("The target blackboard that we should look for this property inside. " +
        "If both is selected, the internal blackboard will be checked first, if a property was found, it will be used. " +
        "The external blackboard in this case won't be checked unless the property wasn't found in the internal one..")]
        public TargetBlackboard MyTargetBlackboard => targetBlackboard;

        public BlackboardPropertyReference() { }
        public BlackboardPropertyReference(BlackboardPropertyReference other)
        {
            targetBlackboard = other.targetBlackboard;
            blackboardPropertyName = other.blackboardPropertyName;
            internalBlackboardPropertyID = other.internalBlackboardPropertyID;
            blackboardCollection = other.blackboardCollection;
            secondaryBlackboard = other.secondaryBlackboard;
            blackboardPropertyNameHash = other.blackboardPropertyNameHash;
        }

        public void Initialize(BlackboardCollection blackboardCollection)
        {
            this.blackboardCollection = blackboardCollection;
            blackboardPropertyNameHash = Animator.StringToHash(blackboardPropertyName);
        }
        public void SetSecondaryBlackboard(SecondaryBlackboard secondaryBlackboard)
        {
            this.secondaryBlackboard = secondaryBlackboard;
        }
    }
}
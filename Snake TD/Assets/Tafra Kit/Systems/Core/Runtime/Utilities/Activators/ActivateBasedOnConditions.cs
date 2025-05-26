using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Conditions;

namespace TafraKit
{
    public class ActivateBasedOnConditions : MonoBehaviour
    {
        [Tooltip("List of Game Objects that should be activated if the conditions were met on awake.")]
        [SerializeField] private GameObject[] targetsToActivate;
        [Tooltip("List of Game Objects that should be deactivated if the conditions were met on awake.")]
        [SerializeField] private GameObject[] targetsToDeactivate;
        [SerializeField] private ConditionsGroup conditions;
        [Tooltip("If conditions are not met, the targets to activate will be disabled, and the targets to deactivate will be enabled.")]
        [SerializeField] private bool revertStatesIfNotMet;

        [Header("Editor")]
        [SerializeField] private bool editorForceCondition;
        [SerializeField] private bool editorConditionState = true;

        private void Awake()
        {
            conditions.Activate();
            bool conditionsMet = conditions.Check();

            #if UNITY_EDITOR
            if(editorForceCondition)
                conditionsMet = editorConditionState;
            #endif

            if(conditionsMet)
            {
                for(int i = 0; i < targetsToActivate.Length; i++)
                {
                    targetsToActivate[i].SetActive(true);
                }
                for(int i = 0; i < targetsToDeactivate.Length; i++)
                {
                    targetsToDeactivate[i].SetActive(false);
                }
            }
            else if(revertStatesIfNotMet)
            {
                for(int i = 0; i < targetsToActivate.Length; i++)
                {
                    targetsToActivate[i].SetActive(false);
                }
                for(int i = 0; i < targetsToDeactivate.Length; i++)
                {
                    targetsToDeactivate[i].SetActive(true);
                }
            }

            conditions.Deactivate();
        }
    }
}
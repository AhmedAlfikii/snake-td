using UnityEngine;
using TafraKit.AI3;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Tasks/Character Controller/Control Category Toggle"), GraphNodeName("Control Category Toggle")]
    public class ControlCategoryToggleTask : TaskNode
    {
        [SerializeField] private string toggleID = "ControlCategoryToggleTask";
        [SerializeField] private BlackboardActorGetter targetActor;
        [SerializeField] private BlackboardScriptableObjectGetter targetControlCategory;
        [SerializeField] private BlackboardBoolGetter targetToggleValue;
        
        private ICharacterController characterController;
        private ScriptableString controlCategory;
  
        protected override void OnInitialize()
        {
            targetActor.Initialize(agent.BlackboardCollection);
            targetControlCategory.Initialize(agent.BlackboardCollection);
            
            controlCategory = (ScriptableString) targetControlCategory.Value;
        }
        protected override void OnStart()
        {
            if(characterController == null)
                characterController = targetActor.Value.GetComponent<ICharacterController>();
            
            if (characterController != null)
                characterController.ToggleControlCategory(controlCategory.Value,targetToggleValue.Value,toggleID);
            else
                TafraDebugger.Log("Control Category Toggle Task","The assigned does not contain ICharacterController", TafraDebugger.LogType.Error);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            if (characterController != null)
                characterController.ToggleControlCategory(controlCategory.Value,!targetToggleValue.Value,toggleID);
        }
    }
}
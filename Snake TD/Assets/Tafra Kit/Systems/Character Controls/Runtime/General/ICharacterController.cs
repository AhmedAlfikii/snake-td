using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    public interface ICharacterController
    {
        public Dictionary<string, List<ITafraPlayable>> ControlsCategories { get; }
        public List<string> AllCategories { get; }
        public HashSet<int> ActiveActionHashes { get; }
        public bool CanPerformNewAction { get; protected set; }

        public void ToggleControlCategory(string controlCategory, bool on, string togglerID)
        {
            if(ControlsCategories.TryGetValue(controlCategory, out List<ITafraPlayable> playables))
            {
                for(int i = 0; i < playables.Count; i++)
                {
                    if (on)
                        playables[i].Resume(togglerID);
                    else
                        playables[i].Pause(togglerID);
                }
            }
        }
        public void ToggleAllControlCategories(bool on, string togglerID)
        {
            for (int i = 0;i < AllCategories.Count;i++) 
            {
                ToggleControlCategory(AllCategories[i], on, togglerID);
            }
        }

        public bool StartPerformingAction(int actionHash)
        {
            if(!CanPerformNewAction)
                return false;

            ActiveActionHashes.Add(actionHash);

            CanPerformNewAction = false;
            
            return true;
        }
        public void FinishPerformingAction(int actionHash)
        {
            ActiveActionHashes.Remove(actionHash);

            if(ActiveActionHashes.Count == 0)
                CanPerformNewAction = true;
        }

    }
}
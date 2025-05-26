using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;
using UnityEngine.UIElements;
using TafraKit.Internal.CharacterControls;
using TafraKitEditor.GraphViews;

namespace TafraKitEditor.CharacterControls
{
    public class AbilityNodeEditor : BTNodeEditor
    {
        public AbilityNodeEditor(BTNode btNode) : base(btNode, "Assets/Tafra Kit/Systems/Artificial Intelligence V3/Editor/USS & UXML/BTNodeEditor.uxml")
        {
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            bool addedLocalActions = false;

            if(btNode is not PassiveAbilityNode)
            {
                evt.menu.AppendAction("Rename", a => Rename());
                addedLocalActions = true;
            }

            if(addedLocalActions)
                evt.menu.AppendSeparator();

            base.BuildContextualMenu(evt);
        }
    }
}
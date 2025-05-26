using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.AI3;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TafraKit.GraphViews;

namespace TafraKitEditor.AI3
{
    [CustomEditor(typeof(AIAgent), true), CanEditMultipleObjects]
    public class AIAgentEditor : Editor
    {
        protected VisualElement root;

        protected AIAgent agent;
        protected List<ExposableProperty> blackboardTrackedProperties = new List<ExposableProperty>();
        protected List<ExposableProperty> tempProperties = new List<ExposableProperty>();

        protected SerializedProperty brainProperty;
        protected PropertyField brainPF;
        protected bool skipNextBrainPFChange;

        protected virtual void OnEnable()
        {
            agent = target as AIAgent;

            blackboardTrackedProperties.Clear();

            brainProperty = serializedObject.FindProperty("brain");

            if(brainProperty.objectReferenceValue != null)
                CopyBlackboardProperties();

            RefreshExposedBlackboard();
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(GUILayout.Button("Refresh Exposed Blackboard"))
                RefreshExposedBlackboard();
        }

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();

            root.schedule.Execute(() =>
            {
                if(brainProperty.objectReferenceValue != null && IsBrainBlackboardDirty())
                {
                    RefreshExposedBlackboard();
                    CopyBlackboardProperties();
                }
            }).Every(1000);

            Draw();

            return root;
        }
        protected virtual void Draw()
        {
            serializedObject.Update();

            brainPF = new PropertyField(brainProperty);
            brainPF.style.marginTop = 10;
            brainPF.style.marginBottom = 2;

            brainPF.RegisterValueChangeCallback((ev) => {
                if (skipNextBrainPFChange)
                {
                    skipNextBrainPFChange = false;
                    return;
                }

                skipNextBrainPFChange = true;
                RefreshExposedBlackboard();
            });

            PropertyField playOnEnablePF = new PropertyField(serializedObject.FindProperty("playOnEnable"));
            CollapsibleContainer exposedBlackboardContainer  = new CollapsibleContainer("Exposed Properties", serializedObject.FindProperty("exposedBlackboard"), true, true);
            exposedBlackboardContainer.name = "exposed-blackboard";
            PropertyField externalBlackboardsPF = new PropertyField(serializedObject.FindProperty("externalBlackboards"));
            PropertyField cachedComponentsPF = new PropertyField(serializedObject.FindProperty("cachedComponents"));
            PropertyField crCachedComponentsPF = new PropertyField(serializedObject.FindProperty("componentProviderCachedComponents"));
            PropertyField modulesContainerPF = new PropertyField(serializedObject.FindProperty("modulesContainer"));

            root.Add(brainPF);
            root.Add(playOnEnablePF);
            root.Add(exposedBlackboardContainer);
            root.Add(externalBlackboardsPF);
            root.Add(cachedComponentsPF);
            root.Add(crCachedComponentsPF);
            root.Add(modulesContainerPF);

            skipNextBrainPFChange = true;
            root.Bind(serializedObject);
        }

        protected void RefreshExposedBlackboard()
        {
            if(Application.isPlaying)
                return;

            if(target == null)
                return;

            Undo.RecordObject(target, "Refresh Exposed Blackboard");

            bool changed = agent.RefreshExposedProperties();

            if (changed)
                EditorUtility.SetDirty(target);

            //Redraw inspector so that the exposed properties property drawer refreshes.
            if (changed && root != null)
            {
                root.Clear();
                Draw();
            }
        }
        protected void CopyBlackboardProperties()
        { 
            Brain brain = (Brain)brainProperty.objectReferenceValue;

            brain.Blackboard.GetAllProperties(tempProperties);

            blackboardTrackedProperties.Clear();
            for (int i = 0; i < tempProperties.Count; i++)
            {
                var prop = tempProperties[i];
                ExposableProperty newProperty = new ExposableProperty(prop.name, prop.tooltip, prop.ID) { expose = prop.expose };
                blackboardTrackedProperties.Add(newProperty);
            }
        }
        protected bool IsBrainBlackboardDirty()
        {
            Brain brain = (Brain)brainProperty.objectReferenceValue;
          
            brain.Blackboard.GetAllProperties(tempProperties);

            //Properties count don't match. Blackboard is dirty.
            if(tempProperties.Count != blackboardTrackedProperties.Count)
                return true;

            for (int i = 0; i < tempProperties.Count; i++)
            {
                var prop = tempProperties[i];
                int trackedPropIndex = GetPropIndexByID(blackboardTrackedProperties, prop.ID);

                //This property no longer exist. Blackboard is dirty.
                if(trackedPropIndex == -100)
                    return true;

                var trackedProp = blackboardTrackedProperties[trackedPropIndex];

                //Property name don't match. Blackboard is dirty.
                if(prop.name != trackedProp.name)
                    return true;
                //Property expose state don't match. Blackboard is dirty.
                if(prop.expose != trackedProp.expose)
                    return true;
            }

            return false;
        }
        protected int GetPropIndexByID(List<ExposableProperty> exposableProperties, int id)
        {
            for (int i = 0; i < exposableProperties.Count; i++)
            {
                if (exposableProperties[i].ID == id)
                    return i;
            }

            return -100;
        }
    }
}
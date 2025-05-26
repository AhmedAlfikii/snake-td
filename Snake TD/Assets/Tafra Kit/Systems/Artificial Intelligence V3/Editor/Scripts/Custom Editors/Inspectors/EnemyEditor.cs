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
    [CustomEditor(typeof(Enemy), true), CanEditMultipleObjects]
    public class EnemyEditor : AIAgentEditor
    {
        protected override void Draw()
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
            PropertyField externalStatsContainerPF = new PropertyField(serializedObject.FindProperty("externalStatsContainer"));
            PropertyField cachedComponentsPF = new PropertyField(serializedObject.FindProperty("cachedComponents"));
            PropertyField crCachedComponentsPF = new PropertyField(serializedObject.FindProperty("componentProviderCachedComponents"));
            PropertyField modulesContainerPF = new PropertyField(serializedObject.FindProperty("modulesContainer"));

            root.Add(brainPF);
            root.Add(playOnEnablePF);
            root.Add(exposedBlackboardContainer);
            root.Add(externalBlackboardsPF);
            root.Add(externalStatsContainerPF);
            root.Add(cachedComponentsPF);
            root.Add(crCachedComponentsPF);
            root.Add(modulesContainerPF);

            skipNextBrainPFChange = true;
            root.Bind(serializedObject);
        }
    }
}
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor
{
    public class BonesTransformToAnimation : EditorWindow
    {
        private static Animator animator;

        [MenuItem("Tafra Games/Windows/Animation/Bones Transform To Animation")]
        private static void Open()
        {
            GetWindow<BonesTransformToAnimation>("Bones Transform To Animation");
        }

        private void CreateGUI()
        {
            rootVisualElement.style.paddingTop = rootVisualElement.style.paddingBottom = rootVisualElement.style.paddingRight = rootVisualElement.style.paddingLeft = 2;

            Label setupHeader = new Label("Setup");
            setupHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            setupHeader.style.fontSize = 14;
            setupHeader.style.marginTop = 5;
            setupHeader.style.marginBottom = 5;

            ObjectField animatorField = new ObjectField("Animator");
            animatorField.objectType = typeof(Animator);
            animatorField.name = "animator";

            ObjectField animationField = new ObjectField("Animation Clip");
            animationField.objectType = typeof(AnimationClip);
            animationField.name = "animation-clip";

            IntegerField keyframeField = new IntegerField("Keyframe");
            keyframeField.name = "keyframe";

            Button applyButton = new Button(() =>
            {
                ApplyKeyframe(animatorField.value, animationField.value, keyframeField.value);
            });
            applyButton.text = "Apply";

            rootVisualElement.Add(setupHeader);
            rootVisualElement.Add(animatorField);
            rootVisualElement.Add(animationField);
            rootVisualElement.Add(keyframeField);
            rootVisualElement.Add(applyButton);
        }
        private void ApplyKeyframe(Object animatorObject, Object clipObject, int keyframe)
        { 
            if (animatorObject == null || clipObject == null) 
                return;

            Animator animator = (Animator)animatorObject;
            AnimationClip clip = (AnimationClip)clipObject;

            if(animator == null || clip == null)
                return;

            RecordUndoForHierarchy(animator.transform);

            float time = keyframe / (float)clip.frameRate;

            clip.SampleAnimation(animator.gameObject, time);
        }

        private void RecordUndoForHierarchy(Transform root)
        {
            Undo.RecordObject(root, "Apply Frame");

            for (int i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                RecordUndoForHierarchy(child);
            }
        }
    }
}
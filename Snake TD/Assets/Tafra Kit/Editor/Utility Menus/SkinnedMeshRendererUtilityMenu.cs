using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEditor;
using UnityEngine;

namespace TafraKitEditor
{
    public class SkinnedMeshRendererUtilityMenu
    {
        /// <summary>
        /// If you have a skinned mesh render that is connected to a certain rig, and you want to connect it to a different rig with the same hierarchy and names..
        /// Then on the skinned mesh renderer, change the root bone to reference the same root bone it used to reference on the original rig to the new rig, then use this menu item to do the same for the rest of the bones.
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("Tafra Games/Utilities/Components/Skinned Mesh Renderer/Update Bones Array", priority = 2)]
        public static void UpdateBonesArray(MenuCommand command)
        {
            SkinnedMeshRenderer smr = command.context as SkinnedMeshRenderer;
            
            if(smr == null)
                return;

            Dictionary<string, Transform> bones = new Dictionary<string, Transform>();

            ZHelper.FillDictionaryWithHierarchy(smr.rootBone, bones);

            Undo.RecordObject(smr, "Update Bones Array");
            
            Transform[] newBones = new Transform[smr.bones.Length];

            for(int i = 0; i < smr.bones.Length; i++)
            {
                Debug.Log(smr.bones[i].name, smr.bones[i]);
                if(bones.TryGetValue(smr.bones[i].name, out Transform newBone))
                    newBones[i] = newBone;
                else
                    Debug.LogError($"The bone with the name {smr.bones[i].name} couldn't be found under the root bone {smr.rootBone.name}.");
            }

            smr.bones = newBones;

            EditorUtility.SetDirty(smr);
        }
    }
}
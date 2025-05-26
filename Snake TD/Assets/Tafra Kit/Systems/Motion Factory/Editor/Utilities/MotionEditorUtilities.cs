using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TafraKitEditor.MotionFactory
{
    public static class MotionEditorUtilities
    {
        public static string NicefyMotionName(string motionName)
        {
            if (motionName.EndsWith("Motion"))
                motionName = motionName.Substring(0, motionName.Length - 6);

            if (motionName.StartsWith("TwoStates"))
                motionName = motionName.Substring(9);
            else if(motionName.StartsWith("Targeted"))
                motionName = motionName.Substring(8);
            else if(motionName.StartsWith("Visibility"))
                motionName = motionName.Substring(10);

            motionName = ObjectNames.NicifyVariableName(motionName);

            return motionName;
        }
    }
}
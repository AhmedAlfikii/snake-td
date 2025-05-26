using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using TafraKit.MotionFactory;

namespace TafraKitEditor.MotionFactory
{
    [InitializeOnLoad]
    public class MotionClassesGatherer
    {
        static MotionClassesGatherer()
        {
            ClassesGatherer.GatherChildrenOf(typeof(TwoStatesMotion));
            ClassesGatherer.GatherChildrenOf(typeof(TargetedMotion));
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class BasicProgressControllerUtility : MonoBehaviour
    {
#if UNITY_EDITOR
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.LeftControl))
                BasicProgressManager.CompleteCurrentLevel();

            if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftControl))
                BasicProgressManager.FailCurrentLevel();

            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl))
                BasicProgressManager.ReplayCurrentLevel();
        }
#endif
    }
}
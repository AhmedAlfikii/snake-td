using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;

namespace TafraKit.Demos
{
    public class TimeScalerDemo : MonoBehaviour
    {
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                TimeScaler.SetTimeScale("alpha1", 0.1f);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                TimeScaler.SetTimeScale("alpha2", 0.3f);
            }
            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                TimeScaler.SetTimeScale("alpha3", 0.5f);
            }

            if(Input.GetKeyDown(KeyCode.Keypad1))
            {
                TimeScaler.RemoveTimeScaleControl("alpha1");
            }
            if(Input.GetKeyDown(KeyCode.Keypad2))
            {
                TimeScaler.RemoveTimeScaleControl("alpha2");
            }
            if(Input.GetKeyDown(KeyCode.Keypad3))
            {
                TimeScaler.RemoveTimeScaleControl("alpha3");
            }

            if(Input.GetKeyDown(KeyCode.Keypad0))
            {
                TimeScaler.RemoveAllTimeScaleControls();
            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    //TO DO: cut off motion duration if they started playing when the other animation was already playing (for example, if "showing" animation was cut midway by "hiding" animation, then hiding animation should be twice as fast as it normally is).
    public class ScaleMotionControl : MonoBehaviour
    {
        [SerializeField] private bool prewarm = true;
        [SerializeField] private bool startVisible;
        [SerializeField] private ScaleMotion showMotion;
        [SerializeField] private ScaleMotion hideMotion;

        private bool isVisible;

        void Awake()
        {
            showMotion.Initialize(this, transform);
            hideMotion.Initialize(this, transform);

            if (prewarm)
            {
                if (startVisible)
                    Show(true);
                else
                    Hide(true);
            }
        }

        public void Show(bool immediate)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            if (hideMotion.IsPlaying())
                hideMotion.Stop();

            showMotion.Play(immediate);

            isVisible = true;
        }

        public void Hide(bool immediate)
        {
            if (showMotion.IsPlaying())
                showMotion.Stop();

            hideMotion.Play(immediate);

            isVisible = false;
        }

        public void SwitchVisibility(bool immediate)
        {
            if (!isVisible)
                Show(immediate);
            else
                Hide(immediate);
        }
    }
}

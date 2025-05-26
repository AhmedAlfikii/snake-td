using System;
using System.Collections;
using UnityEngine;
using ZUI;

namespace TafraKit.Internal.Roguelike
{
    public class PerkCardRevealAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private UIElement myUIE;
        [SerializeField] private string idleFlippedAnimationName = "Idle Flipped";
        [SerializeField] private string revealAnimationName = "Reveal";
        [SerializeField] private float revealAnimationDelay = 1.5f;

        private void OnEnable()
        {
            if(myUIE.Visible)
                OnShow();

            myUIE.OnShow.AddListener(OnShow);
        }

        private void OnDisable()
        {
            myUIE.OnShow.RemoveListener(OnShow);
        }

        private void OnShow()
        {
            StartCoroutine(Reveal());
        }
        IEnumerator Reveal()
        { 
            animator.Play(idleFlippedAnimationName);
        
            yield return Yielders.GetWaitForSecondsRealtime(revealAnimationDelay);

            animator.Play(revealAnimationName);
        }
    }
}
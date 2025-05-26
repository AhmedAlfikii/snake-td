using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.InputSystem;
using UnityEngine;
using UnityEngine.AI;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("General/Randomize Float Parameter")]
    public class RandomizeFloatParameter : CharacterAnimatorModule
    {
        [SerializeField] private string parameter = "Animation Cycle Offset";
        [SerializeField] private float min = 0;
        [SerializeField] private float max = 1;

        [NonSerialized] private int parameterHash;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            parameterHash = Animator.StringToHash(parameter);
            animator.SetFloat(parameterHash, UnityEngine.Random.Range(min, max));
        }

        protected override void OnEnable()
        {
        }
        protected override void OnDisable()
        {
        }
    }
}
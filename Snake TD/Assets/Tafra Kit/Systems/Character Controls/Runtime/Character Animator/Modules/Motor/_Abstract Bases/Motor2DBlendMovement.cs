using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    public abstract class Motor2DBlendMovement : CharacterAnimatorModule
    {
        [Header("Animator Properties")]
        [Tooltip("The name of the float animator parameter that affects the blend tree's forward axis.")]
        [SerializeField] protected string forwardSpeedParam = "Forward Speed";
        [Tooltip("The name of the float animator parameter that affects the blend tree's side axis.")]
        [SerializeField] protected string sideSpeedParam = "Side Speed";
        [Tooltip("The name of the float animator parameter that will contain the absolute speed the character is moving in (ignoring Y).")]
        [SerializeField] protected string absoluteSpeedParam = "Absolute Speed";

        [Header("Speed")]
        [Tooltip("The movement speed of the agent that is ideal for the movement animation (would not make the animation feel \"slidey\". " +
            "The blend-tree axes will reach 1 when the agent's speed reaches this value.")]
        [SerializeField] protected float normalSpeed = 1;

        [Header("Smoothing")]
        [Tooltip("Enable this to smooth out the motion of the blend tree's seeker.")]
        [SerializeField] protected bool enableSmoothing = true;
        [Tooltip("How fast should the animator parameters catch up with the actual velocity? (use this to smooth the seeker of the blend tree). " +
            "CAREFUL: too low of a value and the character will be sliding around.")]
        [SerializeField] protected float lerpingSpeed = 10;

        [Header("Over-speed")]
        [Tooltip("If enabled, the movement animation speed will increase if the agent's speed exceeded the normal speed. Up the value of the max animation speed field.")]
        [SerializeField] protected bool enableOverSpeed;
        [Tooltip("The name of the float animator parameter that controls the speed of the blend tree")]
        [SerializeField] protected string animationSpeedParam = "Movement Speed Multiplier";
        [Tooltip("If the agent's speed exceeded the normal speed, its movement animation speed will increase based on the percentage of extra speed, up to this value.")]
        [SerializeField] protected float maxAnimationSpeed = 1.5f;

        protected Transform motorTransform;
        protected int forwardSpeedParamHash;
        protected int sideSpeedParamHash;
        protected int absoluteSpeedParamHash;
        protected int animationSpeedParamHash;
        protected float curForawrd;
        protected float curSide;
        protected Vector3 localVelocity;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => true;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            forwardSpeedParamHash = Animator.StringToHash(forwardSpeedParam);
            sideSpeedParamHash = Animator.StringToHash(sideSpeedParam);
            absoluteSpeedParamHash = Animator.StringToHash(absoluteSpeedParam);

            if (enableOverSpeed)
                animationSpeedParamHash = Animator.StringToHash(animationSpeedParam);
        }

        public override void LateUpdate()
        {
            float forward = localVelocity.z / normalSpeed;
            float side = localVelocity.x / normalSpeed;
            float magintude = new Vector3(localVelocity.x, 0, localVelocity.z).magnitude;

            if (enableSmoothing)
            {
                float lerpT = lerpingSpeed * Time.deltaTime;

                curForawrd = Mathf.Lerp(curForawrd, forward, lerpT);
                curSide = Mathf.Lerp(curSide, side, lerpT);
            }
            else
            {
                curForawrd = forward;
                curSide = side;
            }

            animator.SetFloat(forwardSpeedParamHash, Mathf.Clamp(curForawrd, -1, 1));
            animator.SetFloat(sideSpeedParamHash, Mathf.Clamp(curSide, -1, 1));
            animator.SetFloat(absoluteSpeedParamHash, magintude);

            if (enableOverSpeed)
            {
                //A workaround to avoid calculating the velocity's magnitude, in an ideal scenario, we should've used the magnitude to get the agent's current speed.
                float highestDirectionalSpeed = Mathf.Max(Mathf.Abs(curForawrd), Mathf.Abs(curSide));

                float cappedMultiplier = Mathf.Min(maxAnimationSpeed, highestDirectionalSpeed);

                //Animation speed should not drop below 1.
                float animationSpeedMultiplier = Mathf.Max(1, cappedMultiplier);

                animator.SetFloat(animationSpeedParamHash, animationSpeedMultiplier);
            }
        }
    }
}
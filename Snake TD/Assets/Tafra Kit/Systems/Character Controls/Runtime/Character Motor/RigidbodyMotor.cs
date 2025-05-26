using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    public class RigidbodyMotor : CharacterMotor
    {
        #region Public Fields
        /// <summary>
        /// Motion that don't require the player's continuous input, and that are there by default, e.g. gravity, and jump initial force.
        /// </summary>
        [HideInInspector] public Vector3 BaseMotionVector;
        /// <summary>
        /// Holds the interpolated (accelerated/decelerated/manipulated) player input, this is what actually applies to the rigidbody's velocity.
        /// </summary>
        [HideInInspector] public Vector3 InputMotionVector;
        /// <summary>
        /// External motion and forces applied to the character (resets at the end of each frame).
        /// </summary>
        [HideInInspector] public Vector3 ExternalMotionVector;
        /// <summary>
        /// Holds the raw player input, for reference.
        /// </summary>
        [HideInInspector] public Vector3 RawInputMotionVector;
        #endregion

        #region Private Serialized Fields
        [Tooltip("The transform that will be considered the guide while moving (typically the object that aims towards the look direction, e.g. head in FPS games or entire body in TPS games).")]
        [SerializeField] private Transform forwardTransform;
        #endregion

        #region Private Fields
        protected Rigidbody myRB;
        #endregion

        #region Public Properties
        public Rigidbody MyRigidbody => myRB;
        public Transform ForwardTransform => forwardTransform;
        public bool HasMovementInput
        {
            get
            {
                return InputMotionVector.sqrMagnitude > 0.001f;
            }
        }
        #endregion

        protected override void Awake()
        {
            myRB = GetComponent<Rigidbody>();

            base.Awake();
        }

        protected override void FixedUpdate()
        {
            BaseMotionVector.y = myRB.linearVelocity.y;
            ExternalMotionVector = Vector3.zero;

            //Tick the modules.
            base.FixedUpdate();

            myRB.linearVelocity = BaseMotionVector + (InputMotionVector * defaultSpeed.Value) + ExternalMotionVector;
        }

        protected override void OnStop()
        {
            base.OnStop();

            ClearMotion();
        }
        protected override void OnPause()
        {
            base.OnPause();

            ClearMotion();
        }
        protected override void OnResume()
        {
            base.OnResume();
        }
        protected override void OnPlay()
        {
            base.OnPlay();
        }
        private void ClearMotion()
        {
            InputMotionVector = Vector3.zero;
            ExternalMotionVector = Vector3.zero;

            Vector3 vel = myRB.linearVelocity;

            vel.x = 0;
            vel.z = 0;

            myRB.linearVelocity = vel;
        }
    }
}
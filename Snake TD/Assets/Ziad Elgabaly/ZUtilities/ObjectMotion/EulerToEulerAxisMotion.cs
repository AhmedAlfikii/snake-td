using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class EulerToEulerAxisMotion : ObjectMotion
    {
        [SerializeField] private float eulerXStart;
        [SerializeField] private float eulerXEnd;
        [SerializeField] private int eulerXMultiplier = 1;

        [Space()]
        
        [SerializeField] private float eulerYStart;
        [SerializeField] private float eulerYEnd;
        [SerializeField] private int eulerYMultiplier = 1;

        [Space()]
        
        [SerializeField] private float eulerZStart;
        [SerializeField] private float eulerZEnd;
        [SerializeField] private int eulerZMultiplier = 1;


        [SerializeField] private bool useLocalSpace;

        private Quaternion normalRotation;

        protected override void Initialize()
        {
            base.Initialize();

            normalRotation = transform.rotation;
        }

        protected override void ApplyMotion(float easedT, float rawT, bool inverted)
        {
            float xRawT = Mathf.PingPong(rawT * eulerXMultiplier, 1);
            float xEasedT = MotionEquations.GetEaseFloat(xRawT, easingType);
            float eulerX = Mathf.LerpUnclamped(inverted ? eulerXEnd : eulerXStart, inverted ? eulerXStart : eulerXEnd, xEasedT);

            float yRawT = Mathf.PingPong(rawT * eulerYMultiplier, 1);
            float yEasedT = MotionEquations.GetEaseFloat(yRawT, easingType);
            float eulerY = Mathf.LerpUnclamped(inverted ? eulerYEnd : eulerYStart, inverted ? eulerYStart : eulerYEnd, yEasedT);
           
            float zRawT = Mathf.PingPong(rawT * eulerZMultiplier, 1);
            float zEasedT = MotionEquations.GetEaseFloat(zRawT, easingType);
            float eulerZ = Mathf.LerpUnclamped(inverted ? eulerZEnd : eulerZStart, inverted ? eulerZStart : eulerZEnd, zEasedT);

            if (!useLocalSpace)
                transform.eulerAngles = new Vector3(eulerX, eulerY, eulerZ);
            else
                transform.localEulerAngles = new Vector3(eulerX, eulerY, eulerZ);
        }


        public override void GoToNormalState()
        {
            base.GoToNormalState();

            transform.rotation = normalRotation;
        }
    }
}
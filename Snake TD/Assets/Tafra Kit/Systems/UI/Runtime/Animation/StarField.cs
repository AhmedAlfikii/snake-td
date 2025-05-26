using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.UI
{
    public class StarField : MonoBehaviour
    {
        public enum MotionType
        { 
            Fade,
            Scale
        }
        public enum StarState
        { 
            Shown,
            Showing,
            Hidden,
            Hiding
        }
        public class StarData
        {
            public Image star;
            public MotionType motionType;
            public FloatRange hideDuration;
            public FloatRange showDuration;
            public FloatRange hiddenDuration;
            public FloatRange shownDuration;
            public EasingType easing;
            private float hiddenValue;
            public float timeOffset;

            public StarState state;
            public float stateStartTime;
            public float stateEndTime;
            public float curTime;
            public float normalOpacity;
            private Vector3 hiddenScale;

            public StarData(Image star, MotionType motionType, FloatRange hideDuration, FloatRange showDuration, FloatRange hiddenDuration, FloatRange shownDuration, EasingType easing, float hiddenValue, float timeOffset, StarState state)
            {
                this.star = star;
                this.motionType = motionType;
                this.hideDuration = hideDuration;
                this.showDuration = showDuration;
                this.hiddenDuration = hiddenDuration;
                this.shownDuration = shownDuration;
                this.easing = easing;
                this.hiddenValue = hiddenValue;
                this.timeOffset = timeOffset;
                this.state = state;

                hiddenScale = Vector3.one * hiddenValue;

                normalOpacity = star.color.a;
            }

            public void SetTime(float time)
            {
                curTime = time + timeOffset;

                switch(state)
                {
                    case StarState.Shown:
                        {
                            if(curTime > stateEndTime)
                            {
                                stateStartTime = curTime;
                                stateEndTime = curTime + hideDuration.GetRandomValue();
                                state = StarState.Hiding;
                            }
                        }
                        break;
                    case StarState.Showing:
                        {
                            float t = (curTime - stateStartTime) / (stateEndTime - stateStartTime);

                            ChangeVisbility(t);

                            if(curTime > stateEndTime)
                            {
                                //Make sure it's completely visible before switching state.
                                ChangeVisbility(1);

                                stateStartTime = curTime;
                                stateEndTime = curTime + shownDuration.GetRandomValue();
                                state = StarState.Shown;
                            }
                        }
                        break;
                    case StarState.Hidden:
                        {
                            if(curTime > stateEndTime)
                            {
                                stateStartTime = curTime;
                                stateEndTime = curTime + showDuration.GetRandomValue();
                                state = StarState.Showing;
                            }
                        }
                        break;
                    case StarState.Hiding:
                        {
                            float t = (curTime - stateStartTime) / (stateEndTime - stateStartTime);
                           
                            ChangeVisbility(1 - t);

                            if(curTime > stateEndTime)
                            {
                                //Make sure it's completely hidden before switching state.
                                ChangeVisbility(0);

                                stateStartTime = curTime;
                                stateEndTime = curTime + hiddenDuration.GetRandomValue();
                                state = StarState.Hidden;
                            }
                        }
                        break;
                }
            }

            private void ChangeVisbility(float t)
            {
                t = MotionEquations.GetEaseFloat(t, easing);

                if(motionType == MotionType.Fade)
                {
                    Color color = star.color;
                    color.a = Mathf.LerpUnclamped(hiddenValue, normalOpacity, t);
                    star.color = color;
                }
                else if(motionType == MotionType.Scale)
                {
                    star.transform.localScale = Vector3.LerpUnclamped(hiddenScale, Vector3.one, t);
                }
            }
        }

        [SerializeField] private List<Image> stars;
        [SerializeField] private MotionType motionType = MotionType.Fade;
        [SerializeField] private bool useUnscaledTime;

        [Header("Animation")]
        [SerializeField] private FloatRange hideDuration = new FloatRange(0.5f, 1.5f);
        [SerializeField] private FloatRange showDuration = new FloatRange(0.5f, 1.5f);
        [SerializeField] private FloatRange hiddenDuration = new FloatRange(0.5f, 1.5f);
        [SerializeField] private FloatRange shownDuration = new FloatRange(0.5f, 1.5f);
        [SerializeField] private EasingType easing = new EasingType(TafraKit.MotionType.EaseInOut, new EasingEquationsParameters());
        [Range(0f, 1f)]
        [SerializeField] private float hiddenValue = 0;

        private List<StarData> starsData = new List<StarData>();
        private float time;

        private void Awake()
        {
            for (int i = 0; i < stars.Count; i++)
            {
                StarData starData = new StarData(stars[i], motionType, hideDuration, showDuration, hiddenDuration, shownDuration, easing, hiddenValue, Random.Range(0, 10), Random.value > 0.5f ? StarState.Shown : StarState.Hidden);

                starsData.Add(starData);
            }
        }

        private void Update()
        {
            time += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            for (int i = 0; i < starsData.Count; i++)
            {
                starsData[i].SetTime(time);
            }
        }
    }
}
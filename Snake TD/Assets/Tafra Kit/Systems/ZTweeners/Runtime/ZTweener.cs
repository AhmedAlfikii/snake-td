using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.ZTweeners
{
    public static class ZTweener
    {
        private static MonoBehaviour tweenPlayer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            GameObject tp = new GameObject("ZTweenPlayer");
            tweenPlayer = tp.AddComponent<EmptyMonoBehaviour>();
            GameObject.DontDestroyOnLoad(tp);
        }

        private static void PlayTween(ZTween tween)
        {
            if (tween.IsPlaying)
                tween.Stop();
            
            if (tween.Player == null)
                tween.Player = tweenPlayer;

            tween.Play();
        }

        #region Float Tweens
        public static ZTweenFloat FloatChange(ZTweenFloat tween, float value, float target, float duration, MonoBehaviour player)
        {
            ZTweenFloat zTween = tween;

            if (zTween == null)
            {
                TafraDebugger.Log("ZTweener", "You can't use null float tweens.", TafraDebugger.LogType.Error);
                return null;
            }

            zTween.Construct(value, target, duration, player);

            PlayTween(zTween);

            return zTween;
        }
        #endregion

        #region Color Tweens
        public static ZTweenColor ChangeColor(ZTweenColor tween, Color color, Color target, float duration, MonoBehaviour player)
        {
            ZTweenColor zTween = tween;

            if (zTween == null)
            {
                TafraDebugger.Log("ZTweener", "You can't use null color tweens.", TafraDebugger.LogType.Error);
                return null;
            }

            zTween.Construct(color, target, duration, player);

            PlayTween(zTween);

            return zTween;
        }
        #endregion

        #region Transform Tweens
        public static ZTweenVector3 Scale(ZTweenVector3 tween, Transform trans, Vector3 target, float duration, MonoBehaviour player)
        {
            ZTweenVector3 zTween = tween;

            //TODO: Pool this.
            if (zTween == null)
                zTween = new ZTweenVector3();

            zTween.Construct(trans.localScale, target, duration, player);

            zTween.SetOnUpdated(() => {
                trans.localScale = zTween.CurValue;
            });

            PlayTween(zTween);

            return zTween;
        }
        public static ZTweenVector3 Move(ZTweenVector3 tween, Transform trans, Vector3 target, float duration, MonoBehaviour player)
        {
            ZTweenVector3 zTween = tween;

            //TODO: Pool this.
            if (zTween == null)
                zTween = new ZTweenVector3();

            zTween.Construct(trans.position, target, duration, player);

            zTween.SetOnUpdated(() => {
                trans.position = zTween.CurValue;
            });

            PlayTween(zTween);

            return zTween;
        }
        public static ZTweenVector3 MoveLocal(ZTweenVector3 tween, Transform trans, Vector3 target, float duration, MonoBehaviour player)
        {
            ZTweenVector3 zTween = tween;

            //TODO: Pool this.
            if (zTween == null)
                zTween = new ZTweenVector3();

            zTween.Construct(trans.localPosition, target, duration, player);

            zTween.SetOnUpdated(() => {
                trans.localPosition = zTween.CurValue;
            });

            PlayTween(zTween);

            return zTween;
        }
        public static ZTweenQuaternion Rotate(ZTweenQuaternion tween, Transform trans, Quaternion target, float duration, MonoBehaviour player)
        {
            ZTweenQuaternion zTween = tween;

            //TODO: Pool this.
            if (zTween == null)
                zTween = new ZTweenQuaternion();

            zTween.Construct(trans.rotation, target, duration, player);

            zTween.SetOnUpdated(() => {
                trans.rotation = zTween.CurValue;
            });

            PlayTween(zTween);

            return zTween;
        }
        public static ZTweenQuaternion RotateLocal(ZTweenQuaternion tween, Transform trans, Quaternion target, float duration, MonoBehaviour player)
        {
            ZTweenQuaternion zTween = tween;

            //TODO: Pool this.
            if (zTween == null)
                zTween = new ZTweenQuaternion();

            zTween.Construct(trans.localRotation, target, duration, player);

            zTween.SetOnUpdated(() => {
                trans.localRotation = zTween.CurValue;
            });

            PlayTween(zTween);

            return zTween;
        }

        public static ZTweenVector3 Euler(ZTweenVector3 tween, Transform trans, Vector3 target, float duration, MonoBehaviour player)
        {
            ZTweenVector3 zTween = tween;

            //TODO: Pool this.
            if (zTween == null)
                zTween = new ZTweenVector3();

            zTween.Construct(trans.eulerAngles, target, duration, player);

            zTween.SetOnUpdated(() => {
                trans.eulerAngles = zTween.CurValue;
            });

            PlayTween(zTween);

            return zTween;
        }
        public static ZTweenVector3 EulerLocal(ZTweenVector3 tween, Transform trans, Vector3 target, float duration, MonoBehaviour player)
        {
            ZTweenVector3 zTween = tween;

            //TODO: Pool this.
            if (zTween == null)
                zTween = new ZTweenVector3();

            zTween.Construct(trans.localEulerAngles, target, duration, player);

            zTween.SetOnUpdated(() => {
                trans.localEulerAngles = zTween.CurValue;
            });

            PlayTween(zTween);

            return zTween;
        }

        #endregion

        #region RectTransform Tweens
        public static ZTweenRect RTAdaptRect(ZTweenRect tween, RectTransform rt, RectTransform targetRT, float duration, MonoBehaviour player)
        {
            ZTweenRect zTween = tween;

            //TODO: Pool this.
            if (zTween == null)
                zTween = new ZTweenRect();

            zTween.Construct(rt, targetRT, duration, player);

            PlayTween(zTween);

            return zTween;
        }
        #endregion

        #region Color Tweens
        public static ZTweenColor ChangeImageColor(ZTweenColor tween, Image image, Color target, float duration, MonoBehaviour player)
        {
            ZTweenColor zTween = tween;

            if (zTween == null)
            {
                TafraDebugger.Log("ZTweener", "You can't use null color tweens.", TafraDebugger.LogType.Error);
                return null;
            }

            zTween.Construct(image.color, target, duration, player);

            zTween.SetOnUpdated(() => {
                image.color = zTween.CurColor;
            });

            PlayTween(zTween);

            return zTween;
        }
        #endregion
    }
}
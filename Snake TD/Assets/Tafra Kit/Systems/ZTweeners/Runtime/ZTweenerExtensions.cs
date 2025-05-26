using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.ZTweeners
{
    public static class ZTweenerExtensions
    {
        #region Float Tweens
        public static ZTweenFloat ZTweenChange(this float f, ZTweenFloat tween, float target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.FloatChange(tween, f, target, duration, player);
        }
        #endregion

        #region Color Tweens
        public static ZTweenColor ZTweenColor(this Color color, ZTweenColor tween, Color target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.ChangeColor(tween, color, target, duration, player);
        }
        #endregion

        #region Transform Tweens
        public static ZTweenVector3 ZTweenScale(this Transform trans, ZTweenVector3 tween, Vector3 target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.Scale(tween, trans, target, duration, player);
        }
        public static ZTweenVector3 ZTweenMove(this Transform trans, ZTweenVector3 tween, Vector3 target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.Move(tween, trans, target, duration, player);
        }
        public static ZTweenVector3 ZTweenMoveLocal(this Transform trans, ZTweenVector3 tween, Vector3 target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.MoveLocal(tween, trans, target, duration, player);
        }
        public static ZTweenQuaternion ZTweenRotate(this Transform trans, ZTweenQuaternion tween, Quaternion target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.Rotate(tween, trans, target, duration, player);
        }
        public static ZTweenQuaternion ZTweenRotateLocal(this Transform trans, ZTweenQuaternion tween, Quaternion target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.RotateLocal(tween, trans, target, duration, player);
        }
        public static ZTweenVector3 ZTweenEuler(this Transform trans, ZTweenVector3 tween, Vector3 target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.Euler(tween, trans, target, duration, player);
        }
        public static ZTweenVector3 ZTweenEulerLocal(this Transform trans, ZTweenVector3 tween, Vector3 target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.EulerLocal(tween, trans, target, duration, player);
        }
        #endregion

        #region RectTransform Tweens
        public static ZTweenVector3 ZTweenScale(this RectTransform rt, ZTweenVector3 tween, Vector3 target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.Scale(tween, rt, target, duration, player);
        }
        public static ZTweenVector3 ZTweenMove(this RectTransform rt, ZTweenVector3 tween, Vector3 target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.Move(tween, rt, target, duration, player);
        }
        public static ZTweenVector3 ZTweenMoveLocal(this RectTransform rt, ZTweenVector3 tween, Vector3 target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.MoveLocal(tween, rt, target, duration, player);
        }
        public static ZTweenQuaternion ZTweenRotate(this RectTransform trans, ZTweenQuaternion tween, Quaternion target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.Rotate(tween, trans, target, duration, player);
        }
        public static ZTweenQuaternion ZTweenRotateLocal(this RectTransform trans, ZTweenQuaternion tween, Quaternion target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.RotateLocal(tween, trans, target, duration, player);
        }
        /// <summary>
        /// Transforms a rect to another rect (size, position, rotation and scale).
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="target"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static ZTweenRect ZTweenAdaptRect(this RectTransform rt, ZTweenRect tween, RectTransform target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.RTAdaptRect(tween, rt, target, duration, player);
        }
        #endregion

        #region Image Tweens
        public static ZTweenColor ZTweenImageColor(this Image image, ZTweenColor tween, Color target, float duration, MonoBehaviour player = null)
        {
            return ZTweener.ChangeImageColor(tween, image, target, duration, player);
        }

        #endregion
    }
}
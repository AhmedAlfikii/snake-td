using System;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace TafraKit
{
    public static class ZHelper
    {

        /// <summary>
        /// Return a list of random number between min [inclusive] and max [exclusive] without repeating the same number.
        /// </summary>
        /// <param name="minNumber">The minimum number to get. [inclusive]</param>
        /// <param name="maxNumber">The maximum number to get. [exclusive]</param>
        /// <param name="outputCount">The count of the wanted random numbers. (0 means all the range)</param>
        public static List<int> RandomizeWithoutRepeat(int minNumber, int maxNumber, int outputCount = 0)
        {
            int numsCount = maxNumber - minNumber;
            List<int> numbers = new List<int>();
            for(int i = 0; i < numsCount; i++)
            {
                numbers.Add(minNumber + i);
            }

            if(outputCount == 0)
                outputCount = numsCount;

            List<int> randomizedNumbers = new List<int>();
            for(int i = 0; i < outputCount; i++)
            {
                int noIndex = UnityEngine.Random.Range(0, numbers.Count);
                randomizedNumbers.Add(numbers[noIndex]);
                numbers.RemoveAt(noIndex);
            }

            return randomizedNumbers;
        }
        /// <summary>
        /// Converts a string to an int list.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static List<int> ConverStringToIntList(string s, string separator)
        {
            List<int> intList = new List<int>();

            int separatorIndex = s.IndexOf(separator);
            while(separatorIndex != -1)
            {
                int n = int.Parse(s.Substring(0, separatorIndex));
                intList.Add(n);

                s = s.Substring(separatorIndex + 1);

                separatorIndex = s.IndexOf(",");
            }
            int finalN = 0;

            if(int.TryParse(s, out finalN))
                intList.Add(finalN);

            return intList;
        }
        /// <summary>
        /// Converts a list of int to a single string.
        /// </summary>
        /// <param name="intList"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string ConvertIntListToString(List<int> intList, string separator)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < intList.Count; i++)
            {
                sb.Append(intList[i]);
                if(i != intList.Count - 1)
                    sb.Append(separator);
            }

            return sb.ToString();
        }
        /// <summary>
        /// Copies a string to the clipboard of the device.
        /// </summary>
        /// <param name="s">The string to be copied.</param>
        public static void CopyToClipboard(string s)
        {
            TextEditor te = new TextEditor();
            te.text = s;
            te.SelectAll();
            te.Copy();
        }
        public static bool IsTablet()
        {
            //If the game is currently running on an iOS device.
            #if UNITY_IOS
            if ((Device.generation.ToString()).IndexOf("iPad") > -1)
                return true;
            else
                return false;
            #endif

            //If the game is currently runnning on an Android device.
            float screenWidth = Screen.width / Screen.dpi;
            float screenHeight = Screen.height / Screen.dpi;
            float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));

            return diagonalInches > 6.5f;
        }
        /// <summary>
        /// Shuffles the elements of a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        public static void ShuffleList<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for(var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
        public static void FillDictionaryWithHierarchy(Transform root, Dictionary<string, Transform> dictToFill)
        {
            dictToFill.Add(root.name, root);
            for(int i = 0; i < root.childCount; i++)
            {
                FillDictionaryWithHierarchy(root.GetChild(i), dictToFill);
            }
        }
        /// <summary>
        /// Checks whether or not the pointer (mouse or touch) is over a detectable game object, both UI or another game object 
        /// (if it's not UI, the camera has to have a Physics Raycast component and its event mask should include the layer of that game object).
        /// </summary>
        /// <returns></returns>
        public static bool IsPointerOverGameObject()
        {
            if(EventSystem.current == null)
                return false;

            //Mouse
            if(EventSystem.current.IsPointerOverGameObject())
                return true;

            //Touches
            for(int i = 0; i < Input.touchCount; i++)
            {
                if(Input.touchCount > 0 && Input.touches[i].phase == TouchPhase.Began)
                {
                    if(EventSystem.current.IsPointerOverGameObject(Input.touches[i].fingerId))
                        return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Checks whether or not the pointer (mouse or touch) is over a detectable game object, both UI or another game object 
        /// (if it's not UI, the camera has to have a Physics Raycast component and its event mask should include the layer of that game object).
        /// </summary>
        /// <returns></returns>
        public static bool IsPointerOverGameObject(int pointerId)
        {
            if(EventSystem.current == null)
                return false;

            return EventSystem.current.IsPointerOverGameObject(pointerId);
        }
        /// <summary>
        /// Converts numbers to a position format, for example: 1 converts to 1st, 2 to 2nd, 3 to 3rd, 4 to 4th, etc...
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string NumberToPosition(int number)
        {
            string position = number.ToString();

            int mostRightDigit = number > 9 ? int.Parse(position.Substring(position.Length - 1)) : number;

            switch(mostRightDigit)
            {
                case 0:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    position += "th";
                    break;
                case 1:
                    if(number != 11)
                        position += "st";
                    else
                        position += "th";
                    break;
                case 2:
                    if(number != 12)
                        position += "nd";
                    else
                        position += "th";
                    break;
                case 3:
                    if(number != 13)
                        position += "rd";
                    else
                        position += "th";
                    break;
            }

            return position;
        }
        /// <summary>
        /// Converts a number to a K/M/B format, e.g. 1,500 converts to 1.5k.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string CompactNumberString(float number)
        {
            string compact;
            if(number < 1000)
            {
                //Hundreds and less.
                compact = number.ToString("0.##");
            }
            else if(number < 1000000)
            {
                //Thousands.
                number /= 1000f;
                number = Mathf.Floor(number * 100) / 100;
                compact = number.ToString("0.##K", CultureInfo.InvariantCulture);
            }
            else if(number < 1000000000)
            {
                //Millions
                number /= 1000000f;
                number = Mathf.Floor(number * 100) / 100;
                compact = number.ToString("0.##M", CultureInfo.InvariantCulture);
            }
            else
            {
                //Billions and above.
                number /= 1000000000f;
                number = Mathf.Floor(number * 100) / 100;
                compact = number.ToString("0.##B", CultureInfo.InvariantCulture);
            }

            return compact;
        }
        /// <summary>
        /// Finds the closest object to the given point.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="components">The array of objects to check.</param>
        /// <param name="point">The point that the distance will be measured for.</param>
        /// <param name="numberOfElementsToUse">How many elements of the array should the function check the distance for? 0 means all of them.</param>
        /// <returns></returns>
        public static T GetClosesetObject<T>(T[] components, Transform point, int numberOfElementsToUse = 0) where T : Component
        {
            T closest = null;
            float closestDistance = Mathf.Infinity;

            int n = numberOfElementsToUse > 0 ? numberOfElementsToUse : components.Length;

            for(int i = 0; i < n; i++)
            {
                float sqrDistance = (components[i].transform.position - point.position).sqrMagnitude;
                if(sqrDistance < closestDistance)
                {
                    closestDistance = sqrDistance;
                    closest = components[i];
                }
            }

            return closest;
        }
        /// <summary>
        /// Outputs the two tangent points that a given point has with a circle.
        /// </summary>
        /// <param name="circlePosition"></param>
        /// <param name="circleRadius"></param>
        /// <param name="point"></param>
        /// <param name="tangentPointA"></param>
        /// <param name="tangentPointB"></param>
        /// <param name="found">Whether or not tangent points were found. (They won't be found if the point is inside the circle)</param>
        public static void TangentPointsToCircle(Vector2 circlePosition, float circleRadius, Vector2 point, out Vector2 tangentPointA, out Vector2 tangentPointB, out bool found)
        {
            float t;

            float dx = circlePosition.x - point.x;
            float dy = circlePosition.y - point.y;

            float distance = Vector2.Distance(circlePosition, point);
            if(distance < circleRadius)
            {
                tangentPointA = Vector2.zero;
                tangentPointB = Vector2.zero;
                found = false;
                return;
            }

            float a = Mathf.Asin(circleRadius / distance);
            float b = Mathf.Atan2(dy, dx);

            t = b - a;
            tangentPointA = circlePosition + new Vector2(circleRadius * Mathf.Sin(t), circleRadius * -Mathf.Cos(t));

            t = b + a;
            tangentPointB = circlePosition + new Vector2(circleRadius * -Mathf.Sin(t), circleRadius * Mathf.Cos(t));

            found = true;
        }
        public static string GetDeviceId(bool dontUseSaved = false)
        {
#if UNITY_EDITOR || !UNITY_ANDROID && !UNITY_IOS
            return SystemInfo.deviceUniqueIdentifier;
#endif

#if UNITY_ANDROID
#pragma warning disable CS0162 // Disable unreachable code detected warning
            bool savedAndroidId = dontUseSaved? false : PlayerPrefs.HasKey("TAFRA_KIT_ZHELPER_DEVICE_ID");
#pragma warning restore CS0162 // End disable unreachable code detected warning
            string androidId;
            if (!savedAndroidId)
            {
                AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
                AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
                androidId = secure.CallStatic<string>("getString", contentResolver, "android_id");

                PlayerPrefs.SetString("TAFRA_KIT_ZHELPER_DEVICE_ID", androidId);
            }
            else
            {
                androidId = PlayerPrefs.GetString("TAFRA_KIT_ZHELPER_DEVICE_ID");
            }

            return androidId;
#elif UNITY_IOS
            //TODO:https://assetstore.unity.com/packages/3d/characters/ios-keychain-plugin-43083
            Debug.Log("TODO: Save the id to iOS KeyChain, because if the player uninstalled your apps and installed again, this id will be different.");

            return UnityEngine.iOS.Device.vendorIdentifier;
#endif
        }
        public static T ExtractClass<T>(UnityEngine.Object obj) where T : class
        {
            if (obj == null)
                return null;

            if (obj is GameObject go)
                return go.GetComponent<T>();
            else
                return obj as T;
        }
        public static T CallGenericMethod<T>(Type methodHolder, string methodName, object methodHolderObject, params Type[] typeArguments)
        {
            object obj = methodHolder.GetMethod(methodName).MakeGenericMethod(typeArguments).Invoke(methodHolderObject, null);

            return (T)obj;
        }
        public static string GetNiceTypeName(Type type)
        {
            if(type == typeof(float))
                return "Float";
            else if(type == typeof(int))
                return "Int";
            else if(type == typeof(TafraActor))
                return "Tafra Actor";
            else if(type == typeof(TafraAdvancedFloat))
                return "Tafra Advanced Float";
            else if(type == typeof(UnityEngine.Object))
                return "Unity Object";
            else if(type == typeof(object))
                return "System Object";
            else
                return type.Name;
        }

        #region Math
        public static float InverseLerpVector3(Vector3 a, Vector3 b, Vector3 v)
        {
            Vector3 ab = b - a;
            Vector3 av = v - a;

            return Vector3.Dot(av, ab) / Vector3.Dot(ab, ab);
        }
        /// <summary>
        /// Remaps a value from a range to another
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from1"></param>
        /// <param name="to1"></param>
        /// <param name="from2"></param>
        /// <param name="to2"></param>
        /// <returns></returns>
        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        public static float GetDifference(float a, float b)
        {
            if(a > b)
                return a - b;
            else
                return b - a;
        }
        public static float ClampAngle(float angle, float minAngle, float maxAngle)
        {
            int extraCycles = Mathf.FloorToInt(Mathf.Abs(angle) / 360f);

            if(extraCycles > 0)
            { 
                float extraDegrees = extraCycles * 360;

                if(angle > 0)
                    angle -= extraDegrees;
                else if(angle < 0)
                    angle += extraDegrees;
            }

            return Mathf.Clamp(angle, minAngle, maxAngle);
        }
        public static float GetContainedAngle(float angle, bool allowNegativeValues = false)
        {
            float originalAngle = angle;
            int extraCycles = Mathf.FloorToInt(Mathf.Abs(angle) / 360f);

            if(extraCycles > 0)
            {
                float extraDegrees = extraCycles * 360;

                if(angle > 0)
                    angle -= extraDegrees;
                else if(angle < 0)
                    angle += extraDegrees;
            }

            return angle;
        }
        public static bool IsNumberRelationValid(float lhs, float rhs, NumberRelation relation)
        {
            return relation switch
            {
                NumberRelation.Equal => lhs == rhs,
                NumberRelation.NotEqual => lhs != rhs,
                NumberRelation.GreaterThan => lhs > rhs,
                NumberRelation.LessThan => lhs < rhs,
                NumberRelation.GreaterThanOrEqual => lhs >= rhs,
                NumberRelation.LessThanOrEqual => lhs <= rhs,
                _ => false,
            };
        }
        public static float PerformOperationOnNumber(float number, float operationValue, NumberOperation operation)
        {
            float output = number;
            switch(operation)
            {
                case NumberOperation.Add:
                    output = number + operationValue;
                    break;
                case NumberOperation.Subtract:
                    output = number - operationValue;
                    break;
                case NumberOperation.Multiply:
                    output = number * operationValue;
                    break;
                case NumberOperation.Divide:
                    output = number / operationValue;
                    break;
                case NumberOperation.Set:
                    output = operationValue;
                    break;
            }

            return output;
        }
        public static float Mod(float x, float m)
        {
            return x - m * Mathf.Floor(x / m);
        }
        #endregion
    }
}
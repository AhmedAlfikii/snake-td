using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public struct DirectionalBool
    {
        public bool left;
        public bool right;
        public bool top;
        public bool bottom;

        public DirectionalBool(bool left, bool right, bool top, bool bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
        public DirectionalBool(bool enableAll)
        {
            left = enableAll;
            right = enableAll;
            top = enableAll;
            bottom = enableAll;
        }
    }
}
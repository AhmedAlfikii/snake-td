using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TafraKit
{
    public class DragMove : MonoBehaviour
    {
        [SerializeField] private float sensitivity = 1;

        #region Delegates & Events
        public delegate void ValueChange(Vector3 value);

        public event ValueChange OnValueChange;
        #endregion

        private bool isControlable;
        private bool active;
        private Vector2 lastPointerPos;
        private Vector2 screenSize;

        void Update()
        {
            if (!isControlable) return;

            if (!active)
            {
                if (Input.GetMouseButtonDown(0) && !ZHelper.IsPointerOverGameObject())
                {
                    active = true;

                    lastPointerPos = Input.mousePosition;
                }
            }
            else
            {
                Vector2 curPointerPos = Input.mousePosition;

                Vector2 delatePos = new Vector2((curPointerPos.x - lastPointerPos.x) / screenSize.x, (curPointerPos.y - lastPointerPos.y) / screenSize.y) * sensitivity;

                lastPointerPos = curPointerPos;

                OnValueChange?.Invoke(delatePos);

                if (Input.GetMouseButtonUp(0))
                {
                    active = false;
                }
            }
        }

        public void ChangeControlableState(bool on)
        {
            isControlable = true;

            screenSize = new Vector2(Screen.width, Screen.height);
        }
    }
}
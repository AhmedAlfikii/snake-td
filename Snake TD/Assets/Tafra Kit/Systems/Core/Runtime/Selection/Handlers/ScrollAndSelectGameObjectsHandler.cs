using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    /// <summary>
    /// Handles scrolling through and selecting 3D objects.
    /// </summary>
    public class ScrollAndSelectGameObjectsHandler : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private LayerMask selectableLayers;

        [Header("Dragging")]
        [Tooltip("The distance the player has to move their mosue from the initial press position to start dragging. This value is a percentage of the screen's width")]
        [Range(0f, 1f)]
        [SerializeField] private float dragThreshold = 0.05f;
        [SerializeField] private bool horizontalDrag = true;
        [SerializeField] private bool verticalDrag = true;

        private UnityEvent pointerDown = new UnityEvent();
        private UnityEvent scrollStarted = new UnityEvent();
        private UnityEvent<Vector3> scrolling = new UnityEvent<Vector3>();
        private UnityEvent<Vector3> scrollEnded = new UnityEvent<Vector3>();

        /// <summary>
        /// The initial point where the player pressed their mouse at (on mouse down).
        /// </summary>
        private Vector3 initialPressPoint;
        /// <summary>
        /// The object that the mouse initially pressed on.
        /// </summary>
        private ISelectable initialSelection;
        private bool isPressed;
        private bool isDragging;
        private Vector3 previousDragPosition;

        public UnityEvent PointerDown => pointerDown;
        public UnityEvent ScrollStarted => scrollStarted;
        public UnityEvent<Vector3> Scrolling => scrolling;
        public UnityEvent<Vector3> ScrollEnded => scrollEnded;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                MousePress();
            else if (isPressed && Input.GetMouseButton(0))
                MouseHold();
            else if (isPressed && Input.GetMouseButtonUp(0))
                MouseRelease();
        }

        private void MousePress()
        {
            if (ZHelper.IsPointerOverGameObject())
                return;

            Vector3 mousePosition = Input.mousePosition;
            initialPressPoint = mousePosition;
            previousDragPosition = mousePosition;

            Ray ray = cam.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, selectableLayers))
            {
                ISelectable selectable = hitInfo.collider.GetComponent<ISelectable>();

                if (selectable != null)
                {
                    initialSelection = selectable;
                    if (selectable.Interactable)
                        selectable.SetSelectableState(SelectableState.Active);
                }
            }

            pointerDown?.Invoke();

            isPressed = true;
        }
        private void MouseHold()
        {
            Vector3 curDragPosition = Input.mousePosition;

            if (!horizontalDrag)
                curDragPosition.x = initialPressPoint.x;
            if (!verticalDrag)
                curDragPosition.y = initialPressPoint.y;

            //If not dragging, then enable dragging if the mouse has moved beyond the drag threshold.
            if (!isDragging)
            {
                //How much of the screen's width was the mouse dragged?
                float deltaPercentage = (curDragPosition - initialPressPoint).magnitude / Screen.width;

                if (deltaPercentage > dragThreshold)
                {
                    isDragging = true;

                    if (initialSelection != null)
                    {
                        if(initialSelection.Interactable)
                        {
                            if(initialSelection.IsSelected)
                                initialSelection.SetSelectableState(SelectableState.Selected);
                            else
                                initialSelection.SetSelectableState(SelectableState.Normal);
                        }

                        initialSelection = null;
                    }

                    scrollStarted?.Invoke();
                }
            }
            else
            {
                //Negative value because we want to scroll, and scrolling happens in the opposite direction of dragging.
                //Dividing by screen width to make it consistent across different resolutions (other wise it would be much faster to scroll in a 4K display then it is in a 720p one).
                scrolling?.Invoke(-(curDragPosition - previousDragPosition) / Screen.width);
            }

            previousDragPosition = curDragPosition;
        }
        private void MouseRelease()
        {
            //If a selectable was activated in the mouse press (and it wasn't lost by starting a drag), then attempt to select it if its still below the mouse.
            if (initialSelection != null)
            {   
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, selectableLayers))
                {
                    ISelectable selectable = hitInfo.collider.GetComponent<ISelectable>();

                    if(selectable != null && selectable == initialSelection)
                    {
                        if (selectable.Interactable)
                            selectable.Select();
                        else
                            selectable.SelectWhileNotInteractable();
                    }
                }

                if(initialSelection.Interactable)
                {
                    if(initialSelection.IsSelected)
                        initialSelection.SetSelectableState(SelectableState.Selected);
                    else
                        initialSelection.SetSelectableState(SelectableState.Normal);
                }

                initialSelection = null;
            }

            if (isDragging)
            {
                Vector3 curDragPosition = Input.mousePosition;

                if (!horizontalDrag)
                    curDragPosition.x = initialPressPoint.x;
                if (!verticalDrag)
                    curDragPosition.y = initialPressPoint.y;

                isDragging = false;
                scrollEnded?.Invoke(-(curDragPosition - previousDragPosition) / Screen.width);
            }

            isPressed = false;
        }
    }
}
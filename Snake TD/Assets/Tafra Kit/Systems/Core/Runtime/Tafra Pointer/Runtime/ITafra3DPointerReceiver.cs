using UnityEngine;

namespace TafraKit
{
    public interface ITafra3DPointerReceiver
    {
        public bool IsDraggable { get; set; }

        void OnPointerEnter();
        void OnPointerExit();
        void OnPointerDown();
        void OnPointerUp();
        void OnPointerUpWithNoDown();
        void OnPointerClickCanceled();
        void OnPointerStay(Vector3 hitPoint);
        void OnDragStarted();
        void OnDragEnded();
    }
}
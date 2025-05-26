using UnityEngine;

namespace TafraKit
{
    public class Tafra3DPointerSettings : SettingsModule
    {
        [SerializeField] private bool enabled;
        [Tooltip("Drag threshold compared to screen width.")]
        [SerializeField, Range(0f, 1f)] private float dragThreshold = 0.01f;
        [SerializeField] private LayerMask detectableLayers = ~0;

        public bool Enabled => enabled;
        public float DragThreshold => dragThreshold;
        public LayerMask DetectableLayers => detectableLayers;
        public override string Name => "Controls/3D Pointer";
        public override string Description => "Control how the 3D pointer works.";
    }
}
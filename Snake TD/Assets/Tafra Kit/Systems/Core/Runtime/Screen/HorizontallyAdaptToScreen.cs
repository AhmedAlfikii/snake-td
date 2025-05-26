using UnityEngine;

namespace TafraKit
{
    public class HorizontallyAdaptToScreen : MonoBehaviour
    {
        private enum AdaptationType
        { 
            Fill,
            Shrink
        }

        [SerializeField] private VerticalSide pivot;
        [SerializeField] private float verticalOffset;
        [SerializeField] private float distanceToCamera;
        [Tooltip("How should the object adapt to the screen?")]
        [SerializeField] private AdaptationType adaptationType;
        [SerializeField] private float width;
        [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);

        private Camera mainCamera;
        private float currentDistanceToCamera;

        public float CurrentDistanceToCamera => currentDistanceToCamera;

        private void Awake()
        {
            mainCamera = Camera.main;

            OnScreenResolutionChanged();
        }

        private void OnScreenResolutionChanged(Camera cam = null)
        {
            if (cam == null)
                cam = mainCamera;

            //For editor.
            if(cam == null)
                cam = Camera.main;

            if(cam == null)
                return;

            switch(pivot)
            {
                case VerticalSide.Top:
                    break;
                case VerticalSide.Bottom:
                    float frustumHeightAtDefaultDistance = 2.0f * distanceToCamera * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                    float frustumWidthAtDefaultDistance = cam.aspect * frustumHeightAtDefaultDistance;

                    if(frustumWidthAtDefaultDistance < width)
                    {
                        //Shrink
                        float requiredFrustumHeight = width / cam.aspect;
                        currentDistanceToCamera = requiredFrustumHeight * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

                        transform.position = cam.transform.position + cam.transform.forward * currentDistanceToCamera;
                        transform.position += cam.transform.up * (-requiredFrustumHeight / 2f + verticalOffset);
                    }
                    else
                    {
                        currentDistanceToCamera = distanceToCamera;

                        //Set as default.
                        transform.position = cam.transform.position + cam.transform.forward * currentDistanceToCamera;
                        transform.position += cam.transform.up * (-frustumHeightAtDefaultDistance / 2f + verticalOffset);
                    }

                    break;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, 0, 0));

            OnScreenResolutionChanged(Camera.main);
        }
    }
}
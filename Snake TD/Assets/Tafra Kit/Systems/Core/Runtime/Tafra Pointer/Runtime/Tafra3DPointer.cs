using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TafraKit
{
    public class Tafra3DPointer : MonoBehaviour
    {
        #region Static Elements
        private struct LayerMaskOverrideData
        {
            public int priority;
            public LayerMask layerMask;

            public LayerMaskOverrideData(int priority, LayerMask layerMask)
            {
                this.priority = priority;
                this.layerMask = layerMask;
            }
        }

        private static Camera cam;
        private static GameObject myInstance;
        private static LayerMask defaultDetectableLayers;
        private static LayerMask detectableLayers;
        private static bool allow2DColliders;
        private static GameObject curHoveredObject;
        private static ITafra3DPointerReceiver curHoveredObjectReceiver;
        private static bool clickedObjectIsDraggable;
        private static bool isDragging;
        private static Vector3 pointerDownPosition;
        private static float dragThreshold;
        private static InfluenceReceiver<LayerMaskOverrideData> layerMaskOverrideReceiver;

        public static GameObject CurrentHoveredObject => curHoveredObject;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Tafra3DPointerSettings settings = TafraSettings.GetSettings<Tafra3DPointerSettings>();

            if (settings == null || !settings.Enabled)
                return;

            dragThreshold = settings.DragThreshold;
            detectableLayers = settings.DetectableLayers;
            defaultDetectableLayers = detectableLayers;
            allow2DColliders = settings.Allow2DColliders;

            myInstance = new GameObject("Tafra3DPointer", typeof(Tafra3DPointer));
            DontDestroyOnLoad(myInstance);

            layerMaskOverrideReceiver = new InfluenceReceiver<LayerMaskOverrideData>(ShouldReplaceLayerMaskOverride, OnActiveLayerMaskOverrideUpdated, null, OnAllLayerMaskOverridesCleared);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnAllLayerMaskOverridesCleared()
        {
            detectableLayers = defaultDetectableLayers;
        }
        private static void OnActiveLayerMaskOverrideUpdated(LayerMaskOverrideData data)
        {
            detectableLayers = data.layerMask;
        }
        private static bool ShouldReplaceLayerMaskOverride(LayerMaskOverrideData newInfluence, LayerMaskOverrideData oldInfluence)
        {
            return newInfluence.priority <= oldInfluence.priority;
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            cam = Camera.main;
        }

        public static void AddLayerMaskOverride(string overrideId, LayerMask layerMask, int priority = 100)
        {
            LayerMaskOverrideData data = new LayerMaskOverrideData(priority, layerMask);

            layerMaskOverrideReceiver.AddInfluence(overrideId, data);
        }
        public static void RemoveLayerMaskOverride(string overrideId)
        {
            layerMaskOverrideReceiver.RemoveInfluence(overrideId);
        }
        #endregion

        private GameObject pointerDownObject;
        private ITafra3DPointerReceiver pointerDownReceiver;

        private void Update()
        {
            if (cam == null)
                return;

            GameObject detectedCollider = null;

            #region 3D Detection
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, detectableLayers) && !ZHelper.IsPointerOverGameObject())
            {
                detectedCollider = hit.transform.gameObject;
            }
            #endregion

            #region 2D Detection
            if (allow2DColliders)
            {
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit2D = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, detectableLayers);

                if (hit2D.collider != null)
                    detectedCollider = hit2D.collider.gameObject;
            }
            #endregion

            if (detectedCollider)
            {
                if (curHoveredObject != detectedCollider)
                {
                    //If there's an object that is currently hovered, let it know that it's no longer hovered.
                    if (curHoveredObject)
                        curHoveredObjectReceiver.OnPointerExit();

                    ITafra3DPointerReceiver receiver = detectedCollider.GetComponent<ITafra3DPointerReceiver>();

                    receiver.OnPointerEnter();

                    curHoveredObject = detectedCollider;
                    curHoveredObjectReceiver = receiver;
                }

                if (curHoveredObject != null)
                    curHoveredObjectReceiver.OnPointerStay(hit.point);
            }
            else
            {
                if (curHoveredObject)
                {
                    curHoveredObjectReceiver.OnPointerExit();
                    curHoveredObject = null;
                    curHoveredObjectReceiver = null;
                }
            }

            if (Input.GetMouseButtonDown(0) && curHoveredObject != null)
            {
                pointerDownObject = curHoveredObject;
                pointerDownReceiver = curHoveredObjectReceiver;

                clickedObjectIsDraggable = curHoveredObjectReceiver.IsDraggable;

                pointerDownPosition = Input.mousePosition;

                curHoveredObjectReceiver.OnPointerDown();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!isDragging)
                {
                    if (curHoveredObject == pointerDownObject)
                    {
                        if (pointerDownObject != null)
                            pointerDownReceiver.OnPointerUp();
                    }
                    else
                    {
                        if (pointerDownObject != null)
                            pointerDownReceiver.OnPointerClickCanceled();

                        if (curHoveredObject != null)
                            curHoveredObjectReceiver.OnPointerUpWithNoDown();
                    }
                }
                else
                {
                    if (pointerDownObject != null)
                        pointerDownReceiver.OnDragEnded();

                    if (curHoveredObject != null && curHoveredObject != pointerDownObject)
                        curHoveredObjectReceiver.OnPointerUpWithNoDown();
                }

                pointerDownObject = null;
                pointerDownReceiver = null;

                clickedObjectIsDraggable = false;

                isDragging = false;
            }

            if (clickedObjectIsDraggable && pointerDownObject != null)
            {
                if (!isDragging)
                {
                    Vector3 curPointerPosition = Input.mousePosition;

                    Vector3 diff = pointerDownPosition - curPointerPosition;

                    float draggedDistance = diff.magnitude;

                    float dragPercentage = draggedDistance / Screen.width;

                    if (dragPercentage > dragThreshold)
                    {
                        isDragging = true;

                        pointerDownReceiver.OnPointerClickCanceled();

                        pointerDownReceiver.OnDragStarted();
                    }
                }
            }
        }
    }
}
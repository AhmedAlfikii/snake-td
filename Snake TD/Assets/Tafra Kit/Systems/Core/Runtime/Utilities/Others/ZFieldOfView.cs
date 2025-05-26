using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace TafraKit
{
    public class ZFieldOfView : MonoBehaviour
    {
        #region Public Fields
        [HideInInspector] public GameObjectUnityEvent OnSightEnter = new GameObjectUnityEvent();
        [HideInInspector] public GameObjectUnityEvent OnSightExit = new GameObjectUnityEvent();
        [HideInInspector] public UnityEvent OnSomeoneSighted = new UnityEvent();
        #endregion

        #region Private Serialized Fields
        [SerializeField] private bool handleActivation;
        [SerializeField] private bool isActiveByDefault;
        [SerializeField] private LayerMask blocksVision;
        [SerializeField] private LayerMask detectables;
        [Range(0, 360)]
        [SerializeField] private float fov = 90;
        [SerializeField] private float distance = 10f;
        [Range(3, 100)]
        [SerializeField] private int rays = 20;
        [SerializeField] private bool hideVisuals;
        #endregion

        #region Private Fields
        private LayerMask allDetectableLayers;
        private float angleStep;
        private Mesh fovMesh;
        private MeshRenderer meshRend;
        private List<GameObject> objectsInSight = new List<GameObject>();
        private bool isActive;
        private bool isVisible;
        private Color defaultColor;
        private Color invisibleColor;
        private IEnumerator changingVisibilityEnumerator;
        private IEnumerator flashingColorEnumerator;
        #endregion

        #region MonoBehavriour Messages
        void Start()
        {
            meshRend = GetComponent<MeshRenderer>();
            defaultColor = meshRend.material.color;
            invisibleColor = defaultColor;
            invisibleColor.a = 0f;

            if (handleActivation)
            {
                isActive = isActiveByDefault;
                isVisible = isActive;
                ChangeVisibility(isVisible);
            }

            angleStep = fov / rays;
            fovMesh = new Mesh();
            GetComponent<MeshFilter>().mesh = fovMesh;
            allDetectableLayers = blocksVision | detectables;
        }

        void Update()
        {
            if (!isActive)
                return;

            float centerAngle = Vector3.Angle(transform.forward, Vector3.forward);
            float crossY = Vector3.Cross(Vector3.forward, transform.forward).y;

            if (crossY < 0)
                centerAngle = 360 - centerAngle;

            float startAngle = centerAngle - fov / 2f;

            Vector3[] verts = new Vector3[rays + 2];
            Vector2[] uv = new Vector2[rays + 2];
            Vector3[] normals = new Vector3[rays + 2];
            int[] triangles = new int[rays * 3];

            verts[0] = Vector3.zero;
            normals[0] = Vector3.up;

            List<GameObject> newObjectsInSight = new List<GameObject>();
            for (int i = 0; i <= rays; i++)
            {
                float radAngle = (startAngle + angleStep * i) * Mathf.Deg2Rad;

                Vector3 dir = new Vector3(Mathf.Sin(radAngle), 0, Mathf.Cos(radAngle));

                float rayHitDistance = distance;

                if (Physics.Raycast(transform.position, dir, out RaycastHit hit, distance, allDetectableLayers))
                {
                    if (detectables == (detectables | (1 << hit.transform.gameObject.layer)))
                    {
                        if (!newObjectsInSight.Contains(hit.transform.gameObject))
                        {
                            newObjectsInSight.Add(hit.transform.gameObject);
                        }
                        if (!objectsInSight.Contains(hit.transform.gameObject))
                        {
                            objectsInSight.Add(hit.transform.gameObject);
                            //OnSightEnter.Invoke(hit.transform.gameObject);
                            OnSomeoneSighted?.Invoke();
                        }
                        if (Physics.Raycast(transform.position, dir, out RaycastHit hit2, distance, blocksVision))
                        {
                            rayHitDistance = Vector3.Distance(transform.position, hit2.point);
                        }
                    }
                    if (blocksVision == (blocksVision | (1 << hit.transform.gameObject.layer)))
                    {
                        rayHitDistance = Vector3.Distance(transform.position, hit.point);
                    }
                }
                //Debug.DrawRay(transform.position, dir * rayHitDistance);

                verts[i + 1] = transform.InverseTransformPoint(transform.position + dir * rayHitDistance);
                //normals[i + 1] = Vector3.up;  //Uncomment this if you care about normals (in case of using lit materials).

                if (i < rays)
                {
                    int triIndex = i * 3;
                    triangles[triIndex] = 0;
                    triangles[triIndex + 1] = i + 1;
                    triangles[triIndex + 2] = i + 2;
                }
            }

            for (int i = 0; i < objectsInSight.Count; i++)
            {
                if (!newObjectsInSight.Contains(objectsInSight[i]))
                {
                    OnSightExit.Invoke(objectsInSight[i].gameObject);
                }
            }
            objectsInSight = new List<GameObject>(newObjectsInSight);

            if(!hideVisuals)
            {
                fovMesh.vertices = verts;
                fovMesh.uv = uv;
                fovMesh.triangles = triangles;
            }
            //fovMesh.normals = normals;    //Uncomment this if you care about normals (in case of using lit materials).
        }
        #endregion

        #region Public Functions
        public bool IsSomeoneSighted()
        {
            return objectsInSight.Count > 0;
        }
        public void SetFOV(int value)
        {
            fov = value;
            angleStep = fov / rays;
        }
        public void SetDistance(float value)
        {
            distance = value;
        }
        public void Activate()
        {
            if (isActive)
                return;

            GetComponent<MeshRenderer>().enabled = true;
            isActive = true; 
        }
        public void ChangeVisibility(bool visible, bool immediate = true, float duration = 0.2f)
        {
            if (visible == isVisible)
                return;

            if (changingVisibilityEnumerator != null)
                StopCoroutine(changingVisibilityEnumerator);

            if (flashingColorEnumerator != null)
                StopCoroutine(flashingColorEnumerator);

            Color currentColor = meshRend.material.color;
            if (visible)
            {
                if (immediate)
                {
                    meshRend.material.color = defaultColor;                  
                }
                else
                {
                    changingVisibilityEnumerator = CompactCouroutines.StartCompactCoroutine(this, 0f, duration, false, (t) => {
                        meshRend.material.color = Color.Lerp(currentColor, defaultColor, t);
                    });
                }
            }
            else
            {
                if (immediate)
                {
                    meshRend.material.color = invisibleColor;
                }
                else
                {
                    changingVisibilityEnumerator = CompactCouroutines.StartCompactCoroutine(this, 0f, duration, false, (t) => {
                        meshRend.material.color = Color.Lerp(currentColor, invisibleColor, t);
                    });
                }           
            }

            isVisible = visible;
            //meshRend.enabled = visible;
        }
        public void FlashColor(Color color,float flashDuration= 0.5f)
        {
            if (!isVisible)
                return;

            Color startColor = meshRend.material.color;
            float halfDuration = flashDuration / 2f;

            if (flashingColorEnumerator != null)
                StopCoroutine(flashingColorEnumerator);

            flashingColorEnumerator = CompactCouroutines.StartCompactCoroutine(this, 0, halfDuration, false, (t) => {
                    meshRend.material.color = Color.Lerp(startColor, color, t);     
            });

            if (flashingColorEnumerator != null)
                StopCoroutine(flashingColorEnumerator);

            flashingColorEnumerator = CompactCouroutines.StartCompactCoroutine(this, 0, halfDuration, false, (t) => {
                meshRend.material.color = Color.Lerp(color, defaultColor, t);
            });
        }
        public void Deactivate()
        {
            if (!isActive)
                return;

            isActive = false;
            GetComponent<MeshRenderer>().enabled = false;
        }
        #endregion
    }
}
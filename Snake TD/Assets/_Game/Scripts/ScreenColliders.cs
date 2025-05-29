using System.Collections;
using UnityEngine;

namespace TafraKit
{
    public class ScreenColliders2D : MonoBehaviour
    {
        [SerializeField] private DirectionalBool sides = new DirectionalBool(true);
        [Range(0f, 1f)]
        [SerializeField] private float topPadding;
        [Range(0f, 1f)]
        [SerializeField] private float rightPadding;
        [Range(0f, 1f)]
        [SerializeField] private float bottomPadding;
        [Range(0f, 1f)]
        [SerializeField] private float leftPadding;

        [SerializeField] private float bonusHeight = 0;

        private Camera cam;
        private BoxCollider2D topCollider;
        private BoxCollider2D rightCollider;
        private BoxCollider2D bottomCollider;
        private BoxCollider2D leftCollider;

        private IEnumerator Start()
        {
            yield return Yielders.EndOfFrame;

            cam = Camera.main;

            Vector3 pos = cam.transform.position;
            pos.z = 0;

            transform.position = pos;

            float aspectRatio = cam.aspect;

            float width = aspectRatio * cam.orthographicSize * 2;
            float height = (1 / aspectRatio * width);


            float thickness = 2f;

            if(sides.top)
            {
                topCollider = gameObject.AddComponent<BoxCollider2D>();
                topCollider.size = new Vector2(width, thickness);
                topCollider.offset = new Vector2(0, (height / 2f + thickness / 2f) - height * topPadding);
            }
            if(sides.right)
            {
                rightCollider = gameObject.AddComponent<BoxCollider2D>();
                rightCollider.size = new Vector2(thickness, height + bonusHeight);
                rightCollider.offset = new Vector2((width / 2f + thickness / 2f) - width * rightPadding, 0);
            }
            if(sides.bottom)
            {
                bottomCollider = gameObject.AddComponent<BoxCollider2D>();
                bottomCollider.size = new Vector2(width, thickness);
                bottomCollider.offset = new Vector2(0, (-height / 2f - thickness / 2f) + height * bottomPadding);
            }
            if(sides.left)
            {
                leftCollider = gameObject.AddComponent<BoxCollider2D>();
                leftCollider.size = new Vector2(thickness, height + bonusHeight);
                leftCollider.offset = new Vector2((-width / 2f - thickness / 2f) + width * leftPadding, 0);
            }
        }
    }
}
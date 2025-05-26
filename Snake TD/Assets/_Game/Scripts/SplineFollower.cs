using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float speed = 2f;
    private float t = 0f;

    void Update()
    {
        if (splineContainer == null || splineContainer.Spline == null)
            return;

        t += (speed / splineContainer.Spline.GetLength()) * Time.deltaTime;
        //t %= 1f;

        var spline = splineContainer.Spline;

        spline.Evaluate(t, out var localPosition, out var localTangent, out _);

        Vector3 worldPosition = splineContainer.transform.TransformPoint(localPosition);
        Vector3 worldTangent = splineContainer.transform.TransformDirection(localTangent);

        transform.position = worldPosition;
        transform.rotation = Quaternion.LookRotation(worldTangent);
    }
}

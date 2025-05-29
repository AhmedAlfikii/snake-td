using UnityEngine;
using UnityEngine.Splines;

public class SplineTest : MonoBehaviour
{
    public SplineContainer splineContainer;
    float t;

    void Update()
    {
        if (splineContainer != null)
            splineContainer.Evaluate(t, out var pos, out var tan, out _);
    }
}

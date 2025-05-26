using System;
using System.Collections.Generic;
using System.Linq;
using TafraKit.Healthies;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SnakeHandler : MonoBehaviour
{

    [SerializeField] private SegmentTypesSettings settings;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private SnakeSegment segmentPrefab;
    [SerializeField] private int segmentCount = 5;
    [SerializeField] private float spacing = 1f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private int testIndx = 0;
    [SerializeField] private int maxColors = 5;

    private List<SnakeSegment> segments = new List<SnakeSegment>();
    private SnakeSegment headSegment;

    private void Awake()
    {
        settings.Initialize();
    }
    void Start()
    {
        Spline spline = splineContainer.Spline;
        float totalLength = spline.GetLength();

        List<int> splits = RandomSplitSum(segmentCount, maxColors);
        List<int> splitsRemaining = new List<int>(splits);
        int j = 0;
        for (int i = 0; i < segmentCount; i++)
        {
            if (splitsRemaining[j] <= 0)
                j++;

            float distanceAlongSpline = i * spacing;
            float t = FindTAtDistance(spline, distanceAlongSpline, 100);

            spline.Evaluate(t, out var localPos, out var localTangent, out _);
            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos);
            Vector3 worldTangent = splineContainer.transform.TransformDirection(localTangent);

            SnakeSegment segment = Instantiate(segmentPrefab, worldPos, Quaternion.LookRotation(worldTangent));

            segment.Initialize(splineContainer, speed, true, spacing);

            Material selectedMat = settings.GetMaterial(j);

            segment.SetType(j, selectedMat);

            splitsRemaining[j]--;

            segments.Add(segment);
        }

        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].SetLinkedSegment(segments[Mathf.Max(0, i - 1)]);

            if (i < segments.Count - 1)
                segments[i].NextSegment = segments[i + 1];
        }

        headSegment = segments[segments.Count - 1];
        headSegment.OnReachedEnd.AddListener(OnHeadReachedEnd);
        headSegment.OnEndMove.AddListener(OnHeadEndMove);
    }
    private void OnDestroy()
    {
        headSegment.OnReachedEnd.RemoveListener(OnHeadReachedEnd);
        headSegment.OnEndMove.RemoveListener(OnHeadEndMove);
    }
    public List<int> RandomSplitSum(int total, int parts)
    {
        HashSet<int> breaks = new HashSet<int>();
        System.Random rand = new System.Random();

        while (breaks.Count < parts - 1)
        {
            breaks.Add(rand.Next(1, total));
        }

        List<int> breakList = breaks.ToList();
        breakList.Sort();

        List<int> result = new List<int>();
        int last = 0;

        foreach (int b in breakList)
        {
            result.Add(b - last);

            last = b;
        }

        result.Add(total - last);

        return result;
    }
    private void OnHeadEndMove()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].EndReached = false;
        }
    }
    private void OnHeadReachedEnd()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].EndReached = true;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DestroySegment();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            RandomSplitSum(30, 5);
        }
    }
    public void DestroySegment()
    {
        if (testIndx >= segments.Count)
        {
            Debug.Log($"FF->Incorrect segment index.");
            return;
        }

        segments[testIndx].Healthy.TakeDamage(new HitInfo(1000));
        segments.RemoveAt(testIndx);
    }
    float FindTAtDistance(Spline spline, float targetDistance, int resolution = 100)
    {
        float totalLength = spline.GetLength();
        targetDistance = Mathf.Clamp(targetDistance, 0f, totalLength);

        float t = 0f;
        float step = 1f / resolution;
        float accumulatedDistance = 0f;
        float3 prevPoint3;
        spline.Evaluate(0f, out prevPoint3, out _, out _);

        for (int i = 1; i <= resolution; i++)
        {
            float currentT = i * step;
            spline.Evaluate(currentT, out float3 point, out _, out _);

            float segmentDist = Vector3.Distance(prevPoint3, point);
            accumulatedDistance += segmentDist;

            if (accumulatedDistance >= targetDistance)
                return currentT;

            prevPoint3 = point;
        }

        return 1f;
    }
}
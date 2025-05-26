using System;
using TafraKit.Healthies;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class SnakeSegment : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float backingSpeed = 3f;
    [SerializeField] private bool active = true;
    [SerializeField] private float distanceThreshold = 1f;

    private SnakeSegment previousSegment;
    private SnakeSegment nextSegment;

    [SerializeField] private bool linkToPreviousSegment = false;

    private int type;
    private Material mat;

    private float t = 0f;
    private float tDelta = 0f;
    private float dist = 0f;
    private float sqrPrevDistance = 0f;
    private UnityEvent onDead = new UnityEvent();
    private UnityEvent onReachedEnd = new UnityEvent();
    private UnityEvent onEndMove = new UnityEvent();
    private Spline spline;
    private Healthy healthy;
    private new Renderer renderer;
    public Healthy Healthy => healthy;
    public UnityEvent OnDead => onDead;
    public UnityEvent OnReachedEnd => onReachedEnd;
    public UnityEvent OnEndMove => onEndMove;
    public int Type => type;
    public float T => t;
    public SnakeSegment PreviousSegment
    {
        get { return previousSegment; }
        set { previousSegment = value; }
    }
    public SnakeSegment NextSegment
    {
        get { return nextSegment; }
        set { nextSegment = value; }
    }
    public bool EndReached = false;

    private void Awake()
    {
        healthy = GetComponent<Healthy>();
        renderer = GetComponent<Renderer>();
    }
    private void OnEnable()
    {
        healthy.Events.OnDeath.AddListener(Die);
    }
    private void OnDisable()
    {
        healthy.Events.OnDeath.RemoveListener(Die);
    }
    private void Start()
    {
        t = GetClosestTOnSpline(transform.position);
    }
    void Update()
    {
        if (active)
            MoveAlongSpline();
    }
    public void Initialize(SplineContainer spline, float speed, bool active, float distanceThreshold)
    {
        splineContainer = spline;
        this.spline = splineContainer.Spline;

        this.speed = speed;
        backingSpeed = speed; //TODO Change

        this.active = active;
        this.distanceThreshold = distanceThreshold;
    }
    public void SetType(int type, Material mat)
    {
        this.type = type;
        this.mat = mat;

        SetMaterial();
    }
    public void SetLinkedSegment(SnakeSegment previousSegment)
    {
        linkToPreviousSegment = true;
        this.previousSegment = previousSegment;
    }
    public void Die(Healthy healthy, HitInfo hitInfo)
    {
        if (nextSegment)
            nextSegment.SetLinkedSegment(previousSegment);

        if (previousSegment)
            previousSegment.nextSegment = nextSegment;

        onDead?.Invoke();
        Destroy(gameObject);
    }
    private void MoveAlongSpline()
    {
        bool forwards = true;

        if (linkToPreviousSegment && previousSegment)
        {
            //tDelta = t - previousSegment.T;

            //dist =  (spline.GetLength() * tDelta) / distanceThreshold;

            //if (dist > distanceThreshold)

            sqrPrevDistance = (transform.position - previousSegment.transform.position).sqrMagnitude;
        
            if (sqrPrevDistance > distanceThreshold * distanceThreshold)
            {
                forwards = false;

                if (EndReached)
                {
                    EndReached = false;

                    Debug.Log($"FF->End move");

                    onEndMove?.Invoke();
                }
            }
            else
            {
                if (EndReached)
                {
                    return;
                }
            }
        }

        if (forwards)
            t += (speed / splineContainer.Spline.GetLength()) * Time.deltaTime;
        else
            t -= (backingSpeed / splineContainer.Spline.GetLength()) * Time.deltaTime;

        spline.Evaluate(t, out var localPosition, out var localTangent, out _);

        Vector3 worldPosition = splineContainer.transform.TransformPoint(localPosition);
        Vector3 worldTangent = splineContainer.transform.TransformDirection(localTangent);

        transform.position = worldPosition;
        transform.rotation = Quaternion.LookRotation(worldTangent);

        if (t >= 1f)
        {
            onReachedEnd?.Invoke();
        }
    }

    private float GetClosestTOnSpline(Vector3 worldPosition, int resolution = 100)
    {
        float closestT = 0f;
        float minDistance = float.MaxValue;

        for (int i = 0; i <= resolution; i++)
        {
            float currentT = i / (float)resolution;
            spline.Evaluate(currentT, out var localPos, out _, out _);
            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos);

            float dist = Vector3.Distance(worldPosition, worldPos);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestT = currentT;
            }
        }

        return closestT;
    }
    private void SetMaterial()
    {
        renderer.material = mat;
    }
}

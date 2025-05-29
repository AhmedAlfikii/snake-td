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
    [SerializeField] private float spacing = 1f;

    private SnakeSegment previousSegment;
    private SnakeSegment nextSegment;

    private bool isTail = false;
    private bool isHead = false;

    private int type;
    private Material mat;

    private float t = 0f;
    private float tDelta = 0f;
    private UnityEvent onDead = new UnityEvent();
    private UnityEvent onReachedEnd = new UnityEvent();
    private UnityEvent onEndMove = new UnityEvent();
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
    void Update()
    {
        if (active)
            MoveAlongSpline();
    }
    public void Initialize(SplineContainer spline, float speed, bool active, float spacing, float t = 0)
    {
        splineContainer = spline;

        this.speed = speed;
        backingSpeed = speed; //TODO Change

        this.active = active;
        this.spacing = spacing;

        this.t = t; 
    }
    public void SetType(int type, Material mat)
    {
        this.type = type;
        this.mat = mat;

        SetMaterial();
    }
    public void SetPreviousSegment(SnakeSegment previousSegment)
    {
        this.previousSegment = previousSegment;
    }
    public void Die(Healthy healthy, HitInfo hitInfo)
    {
        if (nextSegment)
        {
            nextSegment.SetPreviousSegment(previousSegment);

            if (isTail)
                nextSegment.SetTail();
        }

        if (previousSegment)
        {
            previousSegment.nextSegment = nextSegment;

            if (isHead)
            {
                previousSegment.SetHead();
                onEndMove?.Invoke();
            }
        }

        onDead?.Invoke();
        Destroy(gameObject);
    }
    private void MoveAlongSpline()
    {
        bool forwards = true;

        if (isTail && nextSegment)
        {
            tDelta = nextSegment.T - t;

            if (tDelta < spacing)
            {
                return;
            }
        }

        if (isHead && EndReached && 1 - t > 0.01)
        {
            RecursiveEndReachSet(false);
            onEndMove?.Invoke();

            return;
        }

        if (previousSegment)
        {
            tDelta = t - previousSegment.T;
           
            if (tDelta > spacing)
            {
                forwards = false;
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

        splineContainer.Evaluate(t, out var worldPosition, out var worldTangent, out _);

        transform.SetPositionAndRotation(worldPosition, Quaternion.LookRotation(worldTangent));

        if (t >= 0.99f)
        {
            OnEndReached();
        }
    }
    private void OnEndReached()
    {
        RecursiveEndReachSet(true);
        onReachedEnd?.Invoke();
    }
    private void RecursiveEndReachSet(bool value)
    {
        EndReached = value;

        if (previousSegment)
            previousSegment.RecursiveEndReachSet(value);
    }
    private void SetMaterial()
    {
        renderer.material = mat;
    }
    public void SetTail()
    {
        isTail = true;
    }
    public void SetHead()
    {
        isHead = true;
    }
    public void Activate()
    {
        active = true;
    }
}

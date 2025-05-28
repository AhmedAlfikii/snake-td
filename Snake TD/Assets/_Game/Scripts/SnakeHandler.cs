using System.Collections.Generic;
using System.Linq;
using TafraKit.Healthies;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class SnakeHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SegmentTypesSettings settings;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private SnakeSegment segmentPrefab;

    [Header("Snake Properties")]
    [SerializeField] private int segmentCount = 5;
    [SerializeField] private float spacing = 1f;
    [SerializeField] private float spawnOffset = 0f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private int maxColors = 5;

    [Header("Testing")]
    [SerializeField] private TMP_Dropdown testDropDown;
    [SerializeField] private int testIndx = 0;
    [SerializeField] private int rndToDstry = 2;
    [SerializeField] private float gizmoRadius = .5f;

    private UnityEvent onAllSegmentsDead = new UnityEvent();
    private List<SnakeSegment> segments = new List<SnakeSegment>();
    private SnakeSegment headSegment;

    private List<int> splits = new List<int>();

    private UnityEvent onInitialized = new UnityEvent();

    private Dictionary<int, Queue<SnakeSegment>> typeToSegmentDict = new Dictionary<int, Queue<SnakeSegment>>();
    private int remainingSegments;
    private bool isInitialized = false;
    public UnityEvent OnAllSegmentsDead => onAllSegmentsDead;
    public UnityEvent OnInitialized => onInitialized;
    public int MaxColors => maxColors;
    public List<int> Splits => splits;
    private void Awake()
    {
        settings.Initialize();
    }
    void Start()
    {
        if (testDropDown)
            PopulateTestDropdown();
        
        InitializeSnake();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DestroySegment();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            DestroyRandom();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            RandomSplitSum(30, 5);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ActivateSnake();
        }
    }

    private void InitializeSnake()
    {
        float totalLength = splineContainer.Spline.GetLength();

        splits.Clear();
        splits = RandomSplitSum(segmentCount, maxColors);
        List<int> splitsRemaining = new List<int>(splits);

        int j = 0;
        remainingSegments = segmentCount;

        for (int i = 0; i < segmentCount; i++)
        {
            if (splitsRemaining[j] <= 0)
                j++;

            float t = spawnOffset + (spacing / totalLength) * i;

            splineContainer.Evaluate(t, out var worldPos, out var worldTangent, out _);

            SnakeSegment segment = Instantiate(segmentPrefab, worldPos, Quaternion.LookRotation(worldTangent), splineContainer.transform);

            segment.Initialize(splineContainer, speed, false, spacing / totalLength, t);

            Material selectedMat = settings.GetMaterial(j);

            segment.SetType(j, selectedMat);

            splitsRemaining[j]--;

            segment.OnDead.AddListener(OnSegmentDead);

            void OnSegmentDead()
            {
                segment.OnDead.RemoveListener(OnSegmentDead);

                remainingSegments--;

                if (remainingSegments <= 0)
                {
                    OnSnakeDied();
                }
            }

            segments.Add(segment);
        }

        for (int i = 0; i < segments.Count; i++)
        {
            if (i > 0)
                segments[i].SetPreviousSegment(segments[i - 1]);
            else
                segments[i].SetTail();

            if (i < segments.Count - 1)
                segments[i].NextSegment = segments[i + 1];
            else
                segments[i].SetHead();
        }

        for (int i = segments.Count - 1; i >= 0; i--)
        {
            SnakeSegment snakeSegment = segments[i];
            if (!typeToSegmentDict.ContainsKey(snakeSegment.Type))
            {
                typeToSegmentDict[snakeSegment.Type] = new Queue<SnakeSegment>();
            }

            typeToSegmentDict[snakeSegment.Type].Enqueue(snakeSegment);
        }

        headSegment = segments[segments.Count - 1];

        onInitialized?.Invoke();
        isInitialized = true;
    }
    public void ActivateSnake()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].Activate();
        }
    }

    [ContextMenu("Reset Snake")]
    public void ResetSnake()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].Die(null, new HitInfo());
        }
        segments.Clear();

        InitializeSnake();
    }
    private void DestroySegment()
    {
        if (typeToSegmentDict.TryGetValue(testIndx, out var queue) && queue.Count > 0)
        {
            SnakeSegment snakeSegment = queue.Dequeue();

            snakeSegment.Healthy.TakeDamage(new HitInfo(1000));
            segments.Remove(snakeSegment);
        }
    }
    public void DestroySnakeSegment(int type)
    {
        if (typeToSegmentDict.TryGetValue(type, out var queue) && queue.Count > 0)
        {
            SnakeSegment snakeSegment = queue.Dequeue();

            snakeSegment.Healthy.TakeDamage(new HitInfo(1000));
            segments.Remove(snakeSegment);
        }
        else
        {
            Debug.Log($"FF->Couldnt find Segment Type in Dictionary!");
        }
    }
    private void DestroyRandom()
    {
        for (int i = 0; i < rndToDstry; i++)
        {
            testIndx = Random.Range(0, maxColors);
            DestroySegment();
        }
    }
    private void PopulateTestDropdown()
    {
        testDropDown.ClearOptions();

        List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < settings.Materials.Count; i++)
        {
            Color color = settings.GetMaterial(i).GetColor("_BaseColor");
            string hexColor = ColorUtility.ToHtmlStringRGB(color);
            string coloredLabel = $"<color=#{hexColor}>{i}</color>";

            dropdownOptions.Add(new TMP_Dropdown.OptionData(coloredLabel));
        }

        testDropDown.AddOptions(dropdownOptions);
        testDropDown.onValueChanged.AddListener(OnDropdownValueChanged);
    }
    private void OnDropdownValueChanged(int index)
    {
        // Set the label color of the selected option
        TextMeshProUGUI label = (TextMeshProUGUI)testDropDown.captionText;
        if (label != null && index >= 0 && index < settings.Materials.Count)
        {
            label.color = settings.GetMaterial(index).GetColor("_BaseColor");
        }

        testIndx = index;
    }
    private void OnSnakeDied()
    {
        Debug.Log($"FF->Snake Died! GameOver.");

        onAllSegmentsDead?.Invoke();
    }
    private List<int> RandomSplitSum(int total, int parts)
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < segmentCount; i++)
        {
            //float t = spawnOffset + (i * spacing);
            float t = spawnOffset + (spacing / splineContainer.Spline.GetLength()) * i;

            splineContainer.Evaluate(t, out var position, out _, out _);

            Gizmos.DrawWireSphere(position, gizmoRadius);
        }
    }
}
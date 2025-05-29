using System;
using System.Collections.Generic;
using TafraKit;
using TafraKit.ZTweeners;
using UnityEngine;
using UnityEngine.Events;

public class MergableItem : MonoBehaviour, ITafra3DPointerReceiver
{
    [Serializable]
    public class MergableItemStage
    {
        public Color outlineColor;
        [Range(0f, 10f)]
        public float outlineWidth;
        public bool grayscale;
        public bool dance;
    }
    [SerializeField] private List<MergableItemStage> stages = new List<MergableItemStage>();
    [SerializeField] private EulerToEulerAxisMotion dancingMotion;
    [SerializeField] private ScaleToScaleMotion scaleMotion;
    [SerializeField] private Transform visuals;
    [SerializeField] private float visualVisibilityAnimationDuration = 1f;
    [SerializeField] private EasingType visualVisibilityAnimationEasing;

    private List<Renderer> renderers;
    private TafraOutline outline;
    private Rigidbody2D rigidbody2d;
    private PolygonCollider2D collider2d;
    private int currentStage = -1;
    private int type = -1;
    private bool isDraggable = true;
    private bool isVisible = true;
    private ZTweenVector3 visualsScaleTween;

    private UnityEvent<MergableItem> onHovered = new UnityEvent<MergableItem>();
    private UnityEvent<MergableItem> onUnhovered = new UnityEvent<MergableItem>();
    private UnityEvent<MergableItem> onPointerDown = new UnityEvent<MergableItem>();
    private UnityEvent<MergableItem> onDragStarted = new UnityEvent<MergableItem>();
    private UnityEvent<MergableItem> onDragEnded = new UnityEvent<MergableItem>();
    private UnityEvent<MergableItem> onPointerUp = new UnityEvent<MergableItem>();
    public UnityEvent<MergableItem> Hovered => onHovered;
    public UnityEvent<MergableItem> Unhovered => onUnhovered;
    public UnityEvent<MergableItem> PointerDown => onPointerDown;
    public UnityEvent<MergableItem> DragStarted => onDragStarted;
    public UnityEvent<MergableItem> DragEnded => onDragEnded;
    public UnityEvent<MergableItem> PointerUp => onPointerUp;
    public Rigidbody2D Rigidbody2D => rigidbody2d;
    public PolygonCollider2D Collider2D => collider2d;
    public Transform Visuals => visuals;
    public EulerToEulerAxisMotion DancingMotion => dancingMotion;
    public bool IsFullyMerged => currentStage == stages.Count - 1;
    public int CurrentStage => currentStage;
    public int Type => type;
    public int MaxStage => stages.Count;
    public bool IsDraggable { get => isDraggable; set => isDraggable = value; }

    private void Awake()
    {
        outline = GetComponent<TafraOutline>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<PolygonCollider2D>();

        renderers = outline.Renderers;
    }
    public void OnDragEnded()
    {
        onDragEnded?.Invoke(this);
    }

    public void OnDragStarted()
    {
        onDragStarted?.Invoke(this);
    }

    public void OnPointerClickCanceled()
    {
    }

    public void OnPointerDown()
    {
    }

    public void OnPointerEnter()
    {
        onHovered?.Invoke(this);
    }

    public void OnPointerExit()
    {
    }

    public void OnPointerStay(Vector3 hitPoint)
    {
    }

    public void OnPointerUp()
    {
    }

    public void OnPointerUpWithNoDown()
    {
        onPointerUp?.Invoke(this);
    }
    public void SetType(int type)
    {
        this.type = type;
    }
    public void SetStage(int stageIndex)
    {
        if (currentStage == stageIndex)
            return;

        currentStage = stageIndex;

        MergableItemStage targetStage = stages[currentStage];

        for (int i = 0; i < renderers.Count; i++)
        {
            Renderer renderer = renderers[i];
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.material.SetFloat("_Grayscale", targetStage.grayscale ? 1f : 0f);
        }

        outline.OutlineColor = targetStage.outlineColor;
        outline.OutlineWidth = targetStage.outlineWidth;

        if (dancingMotion)
        {
            if (targetStage.dance && !dancingMotion.IsPlaying())
                dancingMotion.Play();
            else if (!targetStage.dance && dancingMotion.IsPlaying())
                dancingMotion.Stop();
        }

        float mergingScale = 1;

        transform.localScale = new Vector3(mergingScale, mergingScale, mergingScale);
    }
    public void SetAsPlaced()
    {
        outline.enabled = false;

        if (dancingMotion)
        {
            dancingMotion.Stop();
            dancingMotion.GoToNormalState();
        }
    }
    public void ChangeVisibility(bool visible, bool instant = false)
    {
        if (!instant)
            visuals.ZTweenScale(visualsScaleTween, visible ? Vector3.one : Vector3.zero, visualVisibilityAnimationDuration).SetEasingType(visualVisibilityAnimationEasing);
        else
        {
            if (visualsScaleTween != null)
                visualsScaleTween.Stop();

            visuals.localScale = visible ? Vector3.one : Vector3.zero;
        }

        isVisible = visible;
    }
    public void SetMaterials(Material material)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = material;
        }
    }
    public void PlayScaleMotion()
    {
        scaleMotion.Play();
    }
}

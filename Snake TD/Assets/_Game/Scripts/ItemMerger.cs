using TafraKit;
using UnityEngine;

public class ItemMerger : MonoBehaviour
{
    [SerializeField] private SFXClips mergeAudio;
    [SerializeField] private SFXClips finalMergeAudio;

    private ItemsSpawner spawner;
    private SnakeHandler snakeHandler;
    private MergableItem draggingItem;
    private MergableItem ghostItem;
    private Vector3 dragOffset;
    private Camera cam;

    private void Awake()
    {
        spawner = FindAnyObjectByType<ItemsSpawner>();
        snakeHandler = FindAnyObjectByType<SnakeHandler>();

        cam = Camera.main;
    }
    private void OnEnable()
    {
        spawner.OnSpawnedItem.AddListener(OnItemSpawned);
    }
    private void OnDisable()
    {
        spawner.OnSpawnedItem.RemoveListener(OnItemSpawned);
    }
    private void Update()
    {
        if (ghostItem == null)
            return;

        if (draggingItem != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 5));
            Vector3 targetPos = mouseWorldPosition + dragOffset;
            targetPos.z = -5;

            ghostItem.transform.position = targetPos;
        }
    }

    private void OnItemSpawned(MergableItem item)
    {
        item.DragStarted.AddListener(OnItemDragStarted);
        item.DragEnded.AddListener(OnItemDragEnded);
    }
    private void OnItemDragStarted(MergableItem item)
    {
        if (item.IsFullyMerged)
        {
            Debug.Log($"FF->item already fully merged");
            //Shoot snake here

            return;
        }

        draggingItem = item;

        draggingItem.ChangeVisibility(false, true);

        CreateItemGhost(item);
    }
    private void OnItemDragEnded(MergableItem item)
    {
        if (draggingItem == null)
            return;

        Destroy(ghostItem.gameObject);
        ghostItem = null;

        draggingItem.ChangeVisibility(true, true);

        MergableItem originalDraggedItem = draggingItem;

        draggingItem = null;

        GameObject hoveredGameObject = Tafra3DPointer.CurrentHoveredObject;

        if (hoveredGameObject == null || hoveredGameObject == originalDraggedItem.gameObject)
            return;

        MergableItem hoveredItem = hoveredGameObject.GetComponent<MergableItem>();

        if (hoveredItem == null)
            return;

        //If the items aren't of the same type, then we can't merge.
        if (hoveredItem.Type != originalDraggedItem.Type)
            return;

        //If the items aren't of the same stage, then we can't merge.
        if (hoveredItem.CurrentStage != originalDraggedItem.CurrentStage)
            return;

        Merge(hoveredItem, originalDraggedItem);
    }

    private void CreateItemGhost(MergableItem item)
    {
        ghostItem = Instantiate(item, item.transform.position, item.transform.rotation);
        ghostItem.Rigidbody2D.bodyType = RigidbodyType2D.Static;
        ghostItem.Collider2D.enabled = false;
        ghostItem.DancingMotion.Stop();
        ghostItem.Visuals.rotation = item.Visuals.rotation;
        ghostItem.ChangeVisibility(true, true);

        Vector3 mousePosition = Input.mousePosition;
        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 5));

        dragOffset = ghostItem.transform.position - mouseWorldPosition;
    }
    private void UnhookFromItem(MergableItem item)
    {
        item.DragStarted.RemoveListener(OnItemDragStarted);
        item.DragEnded.RemoveListener(OnItemDragEnded);
    }
    //Merge the other item into the main item.
    private void Merge(MergableItem mainItem, MergableItem otherItem)
    {
        mainItem.SetStage(mainItem.CurrentStage + 1);

        UnhookFromItem(otherItem);

        spawner.SpawnedItems.Remove(otherItem);

        Destroy(otherItem.gameObject);

        if (mainItem.IsFullyMerged)
        {
            OnFullyMerged(mainItem);
            SFXPlayer.Play(finalMergeAudio);
        }
        else
        {
            Debug.Log($"FF->item merged but not fully!");
            SFXPlayer.Play(mergeAudio);
        }

    }
    private void OnFullyMerged(MergableItem mergedItem)
    {
        //damage type color on snake

        Debug.Log($"FF->Fully Merged Type {mergedItem.Type}!!!");

        if (snakeHandler)
            snakeHandler.DestroySnakeSegment(mergedItem.Type);
    }
}
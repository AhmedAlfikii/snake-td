using System.Collections;
using System.Collections.Generic;
using TafraKit;
using TafraKit.UI;
using UnityEngine;
using UnityEngine.Events;

public class ItemsSpawner : MonoBehaviour
{
    [SerializeField] private SegmentTypesSettings typesSettings;
    [SerializeField] private SFXClips spawnAudio;

    [SerializeField] private float spawnYOffset;
    [SerializeField] private MergableItem itemPrefab;
    private List<MergableItem> spawnedItems = new List<MergableItem>();
    private Camera cam;
    private float screenWidth;
    private float screenHeight;
    private UnityEvent<MergableItem> onSpawnedItem = new UnityEvent<MergableItem>();
    private SnakeHandler snakeHandler;

    public List<MergableItem> SpawnedItems => spawnedItems;
    public UnityEvent<MergableItem> OnSpawnedItem => onSpawnedItem;

    private void Awake()
    {
        int itemStages = 3;
        int neededCopies = Mathf.RoundToInt(Mathf.Pow(2, itemStages - 1));

        snakeHandler = FindFirstObjectByType<SnakeHandler>();

        cam = Camera.main;
    }
    private IEnumerator Start()
    {
        yield return Yielders.EndOfFrame;

        float aspectRatio = cam.aspect;

        screenWidth = aspectRatio * cam.orthographicSize * 2;
        screenHeight = 1 / aspectRatio * screenWidth;
    }
    public void SpawnItem()
    {
        MergableItem item = Instantiate(itemPrefab);
        int itemType = 0;

        if (snakeHandler)
            itemType = Random.Range(0, snakeHandler.MaxColors);
        else
            itemType = Random.Range(0, typesSettings.Materials.Count);

        item.SetType(itemType);
        item.SetMaterials(typesSettings.GetMaterial(itemType));

        Bounds colliderBounds = item.Collider2D.bounds;

        Vector3 boundsMin = colliderBounds.min;
        Vector3 boundsMax = colliderBounds.max;

        float lowerOffset = item.transform.position.y - boundsMin.y + spawnYOffset;
        float rightOffset = boundsMax.x - item.transform.position.x;
        float leftOffset = item.transform.position.x - boundsMin.x;

        float minX = -(screenWidth / 2f) + leftOffset;
        float maxX = (screenWidth / 2f) - rightOffset;

        Vector3 spawnPoint = new Vector3();

        spawnPoint.x = Random.Range(minX, maxX);
        spawnPoint.y = cam.transform.position.y + screenHeight / 2f + lowerOffset;

        item.transform.position = spawnPoint;
        item.transform.rotation = Quaternion.identity;

        item.Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;

        spawnedItems.Add(item);

        onSpawnedItem?.Invoke(item);

        SFXPlayer.Play(spawnAudio);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 pos = Vector3.zero;
        if (cam)
            pos.y = cam.transform.position.y + screenHeight / 2f + spawnYOffset;
        Gizmos.DrawWireCube(pos, Vector3.one);
    }

}

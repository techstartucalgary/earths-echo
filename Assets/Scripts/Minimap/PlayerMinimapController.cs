using UnityEngine;

public class PlayerMinimapController : MonoBehaviour
{
    [Header("World Boundaries")]
    [Tooltip("Minimum X in the game world (left boundary).")]
    [SerializeField] private float worldMinX = 0f;

    [Tooltip("Maximum X in the game world (right boundary).")]
    [SerializeField] private float worldMaxX = 100f;

    [Tooltip("Minimum Y in the game world (bottom boundary).")]
    [SerializeField] private float worldMinY = 0f;

    [Tooltip("Maximum Y in the game world (top boundary).")]
    [SerializeField] private float worldMaxY = 100f;

    [Header("References")]
    [Tooltip("The player's Transform in the scene.")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("RectTransform of the parent object that represents the map area in the UI.")]
    [SerializeField] private RectTransform mapRect;

    [Tooltip("RectTransform of the player marker UI.")]
    [SerializeField] private RectTransform playerMarkerRect;

    private void Update()
    {
        if (playerTransform == null || mapRect == null || playerMarkerRect == null)
            return;

        // 1. Get the player’s position in the world
        Vector3 playerPos = playerTransform.position;

        // 2. Normalize that position based on the world min/max
        //    (so that 0.0 corresponds to worldMin, 1.0 corresponds to worldMax)
        float normalizedX = Mathf.InverseLerp(worldMinX, worldMaxX, playerPos.x);
        float normalizedY = Mathf.InverseLerp(worldMinY, worldMaxY, playerPos.y);

        // 3. Convert normalized coords to map coords within the Map's rect
        //    mapRect.rect.width, mapRect.rect.height give the size in *local* space
        float mapWidth = mapRect.rect.width;
        float mapHeight = mapRect.rect.height;

        float mapPosX = normalizedX * mapWidth;
        float mapPosY = normalizedY * mapHeight;

        // 4. Because the RectTransform pivot might not be at (0,0), 
        //    you may need to adjust for pivot. If your Map is anchored 
        //    from top-left or bottom-left, you might get away with ignoring pivot.
        //
        //    For a pivot at bottom-left (0,0):
        //    anchoredPosition = (mapPosX, mapPosY) 
        //
        //    If your pivot is center-based (0.5, 0.5) or something else,
        //    you need an offset. For simplicity, let's assume bottom-left pivot.

        Vector2 newMarkerPos = new Vector2(mapPosX, mapPosY);

        // 5. Assign the position to the marker's anchoredPosition
        playerMarkerRect.anchoredPosition = newMarkerPos;

        // 6. Clamping (optional, if you want to ensure it never goes outside):
        //    Because we used InverseLerp, 0.0..1.0 is automatically clamped, 
        //    so the marker won't go outside the map area.
    }
}

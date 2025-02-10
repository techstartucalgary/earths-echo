using UnityEngine;

public class MaxPositionFinder : MonoBehaviour
{
    [SerializeField] private string targetLayerName;
    private int targetLayer;

    public static float MaxX { get; private set; } = float.MinValue;
    public static float MaxY { get; private set; } = float.MinValue;
    public static float MinX { get; private set; } = float.MaxValue;
    public static float MinY { get; private set; } = float.MaxValue;

    private void Start()
    {
        targetLayer = LayerMask.NameToLayer(targetLayerName);
        if (targetLayer == -1)
        {
            Debug.LogError("Invalid layer name: " + targetLayerName);
            return;
        }

        FindMaxPositions();
    }
    
    private void Update()
{
    if (Input.GetKeyDown(KeyCode.M)) // Or use a timer instead of keypress
    {
        FindMaxPositions();
        FindObjectOfType<MinimapCameraController>()?.AdjustCameraBounds();
    }
}


    private void FindMaxPositions()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Reset values before recalculating
        MinX = float.MaxValue; MinY = float.MaxValue;
        MaxX = float.MinValue; MaxY = float.MinValue;

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == targetLayer)
            {
                Vector3 position = obj.transform.position;
                if (position.x > MaxX) MaxX = position.x;
                if (position.y > MaxY) MaxY = position.y;
                if (position.x < MinX) MinX = position.x;
                if (position.y < MinY) MinY = position.y;
            }
        }
    }
}

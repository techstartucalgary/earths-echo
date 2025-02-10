using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    [SerializeField] private Camera minimapCamera; // Assign in Inspector

    private void Start()
    {
        if (minimapCamera == null)
        {
            minimapCamera = GetComponent<Camera>();
        }

        AdjustCameraBounds();
    }

    public void AdjustCameraBounds()
    {
        float centerX = (MaxPositionFinder.MinX + MaxPositionFinder.MaxX) / 2f;
        float centerY = (MaxPositionFinder.MinY + MaxPositionFinder.MaxY) / 2f;

        float width = MaxPositionFinder.MaxX - MaxPositionFinder.MinX;
        float height = MaxPositionFinder.MaxY - MaxPositionFinder.MinY;

        // Set the camera position to the center of the map
        minimapCamera.transform.position = new Vector3(centerX, centerY, minimapCamera.transform.position.z);

        // Adjust orthographic size to fit everything
        minimapCamera.orthographicSize = Mathf.Max(width / 2, height / 2);
    }
}

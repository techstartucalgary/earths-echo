using UnityEngine;

public class CameraToTexture : MonoBehaviour
{
    [SerializeField] private Camera sourceCamera; // The camera whose view will be rendered to a texture
    [SerializeField] private RenderTexture outputTexture; // The texture that will receive the rendered image

    private void Start()
    {
        if (sourceCamera == null)
        {
            Debug.LogError("CameraToTexture: No source camera assigned.");
            return;
        }
        if (outputTexture == null)
        {
            Debug.LogError("CameraToTexture: No output texture assigned.");
            return;
        }
        
        // Ensure the camera is rendering to the target texture
        sourceCamera.targetTexture = outputTexture;
    }

    private void OnDestroy()
    {
        // Reset the camera's target texture to avoid issues when disabling/destroying this script
        if (sourceCamera != null)
        {
            sourceCamera.targetTexture = null;
        }
    }
}

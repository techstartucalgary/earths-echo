using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal; // Required for URP

public class CameraSizeTrigger : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera targetCamera; // The camera to modify
    [SerializeField] private float targetSize = 5f; // Camera size when inside the trigger
    [SerializeField] private float transitionSpeed = 2f; // Speed of size change
    private float originalSize; // Stores the original camera size
    private Coroutine transitionCoroutine;

    // Optional: Reference to URP's additional camera data.
    private UniversalAdditionalCameraData urpCameraData;

    private void Start()
    {
        // Auto-assign main camera if not set.
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        
        originalSize = targetCamera.orthographicSize; // Save default size

        // Attempt to get URP-specific additional camera data.
        urpCameraData = targetCamera.GetComponent<UniversalAdditionalCameraData>();
        if (urpCameraData != null)
        {
            // (Optional) Configure URP settings here if needed.
            // For example, you might adjust render scale or other URP-specific properties.
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            transitionCoroutine = StartCoroutine(ChangeCameraSize(targetSize));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            transitionCoroutine = StartCoroutine(ChangeCameraSize(originalSize));
        }
    }

    private IEnumerator ChangeCameraSize(float newSize)
    {
        float startSize = targetCamera.orthographicSize;
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * transitionSpeed;
            targetCamera.orthographicSize = Mathf.Lerp(startSize, newSize, time);
            yield return null;
        }
    }
}

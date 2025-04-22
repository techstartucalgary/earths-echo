using UnityEngine;
using System.Collections;

public class CameraSizeTrigger : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera targetCamera; // The camera to modify
    [SerializeField] private float targetSize = 5f; // Camera size when inside the trigger
    [SerializeField] private float transitionSpeed = 2f; // Speed of size change
    private float originalSize; // Stores the original camera size
    private Coroutine transitionCoroutine;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main; // Auto-assign main camera if not set
        }
        originalSize = targetCamera.orthographicSize; // Save default size
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
            transitionCoroutine = StartCoroutine(ChangeCameraSize(targetSize));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
            transitionCoroutine = StartCoroutine(ChangeCameraSize(originalSize));
        }
    }

    private IEnumerator ChangeCameraSize(float newSize)
    {
        float startSize = targetCamera.orthographicSize;
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime * transitionSpeed;
            targetCamera.orthographicSize = Mathf.Lerp(startSize, newSize, time);
            yield return null;
        }
    }
}

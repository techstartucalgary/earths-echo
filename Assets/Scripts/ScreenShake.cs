using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private Coroutine shakeCoroutine;

    // Call this method to trigger a shake with custom parameters.
    public void Shake(float duration, float magnitude)
    {
        // If a shake is already in progress, stop it before starting a new one.
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(Shaking(duration, magnitude));
    }

    private IEnumerator Shaking(float duration, float magnitude)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // Reduce the shake intensity over time.
            float currentMagnitude = magnitude * (1 - (elapsedTime / duration));
            transform.position = startPosition + Random.insideUnitSphere * currentMagnitude;
            yield return null;
        }

        transform.position = startPosition;
    }
}

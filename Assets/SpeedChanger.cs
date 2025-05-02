using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedChanger : MonoBehaviour
{
    [SerializeField]
    private float timeScaleChange = -0.5f; // Negative to slow down, positive to speed up

    [SerializeField]
    private float blendInTime = 0.1f; // Time in seconds to blend in the time scale change

    [SerializeField]
    private float blendOutTime = 2f; // Time in seconds to blend out the time scale change

    private Coroutine timeScaleCoroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float targetTimeScale = Mathf.Clamp(Time.timeScale + timeScaleChange, 0.1f, 2f);
            if (timeScaleCoroutine != null)
            {
                StopCoroutine(timeScaleCoroutine);
            }
            timeScaleCoroutine = StartCoroutine(BlendTimeScale(targetTimeScale, blendInTime));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (timeScaleCoroutine != null)
            {
                StopCoroutine(timeScaleCoroutine);
            }
            timeScaleCoroutine = StartCoroutine(BlendTimeScale(1f, blendOutTime)); // Reset to normal time scale
        }
    }

    private IEnumerator BlendTimeScale(float targetTimeScale, float blendTime)
    {
        float startTimeScale = Time.timeScale;
        float elapsedTime = 0f;

        while (elapsedTime < blendTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeScale, elapsedTime / blendTime);
            yield return null;
        }

        Time.timeScale = targetTimeScale; // Ensure it reaches the target value
    }
}

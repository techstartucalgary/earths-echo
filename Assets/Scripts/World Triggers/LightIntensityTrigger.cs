using UnityEngine;
using UnityEngine.Rendering.Universal; // Required for Light2D
using System.Collections; // Required for IEnumerator

public class LightIntensityTrigger : MonoBehaviour
{
    public enum LightMode
    {
        Temporary,  // Light changes only while inside the trigger
        Persistent  // Light intensity remains changed even after leaving
    }

    [SerializeField] private Light2D sceneLight; // Reference to the global light
    [SerializeField] private float wantedIntensity; // Intensity when inside the trigger
    [SerializeField] private float transitionSpeed; // Speed of transition
    [SerializeField] private LightMode lightMode = LightMode.Temporary; // Dropdown selection

    private float originalIntensity; // Stores the light intensity before entering
    private bool isInsideTrigger = false; // Tracks whether the player is inside
    private Coroutine lightCoroutine;

    private void Start()
    {
        originalIntensity = sceneLight.intensity; // Store initial light intensity
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isInsideTrigger)
            {
                isInsideTrigger = true;

                if (lightMode == LightMode.Persistent)
                {
                    if (lightCoroutine != null) StopCoroutine(lightCoroutine);
                    lightCoroutine = StartCoroutine(ChangeLightIntensity(wantedIntensity));
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && lightMode == LightMode.Temporary)
        {
            if (sceneLight.intensity != wantedIntensity)
            {
                if (lightCoroutine != null) StopCoroutine(lightCoroutine);
                lightCoroutine = StartCoroutine(ChangeLightIntensity(wantedIntensity));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isInsideTrigger)
            {
                isInsideTrigger = false;

                if (lightMode == LightMode.Temporary)
                {
                    if (lightCoroutine != null) StopCoroutine(lightCoroutine);
                    lightCoroutine = StartCoroutine(ChangeLightIntensity(originalIntensity));
                }
                else if (lightMode == LightMode.Persistent)
                {
                    // Keep the intensity as the new default
                    originalIntensity = wantedIntensity;
                }
            }
        }
    }

    private IEnumerator ChangeLightIntensity(float targetIntensity)
    {
        float startIntensity = sceneLight.intensity;
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime * transitionSpeed;
            sceneLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, time);
            yield return null;
        }
    }

    public void SetLightMode(LightMode mode)
    {
        lightMode = mode;
    }
}

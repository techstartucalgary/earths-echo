using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightController : MonoBehaviour
{
    private Light2D playerLight;
    private Rigidbody2D rb;

    [Header("Directional Light Settings")]
    public float rotationOffset = -90f;

    [Header("Point Light Settings")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 1f;
    public float intensitySpeed = 2f;

    [Header("Common Settings")]
    public bool enableLight = true; 

    void Awake()
    {
        playerLight = GetComponent<Light2D>();
        rb = GetComponent<Rigidbody2D>();

        if (playerLight == null)
        {
            Debug.LogError("No Light2D component found on the player.");
        }
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D component found on the player.");
        }
        Debug.Log("PlayerLightController initialized.");
        playerLight.lightType = Light2D.LightType.Point;
    }

    void Update()
    {
        if (!enableLight || playerLight == null)
        {
            if (playerLight != null)
                playerLight.enabled = false;
            return;
        }

        playerLight.enabled = true;

        switch (playerLight.lightType)
        {
            case Light2D.LightType.Point:
                UpdatePointLight();
                break;

            case Light2D.LightType.Parametric:
                UpdateDirectionalLight();
                break;

                // You can add cases for other light types if needed
        }
    }

    void UpdateDirectionalLight()
    {
        // Adjust light direction based on movement
        Vector2 direction = rb.velocity.normalized;
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            playerLight.transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
        }
    }

    void UpdatePointLight()
    {
        // Example logic: Modulate intensity based on speed
        float speed = rb.velocity.magnitude;
        float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, speed / GetMaxSpeed());
        playerLight.intensity = Mathf.Lerp(playerLight.intensity, targetIntensity, Time.deltaTime * intensitySpeed);
    }

    float GetMaxSpeed()
    {
        // Replace with your player's max speed
        return 10f;
    }

    // Public methods to control the light
    public void EnableLight()
    {
        enableLight = true;
    }

    public void DisableLight()
    {
        enableLight = false;
    }

    public void SetLightColor(Color color)
    {
        if (playerLight != null)
            playerLight.color = color;
    }

    public void SetLightIntensity(float intensity)
    {
        if (playerLight != null)
            playerLight.intensity = intensity;
    }
}

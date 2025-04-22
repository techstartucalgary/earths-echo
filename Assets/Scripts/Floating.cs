using UnityEngine;

public class FloatingLight : MonoBehaviour
{
    private Vector3 startPos;

    public float floatHeight = 0.5f;
    public float floatSpeed = 1f;
    public float driftRadius = 0.1f;
    public float driftSpeed = 0.5f;

    private float timeOffset;
    private Vector2 perlinSeed;

    void Start()
    {
        startPos = transform.position;

        // Add random offset so they don’t float in sync
        timeOffset = Random.Range(0f, 100f);

        // Generate a unique seed for each object’s Perlin noise
        perlinSeed = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));
    }

    void Update()
    {
        float time = Time.time + timeOffset;

        // Vertical float with sine wave
        float newY = Mathf.Sin(time * floatSpeed) * floatHeight;

        // Smooth drift using Perlin noise
        float driftX = (Mathf.PerlinNoise(perlinSeed.x, time * driftSpeed) - 0.5f) * 2f * driftRadius;
        float driftZ = (Mathf.PerlinNoise(perlinSeed.y, time * driftSpeed) - 0.5f) * 2f * driftRadius;

        Vector3 drift = new Vector3(driftX, 0f, driftZ);

        transform.position = startPos + new Vector3(0f, newY, 0f) + drift;
    }
}

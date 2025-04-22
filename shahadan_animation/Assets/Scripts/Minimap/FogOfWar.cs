using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    [SerializeField] private int textureSize = 256; // Lower for better performance
    [SerializeField] private int revealWidth = 3; // Horizontal reveal size
    [SerializeField] private int revealHeight = 7; // Vertical reveal size (taller than width)
    [SerializeField] private Transform player; // Assign in Inspector
    [SerializeField] private RenderTexture fogTexture; // Assign in Inspector

    private Texture2D fogMap;
    private Color[] fogPixels;
    private Vector2 lastRevealedPosition = Vector2.negativeInfinity;
    private float revealThreshold = 0.2f; // Reveal updates when moving 0.2 units
    private float updateInterval = 0.05f; // Check movement every 0.05 seconds
    private float lastUpdateTime;

    private void Start()
    {
        // Create fog texture
        fogMap = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
        fogPixels = new Color[textureSize * textureSize];

        // Fill with black (fully fogged)
        for (int i = 0; i < fogPixels.Length; i++)
        {
            fogPixels[i] = Color.black;
        }

        UpdateFogTexture();
    }

    private void Update()
    {
        // Check the update interval (prevents excessive calculations)
        if (Time.time - lastUpdateTime > updateInterval)
        {
            float distanceMoved = Vector2.Distance(player.position, lastRevealedPosition);
            
            // Only update if player has moved at least `revealThreshold`
            if (distanceMoved > revealThreshold)
            {
                // Reveal all positions between last and current position
                RevealPath(lastRevealedPosition, player.position);
                lastRevealedPosition = player.position;
            }

            lastUpdateTime = Time.time;
        }
    }

    private void RevealPath(Vector3 startPos, Vector3 endPos)
    {
        float stepSize = revealThreshold / 2f; // Break into smaller updates
        float distance = Vector3.Distance(startPos, endPos);
        int steps = Mathf.CeilToInt(distance / stepSize);

        for (int i = 0; i <= steps; i++)
        {
            Vector3 interpolatedPos = Vector3.Lerp(startPos, endPos, i / (float)steps);
            RevealArea(interpolatedPos);
        }
    }

    private void RevealArea(Vector3 worldPosition)
    {
        Vector2 texCoords = WorldToTextureCoords(worldPosition);
        int centerX = Mathf.RoundToInt(texCoords.x);
        int centerY = Mathf.RoundToInt(texCoords.y);

        int halfWidth = Mathf.RoundToInt(revealWidth * 0.5f);
        int halfHeight = Mathf.RoundToInt(revealHeight * 0.5f);

        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int y = -halfHeight; y <= halfHeight; y++)
            {
                int px = centerX + x;
                int py = centerY + y;

                if (px >= 0 && px < textureSize && py >= 0 && py < textureSize)
                {
                    int index = py * textureSize + px;

                    // Smooth fog reveal based on distance
                    float distX = Mathf.Abs(x) / (float)halfWidth;
                    float distY = Mathf.Abs(y) / (float)halfHeight;
                    float edgeFactor = Mathf.Max(distX, distY);
                    float fadeAmount = Mathf.SmoothStep(1.0f, 0.0f, edgeFactor);

                    fogPixels[index].a = Mathf.Lerp(fogPixels[index].a, 0, fadeAmount);
                }
            }
        }

        UpdateFogTexture();
    }

    private void UpdateFogTexture()
    {
        fogMap.SetPixels(fogPixels);
        fogMap.Apply();
        fogTexture.Release(); // Force update
        Graphics.Blit(fogMap, fogTexture);
    }

    private Vector2 WorldToTextureCoords(Vector3 worldPos)
    {
        float normalizedX = Mathf.InverseLerp(MaxPositionFinder.MinX, MaxPositionFinder.MaxX, worldPos.x);
        float normalizedY = Mathf.InverseLerp(MaxPositionFinder.MinY, MaxPositionFinder.MaxY, worldPos.y);
        return new Vector2(normalizedX * textureSize, normalizedY * textureSize);
    }
}

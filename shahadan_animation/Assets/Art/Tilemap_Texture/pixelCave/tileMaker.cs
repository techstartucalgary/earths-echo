using UnityEngine;
using System.Collections.Generic;

public class SpriteLoader : MonoBehaviour
{
    public TextAsset jsonFile;  // Assign the JSON file in the Inspector
    public Texture2D spriteSheetTexture;  // Assign the PNG file in the Inspector

    [System.Serializable]
    public class Frame
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }

    [System.Serializable]
    public class SpriteData
    {
        public Dictionary<string, Frame> frames;
    }

    private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

    void Start()
    {
        // Parse the JSON file
        var data = JsonUtility.FromJson<SpriteData>(jsonFile.text);

        // Loop through each entry in the JSON file
        foreach (var entry in data.frames)
        {
            string spriteName = entry.Key;
            Frame frame = entry.Value;

            // Create a sprite using the texture and frame data
            Sprite sprite = Sprite.Create(
                spriteSheetTexture,
                new Rect(frame.x, frame.y, frame.width, frame.height),
                new Vector2(0.5f, 0.5f), // Pivot point
                100f // Pixels Per Unit
            );

            sprites.Add(spriteName, sprite);
            Debug.Log($"Sprite '{spriteName}' loaded.");
        }

        // Example: Use the sprite (e.g., assign it to a GameObject's SpriteRenderer)
        GameObject exampleObject = new GameObject("ExampleSprite");
        SpriteRenderer renderer = exampleObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprites["bg1"]; // Replace "bg1" with the desired sprite name
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    public ParallaxCamera parallaxCamera;
    List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    private bool initialized = false;

    void Start()
    {
        if (parallaxCamera == null)
            parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();

        if (parallaxCamera != null)
            parallaxCamera.onCameraTranslate += Move;

        SetLayers();

        // Shift every child in every layer individually by their x distance from the camera * their parallax factor
        if (!Application.isPlaying && !initialized)
        {

            Console.WriteLine("ParallaxBackground Start");

            float cameraX = Camera.main.transform.position.x;

            for (int i = 0; i < parallaxLayers.Count; i++)
            {
                // Get all children in the layer
                Transform[] children = parallaxLayers[i].GetComponentsInChildren<Transform>();

                Console.WriteLine("Layer " + i + " has " + children.Length + " children");

                // For each child, shift it by its x distance from the camera * the layer's parallax factor
                foreach (Transform child in children)
                {
                    if (child == parallaxLayers[i].transform)
                        continue;

                    float parallaxFactor = parallaxLayers[i].parallaxFactor;
                    Vector3 newPos = child.position;
                    newPos.x += (cameraX - transform.position.x) * parallaxFactor;
                    child.position = newPos;
                }
            }

            initialized = true;
        }
    }

    void SetLayers()
    {
        parallaxLayers.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

            if (layer != null)
            {
                layer.name = "Layer-" + i;
                parallaxLayers.Add(layer);
            }
        }
    }

    void Move(float delta)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.Move(delta);
        }
    }
}

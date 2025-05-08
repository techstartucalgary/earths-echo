using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] public float parallaxFactor;

    private void Start()
    {
        // Start a coroutine to delay execution
        StartCoroutine(DelayedAdjustChildrenPositions());
    }

    private IEnumerator DelayedAdjustChildrenPositions()
    {
        // Wait for 0.25 seconds
        yield return new WaitForSeconds(0.25f);

        // Ensure this only runs in play mode
        if (!Application.isPlaying) yield break;

        AdjustChildrenPositions();
    }

    private void AdjustChildrenPositions()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("Main Camera not found!");
            return;
        }

        Vector3 cameraStartPosition = mainCamera.transform.position;

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child == transform) continue; // Skip the parent object itself

            Vector3 childPosition = child.position;
            float differenceFromCamera = childPosition.x - cameraStartPosition.x;

            // Shift the child based on the parallax factor and its distance from the camera
            childPosition.x -= 2 * parallaxFactor * differenceFromCamera;
            child.position = childPosition;
        }
    }

    public void Move(float delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta * parallaxFactor;

        transform.localPosition = newPos;
    }
}

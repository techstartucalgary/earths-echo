using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRotation : MonoBehaviour
{
    public Camera mainCamera; // Reference to the main camera
    public Transform playerTransform; // Reference to the player's transform
    public float rotationSpeed = 5f; // Rotation speed (adjustable in the Inspector)
    public float maxAngle = 90f; // Maximum angle up or down

    private bool isFlipped = false; // Track if the player is flipped

    private void Update()
    {
        if (mainCamera == null || playerTransform == null)
        {
            Debug.LogError("Main Camera or Player Transform is not assigned.");
            return;
        }

        // Get mouse position in world space
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Calculate direction from weapon to mouse
        Vector3 direction = mouseWorldPosition - transform.position;
        direction.z = 0; // Ensure the weapon remains in the 2D plane

        // Determine if the player is flipped
        isFlipped = playerTransform.localScale.x < 0;

        // Get the target angle in degrees
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust target angle based on player facing direction
        if (isFlipped)
        {
            targetAngle = Mathf.Clamp(targetAngle, 180f - maxAngle, 180f + maxAngle);
        }
        else
        {
            targetAngle = Mathf.Clamp(targetAngle, -maxAngle, maxAngle);
        }

        // Get the current angle
        float currentAngle = transform.rotation.eulerAngles.z;

        // Smoothly rotate to the target angle using Mathf.LerpAngle
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);

        // Apply the rotation
        transform.rotation = Quaternion.Euler(0, 0, newAngle);
    }
}

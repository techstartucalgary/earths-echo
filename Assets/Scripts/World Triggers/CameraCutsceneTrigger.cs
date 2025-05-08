using UnityEngine;
using System.Collections;

public enum CameraPathMode
{
    Direct,     // Go directly to the cutscene target position.
    CustomPath  // Traverse through a list of waypoints.
}

public class CameraCutsceneTriggerWithSize : MonoBehaviour
{
    [Header("Camera Cutscene Settings")]
    [SerializeField] private Camera targetCamera;            // The camera to pan and resize
    [SerializeField] private Transform player;               // Reference to the player's transform
    [SerializeField] private MonoBehaviour followScript;     // (Optional) A camera follow script to disable during the cutscene
    [SerializeField] private MonoBehaviour playerInputScript;  // (Optional) The player's input script to disable during the cutscene

    [Header("Cutscene Path Options")]
    [SerializeField] private CameraPathMode cameraPathMode = CameraPathMode.Direct;
    [SerializeField] private Vector3 cutsceneTargetPosition;   // The initial position to pan the camera to
    [Tooltip("When using CustomPath mode, these waypoints will be traversed sequentially.")]
    [SerializeField] private Transform[] pathPoints;
    [SerializeField] private float segmentDuration = 1f;       // Duration for each path segment (only used in CustomPath mode)

    [Header("Timing Settings")]
    [SerializeField] private float panOutDuration = 2f;        // Duration for panning to the cutscene target (used in Direct mode)
    [SerializeField] private float holdDuration = 2f;          // Duration to hold at the final target position
    [SerializeField] private float panBackDuration = 2f;       // Duration for panning back to the player

    [Header("Camera Size Settings")]
    [SerializeField] private float targetSize = 10f;           // The orthographic size during the cutscene

    [Header("Settling Settings")]
    [SerializeField] private float settleSpeed = 10f;          // Speed at which the camera "settles" to the player
    [SerializeField] private float settleThreshold = 0.05f;    // Distance threshold before handing off

    private bool cutsceneTriggered = false;
    private bool cutsceneFinished = false;
    private bool playerHasExited = false;
    private float originalSize;

    private void Awake()
    {
        // Auto-assign the main camera if not provided.
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        // Save the original camera size for reverting later.
        originalSize = targetCamera.orthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Try to get the Player component from this collider or its parents.
        var playerInput = other.GetComponentInParent<Player>();
        if (!cutsceneTriggered && playerInput != null)
        {
            cutsceneTriggered = true;

            // If player's transform is not assigned, grab it from the detected PlayerInput component.
            if (player == null)
            {
                player = playerInput.transform;
            }

            // Automatically assign the player input script if not already set.
            if (playerInputScript == null)
            {
                playerInputScript = playerInput;
                Debug.Log($"Assigned playerInputScript: {playerInputScript}");
            }

            // Disable camera follow behavior if assigned.
            if (followScript != null)
            {
                followScript.enabled = false;
                Debug.Log("Disabled followScript.");
            }

            // Disable player input.
            if (playerInputScript != null)
            {
                playerInputScript.enabled = false;
                Debug.Log("Disabled playerInputScript.");
            }
            else
            {
                Debug.LogWarning("playerInputScript is null. Cannot disable player input.");
            }

            StartCoroutine(CutsceneRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // In case the player leaves before the cutscene finishes.
        var playerInput = other.GetComponentInParent<PlayerInput>();
        if (playerInput != null)
        {
            playerHasExited = true;
            if (cutsceneFinished)
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator CutsceneRoutine()
    {
        // Save the starting camera position and size (presumably following the player).
        Vector3 originalPosition = targetCamera.transform.position;
        float originalSize = targetCamera.orthographicSize;

        Debug.Log($"Starting Cutscene. Original Position: {originalPosition}, Original Size: {originalSize}");

        // Determine the outgoing path based on the chosen mode.
        Vector3 finalOutgoingTarget = cutsceneTargetPosition;
        if (cameraPathMode == CameraPathMode.Direct)
        {
            // Direct mode: Pan from original to cutsceneTargetPosition.
            yield return StartCoroutine(TransitionCamera(originalPosition, cutsceneTargetPosition, originalSize, targetSize, panOutDuration));
        }
        else if (cameraPathMode == CameraPathMode.CustomPath)
        {
            // Custom path mode: First pan from original to cutsceneTargetPosition.
            yield return StartCoroutine(TransitionCamera(originalPosition, cutsceneTargetPosition, originalSize, targetSize, panOutDuration));

            // Then traverse each waypoint sequentially.
            foreach (Transform waypoint in pathPoints)
            {
                Vector3 waypointPos = waypoint.position;
                waypointPos.z = targetCamera.transform.position.z; // Preserve Z-position
                Debug.Log($"Moving to waypoint: {waypointPos}");
                yield return StartCoroutine(TransitionCamera(targetCamera.transform.position, waypointPos, targetCamera.orthographicSize, targetSize, segmentDuration));
                // Set final target to the last waypoint.
                finalOutgoingTarget = waypointPos;
            }
        }

        // Hold at the final outgoing target.
        yield return new WaitForSeconds(holdDuration);

        // Pan back: Continuously update the target to the player's current position.
        yield return StartCoroutine(PanBackToPlayer(finalOutgoingTarget, targetSize, panBackDuration));

        // Settling phase: Smoothly correct any remaining offset between the camera and the player's position.
        while (Vector3.Distance(targetCamera.transform.position, PlayerCameraPos()) > settleThreshold)
        {
            targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, PlayerCameraPos(), Time.deltaTime * settleSpeed);
            targetCamera.orthographicSize = Mathf.Lerp(targetCamera.orthographicSize, originalSize, Time.deltaTime * settleSpeed);
            yield return null;
        }

        // Ensure camera is exactly at the player's position and size.
        targetCamera.transform.position = PlayerCameraPos();
        targetCamera.orthographicSize = originalSize;

        Debug.Log($"Cutscene finished. Restored Position: {targetCamera.transform.position}, Restored Size: {targetCamera.orthographicSize}");

        // Mark the cutscene as finished.
        cutsceneFinished = true;

        // Re-enable follow behavior and player input on the next frame.
        yield return null;
        if (followScript != null)
        {
            followScript.enabled = true;
        }
        if (playerInputScript != null)
        {
            playerInputScript.enabled = true;
        }

        // If the player has already exited, destroy this trigger.
        if (playerHasExited)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Returns the player's current position adjusted for the camera's Z value.
    /// </summary>
    private Vector3 PlayerCameraPos()
    {
        Vector3 pPos = player.position;
        pPos.z = targetCamera.transform.position.z;
        return pPos;
    }

    /// <summary>
    /// Pans back toward the player's position by continuously updating the target position.
    /// </summary>
    private IEnumerator PanBackToPlayer(Vector3 fromPos, float fromSize, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPos = fromPos;
        float startSize = fromSize;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            // Continuously update the player's current position.
            Vector3 dynamicTargetPos = PlayerCameraPos();
            targetCamera.transform.position = Vector3.Lerp(startPos, dynamicTargetPos, t);
            targetCamera.orthographicSize = Mathf.Lerp(startSize, originalSize, t);
            yield return null;
        }
    }

    /// <summary>
    /// Smoothly transitions the camera from one position and size to another using easing.
    /// </summary>
    private IEnumerator TransitionCamera(Vector3 fromPos, Vector3 toPos, float fromSize, float toSize, float duration)
    {
        Debug.Log($"Transitioning camera from {fromPos} to {toPos} over {duration} seconds.");
        float elapsedTime = 0f;
        fromPos.z = targetCamera.transform.position.z; // Preserve Z-position
        toPos.z = targetCamera.transform.position.z;   // Preserve Z-position

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            t = Mathf.SmoothStep(0f, 1f, t);
            targetCamera.transform.position = Vector3.Lerp(fromPos, toPos, t);
            targetCamera.orthographicSize = Mathf.Lerp(fromSize, toSize, t);
            yield return null;
        }
        targetCamera.transform.position = toPos;
        targetCamera.orthographicSize = toSize;
    }
}

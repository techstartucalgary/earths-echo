using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ProjectileWeapon projectileWeapon;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Trajectory Line Smoothness/Length")]
    [SerializeField] private int segmentCount = 50;
    [SerializeField] private float curveLength = 3.5f;

    private Vector2[] segments;
    private LineRenderer lineRenderer;

    private float projectileSpeed;
    private float projectileGravityFromRB;

    private const float TIME_CURVE_ADDITON = 0.5f;

    private void Start()
    {
        segments = new Vector2[segmentCount];
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer component is missing!");
            return;
        }

        lineRenderer.positionCount = segmentCount;

        if (projectileWeapon == null)
        {
            Debug.LogError("ProjectileWeapon reference is not assigned!");
            return;
        }

        if (projectileWeapon.projectilePrefab == null)
        {
            Debug.LogError("Projectile prefab is not assigned in ProjectileWeapon!");
            return;
        }

        ProjectileBehaviour projectileBehaviour = projectileWeapon.projectilePrefab.GetComponent<ProjectileBehaviour>();
        if (projectileBehaviour == null)
        {
            Debug.LogError("Projectile prefab does not contain a ProjectileBehaviour script!");
            return;
        }

        projectileSpeed = projectileBehaviour.physicsProjectileVelocity;
        projectileGravityFromRB = projectileBehaviour.gravityScale;
    }

    private void Update()
    {
        // Get pullback progress to adjust speed and gravity dynamically
        float powerPercentage = projectileWeapon.GetPullbackPower();

        // Adjust projectile speed and gravity based on pullback progress
        float adjustedSpeed = Mathf.Lerp(projectileSpeed * 0.5f, projectileSpeed, powerPercentage);
        float adjustedGravity = Mathf.Lerp(projectileGravityFromRB * 2f, projectileGravityFromRB, powerPercentage);

        // Set the start position
        Vector2 startPos = projectileSpawnPoint.position;
        segments[0] = startPos;
        lineRenderer.SetPosition(0, startPos);

        // Set starting velocity
        Vector2 startVelocity = transform.right * adjustedSpeed;

        for (int i = 1; i < segmentCount; i++)
        {
            float timeOffset = (i * Time.fixedDeltaTime * curveLength);

            // Compute gravity offset
            Vector2 gravityOffset = TIME_CURVE_ADDITON * Physics2D.gravity * adjustedGravity * Mathf.Pow(timeOffset, 2);

            // Set position of point in line renderer
            segments[i] = segments[0] + startVelocity * timeOffset + gravityOffset;
            lineRenderer.SetPosition(i, segments[i]);
        }
    }
}

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

		// Start position and velocity
		Vector2 startPos = projectileSpawnPoint.position;
		Vector2 startVelocity = projectileSpawnPoint.right * adjustedSpeed;

		segments[0] = startPos;
		lineRenderer.SetPosition(0, startPos);

		// Iterate through trajectory points
		for (int i = 1; i < segmentCount; i++)
		{
			// Time for this segment
			float time = i * Time.fixedDeltaTime * curveLength;

			// Position using the kinematic equation: x = x0 + vt + 0.5 * a * t^2
			Vector2 position = startPos + startVelocity * time + 0.5f * Physics2D.gravity * adjustedGravity * time * time;

			// Set segment position
			segments[i] = position;
			lineRenderer.SetPosition(i, position);
		}
	}

}

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryLine : MonoBehaviour
{
    [Tooltip("Number of points to sample in the trajectory line.")]
    [SerializeField] private int resolution = 50;
    
    [Tooltip("Time interval between sampled points.")]
    public float timeStep = 0.1f;
    
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Renders a predicted trajectory given the start position, initial velocity, and gravity scale.
    /// </summary>
    /// <param name="startPosition">Where the projectile starts.</param>
    /// <param name="initialVelocity">Initial velocity vector.</param>
    /// <param name="gravityScale">Effective gravity scale to use in the calculation.</param>
	public void RenderTrajectory(Vector3 startPosition, Vector3 initialVelocity, float gravityScale)
	{
		// Option: Use fixedDeltaTime for simulation
		float dt = Time.fixedDeltaTime; // or use a custom timeStep if preferred

		lineRenderer.positionCount = resolution;
		Vector3[] positions = new Vector3[resolution];

		// Gravity used in physics simulation
		Vector3 gravity = Physics2D.gravity * gravityScale;
		
		for (int i = 0; i < resolution; i++)
		{
			float t = i * dt;
			positions[i] = startPosition + initialVelocity * t + 0.5f * gravity * t * t;
		}
		
		lineRenderer.SetPositions(positions);
	}


    /// <summary>
    /// Hides the trajectory line.
    /// </summary>
    public void HideTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
}

// ToxicRain.cs
using UnityEngine;

public class ToxicRain : MonoBehaviour
{
    [Header("Particle System Settings")]
    public ParticleSystem rainParticleSystem;
    public float xStart = -10f;
    public float xEnd = 10f;
    public float yStart = 10f;
    public float yEnd = -10f;
    public float rainSpeed = 5f;
    public float damagePerSecond = 0.001f; // Adjusted for testing

    [Header("Player Reference")]
    public Player player; // Assign via Inspector or use tags

    private ParticleSystem.Particle[] particles;
    private Collider2D playerCollider;

    void Start()
    {
        // Initialize Particle System
        if (rainParticleSystem == null)
        {
            rainParticleSystem = GetComponent<ParticleSystem>();
            if (rainParticleSystem == null)
            {
                Debug.LogError("ParticleSystem component not found on ToxicRain GameObject.");
                enabled = false; // Disable script to prevent further errors
                return;
            }
        }

        // Initialize particles array
        particles = new ParticleSystem.Particle[rainParticleSystem.main.maxParticles];

        // Ensure Player is assigned
        if (player == null)
        {
            // Attempt to find Player via tag
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.GetComponent<Player>();
                if (player == null)
                {
                    Debug.LogError("Player GameObject does not have a Player script attached.");
                    enabled = false;
                    return;
                }
            }
            else
            {
                Debug.LogError("No GameObject with tag 'Player' found in the scene.");
                enabled = false;
                return;
            }
        }

        // Get the Player's Collider2D
        playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("Player does not have a Collider2D component.");
            enabled = false;
            return;
        }

        // Check if the HealthBar is assigned
        if (player.healthBar == null)
        {
            Debug.LogError("Player's HealthBar is not assigned.");
            enabled = false;
            return;
        }
    }
    public float invincibilityDuration = 0.5f; // Half a second
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    void Update()
    {
        if (player == null || playerCollider == null || player.healthBar == null)
            return;

        // Update invincibility timer
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }

        // Retrieve current particles
        int numParticlesAlive = rainParticleSystem.GetParticles(particles);
        bool isPlayerHit = false;

        for (int i = 0; i < numParticlesAlive; i++)
        {
            // Move particle downward
            particles[i].position += Vector3.down * rainSpeed * Time.deltaTime;

            // Reset particle position if it reaches yEnd
            if (particles[i].position.y <= yEnd)
            {
                float newX = Random.Range(xStart, xEnd);
                particles[i].position = new Vector3(newX, yStart, 0);
            }

            // Check collision with Player
            Vector2 particlePos2D = new Vector2(particles[i].position.x, particles[i].position.y);
            if (playerCollider.OverlapPoint(particlePos2D))
            {
                isPlayerHit = true;
                // Optional: Reset particle position to avoid multiple damages per frame
                // particles[i].position = new Vector3(Random.Range(xStart, xEnd), yStart, 0);
            }
        }

        if (isPlayerHit && !isInvincible)
        {
            player.healthBar.Damage(damagePerSecond * Time.deltaTime);
            Debug.Log("ToxicRain: Player damaged by " + (damagePerSecond * Time.deltaTime));

            // Trigger invincibility
            isInvincible = true;
            invincibilityTimer = invincibilityDuration;
        }

        // Apply the updated particle positions
        rainParticleSystem.SetParticles(particles, numParticlesAlive);
    }
}

using UnityEngine;

public class WaterBuoyancy : MonoBehaviour
{
    [Header("Buoyancy Settings")]
    [SerializeField] private float buoyancyStrength = 5f; // Strength of the buoyancy force
    [SerializeField] private float waterDrag = 2f; // Drag when in water
    [SerializeField] private float floatHeight = 0.5f; // Controls how high the object floats
    [SerializeField] private float verticalStabilization = 0.2f; // Helps prevent bouncing
    [SerializeField] private float smoothDamping = 1f; // Smoother force application

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0.3f; // Reduce gravity in water
                rb.drag = waterDrag; // Add drag to simulate water resistance
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float displacement = floatHeight - (other.transform.position.y - transform.position.y);
                float buoyancyForce = buoyancyStrength * displacement - rb.velocity.y * verticalStabilization;

                rb.AddForce(Vector2.up * buoyancyForce * Time.fixedDeltaTime, ForceMode2D.Force);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 1f; // Reset gravity when leaving water
                rb.drag = 0f; // Reset drag
            }
        }
    }
}

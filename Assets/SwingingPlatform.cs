using UnityEngine;

public class PlatformSwing : MonoBehaviour
{
    public float swingForce = 10f;
    public float swingSpeed = 1f;

    private Rigidbody2D rb;
    private float timeCounter = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        timeCounter += Time.fixedDeltaTime;
        float force = Mathf.Sin(timeCounter * swingSpeed) * swingForce;
        rb.AddForce(Vector2.right * force);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}

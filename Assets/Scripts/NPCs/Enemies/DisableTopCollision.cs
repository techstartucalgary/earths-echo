using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DisableTopCollision : MonoBehaviour
{
    private Collider2D _bossCollider;

    private void Awake()
    {
        _bossCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (var contact in collision.contacts)
        {
            // contact.normal is the boss’s surface normal at the contact point.
            // If it's pointing mostly up, the player is hitting from above.
            if (contact.normal.y > 0.5f)
            {
                // ignore any further contacts with that player collider
                Physics2D.IgnoreCollision(_bossCollider, collision.collider);
                break;
            }
        }
    }
}

using System.Collections;
using UnityEngine;

public class EE_ArrowLauncher : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Sensor playerSensor;
    [SerializeField] private string targetTag = "Player";

    [Header("Projectile Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float arrowSpeed = 10f;
    [SerializeField] private float shootCooldown = 2f;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSound;

    private Transform target;
    private float cooldownTimer = 0f;

    void Start()
    {
        playerSensor.setTargetTag(targetTag);
        target = GameObject.FindGameObjectWithTag(targetTag)?.transform;

        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Arrow prefab or fire point not assigned.");
        }
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (playerSensor.IsTargetInRange && cooldownTimer <= 0f)
        {
            FireArrow();
            cooldownTimer = shootCooldown;
        }
    }

    private void FireArrow()
    {
        if (arrowPrefab == null || firePoint == null || target == null)
            return;

        Vector2 direction = (target.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, rotation);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * arrowSpeed;
        }

        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, firePoint.position);
        }
    }
}

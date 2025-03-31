using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private RectTransform bar;    // The actual bar that shrinks
    private Image barImage;         // The image component to change color

    private float maxHealth;        // Maximum health value
    private float currentHealth;    // Current health value
    private float lowHealthThreshold; // Fraction representing low health (e.g., 0.3 for 30%)

    // (Optional) Reference to the parent GameObject if you wish to control activation.
    // For the player, you might keep the bar always active.
    private GameObject healthBarParent;

    void Start()
    {
        bar = GetComponent<RectTransform>();
        barImage = GetComponent<Image>();
        barImage.color = Color.green;

        // If the HealthBar is a child, you can reference its parent.
        healthBarParent = transform.parent ? transform.parent.gameObject : gameObject;
    }

    /// <summary>
    /// Initializes the player's health bar.
    /// </summary>
    /// <param name="maxHealth">The maximum health value for the player.</param>
    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
        this.lowHealthThreshold = 0.3f; // 30% of max health

        UpdateBar();
    }

    /// <summary>
    /// Applies damage to the player's health and updates the bar.
    /// </summary>
    /// <param name="damageAmount">The amount of damage to apply.</param>
    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Change the bar color if current health is below or equal to 30% of max health.
        if (currentHealth / maxHealth <= lowHealthThreshold)
        {
            barImage.color = Color.red;
        }
        else
        {
            barImage.color = Color.green;
        }
        if (currentHealth <= 0f)
        {
            GameManager.instance.GameOver();
        }

        UpdateBar();
    }

    /// <summary>
    /// Heals the player and updates the health bar.
    /// </summary>
    /// <param name="healAmount">The amount to heal.</param>
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update bar color based on the new health percentage.


        UpdateBar();
    }

    /// <summary>
    /// Updates the visual scale of the health bar based on the current health.
    /// </summary>
    private void UpdateBar()
    {
        if (bar != null)
        {
            // Calculate the new width as a percentage of the maximum health.
            float sizePercentage = currentHealth / maxHealth;
            bar.localScale = new Vector3(sizePercentage, 1, 1);
        if (currentHealth / maxHealth <= lowHealthThreshold)
        {
            barImage.color = Color.red;
        }
        else
        {
            barImage.color = Color.green;
        }
        }
    }

    /// <summary>
    /// (Optional) Returns the current health value.
    /// </summary>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    public void ResetHealthBar()
    {
        currentHealth = maxHealth;
        UpdateBar();
    }
}

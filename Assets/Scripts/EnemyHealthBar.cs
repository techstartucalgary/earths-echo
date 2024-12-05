using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private RectTransform bar; // The actual bar that shrinks
    private Image barImage;

    private float maxHealth;
    private float currentHealth;

    private bool isActive = false;
    private GameObject healthBarParent; // The parent object containing the border and bar


    // Start is called before the first frame update
    void Start()
    {
        bar = GetComponent<RectTransform>();
        barImage = GetComponent<Image>();

        healthBarParent = transform.parent.gameObject;

        // Deactivate the health bar parent by default
        healthBarParent.SetActive(false);
    }

    // Initialize the health bar with the maximum health
    public void Initialize(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
        UpdateBar();
    }

    // Apply damage and update the health bar
    public void Damage(float damageAmount)
    {
        if (!isActive) { 
            healthBarParent.SetActive(true);
            isActive = true;
        }
        currentHealth -= damageAmount;

        // Clamp health between 0 and maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Change bar color if health is low
        if (currentHealth / maxHealth < 0.3f)
        {
            barImage.color = Color.red;
        }

        UpdateBar();
    }

    // Update the size of the health bar
    private void UpdateBar()
    {
        if (bar != null)
        {
            // Calculate the new width as a percentage of the original width
            float sizePercentage = currentHealth / maxHealth;

            // Update the width while keeping the bar visually centered
            bar.sizeDelta = new Vector2(sizePercentage * bar.sizeDelta.x, bar.sizeDelta.y);
        }
    }
}

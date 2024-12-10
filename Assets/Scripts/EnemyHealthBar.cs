using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    private RectTransform bar; // The actual bar that shrinks
    private Image barImage;

    private float maxHealth;
    private float currentHealth;
    private float lowHealthThreshold; // Value representing the low health percentage (e.g., 30%)

    private bool isActive = false;
    private GameObject healthBarParent; // The parent object containing the border and bar
    [SerializeField] private TMP_Text nameSpace; // Text to display the enemy's name

    // Start is called before the first frame update
    void Start()
    {
        bar = GetComponent<RectTransform>();
        barImage = GetComponent<Image>();

        healthBarParent = transform.parent.gameObject;

        // Deactivate the health bar parent by default
        healthBarParent.SetActive(false);

    }

    // Initialize the health bar with the maximum health and optionally an enemy name
    public void Initialize(float maxHealth, string enemyName)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
        this.lowHealthThreshold = 0.3f; // Set to 30% of max health
        if (enemyName == null) 
            nameSpace.text = string.Empty;
        else
            nameSpace.text = enemyName;
        UpdateBar();
    }


    // Apply damage and update the health bar
    public void Damage(float damageAmount)
    {
        if (!isActive)
        {
            healthBarParent.SetActive(true); // Activate the health bar
            isActive = true;
        }

        currentHealth -= damageAmount;

        // Clamp health between 0 and maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Change bar color if health is below the low health threshold
        if (currentHealth / maxHealth <= lowHealthThreshold)
        {
            barImage.color = Color.red;
        }
        else
        {
            barImage.color = Color.green; // Reset color if above low health threshold
        }

        UpdateBar();
    }

    // Update the size of the health bar
    private void UpdateBar()
    {
        if (bar != null)
        {
            // Calculate the new width as a percentage of the parent width
            float sizePercentage = currentHealth / maxHealth;

            // Update the width while keeping the height constant
            bar.localScale = new Vector3(sizePercentage, 1, 1);
        }
    }
}

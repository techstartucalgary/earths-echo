using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private RectTransform bar;
    private Image barImage;

    void Start()
    {
        bar = GetComponent<RectTransform>();
        barImage = GetComponent<Image>();
        UpdateBarVisuals();
    }

    // Dedicate a method to updating the bar's visuals 
    // (scale + color), so we can call it whenever we want to refresh.
    private void UpdateBarVisuals()
    {
        // 1. Update bar scale
        SetSize(Health.totalHealth);

        // 2. Check for low health color
        if (Health.totalHealth < 0.3f)
        {
            barImage.color = Color.red;
        }
        else
        {
            // Default color: pick whatever color you consider "full" or "normal"
            barImage.color = Color.green;
        }
    }

    public void Damage(float damage)
    {
        Health.totalHealth -= damage;

        if (Health.totalHealth < 0f)
        {
            Health.totalHealth = 0f;
        }

        // Trigger GameOver if health is 0
        if (Health.totalHealth <= 0f)
        {
            GameManager.instance.GameOver();
        }

        UpdateBarVisuals();
    }

    public void SetSize(float size)
    {
        bar.localScale = new Vector3(size, 1f);
    }

    // Call this to fully reset the health bar visually
    public void ResetHealthBar()
    {
        // No need to subtract damage or anything, 
        // just refresh the UI based on the new Health.totalHealth
        UpdateBarVisuals();
    }
}

using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour
{
    // Common properties for all weapons
    public string weaponName;
    public float damage;
    public float range;
    [TextArea]
    public string description;

    // Abstract attack methods that subclasses must implement
    public abstract void PrimaryAttack();
    public abstract void SideAttack();
    public abstract void UpAttack();
    public abstract void DownAttack();

    // Method to equip the weapon (activate it on the player)
    public virtual void Equip(Transform playerTransform)
    {
        // Activate the weapon GameObject and parent it to the player
        gameObject.SetActive(true);
        transform.SetParent(playerTransform);

        // Determine direction based on player's local scale
        bool isFacingRight = playerTransform.localScale.x > 0;

        // Flip weapon's position and orientation based on the player's direction
        if (isFacingRight)
        {
            transform.localPosition = new Vector3(0.5f, -0.5f, 0); // Adjust as needed
        }
        else
        {
            transform.localPosition = new Vector3(-0.5f, -0.5f, 0); // Adjust as needed
        }

        transform.localRotation = Quaternion.identity;
        Debug.Log(weaponName + " equipped.");
    }

    // Method to unequip the weapon (deactivate it)
    public virtual void Unequip()
    {
        // Deactivate the weapon GameObject instead of destroying it
        gameObject.SetActive(false);
        Debug.Log(weaponName + " unequipped.");
    }

    // Helper method to update weapon's orientation dynamically if needed
    public void UpdateWeaponOrientation(Transform playerTransform)
    {
        bool isFacingRight = playerTransform.localScale.x > 0;

        if (isFacingRight)
        {
            transform.localPosition = new Vector3(0.5f, -0.5f, 0);
        }
        else
        {
            transform.localPosition = new Vector3(-0.5f, -0.5f, 0);
        }
    }

}

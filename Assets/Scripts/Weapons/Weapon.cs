using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // Common properties for all weapons
    public string weaponName;
    public float damage;
    public float range;
    public GameObject weaponPrefab; // Reference to the weapon prefab
    protected GameObject instantiatedWeapon; // Instance of the weapon on the player

    // Abstract attack methods that subclasses must implement
    public abstract void PrimaryAttack();
    public abstract void SideAttack();
    public abstract void UpAttack();
    public abstract void DownAttack();

    // Method to equip the weapon
    public virtual void Equip(Transform playerTransform)
    {
        if (weaponPrefab != null)
        {
            // Instantiate the weapon prefab at the player's position
            instantiatedWeapon = Instantiate(weaponPrefab, playerTransform.position, Quaternion.identity);
            instantiatedWeapon.transform.SetParent(playerTransform);
            instantiatedWeapon.transform.localPosition = new Vector3(0.5f, -0.5f, 0); // Adjust position as necessary
            instantiatedWeapon.transform.localRotation = Quaternion.identity; // Adjust rotation as necessary
            Debug.Log(weaponName + " equipped.");
        }
    }

    // Method to unequip the weapon
    public virtual void Unequip()
    {
        if (instantiatedWeapon != null)
        {
            Destroy(instantiatedWeapon); // Destroy the instantiated weapon
            instantiatedWeapon = null; // Clear the reference
        }
        Debug.Log(weaponName + " unequipped.");
    }
}

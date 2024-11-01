using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
<<<<<<< HEAD
=======
    // Common properties for all weapons
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
    public string weaponName;
    public float damage;
    public float range;
    public GameObject weaponPrefab; // Reference to the weapon prefab
    protected GameObject instantiatedWeapon; // Instance of the weapon on the player

<<<<<<< HEAD
=======
    // Abstract attack methods that subclasses must implement
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
    public abstract void PrimaryAttack();
    public abstract void SideAttack();
    public abstract void UpAttack();
    public abstract void DownAttack();

<<<<<<< HEAD
=======
    // Method to equip the weapon
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
    public virtual void Equip(Transform playerTransform)
    {
        if (weaponPrefab != null)
        {
<<<<<<< HEAD
            // Deactivate the existing instantiated weapon if it exists
            if (instantiatedWeapon != null)
            {
                instantiatedWeapon.SetActive(false); // Deactivate instead of destroying
            }

            // Instantiate the new weapon prefab
            instantiatedWeapon = Instantiate(weaponPrefab, playerTransform.position, Quaternion.identity);
            instantiatedWeapon.transform.SetParent(playerTransform);
            instantiatedWeapon.transform.localPosition = new Vector3(0.5f, -0.5f, 0);
            instantiatedWeapon.transform.localRotation = Quaternion.identity;
            instantiatedWeapon.SetActive(true); // Activate the weapon
=======
            // Instantiate the weapon prefab at the player's position
            instantiatedWeapon = (GameObject)Instantiate(weaponPrefab, playerTransform.position, Quaternion.identity);
            instantiatedWeapon.transform.SetParent(playerTransform);
            instantiatedWeapon.transform.localPosition = new Vector3(0.5f, -0.5f, 0); // Adjust position as necessary
            instantiatedWeapon.transform.localRotation = Quaternion.identity; // Adjust rotation as necessary
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
            Debug.Log(weaponName + " equipped.");
        }
    }

<<<<<<< HEAD
=======
    // Method to unequip the weapon
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
    public virtual void Unequip()
    {
        if (instantiatedWeapon != null)
        {
<<<<<<< HEAD
            instantiatedWeapon.SetActive(false); // Deactivate instead of destroying
            Debug.Log(weaponName + " unequipped.");
        }
=======
            Destroy(instantiatedWeapon); // Destroy the instantiated weapon
            instantiatedWeapon = null; // Clear the reference
        }
        Debug.Log(weaponName + " unequipped.");
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
    }
}

using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public float damage;
    public float range;
    public GameObject weaponPrefab; // Reference to the weapon prefab
    protected GameObject instantiatedWeapon; // Instance of the weapon on the player

    public abstract void PrimaryAttack();
    public abstract void SideAttack();
    public abstract void UpAttack();
    public abstract void DownAttack();

    public virtual void Equip(Transform playerTransform)
    {
        if (weaponPrefab != null)
        {
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
            Debug.Log(weaponName + " equipped.");
        }
    }

    public virtual void Unequip()
    {
        if (instantiatedWeapon != null)
        {
            instantiatedWeapon.SetActive(false); // Deactivate instead of destroying
            Debug.Log(weaponName + " unequipped.");
        }
    }
}

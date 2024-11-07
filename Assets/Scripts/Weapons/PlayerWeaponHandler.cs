using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    private GameObject equippedWeaponInstance; // Store instance of equipped weapon
    public Transform meleeSlot;
    public Transform projectileSlot;

    public void EquipWeapon(GameObject weaponPrefab)
    {
        // Destroy the previous equipped instance if it exists
        if (equippedWeaponInstance != null)
        {
            Destroy(equippedWeaponInstance);
        }

        if (weaponPrefab == null)
        {
            Debug.LogError("Weapon prefab is null.");
            return;
        }

        Weapon weapon = weaponPrefab.GetComponent<Weapon>();
        Transform slot = weapon is MeleeWeapon ? meleeSlot : projectileSlot;

        // Instantiate a new instance of the weapon prefab and store it
        equippedWeaponInstance = Instantiate(weaponPrefab, slot.position, Quaternion.identity, slot);

        // Call Equip method on the weapon instance
        Weapon equippedWeapon = equippedWeaponInstance.GetComponent<Weapon>();
        if (equippedWeapon != null)
        {
            equippedWeapon.Equip(transform);
            Debug.Log("Equipped weapon: " + equippedWeapon.weaponName);
        }
    }
}

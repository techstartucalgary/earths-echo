using UnityEngine;

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
        transform.localPosition = new Vector3(0.5f, -0.5f, 0); // Adjust as needed
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
}

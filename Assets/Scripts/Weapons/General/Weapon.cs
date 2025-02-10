using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    // Common properties for all weapons
    public string weaponName;

    [TextArea]
    public string description;

    FindGrandchildren finder;

    // Abstract attack methods that subclasses must implement
    public abstract void PrimaryAttack();
    public abstract void SideAttack();
    public abstract void UpAttack();
    public abstract void DownAttack();

    // Method to equip the weapon (activate it on the player)
    public virtual void Equip(Transform playerTransform)
    {
        // Find the appropriate attachment point
        Transform attachmentPoint = null;
        finder = new FindGrandchildren();

        if (this is MeleeWeapon)
        {
            // Debug.Log($"Finder result: " + finder.FindDeepChild(playerTransform, "MeleeWeaponAttachment").name);
            attachmentPoint = finder.FindDeepChild(playerTransform, "MeleeWeaponAttachment");
        }
        else if (this is ProjectileWeapon)
        {

            // Debug.Log($"Finder result: " + finder.FindDeepChild(playerTransform, "ProjectileWeaponAttachment").name);
            attachmentPoint = finder.FindDeepChild(playerTransform, "ProjectileWeaponAttachment");
        }

        if (attachmentPoint == null)
        {
            Debug.LogError("Attachment point not found for " + weaponName);
            return;
        }

        // Attach the weapon to the attachment point
        transform.SetParent(attachmentPoint);
        transform.localPosition = Vector3.zero; // Adjust position if needed
        transform.localRotation = Quaternion.identity;

        gameObject.SetActive(true);
        Debug.Log(weaponName + " equipped at " + attachmentPoint.name);
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
        Transform attachmentPoint = transform.parent;
        if (attachmentPoint == null)
        {
            Debug.LogWarning("Weapon is not attached to an attachment point.");
            return;
        }

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

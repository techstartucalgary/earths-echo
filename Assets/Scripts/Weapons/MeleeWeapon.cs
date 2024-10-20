using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float swingSpeed; // Speed of the melee attack

    private void Start()
    {
 
    }

    public override void PrimaryAttack()
    {
        Debug.Log("Melee primary attack with " + weaponName);
        // Implement melee primary attack logic here
    }

    public override void SideAttack()
    {
        Debug.Log("Melee side attack with " + weaponName);
        // Implement melee side attack logic here
    }

    public override void UpAttack()
    {
        Debug.Log("Melee up attack with " + weaponName);
        // Implement melee up attack logic here
    }

    public override void DownAttack()
    {
        Debug.Log("Melee down attack with " + weaponName);
        // Implement melee down attack logic here
    }
}

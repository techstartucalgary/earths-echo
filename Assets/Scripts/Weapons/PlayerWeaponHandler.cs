using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    private Weapon equippedWeapon;

    public void EquipWeapon(Weapon newWeapon)
    {
        if (equippedWeapon != null)
        {
            UnequipWeapon();
        }

        equippedWeapon = newWeapon;
        equippedWeapon.Equip();
        Debug.Log("Player equipped: " + equippedWeapon.weaponName);
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.Unequip();
            equippedWeapon = null;
        }
    }

    private void Update()
    {

        // Handle weapon attack inputs
        if (equippedWeapon != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                equippedWeapon.PrimaryAttack();
            }
            else if ((Input.GetKeyDown(KeyCode.LeftControl)))
            {
                if (Input.GetKeyDown(KeyCode.D)) 
                    equippedWeapon.SideAttack();
            }
            else if ((Input.GetKeyDown(KeyCode.LeftControl)))
            {
                if (Input.GetKeyDown(KeyCode.W))
                    equippedWeapon.UpAttack();
            }
            else if ((Input.GetKeyDown(KeyCode.LeftControl)))
            {
                if (Input.GetKeyDown(KeyCode.S))
                    equippedWeapon.DownAttack();
            }
        }
    }
}

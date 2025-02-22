using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon/Weapon")]
public class WeaponSO : ScriptableObject
{
    [Header("Weapon Visuals")]
    [Tooltip("The sprite representing the weapon.")]
    public Sprite weaponSprite;

    [Header("Weapon Description")]
    [TextArea]
    [Tooltip("A detailed description of the weapon.")]
    public string description;

    [Header("Weapon Stats")]
    [Tooltip("The cooldown time between attacks.")]
    public float cooldownTime = 0.5f;
    [Tooltip("The range of the weapon's attack.")]
    public float range = 0.5f;

    [Header("Weapon Audio")]
    [Tooltip("Audio clips associated with weapon actions (e.g., swing, hit).")]
    public AudioClip[] audioClips;
}

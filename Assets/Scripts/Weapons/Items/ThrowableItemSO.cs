using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewThrowableItem", menuName = "Inventory/ThrowableItem")]

public class ThrowableItemSO : ItemInstSO
{
    [Header("Projectile Specific Stats")]

    public GameObject[] projectilePrefab;

    
    [Tooltip("The speed at which the projectile moves.")]
    public float projectileSpeed = 10f;
}

using UnityEngine;

public interface IDamageable
{
    // Damages the object by a given amount.
    void Damage(float damageAmount, Vector2 impactPos);
    
    // Heals the object by a given amount.
    void Heal(float healAmount);
    
    // Sets whether the object is invincible (ignores damage when true).
    void SetInvincible(bool isInvincible);
    
    // Applies knockback using a given direction and force.
    void ApplyKnockback(Vector3 direction, float force);
}

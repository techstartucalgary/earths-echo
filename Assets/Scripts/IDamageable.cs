using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void Damage(float damageAmount);
    //apply this interface to the scripts of any objects that can be damaged, override the method to affect the healthbar
}

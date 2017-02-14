using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item {

    public int Damage;

    public WeaponItem()
    {
        Description += "\nDamage = " + Damage;
    }
}

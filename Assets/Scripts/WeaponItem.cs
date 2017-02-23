using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item {

    public int Damage;

    void Awake()
    {
        Description += "\nDamage = " + Damage;
    }
}

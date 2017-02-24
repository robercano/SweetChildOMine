using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class WeaponItem : Item {

    public int Damage;

    protected override void Awake()
    {
        base.Awake();
        Description += "\nDamage = " + Damage;
    }
}

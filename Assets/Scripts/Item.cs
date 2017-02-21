using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string Name;
    public Sprite Avatar;
    public string Description;
    public int Amount;
    public int WeightPerUnit;
    public int TotalWeight
    {
        get
        {
            return Amount * WeightPerUnit;
        }
    }

    public GameObject ObjectPrefab;
};

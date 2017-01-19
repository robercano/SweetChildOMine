using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerDigColllider : MonoBehaviour {

    public Miner m_minerScript;

    public void OnTriggerEnter2D(Collider2D coll)
    {
        m_minerScript.OnTriggerEnter2DChild(coll);
    }

    public void OnTriggerExit2D(Collider2D coll)
    {
        m_minerScript.OnTriggerExit2DChild(coll);
    }
}

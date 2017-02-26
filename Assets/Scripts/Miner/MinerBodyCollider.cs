using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerBodyCollider : MonoBehaviour {

	public Miner m_minerScript;

	public void OnCollisionEnter2D(Collision2D coll)
	{
		m_minerScript.OnCollisionEnter2DChild (coll, Miner.ColliderType.ColliderBody);
	}

    public void OnTriggerEnter2D(Collider2D collider)
    {
        m_minerScript.OnTriggerEnter2DChild(collider);
    }
}

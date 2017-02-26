using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerFeetCollider : MonoBehaviour {

	public Miner m_minerScript;

	public void OnCollisionEnter2D(Collision2D coll)
	{
		m_minerScript.OnCollisionEnter2DChild (coll, Miner.ColliderType.ColliderFeet);
	}
}

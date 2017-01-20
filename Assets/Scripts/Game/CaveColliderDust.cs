using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveColliderDust : MonoBehaviour {

	private ParticleSystem m_particleSystem;

	// Use this for initialization
	void Start () {
		m_particleSystem = GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!m_particleSystem.IsAlive ())
			Destroy (gameObject);
	}
}

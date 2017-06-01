using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingCreatureCollider : MonoBehaviour {

	private Action<Collision2D> m_callback;

	public void SetCallback(Action<Collision2D> callback) {
		m_callback = callback;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (m_callback != null) {
			m_callback (collision);
		}
	}
}

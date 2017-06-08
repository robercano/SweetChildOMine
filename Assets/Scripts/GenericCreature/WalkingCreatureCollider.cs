using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingCreatureCollider : MonoBehaviour {

	private Action<Collision2D> m_enterCallback;
	private Action<Collision2D> m_exitCallback;
	private Action<Collision2D> m_stayCallback;

	public void SetEnterCallback(Action<Collision2D> callback) {
		m_enterCallback = callback;
	}

	public void SetExitCallback(Action<Collision2D> callback) {
		m_exitCallback = callback;
	}

	public void SetStayCallback(Action<Collision2D> callback) {
		m_stayCallback = callback;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (m_enterCallback != null) {
			m_enterCallback (collision);
		}
	}

	void OnCollisionExit2D(Collision2D collision) {
		if (m_exitCallback != null) {
			m_exitCallback (collision);
		}
	}

	void OnCollisionStay2D(Collision2D collision) {
		if (m_stayCallback != null) {
			m_stayCallback (collision);
		}
	}
}

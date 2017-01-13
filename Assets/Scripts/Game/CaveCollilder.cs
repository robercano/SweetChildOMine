using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveCollilder : MonoBehaviour {

	private SpriteRenderer m_spriteRenderer;

	void Start()
	{
		m_spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.collider.name == "DigCollider") {
			Debug.Log("Contacts length: " + coll.contacts.Length);
			Debug.Log (gameObject.name + " against " + coll.collider.name);
			foreach (ContactPoint2D contact in coll.contacts)
				Debug.Log ("Contact point: " + contact.point);
			m_spriteRenderer.color = new Color (0.0f, 1.0f, 0.0f, 1.0f);
		}
	}
}

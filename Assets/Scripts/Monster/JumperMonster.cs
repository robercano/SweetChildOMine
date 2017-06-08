using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperMonster : MonoBehaviour {

	public float m_startJumpingAfter = 2.0f; // seconds
	public float m_jumpEvery = 5.0f; // seconds
	private float m_jumpForceY = 100f;
	private float m_jumpForceX = 20f;

	private WalkingCreature m_walkingCreature;
	private Rigidbody2D m_rigidBody;

	void Awake ()
	{
		m_walkingCreature = GetComponent<WalkingCreature> ();
		m_rigidBody = GetComponent<Rigidbody2D>();
		m_walkingCreature.SetFallingCallback (FallingChanged);
		m_walkingCreature.SetBodyCollisionCallback (BodyCollision);

		InvokeRepeating("Jump", m_startJumpingAfter, m_jumpEvery);
	}

	void FixedUpdate ()
	{
		
	}

	void FallingChanged(bool isNowFalling)
	{
//		if (isNowFalling)
//		{
//			Debug.Log ("Falling");
//		} else
//		{
//			Debug.Log ("NOT Falling");
//		}
	}

	void BodyCollision(Collision2D collision) {
		if (!m_walkingCreature.IsFalling ()) {
			m_walkingCreature.TurnBack ();
		}
	}

	void Jump() {
		m_walkingCreature.Jump (new Vector2 (m_jumpForceX, m_jumpForceY));
	}
}

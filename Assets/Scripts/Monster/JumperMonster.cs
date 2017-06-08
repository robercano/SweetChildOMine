using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperMonster : MonoBehaviour {

	public float m_startJumpingAfter = 2.0f; // seconds
	public float m_jumpEvery = 5.0f; // seconds
	private float m_jumpForceY = 100f;
	private float m_jumpForceX = 20f;
	private float m_ySpeedToFloat = 20f; // if vertical speed is less than this, the monster will show "floating"

	private WalkingCreature m_walkingCreature;
	private Rigidbody2D m_rigidBody;
	private Animator m_animator;

	void Awake ()
	{
		m_walkingCreature = GetComponent<WalkingCreature> ();
		m_rigidBody = GetComponent<Rigidbody2D>();
		m_animator = GetComponent<Animator> ();

		m_walkingCreature.SetFallingCallback (FallingChanged);
		m_walkingCreature.SetBodyCollisionCallback (HandleBodyCollision);
		m_walkingCreature.SetWalkCallback (HandleWalking);

		InvokeRepeating("PrepareJump", m_startJumpingAfter, m_jumpEvery);
	}

	void FixedUpdate ()
	{
		if (m_walkingCreature.IsFalling ())
		{
			SetInAirAnimationProperties ();
		}
		else
		{
			m_animator.SetBool ("inAir", false);
		}
	}

	private void SetInAirAnimationProperties()
	{
		m_animator.SetBool ("inAir", true);
		if (m_rigidBody.velocity.y < -m_ySpeedToFloat) {
			m_animator.SetBool ("goingDown", true);
			m_animator.SetBool ("goingUp", false);
		} else if (m_rigidBody.velocity.y > m_ySpeedToFloat) {
			m_animator.SetBool ("goingDown", false);
			m_animator.SetBool ("goingUp", true);
		} else {
			m_animator.SetBool ("goingDown", false);
			m_animator.SetBool ("goingUp", false);
		}
	}

	void FallingChanged(bool isNowFalling)
	{
		if (!isNowFalling)
		{
			m_walkingCreature.Stop ();
			m_animator.SetTrigger ("landing");
		}
	}

	void HandleBodyCollision(Collision2D collision)
	{
		if (!m_walkingCreature.IsFalling ())
		{
			m_walkingCreature.TurnBack ();
		}
	}

	void HandleWalking()
	{
		m_animator.SetTrigger ("walking");
	}

	void PrepareJump()
	{
		m_walkingCreature.Stop ();
		m_animator.SetTrigger ("preparingJump");
	}

	void JumpTriggeredFromAnimation()
	{
		m_walkingCreature.Jump (new Vector2 (m_jumpForceX, m_jumpForceY));
	}

	void WalkTriggeredFromAnimation() {
		m_walkingCreature.Walk ();
	}
}

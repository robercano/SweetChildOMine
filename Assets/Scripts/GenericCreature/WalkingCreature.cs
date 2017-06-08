using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingCreature : MonoBehaviour {

	public float m_walkSpeed = 32.0f;

	private Rigidbody2D m_rigidBody;
	private WalkingCreatureCollider m_feetColliderScript;
	private WalkingCreatureCollider m_bodyColliderScript;
	private BoxCollider2D m_feetCollider;
	private BoxCollider2D m_bodyCollider;

	private float m_faceDirection = 1.0f;
	public float m_initialDirection = -1.0f;
	private int m_layerMask;
	private float DISTANCE_TO_START_FALLING = 2.0f;
	private bool m_isTouchingGround = false;
	private bool m_isFalling = false;

	void Start ()
	{
		InitColliderObjects ();
		m_layerMask = (1 << (LayerMask.NameToLayer("Cave Colliders")));
		UpdateLocalTransformDirection ();
		Walk ();
	}

	void FixedUpdate ()
	{
		if (!m_isFalling && !m_isTouchingGround)
		{
			if (IsGrounded ())
			{
				SettleDown();
			}
			else
			{
				StartFalling ();
			}
		}
	}

	private void StartFalling()
	{
		m_isFalling = true;
		EndMovement ();
		m_rigidBody.isKinematic = false;
		// TODO notify falling
	}

	private void StopFalling()
	{
		if (m_isFalling)
		{
			m_isFalling = false;
			m_rigidBody.isKinematic = true;
			m_rigidBody.velocity = new Vector2 (m_rigidBody.velocity.x, 0f);
			Walk ();
			// TODO notify stop falling
		}
	}
		
	private bool IsGrounded()
	{
		Vector2 leftFeetPosition = new Vector2(m_feetCollider.bounds.min.x, m_feetCollider.bounds.min.y);
		Vector2 rightFeetPosition = new Vector2(m_feetCollider.bounds.max.x, m_feetCollider.bounds.min.y);

		return (
			Physics2D.Raycast(leftFeetPosition, Vector2.down, DISTANCE_TO_START_FALLING, m_layerMask)
			|| Physics2D.Raycast(rightFeetPosition, Vector2.down, DISTANCE_TO_START_FALLING, m_layerMask)
		);
	}

	public void Walk()
	{
		m_rigidBody.velocity = new Vector2(m_faceDirection * m_walkSpeed, m_rigidBody.velocity.y);
	}

	public void SettleDown()
	{
		/* Check if we need to fall down */
		Vector2 leftSource = new Vector2(m_feetCollider.bounds.min.x + 0.5f, m_feetCollider.bounds.min.y);
		Vector2 rightSource = new Vector2(m_feetCollider.bounds.max.x - 0.5f, m_feetCollider.bounds.min.y);

		m_feetCollider.enabled = false;
		m_bodyCollider.enabled = false;
		RaycastHit2D hitLeft = Physics2D.Raycast(leftSource, Vector2.down, Mathf.Infinity, m_layerMask);
		RaycastHit2D hitRight = Physics2D.Raycast(rightSource, Vector2.down, Mathf.Infinity, m_layerMask);
		m_feetCollider.enabled = true;
		m_bodyCollider.enabled = true;

		if ((hitLeft.collider != null) && (hitRight.collider != null))
		{
			m_rigidBody.position = new Vector2(m_rigidBody.position.x, m_rigidBody.position.y - Mathf.Min(hitLeft.distance, hitRight.distance));
		}
	}

	public void Stop()
	{
		EndMovement();
		SettleDown();
	}

	public void MoveStepUp()
	{
		m_rigidBody.position = new Vector2(m_rigidBody.position.x, m_rigidBody.position.y + 1.0f);
	}

	void EndMovement()
	{
		/* Round the coordinates so they are perfect pixel aligned */
		m_rigidBody.position = new Vector2(Mathf.Round(m_rigidBody.position.x), Mathf.Round(m_rigidBody.position.y));
		m_rigidBody.velocity = Vector2.zero;
	}

	private void HandleBodyCollision(Collision2D collision)
	{
		// Check if any of the contact points is in the direction
		// of the current movement. If so we have collided against a wall,
		// readjust position and end movement
		for (int i = 0; i < collision.contacts.Length; ++i)
		{
			if (m_faceDirection * (m_rigidBody.position.x - collision.contacts [i].point.x) < 0.0f)
			{
				m_rigidBody.position = new Vector2 (collision.collider.bounds.center.x -
					m_faceDirection * (collision.collider.bounds.extents.x + m_bodyCollider.bounds.extents.x),
					m_rigidBody.position.y);
				Stop ();
				ChangeDirection ();
				SettleDown ();
				Walk ();
				// TODO: notify bodyCollision in Miner
//
//				if (FSM.CurrentState == MinerStateDigWalk.Instance && collision.gameObject.tag == "CaveCollider") {
//					FSM.ChangeState (MinerStateDig.Instance);
//				} else {
//					ResetMovementTarget (); 
//					FSM.ChangeState (MinerStateIdle.Instance);
//				}
				break;
			}
		}
	}

	private void ChangeDirection()
	{
		m_faceDirection = -1 * m_faceDirection;
		UpdateLocalTransformDirection ();
	}

	private void UpdateLocalTransformDirection()
	{
		transform.localScale = new Vector2(m_initialDirection * m_faceDirection * Mathf.Abs(transform.localScale.x), transform.localScale.y);		
	}

	private void HandleFeetCollision(Collision2D collision)
	{
		m_isTouchingGround = true;
		if (m_isFalling)
		{
			StopFalling ();
		}
		if (CollisionIsHorizontal (collision))
		{
			MoveStepUp ();
		}
	}

	private void HandleFeetCollisionStay(Collision2D collision)
	{
		if (CollisionIsHorizontal (collision))
		{
			MoveStepUp ();
		}
	}

	private bool CollisionIsHorizontal(Collision2D collision)
	{
		// TODO this code could be more elegant
		foreach (ContactPoint2D contact in collision.contacts)
		{
			if (contact.normal == Vector2.left)
			{
				if(contact.point.x < (m_feetCollider.bounds.max.x - 0.5f))
				{
					return true;
				}
			}
			if (contact.normal == Vector2.right)
			{
				if(contact.point.x > (m_feetCollider.bounds.min.x + 0.5f))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void HandleFeetCollisionExit(Collision2D collision)
	{
		m_isTouchingGround = false;
	}

	private void InitColliderObjects()
	{
		m_rigidBody = GetComponent<Rigidbody2D>();
		GameObject feetCollider = transform.FindChild ("FeetCollider").gameObject;
		m_feetCollider = feetCollider.GetComponent<BoxCollider2D>();
		m_feetColliderScript = feetCollider.GetComponent<WalkingCreatureCollider> ();
		m_feetColliderScript.SetEnterCallback (HandleFeetCollision);
		m_feetColliderScript.SetExitCallback (HandleFeetCollisionExit);
		m_feetColliderScript.SetStayCallback (HandleFeetCollisionStay);


		GameObject bodyCollider = transform.FindChild ("BodyCollider").gameObject;
		m_bodyCollider = bodyCollider.GetComponent<BoxCollider2D>();
		m_bodyColliderScript = bodyCollider.GetComponent<WalkingCreatureCollider> ();
		m_bodyColliderScript.SetEnterCallback (HandleBodyCollision);

		CheckAllComponentsArePresent ();
	}

	private void CheckAllComponentsArePresent()
	{
		if (m_rigidBody == null)
		{
			Debug.LogError("Walking creature needs a rigid body");
		}
		if (m_feetCollider == null)
		{
			Debug.LogError("Walking creature needs a feet collider");
		}
		if (m_bodyCollider == null)
		{
			Debug.LogError("Walking creature needs a body collider");
		}
	}

}

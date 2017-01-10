using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour
{
	/* Public section */
	public bool animated = true;

	[HideInInspector]
	public enum ColliderType {
		ColliderFeet,
		ColliderBody
	};
	[HideInInspector]
	public enum MoveDirection {
		None = 0, Left = -1, Right = 1
	};

	/* Private section */
	private Animator m_animator;
	private SpriteRenderer m_spriteRenderer;
	private Rigidbody2D m_rigidBody;

	private float m_walkSpeed = 32.0f; /**< This value is the width of a single sprite multiplied by 2 */
	private float m_movementXTarget;   /**< Last location the user clock on for movement */
	private bool m_jumpUp;

	private MoveDirection m_moveDirection;
	private BoxCollider2D m_feetCollider;
	private BoxCollider2D m_bodyCollider;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        
        m_jumpUp = false;
		m_moveDirection = MoveDirection.None;

        m_movementXTarget = Mathf.Round(m_rigidBody.position.x);

		/* Get children components */
		m_feetCollider = transform.FindChild ("FeetCollider").GetComponent<BoxCollider2D> ();
		m_bodyCollider = transform.FindChild ("BodyCollider").GetComponent<BoxCollider2D> ();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        Move();
    }

    void Move()
    {
        if (Input.GetMouseButton(0))
        {
            /* Get the user click position in world coordinates for the horizontal component */
            m_movementXTarget = Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
        }

		float deltaX = m_movementXTarget - Mathf.Round(m_rigidBody.position.x);
		if (deltaX > float.Epsilon) {
			m_moveDirection = MoveDirection.Right;
		} else if (deltaX < -float.Epsilon) {
			m_moveDirection = MoveDirection.Left;
		} else {
			m_moveDirection = MoveDirection.None;
		}

        /* Check if the sprite really needs to move */
		switch (m_moveDirection)
		{
		case MoveDirection.Left:
		case MoveDirection.Right:
			if (animated)
				m_animator.SetBool ("minerWalk", true);

			transform.localScale = new Vector2 ((float)m_moveDirection * Mathf.Abs (transform.localScale.x), transform.localScale.y);

			if (m_jumpUp) {
				/* TODO: Check manually if we need to jump of fall! */
				MoveUp ();
				m_jumpUp = false;
			} else {
				MoveDown ();
			}

			m_rigidBody.velocity = new Vector2 ((float)m_moveDirection * m_walkSpeed, m_rigidBody.velocity.y);
			break;

		case MoveDirection.None:
			EndMovement ();
			//MoveDown ();
			break;
    	}
	}

	void EndMovement()
	{
		/* We are done moving */
		if (animated)
			m_animator.SetBool ("minerWalk", false);

		/* Round the coordinates so they are perfect pixel aligned */
		m_rigidBody.position = new Vector2 (Mathf.Round (m_rigidBody.position.x), Mathf.Round (m_rigidBody.position.y));
		m_rigidBody.velocity = Vector2.zero;

		m_movementXTarget = m_rigidBody.position.x;
		m_moveDirection = MoveDirection.None;
	}

	void MoveUp()
	{
		m_rigidBody.position = new Vector2 (m_rigidBody.position.x, m_rigidBody.position.y + 1.0f);
	}

    void MoveDown()
    {
        /* Check if we need to fall down */
        float boxExtent = m_feetCollider.bounds.extents.x;
        float absPosX = Mathf.Round(transform.position.x);
        float absPosY = Mathf.Round(transform.position.y);
        Vector2 leftSource = new Vector2(absPosX - boxExtent + 1.0f, absPosY);
        Vector2 rightSource = new Vector2(absPosX + boxExtent - 1.0f, absPosY);

        m_feetCollider.enabled = false;
        RaycastHit2D hitLeft = Physics2D.Raycast(leftSource, Vector2.down);
        RaycastHit2D hitRight = Physics2D.Raycast(rightSource, Vector2.down);
        m_feetCollider.enabled = true;

        if (hitLeft != null && hitRight != null) {
            m_rigidBody.position = new Vector2(m_rigidBody.position.x, m_rigidBody.position.y - Mathf.Min(hitLeft.distance, hitRight.distance));
        }
    }

	public void OnCollisionEnter2DChild(Collision2D coll, ColliderType childCollider)
    {
		/* First check if movement cannot continue further */
		if (childCollider == ColliderType.ColliderBody) {
			bool collision = false;
			if (m_moveDirection == MoveDirection.Left) {
				for (int i = 0; i < coll.contacts.Length; ++i) {
					if (coll.contacts [i].point.x < m_rigidBody.position.x)
						collision = true;
				}
				if (collision)
					m_rigidBody.position = new Vector2 (coll.collider.bounds.center.x +
														coll.collider.bounds.extents.x +
														m_bodyCollider.bounds.extents.x,
														m_rigidBody.position.y);
			} else {
				for (int i = 0; i < coll.contacts.Length; ++i) {
					if (coll.contacts [i].point.x > m_rigidBody.position.x)
						collision = true;
				}
				if (collision)
					m_rigidBody.position = new Vector2 (coll.collider.bounds.center.x -
														coll.collider.bounds.extents.x -
														m_bodyCollider.bounds.extents.x,
														m_rigidBody.position.y);
			}
			m_moveDirection = MoveDirection.None;
			return;
		}

		/* Check the feet for frontal collision */
		if (childCollider == ColliderType.ColliderFeet) {
			/*if (coll.collider.bounds.extents.y > 1.0f) {
				if (m_moveDirection == MoveDirection.Left) {
					m_rigidBody.position = new Vector2 (coll.collider.bounds.center.x +
														coll.collider.bounds.extents.x +
														m_feetCollider.bounds.extents.x,
														m_rigidBody.position.y);
				} else {
					m_rigidBody.position = new Vector2 (coll.collider.bounds.center.x -
														coll.collider.bounds.extents.x -
														m_feetCollider.bounds.extents.x,
														m_rigidBody.position.y);
				}
				m_moveDirection = MoveDirection.None;
			} else {*/
				float distance = (coll.collider.bounds.center.y - 0.5f) - m_rigidBody.position.y;
				if (distance > -0.5f && distance < 0.5f) {
					if (coll.collider.bounds.extents.y <= (0.5f + float.Epsilon)) {
						m_jumpUp = true;
					}
				}
			//}
		}
    }
};
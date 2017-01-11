using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour
{
	[HideInInspector]
	public enum ColliderType {
		ColliderFeet,
		ColliderBody
	};
	[HideInInspector]
	public enum CharacterAction {
		None = 0, MoveLeft = -1, MoveRight = 1, Dig = 2
	};

	/* Private section */
	private Animator m_animator;
	private SpriteRenderer m_spriteRenderer;
	private Rigidbody2D m_rigidBody;

	private float m_walkSpeed = 32.0f; /**< This value is the width of a single sprite multiplied by 2 */
	private float m_movementXTarget;   /**< Last location the user clock on for movement */

	private CharacterAction m_characterAction;
	private BoxCollider2D m_feetCollider;
    private BoxCollider2D m_bodyCollider;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        //m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        
		m_characterAction = CharacterAction.None;

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
        if (Input.GetMouseButton(1) && m_characterAction != CharacterAction.Dig)
        {
            m_characterAction = CharacterAction.None;
            EndMovement();
            MoveDown();
            m_animator.SetTrigger("minerDig");
            m_characterAction = CharacterAction.Dig;
        }

        /* Check if we have reached our destination */
        float deltaX = m_movementXTarget - Mathf.Round(m_rigidBody.position.x);
		if (deltaX > float.Epsilon) {
            StartMovement(CharacterAction.MoveRight);
		} else if (deltaX < -float.Epsilon) {
            StartMovement(CharacterAction.MoveLeft);
		} else {
            /* Destination reached, end movement and return */
            if (m_characterAction != CharacterAction.Dig)
            {
                EndMovement();
                MoveDown();
            }
            return;
		}
        
        /* Check if the sprite really needs to move */
        switch (m_characterAction)
        {
            case CharacterAction.MoveLeft:
            case CharacterAction.MoveRight:
                /* Move it towards the target direction at the configured speed */
                m_rigidBody.velocity = new Vector2((float)m_characterAction * m_walkSpeed, m_rigidBody.velocity.y);

                /* Check if  we need to move down after moving */
                MoveDown();
                break;
        }
    }

    void StartMovement(CharacterAction direction)
    {
        /* Setup the animation only if this is a new direction */
        if (direction != m_characterAction)
        {
            m_animator.SetTrigger("minerWalk");

            /* Flip the object so it faces on the right direction */
            transform.localScale = new Vector2((float)direction * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        m_characterAction = direction;
    }

	void EndMovement()
	{
        /* We are done moving */
        if (m_characterAction != CharacterAction.None)
            m_animator.SetTrigger("minerIdle");

		/* Round the coordinates so they are perfect pixel aligned */
		m_rigidBody.position = new Vector2 (Mathf.Round (m_rigidBody.position.x), Mathf.Round (m_rigidBody.position.y));
		m_rigidBody.velocity = Vector2.zero;

		m_movementXTarget = m_rigidBody.position.x;
		m_characterAction = CharacterAction.None;

    }

	void MoveUp()
	{
		m_rigidBody.position = new Vector2 (m_rigidBody.position.x, m_rigidBody.position.y + 1.0f);
	}

    void MoveDown()
    {
        /* Check if we need to fall down */
        Vector2 leftSource = new Vector2(m_feetCollider.bounds.min.x + 0.5f, m_feetCollider.bounds.min.y);
        Vector2 rightSource = new Vector2(m_feetCollider.bounds.max.x - 0.5f, m_feetCollider.bounds.min.y);

        m_feetCollider.enabled = false;
        m_bodyCollider.enabled = false;
        RaycastHit2D hitLeft = Physics2D.Raycast(leftSource, Vector2.down);
        RaycastHit2D hitRight = Physics2D.Raycast(rightSource, Vector2.down);
        m_feetCollider.enabled = true;
        m_bodyCollider.enabled = true;

        if (hitLeft != null && hitRight != null) {
            m_rigidBody.position = new Vector2(m_rigidBody.position.x, m_rigidBody.position.y - Mathf.Min(hitLeft.distance, hitRight.distance));
        }
    }

    public void OnCollisionEnter2DChild(Collision2D coll, ColliderType childCollider)
    {
        // Avoid moving up when movement has already finished
        if (m_characterAction == CharacterAction.None ||
            m_characterAction == CharacterAction.Dig)
            return;

        // Going upstairs movement: only check feet collision when obstacle is
        // 1 unit high or less. If higher we cannot climb the slope
        if (childCollider == ColliderType.ColliderFeet) {
            // If obstacle higher than 1 unit, we cannot climb it
            if (coll.collider.bounds.extents.y > 0.5f)
                return;
            // Sometimes colliders that are behind the character trigger this callback,
            // if that is the case ignore them
            if (m_characterAction == CharacterAction.MoveRight &&
                coll.collider.bounds.center.x < m_rigidBody.position.x)
                return;
            if (m_characterAction == CharacterAction.MoveLeft &&
                coll.collider.bounds.center.x > m_rigidBody.position.x)
                return;

            // Obstacle is in from of us and has 1 unit high, climb it
            MoveUp();
            return;
        }

        // Sometimes the body collider collides with the floor. If height
        // is 1, assume it is the floor and skip the test
        if (coll.collider.bounds.extents.y <= 0.5f)
            return;

        // Check if any of the contact points is in the direction
        // of the current movent. If so we have collided against a wall,
        // readjust position and end movement
        if (m_characterAction == CharacterAction.MoveLeft)
        {
            for (int i = 0; i < coll.contacts.Length; ++i)
            {
                if (coll.contacts[i].point.x < m_rigidBody.position.x)
                {
                    m_rigidBody.position = new Vector2(coll.collider.bounds.center.x +
                                                        coll.collider.bounds.extents.x +
                                                        m_bodyCollider.bounds.extents.x,
                                                        m_rigidBody.position.y);
                    EndMovement();
                    MoveDown();
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < coll.contacts.Length; ++i)
            {
                if (coll.contacts[i].point.x > m_rigidBody.position.x)
                {
                    m_rigidBody.position = new Vector2(coll.collider.bounds.center.x -
                                                        coll.collider.bounds.extents.x -
                                                        m_bodyCollider.bounds.extents.x,
                                                        m_rigidBody.position.y);
                    EndMovement();
                    MoveDown();
                    break;
                }
            }

        }

    }
};
  
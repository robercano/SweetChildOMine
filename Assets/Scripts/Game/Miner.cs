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
	public enum CharacterState {
		None, WalkLeft, WalkRight, RunLeft, RunRight, Dig
	};
	public enum InputEvent
	{
		None, ShiftLeftClick, LeftClick, DoubleLeftClick, RightClick
	}

	/* Private section */
	private Animator m_animator;
	private Rigidbody2D m_rigidBody;

	private float m_walkSpeed = 32.0f;
	private float m_runSpeed = 64.0f;
	private float m_movementXTarget;

	private CharacterState m_currentState;
	private InputEvent m_inputEvent;
	private BoxCollider2D m_feetCollider;
    private BoxCollider2D m_bodyCollider;

    private HashSet<GameObject> m_nearCaveColliders;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        
		m_currentState = CharacterState.None;
		m_inputEvent = InputEvent.None;

        m_movementXTarget = Mathf.Round(m_rigidBody.position.x);

        /* Get children components */
        m_feetCollider = transform.FindChild ("FeetCollider").GetComponent<BoxCollider2D> ();
		m_bodyCollider = transform.FindChild ("BodyCollider").GetComponent<BoxCollider2D> ();

        m_nearCaveColliders = new HashSet<GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        ProcessState();
    }

    void ProcessState()
    {
		#region State execution
		switch (m_currentState)
		{
		case CharacterState.None:
			// Empty on purpose
			break;
		case CharacterState.WalkLeft:
		case CharacterState.RunLeft:
		case CharacterState.WalkRight:
		case CharacterState.RunRight:
		case CharacterState.Dig:
			{
				// Check end of movement
				float deltaX = m_movementXTarget - Mathf.Round(m_rigidBody.position.x);
				if (deltaX >= -float.Epsilon && deltaX <= float.Epsilon) {
					EndMovement();
					FallDown();
					TransitionState(CharacterState.None);
					break;
				}

				// Still moving, apply speed
				switch (m_currentState)
				{
				case CharacterState.WalkLeft:
					m_rigidBody.velocity = new Vector2(-m_walkSpeed, m_rigidBody.velocity.y);
					break;
				case CharacterState.WalkRight:
					m_rigidBody.velocity = new Vector2(m_walkSpeed, m_rigidBody.velocity.y);
					break;
				case CharacterState.RunLeft:
					m_rigidBody.velocity = new Vector2(-m_runSpeed, m_rigidBody.velocity.y);
					break;
				case CharacterState.RunRight:
					m_rigidBody.velocity = new Vector2(m_runSpeed, m_rigidBody.velocity.y);
					break;
				}

				FallDown();
			}
			break;
		}
		#endregion

		#region Input processing
		if (Input.GetMouseButton(0))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				m_inputEvent = InputEvent.ShiftLeftClick;
			else
				m_inputEvent = InputEvent.LeftClick;
		}
		if (Input.GetMouseButton(1))
		{
			m_inputEvent = InputEvent.RightClick;
		}

		switch (m_inputEvent)
		{
		case InputEvent.LeftClick:
			{
				m_movementXTarget = Mathf.Round (Camera.main.ScreenToWorldPoint (Input.mousePosition).x);

				float deltaX = m_movementXTarget - Mathf.Round(m_rigidBody.position.x);
				if (deltaX < -float.Epsilon) {
					switch (m_currentState)
					{
					case CharacterState.RunRight:
						TransitionState(CharacterState.RunLeft);
						break;
					case CharacterState.WalkRight:
					case CharacterState.None:
					case CharacterState.Dig:
						TransitionState(CharacterState.WalkLeft);
						break;
					}
				} else if (deltaX > float.Epsilon) {
					switch (m_currentState)
					{
					case CharacterState.RunLeft:
						TransitionState(CharacterState.RunRight);
						break;
					case CharacterState.WalkLeft:
					case CharacterState.None:
					case CharacterState.Dig:
						TransitionState(CharacterState.WalkRight);
						break;
					}
				}
			}
			break;
		case InputEvent.ShiftLeftClick:
			{
				m_movementXTarget = Mathf.Round (Camera.main.ScreenToWorldPoint (Input.mousePosition).x);

				float deltaX = m_movementXTarget - Mathf.Round(m_rigidBody.position.x);
				if (deltaX < -float.Epsilon) {
					switch (m_currentState)
					{
					case CharacterState.WalkLeft:
					case CharacterState.WalkRight:
					case CharacterState.RunRight:
					case CharacterState.None:
					case CharacterState.Dig:
						TransitionState(CharacterState.RunLeft);
						break;
					}
				} else if (deltaX > float.Epsilon) {
					switch (m_currentState)
					{
					case CharacterState.WalkLeft:
					case CharacterState.WalkRight:
					case CharacterState.RunLeft:
					case CharacterState.None:
					case CharacterState.Dig:
						TransitionState(CharacterState.RunRight);
						break;
					}
				}
			}
			break;
		case InputEvent.DoubleLeftClick:
			{
				m_movementXTarget = Mathf.Round (Camera.main.ScreenToWorldPoint (Input.mousePosition).x);

				float deltaX = m_movementXTarget - Mathf.Round(m_rigidBody.position.x);
				if (deltaX < -float.Epsilon) {
					if (m_currentState != CharacterState.RunLeft) {
						TransitionState(CharacterState.RunLeft);
					}
				} else if (deltaX > float.Epsilon) {
					if (m_currentState != CharacterState.RunRight) {
						TransitionState(CharacterState.RunRight);
					}
				}
			}
			break;

		case InputEvent.RightClick:
			{
				if (m_currentState != CharacterState.Dig) {
					EndMovement();
					TransitionState(CharacterState.Dig);

					m_movementXTarget = Mathf.Round (Camera.main.ScreenToWorldPoint (Input.mousePosition).x);
				} else {
					//EndMovement();
					//TransitionState(CharacterState.None);
				}
			}
			break;
		case InputEvent.None:
			{
			}
			break;
		}
		m_inputEvent = InputEvent.None;

		#endregion
	}

	void TransitionState(CharacterState state)
	{
		m_currentState = state;
		PlayStateAnimation(state);
	}

    void PlayStateAnimation(CharacterState state)
    {
        /* Setup the animation only if this is a new direction */
		switch (state) {
		case CharacterState.WalkLeft:
			m_animator.SetTrigger ("minerWalk");
			transform.localScale = new Vector2 (-Mathf.Abs (transform.localScale.x), transform.localScale.y);
			break;
		case CharacterState.WalkRight:
			m_animator.SetTrigger ("minerWalk");
			transform.localScale = new Vector2 (Mathf.Abs (transform.localScale.x), transform.localScale.y);
			break;
		case CharacterState.RunLeft:
			m_animator.SetTrigger ("minerRun");
			transform.localScale = new Vector2 (-Mathf.Abs (transform.localScale.x), transform.localScale.y);
			break;
		case CharacterState.RunRight:
			m_animator.SetTrigger ("minerRun");
			transform.localScale = new Vector2 (Mathf.Abs (transform.localScale.x), transform.localScale.y);
			break;
		case CharacterState.Dig:
			m_animator.SetTrigger ("minerDig");
			break;
		case CharacterState.None:
			m_animator.SetTrigger ("minerIdle");
			break;
		}
    }

	void EndMovement()
	{
		/* Round the coordinates so they are perfect pixel aligned */
		m_rigidBody.position = new Vector2 (Mathf.Round (m_rigidBody.position.x), Mathf.Round (m_rigidBody.position.y));
		m_rigidBody.velocity = Vector2.zero;

		m_movementXTarget = m_rigidBody.position.x;
    }

	void MoveUp()
	{
		m_rigidBody.position = new Vector2 (m_rigidBody.position.x, m_rigidBody.position.y + 1.0f);
	}

    void FallDown()
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

        if ((hitLeft.collider != null) && (hitRight.collider != null)) {
            m_rigidBody.position = new Vector2(m_rigidBody.position.x, m_rigidBody.position.y - Mathf.Min(hitLeft.distance, hitRight.distance));
        }
    }

    public void OnCollisionEnter2DChild(Collision2D coll, ColliderType childCollider)
    {
        switch (m_currentState)
        {
            case CharacterState.None:
            case CharacterState.Dig:
                return;

            case CharacterState.WalkLeft:
            case CharacterState.RunLeft:
                switch (childCollider)
                {
                    case ColliderType.ColliderBody:
                        // Check if any of the contact points is in the direction
                        // of the current movent. If so we have collided against a wall,
                        // readjust position and end movement
                        for (int i = 0; i < coll.contacts.Length; ++i)
                        {
                            if (coll.contacts[i].point.x < m_rigidBody.position.x)
                            {
                                m_rigidBody.position = new Vector2(coll.collider.bounds.center.x +
                                                                    coll.collider.bounds.extents.x +
                                                                    m_bodyCollider.bounds.extents.x,
                                                                    m_rigidBody.position.y);
                                EndMovement();
                                FallDown();
                                TransitionState(CharacterState.None);
                                break;
                            }
                        }
                        break;
                    case ColliderType.ColliderFeet:
                        // Only move up if collider is in front of us
                        if (coll.collider.bounds.center.x <= m_rigidBody.position.x)
                            MoveUp();
                        break;
                }
                break;

            case CharacterState.WalkRight:
            case CharacterState.RunRight:
                switch (childCollider)
                {
                    case ColliderType.ColliderBody:
                        // Check if any of the contact points is in the direction
                        // of the current movent. If so we have collided against a wall,
                        // readjust position and end movement
                        for (int i = 0; i < coll.contacts.Length; ++i)
                        {
                            if (coll.contacts[i].point.x > m_rigidBody.position.x)
                            {
                                m_rigidBody.position = new Vector2(coll.collider.bounds.center.x -
                                                                    coll.collider.bounds.extents.x -
                                                                    m_bodyCollider.bounds.extents.x,
                                                                    m_rigidBody.position.y);
                                EndMovement();
                                FallDown();
                                TransitionState(CharacterState.None);
                                break;
                            }
                        }
                        break;
                    case ColliderType.ColliderFeet:
                        // Only move up if collider is in front of us
                        if (coll.collider.bounds.center.x >= m_rigidBody.position.x)
                            MoveUp();
                        break;
                }
                break;
        }
    }

    public void OnTriggerEnter2DChild(Collider2D coll)
    {
        if (coll.tag == "CaveCollider")
        {
            m_nearCaveColliders.Add(coll.gameObject);
        }
    }

    public void OnTriggerExit2DChild(Collider2D coll)
    {
        if (coll.tag == "CaveCollider")
        {
            m_nearCaveColliders.Remove(coll.gameObject);
        }
    }

    public void OnDigging()
	{
        foreach (GameObject go in m_nearCaveColliders)
        {
            go.SendMessage("PlayerHit", 2, SendMessageOptions.DontRequireReceiver);
        }
        m_nearCaveColliders.Clear();
	}

	public void OnStepForward()
	{
		if (transform.localScale.x <= 0.0f) {
			m_rigidBody.position = new Vector2 (m_rigidBody.position.x - 1.0f, m_rigidBody.position.y);
		} else {
			m_rigidBody.position = new Vector2 (m_rigidBody.position.x + 1.0f, m_rigidBody.position.y);
		}
		FallDown ();
	}
 };
  
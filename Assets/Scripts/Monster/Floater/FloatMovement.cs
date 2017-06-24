using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using SCOM.Utils;

using FloatMovementState = SCOM.Utils.FSMState<FloatMovement>;

#pragma warning disable 0252

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class FloatMovement : MonoBehaviour, IStateStorageHandler {

    #region - External state variables 
    public enum MovementType
    {
        StandStill,
        WanderFreely,
        SeekAndDestroy
    }

    public string Key_MovementType;
    private MovementType Value_MovementType;

    public string Key_TargetPosition;
    private Vector2 Value_TargetPosition;
    #endregion // External state variables 

    #region - Module configuration variables
    public float WanderMaxAngleDegrees = 45.0f;
    public float WanderSpeed = 20;
    public float WanderMinTime = 2.0f;
    public float WanderMaxTime = 5.0f;
    #endregion // Module configuration variables

    #region - Internal state variables

    private FiniteStateMachine<FloatMovement> FSM;

    private Rigidbody2D m_rigidBody;
    private Animator m_animator;

    private float m_endWanderingTime = 0.0f;

    #endregion // Internal state variables

    void Start () {
        Value_MovementType = MovementType.StandStill;
        Value_TargetPosition = new Vector2(0.0f, 0.0f);

       FSM = new FiniteStateMachine<FloatMovement>(this, FloatMovementStateIdle.Instance);

        m_rigidBody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }
	
    void FixedUpdate()
    {
        ProcessInternalState();
    }

    #region - External state management
    public void ProcessState(StateStorage state)
    {
        MovementType? movementType = state[Key_MovementType] as MovementType?;
        Vector2? targetPosition = state[Key_TargetPosition] as Vector2?;

        Assert.IsTrue(movementType != null);
        Assert.IsTrue(targetPosition != null);

        if (movementType == Value_MovementType &&
            targetPosition == Value_TargetPosition)
        {
            return;
        }

        Value_MovementType = movementType.Value;
        Value_TargetPosition = targetPosition.Value;

        switch (Value_MovementType)
        {
            case MovementType.StandStill:
                {
                    FSM.ChangeState(FloatMovementStateIdle.Instance);
                }
                break;
            case MovementType.WanderFreely:
                ChangerWanderingDirection();
                break;
        }
    }
    
    private FloatMovementState getRandomWanderState()
    {
        FloatMovementState[] wanderStates = new FloatMovementState[] { FloatMovementStateWanderLeft.Instance, FloatMovementStateWanderRight.Instance };
        int randomIndex = Random.Range(0, wanderStates.Length);

        return wanderStates[randomIndex];
    }

    /*private InternalState getRandomChaseState()
    {
        InternalState[] chaseStates = new InternalState[] { InternalState.ChaseLeft, InternalState.ChaseRight };
        int randomIndex = Random.Range(0, chaseStates.Length);

        return chaseStates[randomIndex];
    }*/
    #endregion // External state management

    #region - Internal state management
    public void ChangeState(FSMState<FloatMovement> newState)
    {
        FSM.ChangeState(newState);
    }
    public FSMState<FloatMovement> GetCurrentState()
    {
        return FSM.CurrentState;
    }
    public FSMState<FloatMovement> GetPreviousState()
    {
        return FSM.PreviousState;
    }
    void ProcessInternalState()
    {
        FSM.ProcessState();
    }
    #endregion // Internal state managements

    #region - Movement methods
    public void StopMovement()
    {
        /* Round the coordinates so they are perfect pixel aligned */
        m_rigidBody.position = new Vector2(Mathf.Round(m_rigidBody.position.x), Mathf.Round(m_rigidBody.position.y));
        m_rigidBody.velocity = Vector2.zero;
    }

    public void StartWanderingRight()
    {
        /* Round the coordinates so they are perfect pixel aligned */
        float wanderAngleRadians = WanderMaxAngleDegrees * Mathf.PI / 180.0f;
        float randomAngle = Random.Range(0.0f, wanderAngleRadians) - wanderAngleRadians / 2.0f;

        float xDirection = Mathf.Acos(randomAngle);
        float yDirection = Mathf.Asin(randomAngle);

        Vector2 direction = new Vector2(xDirection, yDirection);

        m_rigidBody.position = new Vector2(Mathf.Round(m_rigidBody.position.x), Mathf.Round(m_rigidBody.position.y));
        m_rigidBody.velocity = direction.normalized * WanderSpeed;

        transform.localScale = new Vector2(1.0f, transform.localScale.y);

        // Calculate the end of this wandering period
        float wanderingTime = Random.Range(WanderMinTime, WanderMaxTime);
        m_endWanderingTime = Time.time + wanderingTime;
    }

    public void StartWanderingLeft()
    {
        StartWanderingRight();

        m_rigidBody.velocity = new Vector2(-m_rigidBody.velocity.x, m_rigidBody.velocity.y);

        transform.localScale = new Vector2(-1.0f, transform.localScale.y);
    }

    void ChangerWanderingDirection()
    {
        FloatMovementState newState = getRandomWanderState();
        FSM.ChangeState(newState);
    }

    public void CheckWanderingDirectionChange()
    {
        if (Time.time >= m_endWanderingTime)
        {
            ChangerWanderingDirection();
        }
    }

    public void PlayAnimation(string animation)
    {
        m_animator.SetTrigger(animation);
    }
    #endregion // Helper methods

#if DISABLE
    private void ExecuteState()
    {
        switch (m_internalState)
        {
            case InternalState.Idle:
                /* Empty on purpose */
                break;

            case InternalState.WanderLeft:
                
            { 
                Vector2 monsterPos = (Vector2)transform.position;
                Vector2 playerPos = (Vector2)m_player.transform.position;
                playerPos.y += 20.0f;

                Vector2 direction = monsterPos - playerPos;

                if (direction.sqrMagnitude <= m_sqrFollowDistance)
                {
                    if (direction.x <= 0.0f)
                        TransitionState(MonsterState.ChaseLeft);
                    else
                        TransitionState(MonsterState.ChaseRight);

                    return;
                }

                direction = (Vector2)transform.position - m_movementTarget;
                if (direction.sqrMagnitude <= 4.0f)
                    m_targetReached = true;

                if (m_targetReached)
                {
                    float angle = Random.Range(0.0f, 360.0f);

                    direction = (new Vector2(Mathf.Sin(angle), Mathf.Cos(angle))) * m_wanderDistance;

                    //if (direction.x > transform.position.x)
                    //    direction = new Vector2(-direction.x, direction.y);

                    // Check if we can go that far
                    m_collider.enabled = false;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude);
                    m_collider.enabled = true;

                    if (hit.collider != null)
                    {
                        direction.Normalize();
                        m_movementTarget = hit.point - direction * 2.0f * Mathf.Max(m_collider.bounds.size.x, m_collider.bounds.size.y);
                    }
                    else
                    {
                        m_movementTarget = (Vector2)transform.position + direction;
                    }

                    if (m_movementTarget.x > transform.position.x)
                        TransitionState(MonsterState.WanderRight);
                    else
                        TransitionState(MonsterState.WanderLeft);

                    m_targetReached = false;

                    //Debug.Log("Enemy target " + m_movementTarget + ", current pos " + transform.position);
                }

                direction = m_movementTarget - (Vector2)transform.position;
                direction.Normalize();

                m_rigidBody.position = Vector2.MoveTowards(transform.position, m_movementTarget, Time.deltaTime * m_moveSpeed);
            }
        break;
        }

        /*
        switch (m_internalState)
        {
            case InternalState.Idle:
            case InternalState.ChaseLeft:
            case InternalState.ChaseRight:
            case InternalState.AttackLeft:
            case InternalState.AttackRight:
                {
                    Vector2 monsterPos = (Vector2)transform.position;
                    Vector2 playerPos = (Vector2)m_player.transform.position;
                    playerPos.y += 20.0f;

                    Vector2 direction = playerPos - monsterPos;

                    if (direction.sqrMagnitude <= m_sqrAttackDistance)
                    {
                        if (direction.x <= 0.0f)
                            TransitionState(MonsterState.AttackLeft);
                        else
                            TransitionState(MonsterState.AttackRight);

                        m_rigidBody.position = Vector2.MoveTowards(monsterPos, playerPos, Time.deltaTime * m_moveSpeed);
                    }
                    else if (direction.sqrMagnitude <= m_sqrFollowDistance)
                    {
                        if (direction.x <= 0.0f)
                        {
                            if (m_currentState != MonsterState.ChaseLeft)
                                TransitionState(MonsterState.ChaseLeft);
                        }
                        else if (m_currentState != MonsterState.ChaseRight)
                        {
                            TransitionState(MonsterState.ChaseRight);
                        }

                        m_rigidBody.position = Vector2.MoveTowards(monsterPos, playerPos, Time.deltaTime * m_moveSpeed);
                    }
                    else
                    {
                        switch (m_currentState)
                        {
                            case MonsterState.ChaseLeft:
                            case MonsterState.AttackLeft:
                                TransitionState(MonsterState.WanderRight);
                                break;
                            case MonsterState.ChaseRight:
                            case MonsterState.AttackRight:
                                TransitionState(MonsterState.WanderLeft);
                                break;
                        }                     
                    }
                }
                break;
            case InternalState.WanderLeft:
            case InternalState.WanderRight:
                {
                    Vector2 monsterPos = (Vector2)transform.position;
                    Vector2 playerPos = (Vector2)m_player.transform.position;
                    playerPos.y += 20.0f;

                    Vector2 direction = monsterPos - playerPos;

                    if (direction.sqrMagnitude <= m_sqrFollowDistance)
                    {
                        if (direction.x <= 0.0f)
                            TransitionState(MonsterState.ChaseLeft);
                        else
                            TransitionState(MonsterState.ChaseRight);

                        return;
                    }

                    direction = (Vector2)transform.position - m_movementTarget;
                    if (direction.sqrMagnitude <= 4.0f)
                        m_targetReached = true;

                    if (m_targetReached)
                    {
                        float angle = Random.Range(0.0f, 360.0f);

                        direction = (new Vector2(Mathf.Sin(angle), Mathf.Cos(angle))) * m_wanderDistance;

                        //if (direction.x > transform.position.x)
                        //    direction = new Vector2(-direction.x, direction.y);

                        // Check if we can go that far
                        m_collider.enabled = false;
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude);
                        m_collider.enabled = true;

                        if (hit.collider != null)
                        {
                            direction.Normalize();
                            m_movementTarget = hit.point - direction * 2.0f*Mathf.Max(m_collider.bounds.size.x, m_collider.bounds.size.y);
                        }
                        else
                        {
                            m_movementTarget = (Vector2)transform.position + direction;
                        }

                        if (m_movementTarget.x > transform.position.x)
                            TransitionState(MonsterState.WanderRight);
                        else
                            TransitionState(MonsterState.WanderLeft);

                        m_targetReached = false;

                        //Debug.Log("Enemy target " + m_movementTarget + ", current pos " + transform.position);
                    }

                    direction = m_movementTarget - (Vector2)transform.position;
                    direction.Normalize();

                    m_rigidBody.position = Vector2.MoveTowards(transform.position, m_movementTarget, Time.deltaTime * m_moveSpeed);
                }
                break;

        }
        */
    }
#endif
    /*
    void TransitionState(MonsterState state)
    {
        PlayStateAnimation(state);
		m_currentState = state;
    }

    void PlayStateAnimation(MonsterState state)
    {
        m_spriteRenderer.color = Color.white;

        // Audio
        switch (state)
        {
			case MonsterState.WanderLeft:
			case MonsterState.WanderRight:
				if (m_currentState != MonsterState.WanderLeft &&
				    m_currentState != MonsterState.WanderRight) {
					m_audioSource.pitch = m_enemyWanderPitch;
					m_audioSource.clip = m_enemyWander;
					m_audioSource.Play ();
				}
                break;
			case MonsterState.ChaseLeft:
			case MonsterState.ChaseRight:
				if (m_currentState != MonsterState.ChaseLeft &&
				    m_currentState != MonsterState.ChaseRight) {
					m_audioSource.pitch = m_enemyChasePitch;
					m_audioSource.clip = m_enemyChase;
					m_audioSource.Play ();
				}
                break;
        }

        // Animation
        switch (state)
        {
            case MonsterState.WanderLeft:
               
                m_animator.SetTrigger("monsterWalk");
                transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                break;
            case MonsterState.WanderRight:
                m_animator.SetTrigger("monsterWalk");
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                break;
            case MonsterState.ChaseLeft:
                m_animator.SetTrigger("monsterWalk");
                transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                m_spriteRenderer.color = Color.red;
                break;
            case MonsterState.ChaseRight:
                m_animator.SetTrigger("monsterWalk");
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                m_spriteRenderer.color = Color.red;
                break;
            case MonsterState.AttackLeft:
                m_animator.SetTrigger("monsterAttack");
                transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                m_spriteRenderer.color = Color.red;
                break;
            case MonsterState.AttackRight:
                m_animator.SetTrigger("monsterAttack");
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                m_spriteRenderer.color = Color.red;
                break;
            case MonsterState.Idle:
                m_animator.SetTrigger("monsterWalk");
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        m_targetReached = true;
        if (coll.collider.bounds.center.x >= m_rigidBody.position.x)
            MoveUp();
    }

    void OnPlayerAttack()
    {
        AudioSource.PlayClipAtPoint(m_enemyOuch, Camera.main.transform.position);
        Life--;
        if (Life <= 0)
        {
            NumSpawnMonsters--;
			//LevelManager.Instance.EnemyDestroyed();
            DestroyObject(gameObject);
        }
        else
        {
            m_spriteRenderer.color = Color.white;
            StopCoroutine(MonsterHitAnimation());
            StartCoroutine(MonsterHitAnimation());
        }
    }

    private IEnumerator MonsterHitAnimation()
    {
        for (int i = 0; i < 20; ++i)
        {
            m_spriteRenderer.color = new Color(1.0f, Random.Range(0.5f, 1.0f), 1.0f);
            yield return new WaitForSeconds(0.05f);
        }
        m_spriteRenderer.color = Color.white;
    }

    void MoveUp()
    {
        m_rigidBody.position = new Vector2(m_rigidBody.position.x, m_rigidBody.position.y + 2.0f);
    }

    void OnEnemyAttack()
    {
        m_player.OnEnemyAttack(m_collider, Damage);
    }
    */
}
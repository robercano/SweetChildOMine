using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

    public float FollowDistance;
    public float AttackDistance;

    [HideInInspector]
    public enum MonsterState
    {
        Idle, WalkLeft, WalkRight, AttackLeft, AttackRight
    };

    private Animator m_animator;
    private Rigidbody2D m_rigidBody;
    private BoxCollider2D m_collider;

    private float m_sqrFollowDistance;
    private float m_sqrAttackDistance;

    private float m_moveSpeed = 24.0f;
    private MonsterState m_currentState;

    //private GameObject m_player;
    private Miner m_player;

    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<BoxCollider2D>();

        m_currentState = MonsterState.Idle;

        m_player = GameObject.FindObjectOfType<Miner>();

        m_sqrFollowDistance = FollowDistance * FollowDistance;
        m_sqrAttackDistance = AttackDistance * AttackDistance;
    }
	
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
            case MonsterState.Idle:
            case MonsterState.WalkLeft:
            case MonsterState.WalkRight:
            case MonsterState.AttackLeft:
            case MonsterState.AttackRight:

                Vector2 monsterPos = (Vector2)transform.position;
                Vector2 playerPos = (Vector2)m_player.transform.position;
                playerPos.y += 20.0f;

                Vector2 direction = monsterPos - playerPos;

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
                        TransitionState(MonsterState.WalkLeft);
                    else
                        TransitionState(MonsterState.WalkRight);

                    m_rigidBody.position = Vector2.MoveTowards(monsterPos, playerPos, Time.deltaTime * m_moveSpeed);
                }
                else
                {
                    TransitionState(MonsterState.Idle);
                }
                break;
        }
        #endregion
    }

    void TransitionState(MonsterState state)
    {
        m_currentState = state;
        PlayStateAnimation(state);
    }

    void PlayStateAnimation(MonsterState state)
    {
        switch (state)
        {
            case MonsterState.WalkLeft:
                m_animator.SetTrigger("monsterWalk");
                transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                break;
            case MonsterState.WalkRight:
                m_animator.SetTrigger("monsterWalk");
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                break;
            case MonsterState.AttackLeft:
                m_animator.SetTrigger("monsterAttack");
                transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                break;
            case MonsterState.AttackRight:
                m_animator.SetTrigger("monsterAttack");
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                break;
            case MonsterState.Idle:
                m_animator.SetTrigger("monsterWalk");
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log("Collision!");
        if (coll.collider.bounds.center.x >= m_rigidBody.position.x)
            MoveUp();
    }

    void MoveUp()
    {
        m_rigidBody.position = new Vector2(m_rigidBody.position.x, m_rigidBody.position.y + 2.0f);
    }

    void OnEnemyAttack()
    {
        m_player.OnEnemyAttack(m_collider);
    }
}

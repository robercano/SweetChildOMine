using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

    public float FollowDistance;
    public float AttackDistance;
    public int Life;
    public int Damage;

    public AudioClip m_enemyOuch;
    public AudioClip m_enemyWander;
    public AudioClip m_enemyChase;

    public static int NumSpawnMonsters = 0;

    [HideInInspector]
    public enum MonsterState
    {
        Idle, ChaseLeft, ChaseRight, AttackLeft, AttackRight, WanderLeft, WanderRight
    };

    private Animator m_animator;
    private Rigidbody2D m_rigidBody;
    private BoxCollider2D m_collider;
    private SpriteRenderer m_spriteRenderer;
    private AudioSource m_audioSource;

    private float m_sqrFollowDistance;
    private float m_sqrAttackDistance;

    private float m_moveSpeed = 24.0f;
    private float m_wanderDistance = 100.0f;
    private MonsterState m_currentState;
    private Vector2 m_movementTarget;
    private bool m_targetReached;

    private Miner m_player;

    private float m_enemyWanderPitch;
    private float m_enemyChasePitch;

    // Use this for initialization
    void Start () {
        NumSpawnMonsters++;

        m_animator = GetComponent<Animator>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<BoxCollider2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_audioSource = GetComponent<AudioSource>();

        if (Random.Range(0, 2) == 0)
            m_currentState = MonsterState.WanderLeft;
        else
            m_currentState = MonsterState.WanderRight;

        m_movementTarget = transform.position;
        m_targetReached = true;

        m_player = GameObject.FindObjectOfType<Miner>();

        m_sqrFollowDistance = FollowDistance * FollowDistance;
        m_sqrAttackDistance = AttackDistance * AttackDistance;

        m_enemyWanderPitch = Random.Range(1.0f, 1.2f);
        m_enemyChasePitch = Random.Range(1.0f, 1.5f);

        m_audioSource.clip = m_enemyWander;
        m_audioSource.pitch = m_enemyWanderPitch;
        m_audioSource.Play();
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
        if (m_player == null || m_player.Equals(null))
        {
            TransitionState(MonsterState.Idle);
            return;
        }

        switch (m_currentState)
        {
            case MonsterState.Idle:
            case MonsterState.ChaseLeft:
            case MonsterState.ChaseRight:
            case MonsterState.AttackLeft:
            case MonsterState.AttackRight:
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
            case MonsterState.WanderLeft:
            case MonsterState.WanderRight:
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
        #endregion
    }

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
			LevelManager.Instance.EnemyDestroyed();
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Miner : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidBody;
    private BoxCollider2D _boxCollider;

    private float _walkSpeed = 32.0f; /**< This value is the width of a single sprite multiplied by 2 */
    private float _movementXTarget;   /**< Last location the user clock on for movement */
    private bool _canMove;
    private bool _jumpUp;
    private bool _moving;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        
        _canMove = true;
        _jumpUp = false;
        _moving = false;

        _movementXTarget = Mathf.Round(_rigidBody.position.x);
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
            _movementXTarget = Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).x);
            _moving = true;
        }

        float deltaX = _movementXTarget - Mathf.Round(_rigidBody.position.x);

        /* Check if the sprite really needs to move */
        if (_moving && _canMove && (deltaX > float.Epsilon || deltaX < -float.Epsilon))
        {
           // _animator.SetBool("minerWalk", true);

            transform.localScale = new Vector2(Mathf.Sign(deltaX) * Mathf.Abs(transform.localScale.x), transform.localScale.y);

            if (_jumpUp)
            {
                /* TODO: Check manually if we need to jump of fall! */
                _rigidBody.position = new Vector2(_rigidBody.position.x, _rigidBody.position.y + 1.0f);
                _jumpUp = false;
            }
            else
            {
                MakeItFall();
            }

            _rigidBody.velocity = new Vector2(Mathf.Sign(deltaX) * _walkSpeed, _rigidBody.velocity.y);
        }
        if ((_moving && deltaX <= float.Epsilon && deltaX >= -float.Epsilon) ||
            !_canMove)
        {
            /* We are done moving */
            _animator.SetBool("minerWalk", false);

            /* Round the coordinates so they are perfect pixel aligned */
            _rigidBody.position = new Vector2(Mathf.Round(_rigidBody.position.x), Mathf.Round(_rigidBody.position.y));
            _rigidBody.velocity = Vector2.zero;

            _movementXTarget = _rigidBody.position.x;
            _canMove = true;
            _moving = false;

            MakeItFall();
        }
    }

    void MakeItFall()
    {
        /* Check if we need to fall down */
        float boxExtent = _boxCollider.bounds.extents.x;
        float absPosX = Mathf.Round(transform.position.x);
        float absPosY = Mathf.Round(transform.position.y);
        Vector2 leftSource = new Vector2(absPosX - boxExtent + 1.0f, absPosY);
        Vector2 rightSource = new Vector2(absPosX + boxExtent - 1.0f, absPosY);

        _boxCollider.enabled = false;
        RaycastHit2D hitLeft = Physics2D.Raycast(leftSource, Vector2.down);
        RaycastHit2D hitRight = Physics2D.Raycast(rightSource, Vector2.down);
        _boxCollider.enabled = true;

        if (hitLeft != null && hitRight != null) {
            _rigidBody.position = new Vector2(_rigidBody.position.x, _rigidBody.position.y - Mathf.Min(hitLeft.distance, hitRight.distance));
        }

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        float distance = (coll.collider.bounds.center.y - 0.5f) - _rigidBody.position.y;
        if (coll.collider.bounds.extents.y > 1.0f)
        {
            _canMove = false;
        }

        if (distance > -0.5f && distance < 0.5f)
        {
            if (coll.collider.bounds.extents.y <= (0.5f + float.Epsilon))
            {
                _jumpUp = true;
            }
        }
    }
};
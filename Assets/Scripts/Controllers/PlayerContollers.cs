﻿using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContollers : MonoBehaviour
{
    private Collider2D _boxcollider;

    [SerializeField] private GameObject player;

    [Header("Wall")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isFacingRight = true;

    private float horizontal;
    public int speedMove = 5;


    [Header("Jump")]
    [SerializeField] public int limitJump = 1;
    [SerializeField] public float forcejump = 8;
    public bool isGrounded = true;
    private int _currentJump;


    [Header("WallSliding")]
    [SerializeField] public float wallSlidespeed = 0;
    [SerializeField] public Transform wallCheckPoint;
    [SerializeField] Vector2 wallCheckSize;
    private bool isSliding;
    private bool isOnWall;

    [Header("WallJump")]
    [SerializeField] public float wallJumpForce = 18;
    [SerializeField] public float wallJumpDirection = -1f;
    [SerializeField] Vector2 wallJumpAngle;
    private int _currentWallJump;

    [Header("Ladder")]
    private float _vertical;
    private bool _isLadder;
    private bool _isClimbing;

    [Header("Crouch/Ramp")]

    public float crouchHeight, crouchWidth, rampHeight, rampWidth;
    public int speedCrouch = 3;
    public int speedRamp = 2;


    AudioManager audioManager;
    public Animator animator;

    void Awake()
    {
        TryGetComponent(out rb);
        TryGetComponent(out _boxcollider);

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            if (player.CompareTag("NonGrass"))
            {
                audioManager.PlaySFX(audioManager.walk);
            }
            else if (player.CompareTag("Grass"))
            {
                audioManager.PlaySFX(audioManager.walkOnGrass);
            }

        }

        Flip();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcejump);
            isGrounded = false;
            _currentJump++;
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

        Slide();
        WallJumping();

        _vertical = Input.GetAxisRaw("Vertical");
        if (_isLadder && Mathf.Abs(_vertical) > 0f)
        {
            _isClimbing = true;
        }

        Crouch();
        Ramp();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speedMove, rb.linearVelocity.y);
        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(0.5f, 0.1f), 0f, groundLayer);

        if (_isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, _vertical * speedMove);
        }
        else
        {
            rb.gravityScale = 1f;
        }

    }


    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
    //     {;
    //     }
    // }


    // void Move()
    // {
    // Vector3 direction = Input.GetAxis("Horizontal") * Vector2.right;

    // var positionGroundCollision = new Vector3(offsetGroundCollision.x, offsetGroundCollision.y, 0) + _transform.position + direction; 
    // var _isGrounded = Physics2D.OverlapBox(positionGroundCollision, sizeGroundCollision, 0f, detectground);

    // if (_isGrounded!= null && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
    // {
    //     animator.SetBool("isGrounded", true);

    //     if (_isGrounded.CompareTag("NonGrass"))
    //     {
    //         audioManager.PlaySFX(audioManager.walk);
    //         Debug.Log("player walk");
    //     }
    //     else if (_isGrounded.CompareTag("Grass"))
    //     {
    //         audioManager.PlaySFX(audioManager.walkOnGrass);
    //         Debug.Log("player walk on grass");
    //     }
    // }
    // else
    // {
    //     animator.SetBool("isGrounded", false);
    // }

    // if (_isGrounded!= null)
    // {
    //     return;
    // }

    // if (!Input.GetKey(KeyCode.LeftControl))
    // {
    //     if(Input.GetAxis("Horizontal") < 0)
    //        _transform.rotation = Quaternion.Euler(0, 180, 0);
    //     else
    //         _transform.rotation = Quaternion.Euler(0, 0, 0);

    // }

    // _transform.position += Vector3.right * Input.GetAxisRaw("Horizontal") * speedMove * Time.deltaTime;

    //     Vector3 direction = Input.GetAxis("Horizontal") * Vector2.right;

    //     var positionGroundCollision = new Vector3(offsetGroundCollision.x, offsetGroundCollision.y, 0) + _transform.position + direction; 
    //     var _isGrounded = Physics2D.OverlapBox(positionGroundCollision, sizeGroundCollision, 0f, detectground);

    //     // Animation & sons
    //     if (_isGrounded != null && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
    //     {
    //         animator.SetBool("isGrounded", true);

    //         if (_isGrounded.CompareTag("NonGrass"))
    //         {
    //             audioManager.PlaySFX(audioManager.walk);
    //             Debug.Log("player walk");
    //         }
    //         else if (_isGrounded.CompareTag("Grass"))
    //         {
    //             audioManager.PlaySFX(audioManager.walkOnGrass);
    //             Debug.Log("player walk on grass");
    //         }
    //     }
    //     else
    //     {
    //         animator.SetBool("isGrounded", false);
    //     }

    //     // Flip du personnage
    //     if (!Input.GetKey(KeyCode.LeftControl))
    //     {
    //         if (Input.GetAxis("Horizontal") < 0)
    //             _transform.rotation = Quaternion.Euler(0, 180, 0);
    //         else if (Input.GetAxis("Horizontal") > 0)
    //             _transform.rotation = Quaternion.Euler(0, 0, 0);
    //     }

    //     // Mouvement
    //     _transform.position += Vector3.right * Input.GetAxisRaw("Horizontal") * speedMove * Time.deltaTime;
    // }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.green;
    //     Vector3 pos = new Vector3(offsetGroundCollision.x , offsetGroundCollision.y, 0) + transform.position;
    //     Gizmos.DrawWireCube(pos, sizeGroundCollision);
    // }


    // void Jump()
    // {
    //     rb.linearVelocity = Vector2.up * forcejump;
    //     _currentJump++;

    //     if (gameObject.CompareTag("NonGrass"))
    //     {
    //         audioManager.PlaySFX(audioManager.jump);
    //         Debug.Log("player jump");
    //     }
    //     else if (gameObject.CompareTag("Grass"))
    //     {
    //         audioManager.PlaySFX(audioManager.jumpGrass);
    //         Debug.Log("player jump grass");
    //     }
    // }


    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
    //     {
    //         _currentJump = 0;
    //         animator.SetBool("isJumping", true);
    //     }
    // }

    void Slide()
    {
        isOnWall = Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, groundLayer);

        if (isOnWall && rb.linearVelocity.y < 0)
        {
            isSliding = true;
            animator.SetBool("isSliding", true);
        }
        else
        {
            isSliding = false;
            animator.SetBool("isSliding", false);
        }

        if (isSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallSlidespeed);
        }
    }

    void WallJumping()
    {
        wallJumpDirection *= -1;

        if (isSliding || isOnWall)
        {
            rb.AddForce(new Vector2(wallJumpForce * wallJumpDirection * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
            _currentWallJump++;

            if (gameObject.CompareTag("NonGrass"))
            {
                audioManager.PlaySFX(audioManager.jump);
            }
            else if (gameObject.CompareTag("Grass"))
            {
                audioManager.PlaySFX(audioManager.jumpGrass);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Ladder"))
        {
            _isLadder = true;
            audioManager.PlaySFX(audioManager.climbingLadder);
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            _isLadder = false;
            _isClimbing = false;
        }
    }

    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.SetBool("isSneaking", true);
            speedMove = speedCrouch;
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            animator.SetBool("isSneaking", false);
            speedMove = 5;
        }
    }
    
    void Ramp()
    {
        if (Input.GetKeyDown(KeyCode.V))   
        {
            animator.SetBool("isRamping", true);
            speedMove = speedRamp;
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            animator.SetBool("isRamping", false);
            speedMove = 5;
        }
    }
}

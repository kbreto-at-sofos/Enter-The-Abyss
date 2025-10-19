using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    private static readonly int AnimatorYVelocity = Animator.StringToHash("yVelocity");
    private static readonly int AnimatorMagnitud = Animator.StringToHash("magnitud");
    private static readonly int AnimatorIsWallSliding = Animator.StringToHash("isWallSliding");
    private static readonly int AnimatorJump = Animator.StringToHash("jump");


    [SerializeField] private Rigidbody2D rb;
    public Animator animator;
    private bool _isFacingRight = true;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private float _horizontalMovement;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCancelForce = 0.5f;
    [SerializeField] private int maxJumps = 2;
    private int _remainingJumps;

    [Header("Ground Check")]
    public Transform groundCheckPosition;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    private bool _isGrounded;
    
    [Header("Gravity")]
    public float baseGravity = 2;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;
    
    [Header("Wall Check")]
    public Transform wallCheckPosition;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    [Header("Wall Movement")] 
    public float wallSlideSpeed = 2;
    private bool _isWallSliding;
    private bool _isWallJumping;
    private float _wallJumpDirection;
    private float _wallJumpTimer;
    [FormerlySerializedAs("_wallJumpTime")] public float wallJumpTime = 0.5f;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        GroundCheck();
        Gravity();
        WallSlide();
        ProcessWallJump();

        if (!_isWallJumping)
        {
            // Move
            rb.linearVelocity = new Vector2(_horizontalMovement * moveSpeed, rb.linearVelocity.y);    
            Flip();
        }
        
    }

    private void FixedUpdate()
    {
        Animate();
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (_remainingJumps > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                _remainingJumps--;
                AnimateJump();
            }    
        }
        
        if (context.canceled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCancelForce );
            _remainingJumps--;
            AnimateJump();
        }
        
        // WALL JUMP
        if (context.performed && _wallJumpTimer > 0f)
        {
            // allow double jump after wall jumping
            _remainingJumps = maxJumps;
            _isWallJumping = true;
            // Jump away from the wall
            rb.linearVelocity = new Vector2(_wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            _wallJumpTimer = 0;
            AnimateJump();
            
            // Force Flip
            if (transform.localScale.x != _wallJumpDirection)
            {
                ForceFlip();
            }
            
            // the default jump time is 0.5 seconds,
            // so we can jump again after 0.6 seconds.
            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPosition.position, groundCheckSize, 0f, groundLayer))
        {
            _remainingJumps = maxJumps;
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPosition.position, wallCheckSize, 0, wallLayer);
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void WallSlide()
    {
        // Not grounded & On a wall & movement != 0
        if (!_isGrounded && WallCheck() && _horizontalMovement != 0)
        {
            // allow jump after touching a wall
            _remainingJumps = maxJumps;
            _isWallSliding = true;
            //caps fall rate
            var fallRate = Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fallRate);
            
        }else
        {
            _isWallSliding = false;
        }
    }

    private void ProcessWallJump()
    {
        if (_isWallSliding)
        {
            _isWallJumping = false;
            _wallJumpDirection = -transform.localScale.x; // set to the other way that we are facing
            _wallJumpTimer = wallJumpTime; //reset wall jump timer
            
            CancelInvoke(nameof(CancelWallJump));
        }
        else if (_wallJumpTimer > 0f)
        {
            _wallJumpTimer -= Time.deltaTime; 
        }
    }

    private void CancelWallJump()
    {
        _isWallJumping = false;
        
    }

    private void Flip()
    {
        if (_isFacingRight && _horizontalMovement < 0 || !_isFacingRight && _horizontalMovement > 0)
        {
            ForceFlip();
        }
    }

    private void ForceFlip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void Animate()
    {
        
        // estaba obteniendo un decimal muy muy pequeño al usar rb.linearVelocity.y
        // eso estaba rompiendo la animacion al caminar a la izquierda especificamente
        // por eso ahora utilizo Mathf.Round(rb.linearVelocity.y)
        animator.SetFloat(AnimatorYVelocity, Mathf.Round(rb.linearVelocity.y));
        animator.SetFloat(AnimatorMagnitud, rb.linearVelocity.magnitude);
        animator.SetBool(AnimatorIsWallSliding, _isWallSliding);
    }

    private void AnimateJump()
    {
        animator.SetTrigger(AnimatorJump);
    }

    private void OnDrawGizmosSelected()
    {
        // Ground check visual
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPosition.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPosition.position, wallCheckSize);
    }
}

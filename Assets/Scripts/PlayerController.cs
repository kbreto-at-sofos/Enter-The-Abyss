using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{

    
    [SerializeField] private Rigidbody2D rb;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private float horizontalMovement;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpCancelForce = 0.5f;
    [SerializeField] private int maxJumps = 2;
    private int remainingJumps;

    [Header("Ground Check")]
    public Transform groundCheckPosition;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    
    [Header("Gravity")]
    public float baseGravity = 2;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;
    
    
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
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        GroundCheck();
        Gravity();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (remainingJumps > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                remainingJumps--;
            }    
        }
        
        if (context.canceled)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCancelForce );
            remainingJumps--;
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPosition.position, groundCheckSize, 0f, groundLayer))
        {
            remainingJumps = maxJumps;
        }
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
}

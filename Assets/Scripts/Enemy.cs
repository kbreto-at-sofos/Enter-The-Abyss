using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 4f;
    public float jumpForce = 2f;
    public LayerMask groundLayer;

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private bool _shouldJump;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int damage = 1;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // is Grounded?
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        
        // Player Direction
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        
        //Player above detection
        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << player.gameObject.layer);

        //Chase player
        
        if (_isGrounded)
        {
            
            _rb.linearVelocity = new Vector2(direction * chaseSpeed, _rb.linearVelocity.y);
            
            //Jump if there's gap ahead && no ground infront
            //else if there's player above and platform above
            
            //if ground
            RaycastHit2D groundInFront =
                Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
            
            //if gap
            RaycastHit2D gapAhead =
                Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
            
            //If platform above
            RaycastHit2D platformAbove =
                Physics2D.Raycast(transform.position, Vector2.up, 3f, groundLayer);

            if (!groundInFront.collider && !gapAhead.collider)
            {
                _shouldJump = true;
            }else if (isPlayerAbove && platformAbove.collider)
            {
                _shouldJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_isGrounded && _shouldJump)
        {
            _shouldJump = false;
            Vector2 direction = (player.position - transform.position).normalized;
            Vector2 jumpDirection = direction * jumpForce;
            
            _rb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
        }
    }
}

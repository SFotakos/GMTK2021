﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CompanionController m_CompanionController;

    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [SerializeField] private float m_DodgeForce = 300f;                          // Amount of force added when the player dodge.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.

    public Rigidbody2D playerRigidbody2D;
    public bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    private float m_ElapsedTime;
    private float m_GroundedTime = 0.10f;

    private void Awake()
    {
        playerRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (m_ElapsedTime > m_GroundedTime)
        {
            m_Grounded = false;
            m_ElapsedTime = 0;
        }
        else
        {
            m_ElapsedTime += Time.fixedDeltaTime;
        }
        

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
            }
        }
    }

    public void Move(float move, bool dodge, bool jump)
    {
        if (dodge)
        {
            playerRigidbody2D.AddForce(new Vector2(m_DodgeForce * move, 0f));
            m_CompanionController.ChangeJointState(false);
        } else {
            Vector3 targetVelocity = new Vector2(move, playerRigidbody2D.velocity.y);
            playerRigidbody2D.velocity = Vector3.SmoothDamp(playerRigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        }

        if (move > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (move < 0 && m_FacingRight)
        {
            Flip();
        }

        if (m_Grounded && jump)
        {
            m_Grounded = false;
            playerRigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Panyon"))
        {
            m_CompanionController.ChangeJointState(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Abyss"))
        {
            transform.position = Vector2.zero;
        }
    }
}

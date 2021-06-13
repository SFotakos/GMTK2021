using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public CompanionController m_CompanionController;
    [SerializeField] private PlayerCombat m_PlayerCombat;
    [SerializeField] private Animator m_Animator;

    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.

    const float k_GroundedRadius = .45f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.

    public Rigidbody2D playerRigidbody2D;
    public bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    private float m_ElapsedTime;
    private float m_GroundedTime = 0.08f;
    public System.Action m_GroundedCallback;

    private void FixedUpdate()
    {
        if (m_PlayerCombat.isDead)
            return;

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
        Collider2D[] platforms = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i].gameObject != gameObject)
            {
                m_Grounded = true;
                // This avoid the grounded grace period triggering.
                if (playerRigidbody2D.velocity.y <= 0.01f)
                    m_GroundedCallback();
            }
        }

        if (!m_CompanionController.isJoined)
        {
            Collider2D[] player = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, LayerMask.GetMask("Player"));
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].gameObject != gameObject)
                {
                    m_CompanionController.ChangeJointState(true);
                    break;
                }
            }
        }
    }

    public void Move(float move, bool dodge, bool jump)
    {
        if (dodge && m_Grounded && move != 0) // If grounded and moving, dodge.
        {
            //Add invulnerability during dodge
            m_CompanionController.ChangeJointState(false);
        }
        else
        {
            Vector3 targetVelocity = new Vector2(move, playerRigidbody2D.velocity.y);
            playerRigidbody2D.velocity = Vector3.SmoothDamp(playerRigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        }

        //Flip player
        if (move > 0 && !m_FacingRight) Flip();
        else if (move < 0 && m_FacingRight) Flip();

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Abyss"))
        {
            InstantKill();
        }
    }

    public void InstantKill()
    {
        m_PlayerCombat.TakeDamage(m_PlayerCombat.maxHealth);
    }

    public void Reset()
    {
        // Reset game
        transform.position = Vector2.zero;
        m_CompanionController.ChangeJointState(false);
        m_CompanionController.transform.position = Vector2.zero + m_CompanionController.companionOffset;
        m_CompanionController.ChangeJointState(true);
        m_Animator.SetBool("Died", false);
        m_CompanionController.Reset();
    }
}

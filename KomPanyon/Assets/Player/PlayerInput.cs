using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public PlayerController m_Controller;
    public Animator animator;

    [SerializeField] private float m_PlayerSpeed = 350f;
    float m_HorizontalMovement = 0f;
    bool m_ShouldJump = false;
    bool m_ShouldDodge = false;

    private float m_DodgeElapsedTime;
    [SerializeField] private float m_DodgeCooldown = 0.5f;

    private float m_AttackElapsedTime;
    [SerializeField] private float m_AttackCooldown = 1f;
    bool m_ShouldAttack = false;

    private void Awake()
    {
        m_Controller.m_GroundedCallback = Landed;
    }

    void Update()
    {
        m_HorizontalMovement = Input.GetAxisRaw("Horizontal") * m_PlayerSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            m_ShouldJump = true;
        }

        if (m_DodgeElapsedTime > m_DodgeCooldown)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                m_ShouldDodge = true;
                m_DodgeElapsedTime = 0f;
            }
        } else
        {
            m_DodgeElapsedTime += Time.deltaTime;
        }

        if (m_AttackElapsedTime > m_AttackCooldown)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                m_ShouldAttack = true;
                m_AttackElapsedTime = 0f;
            }
        }
        else
        {
            m_AttackElapsedTime += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (m_ShouldAttack && !animator.GetBool("isJumping") && !animator.GetBool("isDodging"))
        {
            m_Controller.Attack();
            animator.SetBool("isAttacking", true);
        }
        m_ShouldAttack = false;

        m_Controller.Move(m_HorizontalMovement * Time.fixedDeltaTime, m_ShouldDodge, m_ShouldJump);
        animator.SetFloat("Speed", Mathf.Abs(m_HorizontalMovement));

        if (m_ShouldJump) animator.SetBool("isJumping", true);
        m_ShouldJump = false;

        if (m_ShouldDodge && !animator.GetBool("isJumping"))
        {
            m_ShouldDodge = false;
            animator.SetBool("isDodging", true);
        }
        
        
    }

    private void Landed()
    {
        animator.SetBool("isJumping", false);
    }

    public void EndedDodge()
    {
        animator.SetBool("isDodging", false);
    }

    public void EndedAttack()
    {
        animator.SetBool("isAttacking", false);
    }
}

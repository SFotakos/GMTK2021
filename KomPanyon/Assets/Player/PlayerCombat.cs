using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private CompanionController m_CompanionController;
    private GameController m_GameController;

    private float m_AttackElapsedTime = 1f;
    [SerializeField] private float m_AttackCooldown = .7f;
    bool m_ShouldAttack = false;

    [SerializeField] Transform m_AttackPoint;
    [SerializeField] int attackDamage = 5;
    [SerializeField] float m_AttackRange = 0.5f;
    [SerializeField] LayerMask m_EnemyLayers;
    [SerializeField] public int maxHealth = 5;
    int currentHealth;

    float m_HurtDelay = .7f;
    float m_HurtTimer = 0f;
    bool m_CanBeHurt = true;
    public bool isDead = false;

    [SerializeField] public List<Image> hearts;

    private void Awake()
    {
        currentHealth = maxHealth;
        m_GameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        if (isDead)
            return;

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

        if (!m_CanBeHurt)
        {
            if (Time.time >= m_HurtTimer)
            {
                m_CanBeHurt = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;

        if (animator.GetBool("isAttacking"))
        {
            Vector3 m_Velocity = Vector3.zero;
            GetComponent<Rigidbody2D>().velocity = Vector3.SmoothDamp(GetComponent<Rigidbody2D>().velocity, Vector3.zero, ref m_Velocity, 0.05f);
        }

        if (m_ShouldAttack && !animator.GetBool("isJumping") && !animator.GetBool("isDodging"))
        {
            animator.SetBool("isAttacking", true);
            Attack();
        }
        m_ShouldAttack = false;
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(m_AttackPoint.position, m_AttackRange, m_EnemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Kom hit" + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            m_CompanionController.Attack(enemy.gameObject);
            break;
        }
    }

    public void TakeDamage(int damage = 1)
    {
        if (m_CanBeHurt && !animator.GetBool("isDodging") && !animator.GetBool("isAttacking"))
        {

            m_CanBeHurt = false;
            m_HurtTimer = Time.time + m_HurtDelay;

            for (int i = 0; i < damage; i++)
            {
                currentHealth -= 1;
                hearts[currentHealth].enabled = false;
            }

            animator.SetTrigger("Hurt");

            if (currentHealth <= 0)
            {
                animator.SetBool("Died", true);
                isDead = true;
                m_CompanionController.Died();
                GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                animator.SetFloat("Speed", 0f);
                animator.SetBool("isJumping", false);
                animator.SetBool("isDodging", false);
                animator.SetBool("isAttacking", false);
            }
        }
    }

    //Called by the end of the death animation
    public void Died()
    {
        m_GameController.Died();
        StartCoroutine(RestartGame());
    }

    private void OnDrawGizmosSelected()
    {
        if (m_AttackPoint != null)
        {
            Gizmos.DrawWireSphere(m_AttackPoint.position, m_AttackRange);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        m_GameController.Restart();
        Reset();
    }

    public void Reset()
    {
        currentHealth = maxHealth;
        m_HurtDelay = .7f;
        m_HurtTimer = 0f;
        m_CanBeHurt = true;
        isDead = false;
    }
}

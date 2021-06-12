using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private CompanionController m_CompanionController;

    private float m_AttackElapsedTime = 1f;
    [SerializeField] private float m_AttackCooldown = 1f;
    bool m_ShouldAttack = false;

    [SerializeField] Transform m_AttackPoint;
    [SerializeField] int attackDamage = 5;
    [SerializeField] float m_AttackRange = 0.5f;
    [SerializeField] LayerMask m_EnemyLayers;
    
    void Update()
    {
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

    private void OnDrawGizmosSelected()
    {
        if (m_AttackPoint != null)
        {
            Gizmos.DrawWireSphere(m_AttackPoint.position, m_AttackRange);
        }
    }
}

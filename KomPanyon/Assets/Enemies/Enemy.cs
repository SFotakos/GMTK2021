using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Animator m_Animator;
    Rigidbody2D rb;
    public int maxHealth = 100;
    int m_CurrentHealth;
    [SerializeField] float m_PlayerSearchRadius = 3f;

    // Start is called before the first frame update
    void Awake()
    {
        m_CurrentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Collider2D[] player = Physics2D.OverlapCircleAll(transform.position, m_PlayerSearchRadius, LayerMask.GetMask("Player"));
        for (int i = 0; i < player.Length; i++)
        {
            if (player[i].gameObject != gameObject)
            {
                if (player[i].CompareTag("Kom"))
                {
                    float direction = player[i].transform.position.x - transform.position.x;
                    direction = Mathf.Abs(direction) < 2f ? direction*2 : direction;
                    rb.velocity = new Vector2(direction * 30f * Time.fixedDeltaTime, 0);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        m_CurrentHealth -= damage;
        m_Animator.SetTrigger("Hurt");

        if (m_CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy Died");
        m_Animator.SetBool("isDead", true);

        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.DrawWireSphere(transform.position, m_PlayerSearchRadius);
        
    }

}

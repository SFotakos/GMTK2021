using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Animator m_Animator;
    public int maxHealth = 100;
    int m_CurrentHealth;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentHealth = maxHealth;
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
    }
}

using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Animator m_Animator;
    [SerializeField] Rigidbody2D rb;
    public int maxHealth = 100;
    int m_CurrentHealth;
    [SerializeField] float m_PlayerSearchRadius = 4f;
    [SerializeField] bool canFly = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_CurrentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        Collider2D[] player = Physics2D.OverlapCircleAll(transform.position, m_PlayerSearchRadius, LayerMask.GetMask("Player"));
        if (player.Length == 0)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].gameObject != gameObject)
                {
                    if (player[i].CompareTag("Kom"))
                    {
                        Vector3 direction = player[i].transform.position - transform.position;
                        direction = Mathf.Abs(direction.magnitude) < 2f ? direction * 2 : direction;
                        LookTowards(direction.x < 0 ? 1 : -1);
                        rb.velocity = new Vector2(
                            direction.x * 30f * Time.fixedDeltaTime,
                            canFly ? direction.y * 30f * Time.fixedDeltaTime : 0f);
                    }
                }
            }
        }




        //RaycastHit2D hit = Physics2D
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

    private void LookTowards(int orientation)
    {
        Vector3 theScale = transform.localScale;
        theScale.x = orientation;
        transform.localScale = theScale;
    }


}

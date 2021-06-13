using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Animator m_Animator;
    [SerializeField] Rigidbody2D rb;
    public int maxHealth = 100;
    int m_CurrentHealth;
    [SerializeField] float enemySpeed = 30f;
    [SerializeField] float m_PlayerSearchRadius = 4f;
    [SerializeField] bool canFly = false;

    private bool m_Grounded = true;
    // Start is called before the first frame update
    void Awake()
    {
        m_CurrentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        m_Grounded = false;
        Collider2D[] platforms = Physics2D.OverlapCircleAll(transform.position, .4f, LayerMask.GetMask("Platform"));
        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i].gameObject != gameObject)
            {
                m_Grounded = true;
            }
        }

        Collider2D[] player = Physics2D.OverlapCircleAll(transform.position, m_PlayerSearchRadius, LayerMask.GetMask("Player"));
        if (player.Length == 0)
        {
            if (!canFly)
                rb.velocity = new Vector3(0f, m_Grounded ? -0.2f : -10f, 0f);
            else
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
                            direction.x * enemySpeed * Time.fixedDeltaTime,
                            canFly ? direction.y * enemySpeed * Time.fixedDeltaTime : m_Grounded ? -0.2f : -10f) ;
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

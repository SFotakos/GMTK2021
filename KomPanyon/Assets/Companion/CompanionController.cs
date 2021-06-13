using System.Collections;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody2D;
    [SerializeField] private PlayerController m_PlayerController;
    [SerializeField] private LayerMask m_GroundMask;

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;

    public bool isJoined = true;
    [SerializeField] float m_DisjointDistance = 5f;
    public Vector2 companionOffset = new Vector2(2.5f, 2.5f);
    [SerializeField] float m_TargetAcquisitionOffset = 0.3f;
    [SerializeField] float m_ReturnSpeed = 6f;

    float m_MinPlayerSpeed = 0.05f;
    float m_TimeToMoveElapsed;
    float m_TimeToMove = 0.55f;

    float m_TimeToDisjointElapsed;
    float m_TimeToDisjoint = 2f;

    float m_DisjointTimeElapsed;
    float m_DisjointTimeToKill = 6f;

    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private LineRenderer m_LineRenderer;
    [SerializeField] Color m_InactiveColor;

    [SerializeField] Color m_JointAttachedColor;
    [SerializeField] Color m_JointDetachingColor;

    [SerializeField] int attackDamage = 30;
    [SerializeField] float attackMaxDuration = 3f;
    Coroutine m_AttackCoroutine;
    bool m_Attacking = false;
    Vector3 m_EnemyPosition;

    bool isDead = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDead)
            return;

        float distance = m_LineRenderer.GetPosition(m_LineRenderer.positionCount - 1).magnitude - m_LineRenderer.GetPosition(0).magnitude;
        if (Mathf.Abs(distance) >= m_DisjointDistance)
        {
            if (m_TimeToDisjointElapsed > m_TimeToDisjoint)
            {
                ChangeJointState(false);
                m_TimeToDisjointElapsed = 0;
            }
            else
            {
                m_TimeToDisjointElapsed += Time.deltaTime;
            }

            // If not disjointed show player that the connection is breaking
            if (isJoined)
            {
                m_LineRenderer.startColor = m_JointDetachingColor;
                m_LineRenderer.endColor = m_JointDetachingColor;
            }
        }
        else
        {
            if (isJoined)
            {
                m_LineRenderer.startColor = m_JointAttachedColor;
                m_LineRenderer.endColor = m_JointAttachedColor;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;

        if (!isJoined)
        {
            if (m_DisjointTimeElapsed > m_DisjointTimeToKill)
            {
                m_PlayerController.InstantKill();
            }
            else
            {
                m_DisjointTimeElapsed += Time.fixedDeltaTime;
            }
            return;
        }

        // Direct tether towards player
        m_LineRenderer.SetPosition(0, transform.position);
        m_LineRenderer.SetPosition(1, m_PlayerController.transform.position);

        if (!m_Attacking)
        {
            // Detect target position behind the player
            int m_Orientation = m_PlayerController.m_FacingRight == true ? -1 : 1;
            Vector3 targetPosition = new Vector3(m_PlayerController.transform.position.x + companionOffset.x * m_Orientation, m_PlayerController.transform.position.y + companionOffset.y, 0);

            if (Vector3.Distance(transform.position, targetPosition) > m_TargetAcquisitionOffset)
            {
                // If far from target position, move towards it.
                LookTowards(m_Orientation);
                m_Rigidbody2D.velocity = Vector3.zero;
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_ReturnSpeed * Time.fixedDeltaTime);
            }
            else
            {
                // If player is moving, follow him after a short delay.
                if (Mathf.Abs(m_PlayerController.playerRigidbody2D.velocity.x) > m_MinPlayerSpeed)
                {
                    if (m_TimeToMoveElapsed > m_TimeToMove)
                    {
                        LookTowardsPlayer();
                        Move(m_PlayerController.playerRigidbody2D.velocity.x * 1.75f, m_PlayerController.playerRigidbody2D.velocity.y * 2);
                        m_TimeToMoveElapsed = 0;
                    }
                    else
                    {
                        m_TimeToMoveElapsed += Time.fixedDeltaTime;
                    }
                }
                else
                {
                    // Flip Towards player
                    LookTowardsPlayer();
                }
            }
        }

        if (m_Attacking)
        {
            //LookTowards(m_Orientation);
            m_Rigidbody2D.velocity = Vector3.one * 0.03f;
            transform.position = Vector2.MoveTowards(transform.position, m_EnemyPosition, m_ReturnSpeed * Time.fixedDeltaTime * 2);
        }
    }

    public void Move(float moveHorizontal, float moveVertical)
    {
        Vector3 targetVelocity = new Vector2(moveHorizontal, moveVertical);
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }

    private void LookTowardsPlayer()
    {
        Vector3 playerScale = m_PlayerController.transform.localScale;
        m_SpriteRenderer.transform.localScale = playerScale;
    }

    private void LookTowards(int orientation)
    {
        Vector3 theScale = m_SpriteRenderer.transform.localScale;
        theScale.x = orientation;
        m_SpriteRenderer.transform.localScale = theScale;
    }

    public void ChangeJointState(bool state)
    {
        if (state) m_DisjointTimeElapsed = 0f;

        // If disjointed enable gravity
        m_Rigidbody2D.simulated = !state;

        m_LineRenderer.enabled = state;
        isJoined = state;

        if (state)
        {
            m_SpriteRenderer.color = Color.white;
            m_LineRenderer.startColor = m_JointAttachedColor;
            m_LineRenderer.endColor = m_JointAttachedColor;
        }
        else
        {
            m_SpriteRenderer.color = m_InactiveColor;
            m_LineRenderer.SetPosition(0, Vector3.zero);
            m_LineRenderer.SetPosition(1, Vector3.zero);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Abyss"))
        {
            // Reset game
            m_PlayerController.InstantKill();
        }

        if (collision.CompareTag("Enemy") && m_Attacking)
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(attackDamage);
            Debug.Log("Panyon hit " + collision.gameObject.name);
            EndAttack();
        }
    }

    public void Attack(GameObject enemy)
    {
        if (isJoined)
        {
            m_Attacking = true;
            m_Rigidbody2D.simulated = true;
            m_AttackCoroutine = StartCoroutine(Attacking(attackMaxDuration));
            m_EnemyPosition = enemy.transform.position;
        }

    }

    public void Died()
    {
        isDead = true;
        ChangeJointState(false);
        if (m_AttackCoroutine != null)
            StopCoroutine(m_AttackCoroutine);
        m_Attacking = false;
    }

    private void EndAttack()
    {
        if (m_AttackCoroutine != null)
            StopCoroutine(m_AttackCoroutine);
        m_Attacking = false;
        m_Rigidbody2D.simulated = false;
    }

    IEnumerator Attacking(float attackDuration)
    {
        yield return new WaitForSeconds(attackDuration);
        EndAttack();
    }

    public void Reset()
    {
        isDead = false;
        ChangeJointState(true);
    }
}

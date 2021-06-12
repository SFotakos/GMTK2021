using UnityEngine;

public class CompanionController : MonoBehaviour
{

    private Rigidbody2D m_Rigidbody2D;
    [SerializeField] private Transform m_PlayerTransform;
    [SerializeField] private Rigidbody2D m_PlayerRigidbody2D;
    [SerializeField] private LayerMask m_PlayerMask;

    public float m_CompanionSpeed = 250f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;

    bool m_FacingRight = true;

    bool m_IsJoined = true;
    [SerializeField] Vector2 m_CompanionOffset = new Vector2(1.3f, 2.5f);
    [SerializeField] float m_ReturnSpeed = 3.3f;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (m_IsJoined)
        {
            if (Mathf.Abs(m_PlayerRigidbody2D.velocity.x) > 0.05f)
            {
                Move(m_PlayerRigidbody2D.velocity.x);
            } else {
                m_Rigidbody2D.velocity = Vector3.zero;
                int m_Orientation = m_FacingRight == true ? -1 : 1;
                Vector3 targetPosition = new Vector3(m_PlayerTransform.position.x + m_CompanionOffset.x * m_Orientation, m_CompanionOffset.y, 0);
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_ReturnSpeed * Time.fixedDeltaTime);
            }
        }
    }

    public void Move(float move)
    {
        Vector3 targetVelocity = new Vector2(move, m_Rigidbody2D.velocity.y);
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        if (move > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (move < 0 && m_FacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}

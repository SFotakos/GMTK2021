using UnityEngine;

public class CompanionController : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody2D;
    [SerializeField] private PlayerController m_PlayerController;
    [SerializeField] private LayerMask m_GroundMask;

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;

    public bool m_IsJoined = true;
    [SerializeField] float m_DisjointDistance = 5f;
    public Vector2 companionOffset = new Vector2(2.5f, 2.5f);
    [SerializeField] float m_TargetAcquisitionOffset = 0.3f;
    [SerializeField] float m_ReturnSpeed = 5f;

    float m_MinPlayerSpeed = 0.05f;
    float m_TimeToMoveElapsed;
    float m_TimeToMove = 0.55f;

    float m_TimeToDisjointElapsed;
    float m_TimeToDisjoint = 2f;

    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private LineRenderer m_LineRenderer;
    [SerializeField] Color m_InactiveColor;

    [SerializeField] Color m_JointAttachedColor;
    [SerializeField] Color m_JointDetachingColor;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float distance = m_LineRenderer.GetPosition(m_LineRenderer.positionCount-1).magnitude - m_LineRenderer.GetPosition(0).magnitude;
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
            if (m_IsJoined)
            {
                m_LineRenderer.startColor = m_JointDetachingColor;
                m_LineRenderer.endColor = m_JointDetachingColor;
            }
        } else
        {
            if (m_IsJoined)
            {
                m_LineRenderer.startColor = m_JointAttachedColor;
                m_LineRenderer.endColor = m_JointAttachedColor;
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_IsJoined)
        {
            // Direct tether towards player
            m_LineRenderer.SetPosition(0, transform.position);
            m_LineRenderer.SetPosition(1, m_PlayerController.transform.position);

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
                        Move(m_PlayerController.playerRigidbody2D.velocity.x * 2, m_PlayerController.playerRigidbody2D.velocity.y * 2);
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

                    // Sway
                    //if (m_ElapsedTime > m_TimeToMove)
                    //{
                    //    float swayValue = Random.Range(0.3f, 1f);
                    //    float swayOrientation = Random.Range(-1, 1);
                    //    float horizontalSway = (transform.position.x + swayValue) * swayOrientation;
                    //    float verticalSway = (transform.position.y + swayValue) * swayOrientation;
                    //    Move(horizontalSway * Time.fixedDeltaTime, verticalSway * Time.fixedDeltaTime);
                    //    m_ElapsedTime = 0;
                    //}
                    //else
                    //{
                    //    m_ElapsedTime += Time.fixedDeltaTime;
                    //}
                }
            }
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
        // If disjointed enable gravity
        m_Rigidbody2D.simulated = !state;

        m_LineRenderer.enabled = state;
        m_IsJoined = state;

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
            transform.position = Vector2.zero;
            ChangeJointState(false);
            m_PlayerController.transform.position = Vector2.zero + companionOffset;
            ChangeJointState(true);
        }
    }
}

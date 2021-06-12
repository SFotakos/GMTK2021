using UnityEngine;

public class CompanionController : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody2D;
    [SerializeField] private PlayerController m_PlayerController;
    [SerializeField] private LayerMask m_PlayerMask;

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;
    private Vector3 m_Velocity = Vector3.zero;

    private bool m_FacingRight = true;

    bool m_IsJoined = true;
    [SerializeField] Vector2 m_CompanionOffset = new Vector2(1.3f, 2.5f);
    [SerializeField] float m_TargetAcquisitionOffset = 0.5f;
    [SerializeField] float m_ReturnSpeed = 5f;
    
    float m_MinPlayerSpeed = 0.05f;
    float m_ElapsedTime;
    float m_TimeToMove = 0.55f;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (m_IsJoined)
        {
            // Detect target position behind the player
            int m_Orientation = m_PlayerController.m_FacingRight == true ? -1 : 1;
            Vector3 targetPosition = new Vector3(m_PlayerController.transform.position.x + m_CompanionOffset.x * m_Orientation, m_PlayerController.transform.position.y + m_CompanionOffset.y, 0);

            if (Vector3.Distance(transform.position, targetPosition) > m_TargetAcquisitionOffset * m_Orientation)
            {
                // If far from target position, move towards it.
                m_Rigidbody2D.velocity = Vector3.zero;
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, m_ReturnSpeed * Time.fixedDeltaTime);
            } else {
                // If player is moving, follow him after a short delay.
                if (Mathf.Abs(m_PlayerController.playerRigidbody2D.velocity.x) > m_MinPlayerSpeed)
                {
                    if (m_ElapsedTime > m_TimeToMove)
                    {
                        Move(m_PlayerController.playerRigidbody2D.velocity.x, m_PlayerController.playerRigidbody2D.velocity.y);
                        m_ElapsedTime = 0;
                    }
                    else
                    {
                        m_ElapsedTime += Time.fixedDeltaTime;
                    }
                } else
                {
                    // Flip Towards player
                    transform.localScale = m_PlayerController.transform.localScale;

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

        if (moveHorizontal > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (moveHorizontal < 0 && m_FacingRight)
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

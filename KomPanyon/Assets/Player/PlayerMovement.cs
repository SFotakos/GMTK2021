using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController m_Controller;
    public float m_PlayerSpeed = 350f;

    float m_HorizontalMovement = 0f;
    bool m_ShouldJump = false;
    bool m_ShouldDodge = false;

    void Update()
    {
        m_HorizontalMovement = Input.GetAxisRaw("Horizontal") * m_PlayerSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            m_ShouldJump = true;
        }

        if (Input.GetButtonDown("Dodge"))
        {
            m_ShouldDodge = true;
        }
    }

    private void FixedUpdate()
    {
        m_Controller.Move(m_HorizontalMovement * Time.fixedDeltaTime, m_ShouldDodge, m_ShouldJump);
        m_ShouldJump = false;
        m_ShouldDodge = false;
    }
}

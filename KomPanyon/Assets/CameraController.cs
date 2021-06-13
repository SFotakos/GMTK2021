using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private PlayerController m_PlayerController;
    [SerializeField] private Vector2 m_CameraOffset = new Vector2(0, 1.5f);
    
    // Update is called once per frame
    void FixedUpdate()
    {
        float targetY = m_PlayerController.transform.position.y < 0 ? m_CameraOffset.y : m_PlayerController.transform.position.y + m_CameraOffset.y;
        transform.position = new Vector3(
            (m_PlayerController.transform.position.x + m_CameraOffset.x) * 50f* Time.fixedDeltaTime,
            targetY * 50f * Time.fixedDeltaTime,
            transform.position.z);


    }
}

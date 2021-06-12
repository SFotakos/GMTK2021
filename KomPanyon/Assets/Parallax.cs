using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float m_Length, m_StartPos;
    [SerializeField] private GameObject m_Cam;
    [SerializeField] private float m_ParallaxEffect;

    void Start()
    {
        m_StartPos = transform.position.x;
        m_Length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float movedFromCamera = m_Cam.transform.position.x * (1- m_ParallaxEffect);
        float dist = m_Cam.transform.position.x * m_ParallaxEffect;
        transform.position = new Vector3(m_StartPos + dist, transform.position.y, transform.position.z);

        if (movedFromCamera > m_StartPos + m_Length)
            m_StartPos += m_Length;
        else if (movedFromCamera < m_StartPos - m_Length)
            m_StartPos -= m_Length;
    }
}

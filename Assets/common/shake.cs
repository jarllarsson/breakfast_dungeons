using UnityEngine;
using System.Collections;

public class shake : MonoBehaviour 
{
    public bool m_active = true;
    public float m_time;
    public bool m_easeOut;
    private float m_currentTime;
    public Vector2 m_phaseoffset;
    public Vector2 m_phasescale;
    public Vector2 m_posOffset;
    public Vector2 m_speed;
    public Vector2 m_speedMp;
    Vector3 startPos;
    // Use this for initialization
    void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (m_active)
        {
            float t = m_currentTime / m_time;
            float ease = 1.0f;
            if (m_easeOut) ease = 1.0f-t;
            transform.localPosition = new Vector3(startPos.x + m_posOffset.x + ease * m_phasescale.x * Mathf.Sin(t * m_speed.x * m_speedMp.x + m_phaseoffset.x),
                                                  startPos.y + m_posOffset.y + ease * m_phasescale.y * Mathf.Sin(t * m_speed.y * m_speedMp.y + m_phaseoffset.y), 
                                                  startPos.z);
            m_currentTime += Time.deltaTime;
            if (m_currentTime > m_time)
            {    
                m_active = false;
                m_currentTime = 0.0f;
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime);
        }
    }

    public void Activate(float p_time, Vector2 p_amplitude, Vector2 p_speed)
    {
        m_phasescale = p_amplitude;
        m_speed = p_speed;
        m_time = p_time;
        m_currentTime = 0.0f;
        startPos = transform.localPosition;
        m_active = true;
    }
}

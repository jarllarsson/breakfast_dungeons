using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour {
    private Vector3 m_goalPos, m_vel;
    public Vector3 m_startOffset;
    public float m_time;
	// Use this for initialization
	void Start () 
    {
        m_goalPos = transform.position;
        transform.position += m_startOffset;
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.position = Vector3.SmoothDamp(transform.position, m_goalPos, ref m_vel, m_time);
	}

    public void reset(Vector3 p_offsetForCurrent)
    {
        m_goalPos = transform.position+p_offsetForCurrent;
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class textChildAligner : MonoBehaviour 
{
    public Transform m_textHolder;
    public Text m_text;
    public float m_offset;
    public float m_speed;
    private Vector3 m_vel;
    private Vector3 m_startPos;
	// Use this for initialization
	void Start () {
        m_startPos = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 npos = m_startPos;
        if (m_text.enabled == true)
        {
            npos = new Vector3(transform.localPosition.x,
            m_textHolder.localPosition.y + m_offset, 
            transform.localPosition.z);
        }
        if (m_speed > 0.0f)
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition,
                                        npos,
                                        ref m_vel, m_speed);
        else
            transform.localPosition = npos;
	}
}

using UnityEngine;
using System.Collections;

public class ballFx : MonoBehaviour {
    public Transform m_ball, m_points;
    private float[] m_smoothVelocityList;
    public float m_smoothVelocity;
    private int m_currentIdx=0;
    public TrailRenderer m_trail;
    int m_colMatId;
	// Use this for initialization
	void Start () 
    {
        m_smoothVelocityList = new float[20];
        rigidbody2D.AddTorque(1.0f);
        m_colMatId=Shader.PropertyToID("_TintColor");
	}
	
	// Update is called once per frame
	void Update () {
        m_ball.transform.rotation = Quaternion.identity;

        float lineAlpha=0.0f;
        if (m_smoothVelocity > 10.0f)
        {
            lineAlpha = Mathf.Clamp01((m_smoothVelocity - 10.0f) * 0.05f);
        }
        else
            lineAlpha = 0.0f;
        Color oldCol = m_trail.materials[0].GetColor("_TintColor");
        m_trail.materials[0].SetColor("_TintColor", new Color(oldCol.r, oldCol.g, oldCol.b, lineAlpha));
	}

    void FixedUpdate()
    {
        m_smoothVelocityList[m_currentIdx] = rigidbody2D.velocity.magnitude;
        m_currentIdx++;
        if (m_currentIdx >= m_smoothVelocityList.Length)
            m_currentIdx = 0;
        m_smoothVelocity = 0.0f;
        foreach (float v in m_smoothVelocityList)
            m_smoothVelocity += v;
        m_smoothVelocity /= m_smoothVelocityList.Length;
    }
}

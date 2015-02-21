using UnityEngine;
using System.Collections;

public class controller : MonoBehaviour {
    public float m_movePower=1.0f;
    public float m_tiltMultiplier=1.0f, m_tiltMax=10.0f;
    public LineRenderer m_lineRenderer;
    public SpringJoint2D m_spring;
	// Use this for initialization
	void Start () {
	
	}
	
    void Update()
    {
        m_lineRenderer.SetPosition(0, transform.position);
        m_lineRenderer.SetPosition(1, m_spring.connectedBody.transform.position);
    }

	// Update is called once per frame
	void FixedUpdate () 
    {
        Vector2 dirInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 tilt=new Vector2(Input.acceleration.x, Input.acceleration.y);
        tilt *= Mathf.Clamp(m_tiltMultiplier * tilt.magnitude, 1.0f, m_tiltMultiplier);
        tilt.x = Mathf.Clamp(tilt.x, -m_tiltMax, m_tiltMax);
        tilt.y = Mathf.Clamp(tilt.y, -m_tiltMax, m_tiltMax);
        Debug.Log(tilt);
        dirInput += tilt;
        Debug.DrawLine(transform.position,transform.position + new Vector3(dirInput.x, dirInput.y, 0.0f),Color.white);
        float dirInputSqrMagnitude = dirInput.sqrMagnitude;
        rigidbody2D.AddForce(dirInput * m_movePower);
	}

}

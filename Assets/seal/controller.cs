using UnityEngine;
using System.Collections;

public class controller : MonoBehaviour {
    public float m_movePower=1.0f;
    public float m_tiltMultiplier=1.0f, m_tiltMax=10.0f;
    public LineRenderer m_lineRenderer;
    public SpringJoint2D m_spring;
    public Transform m_face;
    public Transform m_tail;
    public Transform m_springConnector;
    public swing m_paddle1, m_paddle2;
    private int m_dirAnimHash;
    public Animator m_faceAnimation;
    public float m_flipSpeed = 10.0f;
    private float m_flipT=1.0f;
    private float m_maxAbsVelMp = 1.0f;
	// Use this for initialization
	void Start () {
	    m_dirAnimHash=Animator.StringToHash("facing");
	}
	
    void Update()
    {
        Vector3 myLocalConnectorPoint = transform.InverseTransformPoint(m_springConnector.position);
        m_spring.anchor = new Vector2(myLocalConnectorPoint.x, myLocalConnectorPoint.y);
        m_lineRenderer.SetPosition(0, m_springConnector.position);
        m_lineRenderer.SetPosition(1, m_spring.connectedBody.transform.position);

        // anims
        float velMagnitude = rigidbody2D.velocity.magnitude;
        if (Mathf.Abs(velMagnitude) > m_maxAbsVelMp) m_maxAbsVelMp = Mathf.Abs(velMagnitude);
        if (velMagnitude>0.5f)
        {
            Vector2 vel=rigidbody2D.velocity;
            float dirSign = 1.0f;
            m_paddle1.m_speedMp = 1.0f + (velMagnitude - 0.5f) * 0.05f;
            m_paddle2.m_speedMp = 1.0f + (velMagnitude - 0.5f) * 0.05f;
            if (vel.y>2.0f)
                m_faceAnimation.SetInteger(m_dirAnimHash,1); // backface
            else
                m_faceAnimation.SetInteger(m_dirAnimHash,0); // frontface
            float speedMp = Mathf.Max(1.0f, m_maxAbsVelMp);
            if (vel.x > 0.0f)
            {
                m_flipT = Mathf.Lerp(m_flipT, -1.0f, -(m_flipT - 2.0f) * m_flipSpeed * speedMp * Time.deltaTime);
                m_face.localScale = new Vector3(blorp(m_flipT, -vel.x), 1.0f, 1.0f);
                dirSign = m_face.localScale.x;
            }
            else
            {
                m_flipT = Mathf.Lerp(m_flipT, 1.0f, (m_flipT + 2.0f) * m_flipSpeed * speedMp * Time.deltaTime);
                m_face.localScale = new Vector3(blorp(m_flipT, -vel.x), 1.0f, 1.0f);
            }
            //
            m_tail.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Rad2Deg * Mathf.Atan2(-vel.y, -vel.x * dirSign));
        }
        else
        {
            if (m_face.localScale.x<0.0f)
                m_face.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            else
                m_face.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        
    }

    float blorp(float p_t, float p_dir)
    {
        float half = 0.9f;
        float endhalf = 0.8f;
        float extra = 0.4f;
        if (p_dir > 0.0f && p_t > -half)
        {
            if (p_t < 0.9f)
                p_t = Mathf.Max(endhalf, p_t + extra);
            else if (p_t >= 0.9f)
                p_t = p_t + extra * (1.0f - p_t) * 10.0f;
            return p_t; 
        }
        if (p_dir <= 0.0f && p_t < half)
        {
            if (p_t > -0.9f)
                p_t = Mathf.Min(-endhalf, p_t - extra);
            else if (p_t <= -0.9f)
                p_t = p_t - extra * (1.0f + p_t) * 10.0f;
            return p_t; 
        }
        return p_t;
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
        m_maxAbsVelMp *= 0.95f;
	}

}

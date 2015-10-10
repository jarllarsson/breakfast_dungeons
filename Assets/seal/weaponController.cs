using UnityEngine;
using System.Collections;

public class weaponController : MonoBehaviour 
{

	public controller m_player;
	public Rigidbody2D m_rb;
    public float m_attackForce = 20.0f;
    public float m_attackTime = 0.0f;
    private float m_attackTimeLim = 1.0f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
        m_attackTime -= Time.deltaTime;
	}

	void FixedUpdate()
	{
		if (m_player.m_weaponActivate)
		{
            m_player.m_weaponActivate = false;
            m_attackTime = m_attackTimeLim;
			m_rb.isKinematic = false;
            m_rb.AddForce(m_player.GetMoveDir() * m_attackForce);
		}
		else if (m_attackTime <= 0.0f)
        {
            m_rb.isKinematic = true;
            transform.position = m_player.transform.position;
		}


        
	}
}

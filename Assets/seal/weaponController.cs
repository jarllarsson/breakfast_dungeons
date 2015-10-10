using UnityEngine;
using System.Collections;

public class weaponController : MonoBehaviour 
{

	public controller m_player;
	public Rigidbody2D m_rb;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void FixedUpdate()
	{
		if (m_player.m_isRushing)
		{
			m_rb.isKinematic = false;
		}
		else
		{
            m_rb.isKinematic = true;
            transform.position = m_player.transform.position;
		}
	}
}

using UnityEngine;
using System.Collections;

public class ballFx : MonoBehaviour {
    public Transform m_ball, m_points;
	// Use this for initialization
	void Start () 
    {
        rigidbody2D.AddTorque(1.0f);
	}
	
	// Update is called once per frame
	void Update () {
        m_ball.transform.rotation = Quaternion.identity;
	}
}

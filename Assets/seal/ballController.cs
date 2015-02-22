using UnityEngine;
using System.Collections;

public class ballController : MonoBehaviour {

    public float m_extrapowerMax;
    public float m_extrapowerInc;
    private float m_currentBoost;
    public float m_extrapowerApplyTime = 0.0f;
    public float m_extrapowerCooldownTime = 0.0f;
    public float m_extrapowerApplyTick = 0.0f;
    public float m_extrapowerCooldownTick = 1.0f;
    public float m_boostMagnitudeLim = 14.0f;

	// Use this for initialization
	void Start () {
        m_extrapowerCooldownTick = m_extrapowerCooldownTime;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(rigidbody2D.velocity.magnitude);
	}

    void FixedUpdate()
    {
        if (rigidbody2D.velocity.magnitude > m_boostMagnitudeLim)
        {
            if (m_extrapowerCooldownTick >= m_extrapowerCooldownTime &&
                m_extrapowerApplyTick > 0.0f)
            {
                m_currentBoost += m_extrapowerInc * Time.deltaTime;
                rigidbody2D.AddForce(rigidbody2D.velocity.normalized * m_currentBoost);
                m_extrapowerApplyTick -= Time.deltaTime;
            }
            if (m_extrapowerApplyTick<=0.0f)
            {
                m_currentBoost = 0.0f;
                m_extrapowerCooldownTick = 0.0f;
            }
        }
        else
        {
            m_extrapowerApplyTick = m_extrapowerApplyTime;
            m_extrapowerCooldownTick += Time.deltaTime;
            if (m_extrapowerCooldownTick >= m_extrapowerCooldownTime) m_extrapowerApplyTick = m_extrapowerApplyTime;
        }
    }
}

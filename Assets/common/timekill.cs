using UnityEngine;
using System.Collections;

public class timekill : MonoBehaviour {
    private float m_life = 0.0f;
    public float m_lifeLim = 1.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        m_life += Time.deltaTime;
        if (m_life >= m_lifeLim)
            Destroy(gameObject);
	}
}

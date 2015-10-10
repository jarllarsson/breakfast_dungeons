using UnityEngine;
using System.Collections;

public class deltaScore : MonoBehaviour 
{
    public TextMesh m_text;
    private bool m_run=false;
    private float m_ticker = 0.0f;
    private float m_tickerLim = 1.0f;
    float m_baseCharSz;
    private Vector3 m_startPos;
    public Animator m_fx;
	// Use this for initialization
	void Start () 
    {
        m_startPos = transform.localPosition;
        m_baseCharSz = m_text.characterSize;
        m_text.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        m_fx.enabled = false;
        if (!m_run)
            m_text.gameObject.GetComponent<Renderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (isRunning())
        {
            m_ticker += Time.deltaTime;
            if (m_ticker>=m_tickerLim)
            {
                m_run = false;
                m_fx.enabled = false;
                m_text.gameObject.GetComponent<Renderer>().enabled = false;
                m_ticker = 0.0f;
                m_text.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                transform.localPosition = m_startPos;
                m_text.transform.localPosition = Vector3.zero;
                m_text.characterSize = m_baseCharSz;
                m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, 1.0f);
            }
            else if (m_ticker>0.0f)
            {
                m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b,
                    1.0f-(m_ticker / m_tickerLim));
                m_text.characterSize = Mathf.Lerp(m_text.characterSize, m_baseCharSz, m_ticker / m_tickerLim);
            }
        }
	}

    public bool isRunning()
    {
        return m_run;
    }

    public void run(int m_score)
    {
        m_text.characterSize = m_baseCharSz * 4.0f;
        m_text.text = m_score<0?"-":"+" + m_score.ToString();
        m_ticker = -0.1f;
        m_run = true;
        m_text.gameObject.GetComponent<Renderer>().enabled = true;
        m_text.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        m_text.transform.localPosition = Vector3.zero;
        transform.localPosition = m_startPos + new Vector3(Random.Range(-0.25f, 0.5f), Random.Range(0.25f, 0.0f), 0.0f);
        m_text.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(0.0f, 1.0f), Random.Range(-1.0f, 0.5f)) * 250.0f);
        m_fx.enabled = true;
        m_fx.Play("score_effect",-1,0f);
    }

    public void run(int m_score,Vector3 p_pos)
    {
        m_text.characterSize = m_baseCharSz * 4.0f;
        m_text.text = m_score < 0 ? "-" : "+" + m_score.ToString();
        m_ticker = -0.1f;
        m_run = true;
        m_text.gameObject.GetComponent<Renderer>().enabled = true;
        m_text.gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        m_text.transform.localPosition = Vector3.zero;
        transform.position = p_pos + new Vector3(Random.Range(-0.25f, 0.5f), Random.Range(0.25f, 0.0f), 0.0f);
        m_text.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(0.0f, 1.0f), Random.Range(-1.0f, 0.5f)) * 250.0f);
        m_fx.enabled = true;
        m_fx.Play("score_effect", -1, 0f);
    }

}

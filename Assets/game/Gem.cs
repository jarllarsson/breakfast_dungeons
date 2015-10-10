using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour {
    public int m_val;
    public Sprite[] m_gemSprites;
    public SpriteRenderer m_renderer;
    public GameObject m_myself;
    private bool m_picked = false;
    public wobble m_wobble;
    private float m_endAnimTime=0.5f, m_endAnimTick=0.0f;
    private float m_startAnimTime = 0.5f, m_startAnimTick = 0.0f;
    private float scale=1.0f;
    float upV = 0.0f;
    public GameObject m_gemBoom;
	// Use this for initialization
	void Start () 
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-200.0f, 200.0f), Random.Range(-200.0f, 200.0f)));
        int maxVal=m_gemSprites.Length;
        if (m_val>maxVal)
        {
            GameObject gem;
            gem = Instantiate(m_myself, transform.position, Quaternion.identity) as GameObject;
            Gem gemscript = gem.GetComponent<Gem>();
            int newVal = Mathf.Max(1,m_val - m_gemSprites.Length);
            gemscript.m_val = Mathf.Max(1, newVal);
            m_val -= newVal;
        }
        if (m_val > 2)
            scale = 1.2f;
        else if (m_val>3)
            scale = 1.5f;
        transform.localScale = Vector3.one * scale;
        m_renderer.sprite = m_gemSprites[Mathf.Min(m_gemSprites.Length-1,m_val-1)];
	}
	
	// Update is called once per frame
	void Update () {
	    if (m_picked)
        {
            m_endAnimTick += Time.deltaTime;
            float t = m_endAnimTick / m_endAnimTime;
            upV += 10.0f * Time.deltaTime;
            m_renderer.transform.localPosition += Vector3.up * upV * Time.deltaTime;
            m_renderer.transform.localScale = new Vector3(Mathf.PingPong(t * upV, scale * 2.0f) - scale, Mathf.Lerp(scale, scale * 1.3f, t), scale);
            m_renderer.color = Color.Lerp(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), t*t );

            if (m_endAnimTick>m_endAnimTime)
            {
                Destroy(gameObject);
            }
        }
        else if (m_startAnimTick < m_startAnimTime)
        {
            m_startAnimTick += Time.deltaTime;
            float t = m_startAnimTick / m_startAnimTime;

            m_renderer.transform.localScale = new Vector3(Mathf.Lerp(scale*1.5f, scale, t), Mathf.Lerp(scale*1.5f, scale, t), scale);
            m_renderer.color = Color.Lerp(new Color(1.0f, 1.0f, 1.0f, 0.0f), Color.white, t * t);

            if (m_endAnimTick >= m_endAnimTime)
            {
                transform.localScale = Vector3.one * scale;
            }
        }
	}

    public int getVal()
    {
        return m_val;
    }

    public void pickup()
    {
        m_startAnimTick = m_startAnimTime;
        gameObject.tag = "pickedgem";
        m_wobble.enabled = false;
        Instantiate(m_gemBoom, transform.position, Quaternion.identity);
        m_picked = true;
        transform.localScale = Vector3.one * scale;
    }
}

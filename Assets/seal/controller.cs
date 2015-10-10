using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class controller : MonoBehaviour {
    public float m_movePower = 1.0f;
    public float m_rushPower = 2.0f;
    public Vector3 m_tiltMultiplier, m_tiltMax;
    public LineRenderer m_lineRenderer;
    public SpringJoint2D m_spring;
    public Transform m_face;
    public Transform m_tail;
    public Transform m_springConnector;
    public swing m_paddle1, m_paddle2;
    private int m_dirAnimHash, m_deadStateAnimHash;
    public Animator m_faceAnimation;
    public float m_flipSpeed = 10.0f;
    private float m_flipT=1.0f;
    private float m_maxAbsVelMp = 1.0f;
    Vector3 startTilt;
    Vector3[] startTiltList;
    int startYidx = 0;
    float calibrationTime = 2.0f;


    public bool m_isRushing = false;


    public Text dbgText;

    public Vector2 m_outsideHDist;
    private Vector3 m_startPos;
    public float m_normalDrag = 5.0f, m_outsideDrag = 30.0f, m_veryOutsideDrag=35.0f;
	// Use this for initialization
    public bool m_handleInput=true;
    public SpriteRenderer m_spriteRendererHead;
    public Sprite m_deadHeadSprite;

    public Transform m_gameOverText;
    public ParticleSystem m_heart;

    public bool m_trigDeath;
    private bool m_dead;

    public int m_hp = 2;
    public wobble m_hpWobbler;
    public float m_invulnTime = 2.0f;
    private float m_invulnTick;

    public GameObject m_hitFx, m_gemFx;
    private static shake m_camShake;
    public AudioSource m_soundSource;
    public AudioClip m_hurtsound;
    public AudioClip[] m_barksounds;

    public SpriteRenderer m_ballSprite;
    private int m_ballComboCount;
    public float m_ballComboCoolDownTime;
    private float m_ballComboCooldownTick;
    public Image m_healthUI;
    public Sprite[] m_healthUISprites;
    public bool m_weaponActivate;
	void Start () {
	    m_dirAnimHash=Animator.StringToHash("facing");
        m_deadStateAnimHash = Animator.StringToHash("dead");
        startTilt=Input.acceleration;
        startTiltList = new Vector3[10];
        m_startPos = transform.position;
        if (m_camShake == null) m_camShake = GameObject.FindGameObjectWithTag("camshaker").GetComponent<shake>();
	}

    public void registerEnemyHurtbyBall()
    {
        if (m_ballComboCooldownTick > 0.0f)
            m_ballComboCount++;
        m_ballComboCooldownTick = m_ballComboCoolDownTime;
        StartCoroutine("happyEffect");
    }

    public int getBallCombo()
    {
        return m_ballComboCount;
    }
	
    IEnumerator happyEffect()
    {
        yield return new WaitForSeconds(0.6f);
        m_hp++;
        if (m_hp > 2) m_hp = 2;
        updateHealthUI();
        if (!m_soundSource.isPlaying)
        {
            m_soundSource.PlayOneShot(m_barksounds[Random.Range(0, m_barksounds.Length)]);
        }
        m_heart.Play();
    }

    void updateHealthUI()
    {
        int imgIdx=m_hp-1;
        if (imgIdx >= 0 && imgIdx < m_healthUISprites.Length)
        {
            m_healthUI.sprite = m_healthUISprites[m_hp - 1];
            if (m_hp == 2)
                m_hpWobbler.m_speedMp = 1.0f;
            else
                m_hpWobbler.m_speedMp = 4.0f;
            m_healthUI.enabled = true;
        }
        else if (imgIdx < 0)
            m_healthUI.enabled = false;
    }

    void Update()
    {
        if (m_trigDeath) kill();
        if (m_ballComboCooldownTick>0.0f)
        {
            m_ballComboCooldownTick -= Time.deltaTime;
            m_ballSprite.color = Color.Lerp(Color.Lerp(Color.yellow,Color.magenta,Mathf.Clamp01((float)m_ballComboCount/3.0f)),
                                            Color.white,1.0f-(m_ballComboCooldownTick/m_ballComboCoolDownTime));
        }
        else
        {
            if (m_ballComboCount > 0) m_ballSprite.color = Color.white;
            m_ballComboCount = 0;         
        }

        Vector3 myLocalConnectorPoint = transform.InverseTransformPoint(m_springConnector.position);
        m_spring.anchor = new Vector2(myLocalConnectorPoint.x, myLocalConnectorPoint.y);
        m_lineRenderer.SetPosition(0, m_springConnector.position);
        m_lineRenderer.SetPosition(1, m_spring.connectedBody.transform.position);
        float dist = (m_springConnector.position - m_spring.connectedBody.transform.position).magnitude;
        m_lineRenderer.materials[0].mainTextureScale = new Vector2(dist/4.0f, 1.0f);
        if (!m_dead)
        {
            // anims
            Vector2 vel = GetComponent<Rigidbody2D>().velocity;
            float velMagnitude = vel.magnitude;
            if (Mathf.Abs(velMagnitude) > m_maxAbsVelMp) m_maxAbsVelMp = Mathf.Abs(velMagnitude);
            float dirSign = 1.0f;
            if (velMagnitude > 5.0f)
            {
                m_paddle1.m_speedMp = 1.0f + (velMagnitude - 0.5f) * 0.05f;
                m_paddle2.m_speedMp = 1.0f + (velMagnitude - 0.5f) * 0.05f;
                if (vel.y > 2.0f)
                    m_faceAnimation.SetInteger(m_dirAnimHash, 1); // backface
                else
                    m_faceAnimation.SetInteger(m_dirAnimHash, 0); // frontface
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
            }
            else
            {
                if (m_face.localScale.x < 0.0f)
                    m_face.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                else
                    m_face.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                dirSign = m_face.localScale.x;
            }
            if (velMagnitude > 0.5f)
                m_tail.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Rad2Deg * Mathf.Atan2(-vel.y, -vel.x * dirSign));

            // if outside
            Vector3 deltaPosFromStart = transform.position - m_startPos;
            if (deltaPosFromStart.x > -m_outsideHDist.x && deltaPosFromStart.x < m_outsideHDist.x &&
                deltaPosFromStart.y > -m_outsideHDist.y && deltaPosFromStart.y < m_outsideHDist.y)
            {
                GetComponent<Rigidbody2D>().drag = m_normalDrag;
            }
            else
            {
                if (deltaPosFromStart.x > -m_outsideHDist.x - 8.0f && deltaPosFromStart.x < m_outsideHDist.x + 8.0f &&
                deltaPosFromStart.y > -m_outsideHDist.y - 8.0f && deltaPosFromStart.y < m_outsideHDist.y + 8.0f)
                {
                    GetComponent<Rigidbody2D>().drag = m_outsideDrag;
                }
                else
                    GetComponent<Rigidbody2D>().drag = m_veryOutsideDrag;
            }
        }

        handleInvuln();
    }

    void handleInvuln()
    {
        if (m_invulnTick > 0.0f)
        {
            if (m_invulnTick < m_invulnTime * 0.5f && m_invulnTick > m_invulnTime * 0.48f && !m_soundSource.isPlaying)
                m_soundSource.PlayOneShot(m_hurtsound);
            m_face.GetComponent<Renderer>().enabled = !m_face.GetComponent<Renderer>().enabled;
            m_paddle1.GetComponent<Renderer>().enabled = !m_paddle1.GetComponent<Renderer>().enabled;
            m_paddle2.GetComponent<Renderer>().enabled = !m_paddle2.GetComponent<Renderer>().enabled;
            m_tail.GetComponent<Renderer>().enabled = !m_tail.GetComponent<Renderer>().enabled;
        }
        else if (m_invulnTick>-10000.0f)
        {
            m_face.GetComponent<Renderer>().enabled = true;
            m_paddle1.GetComponent<Renderer>().enabled = true;
            m_paddle2.GetComponent<Renderer>().enabled = true;
            m_tail.GetComponent<Renderer>().enabled = true;
            m_invulnTick = -20000.0f;
        }

        m_invulnTick -= Time.deltaTime;
    }

    IEnumerator kill()
    {
        Debug.Log("die1");
        yield return new WaitForSeconds(2.0f);
        Debug.Log("die2");
        if (!m_dead && m_hp<=0)
        {
            m_handleInput = false;
            m_spriteRendererHead.sprite = m_deadHeadSprite;
            m_faceAnimation.SetBool(m_deadStateAnimHash, true);
            m_paddle1.enabled = false;
            m_paddle2.enabled = false;
            m_dead = true;
            GameObject goText = Instantiate(m_gameOverText, transform.position+new Vector3(0.0f,0.0f,10.0f), Quaternion.identity) as GameObject;
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

        Vector2 dirInput = Vector2.zero;
        string ori;
        if (Input.deviceOrientation == DeviceOrientation.FaceUp)
            ori = "faceup";
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
            ori = "LLeft";
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
            ori = "LRight";
        else
            ori = "other";
#if (!UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID))
        Vector3 origTilt=new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z)-startTilt;
        Vector3 tilt = origTilt;
        tilt.x *= Mathf.Clamp(m_tiltMultiplier.x * tilt.magnitude, 1.0f, m_tiltMultiplier.x);
        tilt.y *= Mathf.Clamp(m_tiltMultiplier.y * tilt.magnitude, 1.0f, m_tiltMultiplier.y);
        tilt.z *= Mathf.Clamp(m_tiltMultiplier.z * tilt.magnitude, 1.0f, m_tiltMultiplier.z);
        tilt.x = Mathf.Clamp(tilt.x, -m_tiltMax.x, m_tiltMax.y);
        tilt.y = Mathf.Clamp(tilt.y, -m_tiltMax.y, m_tiltMax.y);
        tilt.z = Mathf.Clamp(tilt.z, -m_tiltMax.z, m_tiltMax.z);
        Debug.Log(tilt);
        dbgText.text="x: "+origTilt.x+" y: "+origTilt.y+" z: "+origTilt.z+" "+ori;
        dirInput = new Vector2(tilt.x,tilt.y);
#else
        dirInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#endif        

        if (Input.GetAxis("Jump") > 0.0f)
            m_isRushing = true;
        else
        {
            if (m_isRushing)
                m_weaponActivate = true;
            m_isRushing = false;

        }
        if (m_handleInput)
        {
            Debug.DrawLine(transform.position, transform.position + new Vector3(dirInput.x, dirInput.y, 0.0f), Color.white);
            float dirInputSqrMagnitude = dirInput.sqrMagnitude;
            //GetComponent<Rigidbody2D>().AddForce(dirInput * m_movePower);
            if (!m_isRushing)
                GetComponent<Rigidbody2D>().velocity = dirInput * m_movePower;
            else
                GetComponent<Rigidbody2D>().velocity = dirInput * m_rushPower;
            m_maxAbsVelMp *= 0.95f;
        }

	}

    void OnTriggerEnter2D(Collider2D p_other)
    {
        bool res = HandlePain(p_other);
        if (!res) PickupGem(p_other);
    }

    void OnTriggerStay2D(Collider2D p_other)
    {
        bool res = HandlePain(p_other);
        if (!res) PickupGem(p_other);
    }

    public bool isDead()
    {
        return m_dead;
    }

    public Vector2 GetMoveDir()
    {
        return GetComponent<Rigidbody2D>().velocity.normalized;
    }

    bool HandlePain(Collider2D p_other)
    {
        Debug.Log(p_other.gameObject.tag);
        if (p_other.gameObject.tag == "playerHurt" && m_invulnTick<=0.0f && !m_dead)
        {
            m_hp--; 
            if (m_hp <= 0)
                StartCoroutine("kill");
            updateHealthUI();
            Instantiate(m_hitFx, transform.position, Quaternion.identity);
            m_invulnTick = m_invulnTime;
            Vector3 hitDir = (transform.position - p_other.transform.position).normalized;
            GetComponent<Rigidbody2D>().AddForce(hitDir * 20000.0f);
            if (m_camShake) m_camShake.Activate(1.0f, hitDir * 5.0f, new Vector2(10.0f, 20.0f));
            return true;
        }
        return false;
    }

    bool PickupGem(Collider2D p_other)
    {
        Debug.Log(p_other.gameObject.tag);
        if (p_other.gameObject.tag == "gem" && !m_dead)
        {
            Gem gem = p_other.gameObject.GetComponent<Gem>();
            gem.pickup();
            ScoreSystem.add(gem.getVal());
            Instantiate(m_gemFx, transform.position, Quaternion.identity);
            return true;
        }
        return false;
    }
}

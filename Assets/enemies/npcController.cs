using UnityEngine;
using System.Collections;

public class npcController : MonoBehaviour 
{
    public static System.Random rnd = new System.Random();

    public enum DeathCause
    {
        SMASHED,
        ZAPPED, // unused
        CRUSH, // unused
        BURNED // unused
    }

    public float m_moveSpeed=100.0f;
    public bool m_move=true;
    public Transform m_dirFlipObj;
    public float m_velocityOnBallToKillMe = 10.0f;
    public float m_atkTime = 0.3f, m_atkTimeTick;

    public float m_chaseDistance, m_attackDistance;

    public Animator m_npcBaseAnim;
    private int m_animActionState;
    float m_stillTickLim = 1.0f;
    float m_stillTick;
    private int m_score = 1;
    public ParticleSystem m_bloodFx, m_bloodFxZap;
    private float m_deathClock = 0.0f;
    private float m_deathClockStart;
    public bool m_dying = false;
    private Vector2 m_dir;
    public SpriteRenderer m_spriteBody;
    //
    public static int m_totalCurrent = 0;
    public static int m_totalMax = 10;

    public Transform m_gib;
    public int m_gibsMax, m_gibsMin;
    public bool m_activeDirectionChange=true;
    public static bool m_playerIsDead;
    public static controller m_player;
    private float m_origMoveSpeed;

    public float m_walkDirChangeRate = 0.1f, m_chaseUpdateRate = 0.9f;


    private bool m_isMoving = false;
    private float m_distanceToPlayer;

	// Use this for initialization
    private float m_ownDt = 0.0f;

    public static bool m_firstBlood = false;
    private static shake m_camShake;

    public bool m_onFire;
    private float m_burnClock;
    public Sprite m_burnFace;
    public Transform m_burnParticle;
    public float m_velocityAffectOnAnimSpd=0.4f, m_animSpdMax=3.0f;


    public AudioSource m_soundSource;
    public AudioClip[] m_hitSounds;
    public GameObject m_hitSparks; 
    private bool m_isAttacking;
    public float m_attackCooldown=4.0f;
    private float m_attackCooldownTick;
    public Collider2D m_playerHurtArea;

    public GameObject m_atkSmashFx;
    public GameObject m_deathGem;
    private int m_comboOnHit = 0;

    public void disableAtk()
    {
        m_isAttacking = false;
        m_npcBaseAnim.SetInteger(m_animActionState, 1);
        m_attackCooldownTick = m_attackCooldown;
        m_playerHurtArea.enabled = false;
    }

	void Start () 
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        m_dir = new Vector2(1.0f, 1.0f);
        m_origMoveSpeed = m_moveSpeed;
        if (m_player == null) m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<controller>();
        if (m_camShake == null) m_camShake = GameObject.FindGameObjectWithTag("camshaker").GetComponent<shake>();

        m_animActionState = Animator.StringToHash("npcActionState");
        //
        m_totalCurrent++;
        m_playerHurtArea.enabled = false;
	}

    void OnDestroy()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }


    void Update()
    {
        m_ownDt += Time.deltaTime;
        if (m_ownDt > 0.1f && !m_isAttacking)
        {
            float velocity = GetComponent<Rigidbody2D>().velocity.magnitude;
            if (m_onFire && !m_dying)
            {
                m_burnClock -= m_ownDt;
                if (m_burnClock < 0.0f)
                    kill(DeathCause.BURNED, 0.1f);
            }
            m_isMoving = false;
            if (m_dying) // enemy is dying state
            {
                m_deathClock -= m_ownDt;
                if (m_deathClockStart > 0.0f)
                {
                    float t = 1.0f - m_deathClock / m_deathClockStart;
                    m_spriteBody.color = Color.Lerp(Color.white, Color.red, t);
                    transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.8f, 1.0f, 1.0f), t);
                    transform.position += new Vector3(Random.Range(-0.5f, 0.5f) * (1.0f + t), Random.Range(-0.05f, 0.05f), 0.0f);
                }
                if (m_deathClock < 0.0f)
                {
                    Destroy(gameObject);
                    int gibs = Random.Range(m_gibsMin, m_gibsMax);
                    for (int i = 0; i < gibs; i++)
                    {
                        if (m_gib)
                        {
                            Transform gib = Instantiate(m_gib, transform.position + new Vector3(((float)i - ((float)gibs * 0.5f)) * 0.1f, 1.0f, 0.0f), Quaternion.identity) as Transform;
                            gib.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-200.0f, 200.0f), Random.Range(200.0f, 700.0f)));
                            gib.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-20.0f, 20.0f));
                        }
                    }
                }

            }
            else     // enemy is alive state
            {
                // Control
                if (m_playerIsDead && !m_onFire) // player is dead
                {
                    if (Mathf.Abs(m_player.transform.position.x - transform.position.x) < 10.0f)
                    {
                        // Here we can do stuff to the player when it is dead
                    }
                }
                if (m_activeDirectionChange) // player is alive and enemy is allowed to change its direction
                {
                    m_distanceToPlayer = (transform.position - m_player.transform.position).magnitude;
                    int rmax = 1000;
                    int r = rnd.Next(0, rmax);
                    // if enemy is outside its chase distance
                    if (/*!m_playerIsDead && */m_distanceToPlayer > m_chaseDistance)
                    {
                        if (r < (int)((float)rmax * m_walkDirChangeRate))
                        {
                            if (m_onFire)
                                setDirToRandom(5.0f,0.1f); // frantic fire run
                            else
                                setDirToRandom(1.0f); // normal direction change
                        }
                    }
                    else // if player is alive and enemy is outside its chase distance
                    {
                        if (r < (int)((float)rmax * m_chaseUpdateRate))
                        {
                            setDirToPlayer(1.0f);
                        }
                    }
                }
                if ((!m_activeDirectionChange || (m_activeDirectionChange && m_stillTick <= 0.0f)) && // if not active dir change, or active dir change and able to move
                    m_move)
                {
                    m_dirFlipObj.localScale = new Vector3(m_dir.x < 0.0f ? -1.0f : 1.0f, 1.0f, 1.0f);
                    m_isMoving = true;
                }
                m_stillTick -= m_ownDt;

                m_npcBaseAnim.speed = Mathf.Min(m_animSpdMax,velocity*m_velocityAffectOnAnimSpd);

                if (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > 0.01f)
                    m_npcBaseAnim.SetInteger(m_animActionState, 1); // walk
                else
                    m_npcBaseAnim.SetInteger(m_animActionState, 0); // idle

                m_attackCooldownTick -= m_ownDt;
                if (m_attackCooldownTick<=0.0f) tryAttack();

            }
            m_ownDt = 0.0f;
        }
        if (m_isAttacking)
        {
            if (m_atkTimeTick >= m_atkTime)
            {
                disableAtk();
            }
            m_atkTimeTick += Time.deltaTime;
        }
        Debug.DrawLine(transform.position, transform.position + new Vector3(m_dir.x, m_dir.y, 0.0f), Color.red);
    }

    void setDirToRandom(float m_speedMp, float m_coolDownFac=1.0f)
    {
        m_dir = new Vector2(Random.Range(-1.0f,1.0f),Random.Range(-1.0f,1.0f));
        m_dir.Normalize();
        m_moveSpeed = m_speedMp*m_origMoveSpeed;
        m_stillTick = m_coolDownFac*m_stillTickLim;
    }
    void setDirToPlayer(float m_speedMp)
    {
        m_moveSpeed = m_origMoveSpeed * m_speedMp;
        m_dir.x = (m_player.transform.position.x + Random.Range(-2.0f, 2.0f) - transform.position.x);
        m_dir.y = (m_player.transform.position.y + Random.Range(-2.0f, 2.0f) - transform.position.y);
        m_dir.Normalize();
        //m_stillTick = m_stillTickLim * 0.5f;
    }

    void tryAttack()
    {
        if (m_distanceToPlayer<=m_attackDistance && !m_player.isDead())
        {
            m_npcBaseAnim.SetInteger(m_animActionState, 2);
            //m_stillTick = m_stillTickLim * 0.5f;
            m_isAttacking = true;
            m_atkTimeTick = 0.0f;
            m_isMoving = false;
            m_npcBaseAnim.speed = 1.0f;
            m_playerHurtArea.enabled = true;
            Instantiate(m_atkSmashFx, m_playerHurtArea.transform.position, Quaternion.identity);
        }
    }

	// Update is called once per frame
	void FixedUpdate () 
    {
        if (m_isMoving && !m_isAttacking) GetComponent<Rigidbody2D>().AddForce(m_dir * m_moveSpeed); // move
	}

    public void kill(DeathCause p_cause, float p_deathClock = 0.0f)
    {
        if (!m_dying)
        {
            //ScoreSystem.add(m_score);                                 SCORE
            m_deathClock = p_deathClock;
            m_deathClockStart = p_deathClock;
            m_totalCurrent--;
            m_firstBlood = true;
            if (p_cause != DeathCause.SMASHED)
            {
                if (p_cause != DeathCause.ZAPPED)
                {
                    if (m_bloodFx) 
                        Instantiate(m_bloodFx, transform.position + Vector3.up * GetComponent<Collider2D>().bounds.extents.y * 0.5f, Quaternion.identity);
                }
                else
                {   
                    if (m_bloodFxZap) 
                        Instantiate(m_bloodFxZap, transform.position + Vector3.up * GetComponent<Collider2D>().bounds.extents.y * 0.5f, Quaternion.identity);
                }
            }
            m_dying = true;
        }
    }


    public void silentKillWithBlood(float p_deathClock = 0.0f)
    {
        if (!m_dying)
        {
            m_deathClock = p_deathClock;
            m_deathClockStart = p_deathClock;
            m_totalCurrent--;
            Instantiate(m_bloodFx, transform.position + Vector3.up * GetComponent<Collider2D>().bounds.extents.y * 0.5f, Quaternion.identity);
            m_dying = true;
        }
    }
    // just remove
    public void silentKill()
    {
        m_totalCurrent--;
        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        OnColl2D(coll.collider, coll);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        OnColl2D(coll.collider, coll);
    }

    void OnColl2D(Collider2D coll, Collision2D collision = null)
    {
        //Debug.Log(coll.gameObject.tag);
        if (coll.gameObject.tag == "ball" && !m_dying && !m_player.isDead())
        {


            ballFx ballScript = coll.gameObject.GetComponent<ballFx>();
            float velocity = 0.0f;
            if (ballScript)
                velocity = ballScript.m_smoothVelocity;
            else
                velocity = coll.GetComponent<Rigidbody2D>().velocity.magnitude;

            if (velocity >= m_velocityOnBallToKillMe)
            {
                Vector2 pos=collision.contacts[0].point;
                GetComponent<Rigidbody2D>().fixedAngle = false;
                GetComponent<Rigidbody2D>().AddForceAtPosition(coll.GetComponent<Rigidbody2D>().velocity.normalized * velocity * 2000.0f, pos);
                if (m_hitSparks) Instantiate(m_hitSparks, new Vector3(pos.x,pos.y,transform.position.z), Quaternion.identity);
                if (!m_soundSource.isPlaying) m_soundSource.PlayOneShot(m_hitSounds[Random.Range(0, m_hitSounds.Length)]);
                if (m_player) m_player.registerEnemyHurtbyBall();
                if (m_player) m_comboOnHit = m_player.getBallCombo();
                GameObject gem;
                gem = Instantiate(m_deathGem, transform.position, Quaternion.identity) as GameObject;
                Gem gemscript = gem.GetComponent<Gem>();
                gemscript.m_val = Mathf.Max(1, m_comboOnHit);
                kill(DeathCause.SMASHED, 0.5f);               
                if (m_camShake) m_camShake.Activate(0.5f, coll.GetComponent<Rigidbody2D>().velocity.normalized * velocity * 0.1f, new Vector2(velocity, velocity));
            }
        }
        else if (coll.gameObject.tag == "obstruction" && coll.GetComponent<Rigidbody2D>().velocity.y<-0.5f)
        {
            kill(DeathCause.CRUSH);
        }
        else if (coll.gameObject.tag == "firebomb")
        {
            //firebomb firebombScript = coll.gameObject.GetComponent<firebomb>();
            //coll.gameObject.tag = "Untagged";
            //firebombScript.explode();
        }
    }


    public void ignite()
    {
        if (!m_onFire)
        {
            m_burnClock = Random.Range(8.0f, 10.0f);
            m_onFire = true;
            m_spriteBody.color = Color.red;
            Transform brn = Instantiate(m_burnParticle, transform.position, Quaternion.Euler(new Vector3(-90.0f, 0.0f, 0.0f))) as Transform;
            brn.parent = transform;
        }
    }


}

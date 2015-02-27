using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreRenderer : MonoBehaviour 
{
    public Text m_scoreText, m_comboTextGem, m_comboTextBall;
    public Image m_comboBall, m_comboGem;
    public deltaScore[] m_scoreFx;
    public AudioSource m_audioSource;
    public AudioClip[] m_scoreSound;
    public AudioClip m_comboSound;
    public float m_cooldownticklim = 1.0f;
    private float m_cooldowntick;
    public Color m_activeCol, m_inactiveCol;
    private int m_oldScore = 0;
    private Vector3 m_txtSzDefault;
    private int m_comboCount;
    private int m_soundCount;
    private int m_currentScoreFx = 0;
    bool comboSnd = false;
    public controller m_player;
    int m_oldBallCombo = -1;
    public Transform m_leftTopEdge3d;

	// Use this for initialization
	void Start () {
        m_txtSzDefault = m_scoreText.rectTransform.localScale;
        m_comboTextBall.enabled = false;
        m_comboBall.enabled = false;
        m_comboTextGem.enabled = false;
        m_comboGem.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        int score = ScoreSystem.getScore();
        if (score!=m_oldScore)
        {
            if (score-m_oldScore>0) // only add positive change to combo
            {
                if (m_cooldowntick > 0.0f || (!comboSnd && m_comboCount == 0 && score - m_oldScore > 1))
                {
                    if (m_soundCount < m_scoreSound.Length - 1)
                        m_soundCount++;
                    m_comboCount += score - m_oldScore;
                }
                if (!comboSnd) { m_audioSource.Stop(); m_audioSource.PlayOneShot(m_scoreSound[m_soundCount]); }
                comboSnd = false;
                m_cooldowntick = m_cooldownticklim;
            }

            string scorestr = score.ToString();
            m_scoreText.text = scorestr;
            if (m_comboCount > 0)
            {
                m_comboTextGem.text = "Gem +" + m_comboCount;
                m_comboTextGem.enabled = true;
                m_comboGem.enabled = true;
            }
            else
            {
                m_comboTextGem.enabled = false;
                m_comboGem.enabled = false;
            }


            for (int i = 0; i < m_scoreFx.Length;i++ )
            {
                int fxidx = (m_currentScoreFx + i)%m_scoreFx.Length;
                if (!m_scoreFx[fxidx].isRunning())
                {

                    m_scoreFx[fxidx].run(score - m_oldScore);
                    m_currentScoreFx++;
                    if (m_currentScoreFx >= m_scoreFx.Length) m_currentScoreFx = 0;
                    break;
                }
            }
        }

        int ballCombo = m_player.getBallCombo();
        if (m_oldBallCombo != ballCombo)
        {
            m_oldBallCombo = ballCombo;
            if (ballCombo > 0)
            {
                m_comboTextBall.text = "Hit +" + ballCombo;
                m_comboTextBall.enabled = true;
                m_comboBall.enabled = true;
            }
            else
            {
                m_comboTextBall.enabled = false;
                m_comboBall.enabled = false;
            }
        }

        if (m_cooldowntick > 0.0f)
            m_cooldowntick -= Time.deltaTime;
        else
        {
            if (m_comboCount > 0)
            {
                m_scoreText.text = score.ToString();
                ScoreSystem.add(m_comboCount);
                score = ScoreSystem.getScore();
                m_audioSource.PlayOneShot(m_comboSound);

                for (int i = 0; i < m_scoreFx.Length; i++)
                {
                    int fxidx = (m_currentScoreFx + i) % m_scoreFx.Length;
                    if (!m_scoreFx[fxidx].isRunning())
                    {
                        m_scoreFx[fxidx].run(m_comboCount, m_leftTopEdge3d.position);
                        m_currentScoreFx++;
                        if (m_currentScoreFx >= m_scoreFx.Length) m_currentScoreFx = 0;
                        break;
                    }
                }
                m_comboTextGem.enabled = false;
                m_comboGem.enabled = false;
                m_comboCount = 0;
                comboSnd = true;
            }
            m_soundCount = 0;
            m_cooldowntick = 0.0f;
        }
        float t=m_cooldowntick/m_cooldownticklim;
        m_scoreText.rectTransform.localScale = m_txtSzDefault * (1.0f + Mathf.PingPong(0.4f*t * m_cooldowntick * m_cooldowntick, 0.25f));
        m_scoreText.color = Color.Lerp(m_inactiveCol, m_activeCol, t);
        m_oldScore = score;
	}
}

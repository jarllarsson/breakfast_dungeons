using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreRenderer : MonoBehaviour 
{
    public Text m_scoreText, m_comboText;
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
	// Use this for initialization
	void Start () {
        m_txtSzDefault = m_scoreText.rectTransform.localScale;
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
                m_comboText.text = "Combo: " + m_comboCount;
                m_comboText.enabled = true;
            }
            else
            {
                m_comboText.enabled = false;
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
        if (m_cooldowntick > 0.0f)
            m_cooldowntick -= Time.deltaTime;
        else
        {
            if (m_comboCount > 0)
            {
                m_scoreText.text = score.ToString();
                ScoreSystem.add(m_comboCount);
                m_audioSource.PlayOneShot(m_comboSound);
                m_comboCount = 0;
                comboSnd = true;
            }
            m_soundCount = 0;
            m_cooldowntick = 0.0f;
        }
        float t=m_cooldowntick/m_cooldownticklim;
        m_scoreText.rectTransform.localScale = m_txtSzDefault * (1.0f + Mathf.PingPong(t * m_cooldowntick * m_cooldowntick, 0.25f));
        m_scoreText.color = Color.Lerp(m_inactiveCol, m_activeCol, t);
        m_oldScore = score;
	}
}

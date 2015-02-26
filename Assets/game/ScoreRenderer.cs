using UnityEngine;
using System.Collections;

public class ScoreRenderer : MonoBehaviour 
{
    public TextMesh[] m_scoreText;
    public deltaScore[] m_scoreFx;
    public AudioSource m_audioSource;
    public AudioClip[] m_scoreSound;
    public AudioClip m_comboSound;
    public float m_cooldownticklim = 1.0f;
    private float m_cooldowntick;
    public Color m_activeCol, m_inactiveCol;
    private int m_oldScore = 0;
    private float m_txtSzDefault;
    private int m_comboCount;
    private int m_soundCount;
    bool comboSnd = false;
	// Use this for initialization
	void Start () {
        m_txtSzDefault = m_scoreText[0].characterSize;
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
            foreach (TextMesh txt in m_scoreText)
            {
                txt.text = scorestr;
                if (m_comboCount > 0)
                {
                    txt.text += " (:" + m_comboCount + ")";
                }
            }
            
            foreach(deltaScore dscore in m_scoreFx)
            {
                if (!dscore.isRunning())
                {
                    dscore.run(score-m_oldScore);
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
                foreach (TextMesh txt in m_scoreText)
                    txt.text = score.ToString();
                ScoreSystem.add(m_comboCount);
                m_audioSource.PlayOneShot(m_comboSound);
                m_comboCount = 0;
                comboSnd = true;
            }
            m_soundCount = 0;
            m_cooldowntick = 0.0f;
        }
        float t=m_cooldowntick/m_cooldownticklim;
        foreach (TextMesh txt in m_scoreText)
            txt.characterSize = m_txtSzDefault * (1.0f + Mathf.PingPong(t * m_cooldowntick * m_cooldowntick, 0.5f));
        m_scoreText[0].color = Color.Lerp(m_inactiveCol, m_activeCol, t);
        m_oldScore = score;
	}
}

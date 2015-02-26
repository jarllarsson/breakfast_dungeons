using UnityEngine;
using System.Collections;

public class ScoreSystem : MonoBehaviour 
{
    public int debugscore = 0;
    private static int m_score=0;
	// Use this for initialization
	void Start () 
    {
	
	}

    void OnDestroy()
    {
        m_score = 0;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (debugscore > m_score) m_score = debugscore;
	}

    public static void add(int p_value)
    {
        m_score += p_value;
    }

    public static void subtract(int p_value)
    {
        m_score -= p_value;
    }

    public static void reset()
    {
        m_score = 0;
    }


    public static void multiply(float p_multiplier)
    {
        m_score = (int)( (float)m_score * p_multiplier );
    }

    public static int getScore()
    {
        return m_score;
    }
}

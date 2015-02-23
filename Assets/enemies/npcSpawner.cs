using UnityEngine;
using System.Collections;

// spawns npcs when repositioned
public class npcSpawner : MonoBehaviour {
    public Transform[] m_npcPrefab;
    public int[] m_spawnAtDifficulty; // same as prefab count
    public int[] m_spawnProbabilityIds;
    public int m_amount = 6;
    public float m_spread=30.0f;
    private float counter = 0.0f;
    public static System.Random rnd;
    static bool hasReinitedStaticNPCCOntrollerStat = false;
    void Awake()
    {
        if (!hasReinitedStaticNPCCOntrollerStat)
        {
            npcController.m_player = null;
            npcController.m_playerIsDead = false;
            npcController.m_totalCurrent = 0;
            npcController.m_firstBlood = false;
            hasReinitedStaticNPCCOntrollerStat = true;
        }
    }

    void OnDestroy()
    {
        hasReinitedStaticNPCCOntrollerStat = false;
    }

	// Use this for initialization
	void Start () {
        rnd = new System.Random(60000 * (int)System.DateTime.Now.Minute + 1000 * (int)System.DateTime.Now.Second + (int)System.DateTime.Now.Millisecond);
	}

	
	// Update is called once per frame
	void Update () {
        if (counter>100.0f)
        {
            counter = 0.0f;
            int allowable = Mathf.Min(npcController.m_totalCurrent + m_amount, npcController.m_totalMax - 1) - npcController.m_totalCurrent;
            //Debug.Log("allowable" + allowable + "c: "+npcController.m_totalCurrent);
            if (allowable>0)
            {
                int difficulty = 0;
                for (int i = 0; i < allowable; i++)
                {
                    Vector3 pos=new Vector3(transform.position.x + Random.Range(m_spread * -0.5f, m_spread * 0.5f), transform.position.y, 0.0f);
                    Debug.DrawLine(pos - Vector3.right, pos + Vector3.right, Color.red, 2.0f);
                    Debug.DrawLine(pos - Vector3.up, pos + Vector3.up, Color.red, 2.0f);
                    int probabilityIdx = 0;
                    int typeId = 0;
                    bool allowedForDifficulty=false;
                    int tries=0;
                    do
                    {
                        if (tries < 10)
                        {
                            probabilityIdx = rnd.Next(0, m_spawnProbabilityIds.Length);
                            typeId = m_spawnProbabilityIds[probabilityIdx];
                            allowedForDifficulty = m_spawnAtDifficulty[typeId] <= difficulty;
                        }
                        else
                        {
                            typeId = 0; allowedForDifficulty = true;
                        }
                        tries++;
                    } while (!allowedForDifficulty);
                    //Debug.Log(tries);
                    Instantiate(m_npcPrefab[typeId],
                                pos,
                                Quaternion.identity);

                }
            }
        }
        counter++;
	}

    void OnDrawGizmos()
    {
        Color old = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1.0f);
        Gizmos.DrawLine(transform.position - Vector3.right * m_spread * 0.5f, transform.position + Vector3.right * m_spread * 0.5f);
        Gizmos.color = old;
    }
}

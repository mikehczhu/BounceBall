using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainController : MonoBehaviour {

    private static UIMainController m_instance;
    public static UIMainController Instance { get { return m_instance; } }

    private int m_MasterCnt;
    private int m_BulletCnt;


    private void Awake()
    {
        m_instance = this;
        
    }
    // Use this for initialization
    void Start ()
    {
        m_MasterCnt = 4;
        m_BulletCnt = 4;

        EventSystem.Instance.Subscribe((int)GameEvent.kill_a_master, KillAMaster);
        EventSystem.Instance.Subscribe((int)GameEvent.use_a_bullet, UseABullet);
        EventSystem.Instance.Subscribe((int)GameEvent.game_over, GameOver);

        EventSystem.Instance.Publish((int)GameEvent.update_master_cnt);
        EventSystem.Instance.Publish((int)GameEvent.update_bullet_cnt);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    public int GetMasterCnt()
    {
        return m_MasterCnt;
    }
    public int GetBulletCnt()
    {
        return m_BulletCnt;
    }
    public void KillAMaster()
    {
        if (m_MasterCnt > 0)
        {
            --m_MasterCnt;
            EventSystem.Instance.Publish((int)GameEvent.update_master_cnt);
            if(m_MasterCnt<=0)
            {
                EventSystem.Instance.Publish((int)GameEvent.show_win);
            }
        }
    }
    public void UseABullet()
    {
        if (m_BulletCnt > 0)
        {
            --m_BulletCnt;
            EventSystem.Instance.Publish((int)GameEvent.update_bullet_cnt);
        }
        else
        {
            EventSystem.Instance.Publish((int)GameEvent.show_no_bullet);
        }
    }
    public void GameOver()
    {
        GameObject.Find("Rope").GetComponent<Rope>().enabled = false;
        Time.timeScale = 0;
    }
}

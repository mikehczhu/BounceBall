using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainView : MonoBehaviour {

    private Text m_MasterCnt;
    private Text m_BulletCnt;
    private GameObject m_GameOverTips;

    private void Awake()
    {
        
    }
    // Use this for initialization
    void Start () {
        
        

        m_MasterCnt = GameObject.Find("MasterCnt").GetComponent<Text>();
        m_BulletCnt = GameObject.Find("BulletCnt").GetComponent<Text>();
        m_GameOverTips = GameObject.Find("GameOverTips");
        m_GameOverTips.SetActive(false);


        EventSystem.Instance.Subscribe((int)GameEvent.update_master_cnt, UpdateMasterCnt);
        EventSystem.Instance.Subscribe((int)GameEvent.update_bullet_cnt, UpdateBulletCnt);
        EventSystem.Instance.Subscribe((int)GameEvent.show_no_bullet, ShowNoBullet);
        EventSystem.Instance.Subscribe((int)GameEvent.show_win, ShowWin);
        EventSystem.Instance.Subscribe((int)GameEvent.player_die, PlayerDie);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void UpdateMasterCnt()
    {
        m_MasterCnt.text = UIMainController.Instance.GetMasterCnt().ToString();
    }
    public void UpdateBulletCnt()
    {
        m_BulletCnt.text = UIMainController.Instance.GetBulletCnt().ToString();
    }
    public void ShowNoBullet()
    {
        m_GameOverTips.SetActive(true);
        m_GameOverTips.transform.Find("Text").GetComponent<Text>().text = "子弹用完了，游戏结束！";
        EventSystem.Instance.Publish((int)GameEvent.game_over);
    }
    public void ShowWin()
    {
        m_GameOverTips.SetActive(true);
        m_GameOverTips.transform.Find("Text").GetComponent<Text>().text = "消灭了所有怪物,游戏胜利！";
        EventSystem.Instance.Publish((int)GameEvent.game_over);
    }
    public void PlayerDie()
    {
        m_GameOverTips.SetActive(true);
        m_GameOverTips.transform.Find("Text").GetComponent<Text>().text = "游戏结束！";
        EventSystem.Instance.Publish((int)GameEvent.game_over);
    }
}

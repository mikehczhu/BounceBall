using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="collision"> 碰撞体 </param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "ball" || collision.transform.tag == "RopeBall")
        {
            Destroy(this.gameObject);
            EventSystem.Instance.Publish((int)GameEvent.kill_a_master);
        }   
    }
}

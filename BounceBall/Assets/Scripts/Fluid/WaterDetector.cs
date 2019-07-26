using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDetector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 弹力球碰撞到水面
        if(collision.GetComponent<Ball>()!=null)
        {
            Destroy(collision.gameObject, 0.1f);
            transform.parent.GetComponent<Fluid>().Wave(transform.position.x, collision.GetComponent<Ball>().speed.y * collision.GetComponent<Ball>().quality / 500.0f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 弹力球碰撞到水面
        if (collision.tag == "ball") 
        {
            Destroy(collision.gameObject, 0.1f);
            // 小球在 y 轴上的速度
            float ballSpeed = collision.GetComponent<Ball>().speed.y;
            // 小球的动能
            float Ek = collision.GetComponent<Ball>().quality * ballSpeed * ballSpeed / 2.0f;
            // 衰减后的动能
            Ek *= 0.0001f;
            //transform.parent.GetComponent<Fluid>().Wave(transform.position.x, 
            //    Mathf.Sqrt(2*Ek/ transform.parent.GetComponent<Fluid>().m_Mass));
            transform.parent.GetComponent<Fluid>().Wave(transform.GetComponent<BoxCollider2D>().offset.x,
                Mathf.Sqrt(2 * Ek / transform.parent.GetComponent<Fluid>().m_Mass));

        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeNodeCollision : MonoBehaviour {

    public enum NodeType { Root,Mid,Leaf};

    public NodeType type;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (type == NodeType.Leaf && collision.transform.tag == "ball")
        {
            // 碰撞点
            ContactPoint2D contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 hitPoint = contact.point;
            // 碰撞点指向圆心的向量
            Vector3 normal = Vector3.Normalize(transform.position - hitPoint);
            transform.parent.GetComponent<Rope>().AddForce(normal * 1000);
        }
        else if(type==NodeType.Mid && collision.transform.tag == "ball")
        {
            transform.parent.GetComponent<Rope>().BreakTheRope(Int32.Parse(transform.name));
        }
    }
}

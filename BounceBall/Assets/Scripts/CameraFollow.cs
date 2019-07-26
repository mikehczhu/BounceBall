using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private GameObject target;

	// Use this for initialization
	void Start () {
        target = GameObject.Find("Player").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        if (target == null)
            return;
        transform.position = target.transform.position + new Vector3(0, 0, -40);
	}
}

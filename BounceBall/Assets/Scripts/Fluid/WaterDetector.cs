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
        if (collision.GetComponent<Rigidbody2D>() != null)
        {
            transform.parent.GetComponent<Fluid>().Splash(transform.position.x, collision.GetComponent<Rigidbody2D>().velocity.y * collision.GetComponent<Rigidbody2D>().mass / 40.0f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fluid : MonoBehaviour {

    private GameObject particle;
    private float width;
    private float height;
    private Vector3[][] buffer;
    private float renderBuffer;
    private Vector3 normal;
    private Vector3 tangent;
    private float k1;
    private float k2;
    private float k3;

	// Use this for initialization
	void Start () {
        particle = Resources.Load<GameObject>("Prefabs/Fluid");
        width = 20;
        height = 20;
        float count = width * height;
        buffer[0] = new Vector3[(int)count];
        buffer[1] = new Vector3[(int)count];

	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Evaluate()
    {

    }
}

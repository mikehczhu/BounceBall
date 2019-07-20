using UnityEngine;
using System.Collections;

public class Clickspawn : MonoBehaviour {

    public GameObject Brick;
	
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject brick = Instantiate(Brick, Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0,0,-Camera.main.transform.position.z)), Brick.transform.rotation) as GameObject;
            brick.transform.Rotate(0, 0, UnityEngine.Random.Range(0, 0));
            brick.GetComponent<Rigidbody2D>().mass = brick.transform.localScale.x * brick.transform.localScale.y/2;
            Destroy(brick, 1.4f);
        }
	}
}

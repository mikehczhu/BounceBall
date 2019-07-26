using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Vector3 position;        // 上一时刻的位置
    public Vector3 positionT;       // 当前时刻的位置
    public Vector3 speed;           // 上一时刻的速度
    public Vector3 speedT;          // 当前时刻的速度
    public Vector3 acceleration;    // 当前时刻的加速度
    public Vector3 force;           // 人物受到的推力
    public Vector3 totalForce;      // 人物受到的合力
    public float quality;           // 人物的质量,kg
    public float moveSpeed;
    private GameObject bullet;      // 子弹

    // Use this for initialization
    void Start()
    {
        position = transform.position;
        speed = new Vector3(0, 0, 0);
        quality = 60.0f;
        moveSpeed = 100.0f;
        bullet = Resources.Load<GameObject>("Prefabs/Bounce/ball");
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
        totalForce = CalTotalForce(force);
        force = Vector3.zero;
        acceleration = CalAcceleration(totalForce);
        speedT = CalSpeed(acceleration);
        positionT = CalPosition(speedT);
        transform.position = positionT;
    }

    // 处理玩家的输入
    private void ProcessInput()
    {
        float x = Input.GetAxis("Horizontal");
        if (x > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (x < 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            force = new Vector3(0, 50000, 0);
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            force += new Vector3(0, -30000, 0);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            force += new Vector3(-30000, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            force += new Vector3(30000, 0, 0);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject go = Instantiate(bullet, transform.Find("shootPos").position, transform.rotation);
            Destroy(go.gameObject, 3f);
        }
    }



    // 计算人物的合力 ---> 重力，特定方向的推力
    // param force 特定方向的推力
    private Vector3 CalTotalForce(Vector3 force)
    {
        Vector3 totalForce = force + new Vector3(0, quality * MyPhysics.Instance.G, 0);
        return totalForce;
    }
    // 计算人物的加速度
    private Vector3 CalAcceleration(Vector3 force)
    {
        return force / quality;
    }
    // 计算人物的速度
    private Vector3 CalSpeed(Vector3 acceleration)
    {
        float speedXt = acceleration.x * Time.deltaTime + speed.x;
        float speedYt = acceleration.y * Time.deltaTime + speed.y;
        speed.x = speedXt;
        speed.y = speedYt;
        return new Vector3(speedXt, speedYt, 0);
    }
    // 计算人物的位置
    private Vector3 CalPosition(Vector3 speed)
    {
        float positionXt = speed.x * Time.deltaTime + position.x;
        float positionYt = speed.y * Time.deltaTime + position.y;
        position.x = positionXt;
        position.y = positionYt;
        return new Vector3(positionXt, positionYt, 0);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "ball")
            return;
        Time.timeScale = 0;
    }
}

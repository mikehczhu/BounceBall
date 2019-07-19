using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public Vector3 position;        // 上一时刻的位置
    public Vector3 positionT;       // 当前时刻的位置
    public Vector3 speed;           // 上一时刻的速度
    public Vector3 speedT;          // 当前时刻的速度
    public Vector3 acceleration;    // 当前时刻的加速度
    public Vector3 force;           // 小球受到的推力
    public Vector3 totalForce;      // 小球受到的合力
    public float quality;           // 小球的质量,kg
    public float deltaTime;         // 当前帧与上一帧的时间间隔
    public float lastFrame;         // 上一帧对应的时间
    public Vector3 moveVector;      // 小球移动的方向
    public float bounce;            // 小球的反弹系数
    private bool isDestory;         // 标志位，用来标识小球是否被销毁


    // Use this for initialization
    void Start () {
        position = transform.position;
        speed = new Vector3(0, 0, 0);
        quality = 10.0f;
        lastFrame = Time.time;
        force = Vector3.zero;
        bounce = 0.8f;
        isDestory = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (isDestory)
            return;
        float currentFrame = Time.time;
        deltaTime = currentFrame - lastFrame;
        lastFrame = currentFrame;
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
        if(Input.GetMouseButtonDown(0))
        {
            force = new Vector3(1000, 1000, 0);
        }
    }
    // 计算小球的合力 ---> 重力，特定方向的推力
    // param force 特定方向的推力
    private Vector3 CalTotalForce(Vector3 force)
    {
        Vector3 totalForce = force + new Vector3(0, quality * Physics.Instance.G, 0);
        return totalForce;
    }
    // 计算小球的加速度
    private Vector3 CalAcceleration(Vector3 force)
    {
        return force / quality;
    }
    // 计算小球的速度
    private Vector3 CalSpeed(Vector3 acceleration)
    {
        float speedXt = acceleration.x * deltaTime + speed.x;
        float speedYt = acceleration.y * deltaTime + speed.y;
        speed.x = speedXt;
        speed.y = speedYt;
        return new Vector3(speedXt, speedYt, 0);
    }
    // 计算小球的位置
    private Vector3 CalPosition(Vector3 speed)
    {
        float positionXt = speed.x * deltaTime + position.x;
        float positionYt = speed.y * deltaTime + position.y;
        moveVector = Vector3.Normalize(new Vector3(positionXt, positionYt, 0) - new Vector3(position.x, position.y, 0));
        position.x = positionXt;
        position.y = positionYt;
        return new Vector3(positionXt, positionYt, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDestory)
            return;
        if (Vector3.Distance(speed, Vector3.zero) < 0.5f)
        {
            isDestory = true;
            Destroy(this.gameObject);
            return;
        }
        // 小球速度的大小
        float speedSize = Vector3.Distance(speed, Vector3.zero);
        // 小球的动能
        float Ek = quality * speedSize * speedSize / 2.0f;
        // 衰减后的动能
        Ek *= bounce;
        // 小球与墙面得碰撞点
        ContactPoint2D contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 hitPoint = contact.point;
        // 小球与墙面碰撞点的法线
        Vector3 normal = Vector3.Normalize(position - hitPoint);

        // 入射向量
        Vector3 incidentDir = Vector3.Normalize(-moveVector);
        // 反射向量
        Vector3 speedDir = -incidentDir + normal * 2.0f * Vector3.Dot(incidentDir, normal);
        // 碰撞后反弹的初始速度
        speed = Mathf.Sqrt(2 * Ek / quality) * speedDir;
    }
}

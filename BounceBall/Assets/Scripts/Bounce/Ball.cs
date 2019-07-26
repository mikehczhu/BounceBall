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
    public Vector3 moveVector;      // 小球移动的方向
    public float bounce;            // 小球的反弹系数
    private bool isDestory;         // 标志位，用来标识小球是否被销毁


    // Use this for initialization
    void Start () {
        position = transform.position;
        speed = transform.right * 100;
        quality = 10.0f;
        force = Vector3.zero;
        //bounce = 1.0f;
        isDestory = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (isDestory)
            return;
        totalForce = MyPhysics.Instance.CalTotalForce(force, quality);
        acceleration = MyPhysics.Instance.CalAcceleration(totalForce, quality);
        force = Vector3.zero;
        speedT = MyPhysics.Instance.CalSpeed(acceleration, ref speed);
        positionT = MyPhysics.Instance.CalPosition(speedT, ref position, ref moveVector);
        transform.position = positionT;
	}

    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="collision"> 碰撞体 </param>
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

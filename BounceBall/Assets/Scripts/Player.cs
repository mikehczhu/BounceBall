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
    public Vector3 moveVector;      // 人物移动的方向
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
        totalForce = MyPhysics.Instance.CalTotalForce(force, quality);
        force = Vector3.zero;
        acceleration = MyPhysics.Instance.CalAcceleration(totalForce, quality);
        speedT = MyPhysics.Instance.CalSpeed(acceleration,ref speed);
        positionT = MyPhysics.Instance.CalPosition(speedT, ref position,ref moveVector); 
        transform.position = positionT;
    }

    /// <summary>
    /// 处理玩家的输入
    /// </summary>
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
            if (UIMainController.Instance.GetBulletCnt() > 0)
            {
                GameObject go = Instantiate(bullet, transform.Find("shootPos").position, transform.rotation);
                Destroy(go.gameObject, 2f);
            }
            EventSystem.Instance.Publish((int)GameEvent.use_a_bullet);
        }
    }

    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="collision"> 碰撞体 </param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "ball")
            return;
        Destroy(gameObject, 0.1f);
        EventSystem.Instance.Publish((int)GameEvent.player_die);
    }
}

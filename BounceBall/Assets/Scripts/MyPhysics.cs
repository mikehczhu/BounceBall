using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPhysics : MonoBehaviour {

    private static MyPhysics m_instance;
    public static MyPhysics Instance { get { return m_instance; } }

    public float G;                 // 重力加速度

    private void Awake()
    {
        m_instance = this;
    }
    private void Start()
    {
        G = -9.8f;
    }

    /// <summary>
    /// 计算物体获得的合力
    /// </summary>
    /// <param name="force"> 物体受到的除重力以外的力 </param>
    /// <param name="quality"> 物理的质量 </param>
    /// <returns></returns>
    public Vector3 CalTotalForce(Vector3 force,float quality)
    {
        Vector3 totalForce = force + new Vector3(0, quality * MyPhysics.Instance.G, 0);
        return totalForce;
    }
    /// <summary>
    /// 计算物体的加速度
    /// </summary>
    /// <param name="force"> 物体受到的合力 </param>
    /// <param name="quality"> 物体的质量 </param>
    /// <returns></returns>
    public Vector3 CalAcceleration(Vector3 force,float quality)
    {
        return force / quality;
    }
    /// <summary>
    /// 计算物体的速度
    /// </summary>
    /// <param name="acceleration"> 物体的加速度 </param>
    /// <param name="speed"> 物体的当前速度 </param>
    /// <returns></returns>
    public Vector3 CalSpeed(Vector3 acceleration,ref Vector3 speed)
    {
        float speedXt = acceleration.x * Time.deltaTime + speed.x;
        float speedYt = acceleration.y * Time.deltaTime + speed.y;
        speed.x = speedXt;
        speed.y = speedYt;
        return new Vector3(speedXt, speedYt, 0);
    }
    /// <summary>
    /// 计算物体的位置
    /// </summary>
    /// <param name="speed"> 物体的当前速度 </param>
    /// <param name="position"> 物体的当前位置 </param>
    /// <returns></returns>
    public Vector3 CalPosition(Vector3 speed,ref Vector3 position,ref Vector3 moveVector)
    {
        float positionXt = speed.x * Time.deltaTime + position.x;
        float positionYt = speed.y * Time.deltaTime + position.y;
        moveVector = Vector3.Normalize(new Vector3(positionXt, positionYt, 0) - new Vector3(position.x, position.y, 0));
        position.x = positionXt;
        position.y = positionYt;
        return new Vector3(positionXt, positionYt, 0);
    }
}

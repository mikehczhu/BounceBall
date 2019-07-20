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
}

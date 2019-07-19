using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics : MonoBehaviour {

    private static Physics m_instance;
    public static Physics Instance { get { return m_instance; } }

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

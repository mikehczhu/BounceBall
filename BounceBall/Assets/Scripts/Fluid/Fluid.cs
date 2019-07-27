using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fluid : MonoBehaviour
{
    private List<FluidNode> m_Fluid = new List<FluidNode>();

    private const float m_Spring = 0.02f;
    private const float m_Damping = 0.04f;
    private const float m_Spread = 0.05f;

    private float m_BaseHeight;

    private Vector3 m_TopLeftPos;
    private Vector3 m_BottomDownPos;

    public GameObject m_FluidMesh;

    [HideInInspector]
    public float m_Mass;


    void Start()
    {
        m_TopLeftPos = transform.parent.Find("TopLeftPos").position;
        m_BottomDownPos = transform.parent.Find("BottomDownPos").position;
        CreateFluid();
    }

    public void Wave(float xpos, float velocity)
    {;
        if (xpos >= m_Fluid[0].m_LeftTopPos.x && xpos <= m_Fluid[m_Fluid.Count - 1].m_BottomDownPos.x)
        {
            xpos -= m_Fluid[0].m_LeftTopPos.x;

            int index = Mathf.RoundToInt(m_Fluid.Count * (xpos / (m_Fluid[m_Fluid.Count - 1].m_LeftTopPos.x - m_Fluid[0].m_LeftTopPos.x)));

            FluidNode node = m_Fluid[index];
            node.m_Velocity += velocity;
            m_Fluid[index] = node;
        }
    }

    public void CreateFluid()
    {
        float Left = m_TopLeftPos.x;
        float Width = m_BottomDownPos.x - m_TopLeftPos.x;
        float Top = m_TopLeftPos.y;
        float Bottom = m_BottomDownPos.y;
        int nodeCount = Mathf.RoundToInt(Width) / 2;
        m_Mass = 1;
        m_BaseHeight = Top;

        for (int i = 0; i < nodeCount; i++)
        {
            FluidNode node = new FluidNode();
            node.m_LeftTopPos = new Vector2(Left + Width * i / nodeCount, Top);
            node.m_BottomDownPos = new Vector2(Left + Width * (i + 1) / nodeCount, Bottom);
            node.m_Acceleration = 0;
            node.m_Velocity = 0;

            node.m_Mesh = new Mesh();

            Vector3[] Vertices = new Vector3[4];
            Vertices[0] = new Vector3(node.m_LeftTopPos.x, node.m_LeftTopPos.y, 0);
            Vertices[1] = new Vector3(node.m_BottomDownPos.x, node.m_LeftTopPos.y, 0);
            Vertices[2] = new Vector3(node.m_LeftTopPos.x, node.m_BottomDownPos.y, 0);
            Vertices[3] = new Vector3(node.m_BottomDownPos.x, node.m_BottomDownPos.y, 0);

            Vector2[] UVs = new Vector2[4];
            UVs[0] = new Vector2(0, 1);
            UVs[1] = new Vector2(1, 1);
            UVs[2] = new Vector2(0, 0);
            UVs[3] = new Vector2(1, 0);

            int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };

            node.m_Mesh.vertices = Vertices;
            node.m_Mesh.uv = UVs;
            node.m_Mesh.triangles = tris;

            node.m_MeshObject = Instantiate(m_FluidMesh, Vector3.zero, Quaternion.identity) as GameObject;
            node.m_MeshObject.GetComponent<MeshFilter>().mesh = node.m_Mesh;
            node.m_MeshObject.transform.parent = transform;
            //node.m_MeshObject.transform.position = new Vector3((node.m_LeftTopPos.x + node.m_BottomDownPos.x) / 2,
            //    (node.m_LeftTopPos.y + node.m_BottomDownPos.y) / 2, 0);

            node.m_MeshObject.AddComponent<BoxCollider2D>();
            node.m_MeshObject.GetComponent<BoxCollider2D>().isTrigger = true;
            node.m_MeshObject.AddComponent<FluidCollision>();

            m_Fluid.Add(node);
        }
    }

    void UpdateMeshes()
    {
        for (int i = 0; i < m_Fluid.Count; i++)
        {

            Vector3[] Vertices = new Vector3[4];

            Vertices[0] = new Vector3(m_Fluid[i].m_LeftTopPos.x, m_Fluid[i].m_LeftTopPos.y, 0);
            Vertices[1] = new Vector3(m_Fluid[i].m_BottomDownPos.x, m_Fluid[i].m_LeftTopPos.y, 0);
            Vertices[2] = new Vector3(m_Fluid[i].m_LeftTopPos.x, m_Fluid[i].m_BottomDownPos.y, 0);
            Vertices[3] = new Vector3(m_Fluid[i].m_BottomDownPos.x, m_Fluid[i].m_BottomDownPos.y, 0);

            m_Fluid[i].m_Mesh.vertices = Vertices;
        }
    }

    void FixedUpdate()
    {
        // 实现碰撞到的水面的阻尼运动
        for (int i = 0; i < m_Fluid.Count; i++)
        {
            float force = m_Spring * (m_Fluid[i].m_LeftTopPos.y - m_BaseHeight) + m_Fluid[i].m_Velocity * m_Damping;
            FluidNode node = m_Fluid[i];
            node.m_Acceleration = -force / m_Mass;
            node.m_LeftTopPos = new Vector2(node.m_LeftTopPos.x, node.m_LeftTopPos.y + node.m_Velocity);
            node.m_Velocity += node.m_Acceleration;
            m_Fluid[i] = node;
        }

        // 碰撞到的水面带动附近的水面一起运动
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < m_Fluid.Count; i++)
            {
                if (i > 0)
                {
                    FluidNode node = m_Fluid[i];
                    node.m_LeftHeightDiff = m_Spread * (m_Fluid[i].m_LeftTopPos.y - m_Fluid[i - 1].m_LeftTopPos.y);
                    m_Fluid[i] = node;
                    node = m_Fluid[i - 1];
                    node.m_Velocity = node.m_Velocity + m_Fluid[i].m_LeftHeightDiff;
                    m_Fluid[i - 1] = node;
                }
                if (i < m_Fluid.Count - 1)
                {
                    FluidNode node = m_Fluid[i];
                    node.m_RightHeightDiff = m_Spread * (m_Fluid[i].m_LeftTopPos.y - m_Fluid[i + 1].m_LeftTopPos.y);
                    m_Fluid[i] = node;
                    node = m_Fluid[i + 1];
                    node.m_Velocity = node.m_Velocity + m_Fluid[i].m_RightHeightDiff;
                    m_Fluid[i + 1] = node;
                }
            }
        }
        for (int i = 0; i < m_Fluid.Count; i++)
        {
            if (i > 0)
            {
                FluidNode node = m_Fluid[i - 1];
                node.m_LeftTopPos = new Vector2(node.m_LeftTopPos.x, node.m_LeftTopPos.y + m_Fluid[i].m_LeftHeightDiff);
                m_Fluid[i - 1] = node;
            }
            if (i < m_Fluid.Count - 1)
            {
                FluidNode node = m_Fluid[i + 1];
                node.m_LeftTopPos = new Vector2(node.m_LeftTopPos.x, node.m_LeftTopPos.y + m_Fluid[i].m_RightHeightDiff);
                m_Fluid[i + 1] = node;
            }
        }

        UpdateMeshes();

    }

    struct FluidNode
    {
        public Vector2 m_LeftTopPos;
        public Vector2 m_BottomDownPos;
        public float m_Velocity;
        public float m_Acceleration;
        public float m_LeftHeightDiff;
        public float m_RightHeightDiff;
        public Mesh m_Mesh;
        public GameObject m_MeshObject;
    }
}
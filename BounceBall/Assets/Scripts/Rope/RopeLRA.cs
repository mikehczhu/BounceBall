using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLRA : MonoBehaviour
{
    struct RopeNode
    {
        public Vector2 oldPos;
        public Vector2 newPos;
        public Transform node;
        public float length;
    }

    List<RopeNode> m_RopeNodes = new List<RopeNode>();
    private GameObject m_NodePrefab;
    private GameObject m_CirclePrefab;
    private int m_NodeCount;
    private float m_NodeSpace;
    private float damping;

    private void Start()
    {
        m_NodePrefab = Resources.Load<GameObject>("Prefabs/Rope/node");
        m_CirclePrefab = Resources.Load<GameObject>("Prefabs/Rope/Cube");
        m_NodeCount = 20;
        m_NodeSpace = 1.75f;
        damping = 0.1f;

        Vector2 startPos = transform.position;
        for(int i=0;i<m_NodeCount;++i)
        {
            if (i != m_NodeCount - 1)
            {
                GameObject go = Instantiate(m_NodePrefab, startPos, Quaternion.identity);
                go.transform.parent = transform;
                RopeNode node = new RopeNode();
                node.oldPos = startPos;
                node.newPos = startPos;
                node.node = go.transform;
                node.length = m_NodeSpace;
                m_RopeNodes.Add(node);
            }
            else
            {
                GameObject go = Instantiate(m_CirclePrefab, startPos, Quaternion.identity);
                go.transform.parent = transform;
                RopeNode node = new RopeNode();
                node.oldPos = startPos;
                node.newPos = startPos;
                node.node = go.transform;
                node.length = m_NodeSpace;
                m_RopeNodes.Add(node);
            }
            startPos.y -= m_NodeSpace;
            startPos.x -= m_NodeSpace / 12;
        } 
    }
    private void Update()
    {
        Simulation();
    }
    private void Simulation()
    {
        Vector2 g = new Vector2(0, -9.8f);
        for(int i=0;i<m_NodeCount;++i)
        {
            Vector2 acceleration = g;
            RopeNode node = m_RopeNodes[i];
            Vector2 newPos = node.newPos + damping * (node.newPos - node.oldPos) + acceleration * Time.deltaTime * Time.deltaTime;
            node.oldPos = node.newPos;
            node.newPos = newPos;
            m_RopeNodes[i] = node;
        }
        Constraion();
    }
    private void Constraion()
    {
        for(int i=0;i<m_NodeCount - 1;++i)
        {
            RopeNode node1 = m_RopeNodes[i];
            RopeNode node2 = m_RopeNodes[i + 1];
            node1.newPos = node1.newPos + (node2.newPos - node1.newPos) * ((Vector2.Distance(node2.newPos,node1.newPos) - m_NodeSpace) / (2 * (Vector2.Distance(node2.newPos, node1.newPos))));
            node2.newPos = node2.newPos - (node2.newPos - node1.newPos) * ((Vector2.Distance(node2.newPos,node1.newPos) - m_NodeSpace) / (2 * (Vector2.Distance(node2.newPos, node1.newPos))));
            m_RopeNodes[i] = node1;
            m_RopeNodes[i + 1] = node2;
        }
        RopeNode node = m_RopeNodes[0];
        node.newPos = transform.position;
        m_RopeNodes[0] = node;
        for(int i=0;i<m_NodeCount;++i)
        {
            m_RopeNodes[i].node.position = m_RopeNodes[i].newPos;
        }
        print(m_RopeNodes[m_NodeCount - 1].node.position);
    }
}

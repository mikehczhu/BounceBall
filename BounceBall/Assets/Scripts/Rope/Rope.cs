using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{

    private List<RopeNode> m_RopeNodes = new List<RopeNode>();  // 绳子的节点和系挂的物体
    private float m_RopeSpace = 1.75f;                          // 绳子节点间的间隙
    private int segmentCount = 20;                              // 绳子节点和系挂物体的个数
    private GameObject node;                                    // 绳子节点
    private GameObject cube;                                    // 绳子系挂物
    private float quality;                                      // 每个节点的质量
    private Vector3 force;                                      // 绳子下面的系挂物受到的小球的冲击力

    // Use this for initialization
    void Start()
    {
        node = Resources.Load<GameObject>("Prefabs/Rope/node");
        cube = Resources.Load<GameObject>("Prefabs/Rope/Cube");
        Vector3 ropeStartPoint = transform.position;
        for (int i = 0; i < segmentCount - 1; ++i)
        {
            GameObject go = Instantiate(node, ropeStartPoint, Quaternion.identity) as GameObject;
            go.transform.parent = transform;
            go.transform.name = i.ToString();
            m_RopeNodes.Add(new RopeNode(ropeStartPoint, go.transform));
            ropeStartPoint.y -= m_RopeSpace;
            ropeStartPoint.x -= m_RopeSpace / 12;
        }
        GameObject temp = Instantiate(cube, ropeStartPoint, Quaternion.identity) as GameObject;
        temp.transform.parent = transform;
        m_RopeNodes.Add(new RopeNode(ropeStartPoint, temp.transform));
        ropeStartPoint.y -= m_RopeSpace;
        ropeStartPoint.x -= m_RopeSpace / 12;
        quality = 1.0f;
        force = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        SimulationRope();
    }
    /// <summary>
    /// 模拟绳子的运动
    /// </summary>
    private void SimulationRope()
    {
        Vector3 gravity = new Vector3(0, MyPhysics.Instance.G * quality, 0);
        // 处理绳子
        for (int i = 0; i < segmentCount - 1; ++i)
        {
            RopeNode firstSegment = this.m_RopeNodes[i];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += gravity * Time.deltaTime / quality;
            this.m_RopeNodes[i] = firstSegment;

        }
        // 处理绳子下面的物体
        gravity = new Vector3(0, MyPhysics.Instance.G * quality) * 3 + force;
        force = Vector3.zero;
        {
            RopeNode firstSegment = this.m_RopeNodes[m_RopeNodes.Count - 1];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += gravity * Time.deltaTime / (3 * quality);
            this.m_RopeNodes[m_RopeNodes.Count - 1] = firstSegment;
        }
        for (int i = 0; i < 50; ++i)
        {
            ApplyConstraint();
        }
    }
    /// <summary>
    /// 给绳子下面的系挂物添加力
    /// </summary>
    /// <param name="force"> 冲击力</param>
    public void AddForce(Vector3 force)
    {
        this.force = force;
    }
    /// <summary>
    /// 断开绳子
    /// </summary>
    /// <param name="index"> 从index节点处断开绳子 </param>
    public void BreakTheRope(int index)
    {
        //print("index: " + index);
        //segmentCount = index + 1;
    }
    /// <summary>
    /// 给绳子添加约束
    /// </summary>
    private void ApplyConstraint()
    {
        RopeNode firstNode = this.m_RopeNodes[0];
        firstNode.posNow = transform.position;
        firstNode.node.position = firstNode.posNow;
        this.m_RopeNodes[0] = firstNode;


        for (int i = segmentCount - 1; i > 0; --i)
        {
            RopeNode firstSeg = this.m_RopeNodes[i];
            RopeNode secondSeg = this.m_RopeNodes[i - 1];
            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float additional = Mathf.Abs(dist - m_RopeSpace);
            Vector3 changeDir = Vector3.zero;
            if (dist > m_RopeSpace)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < m_RopeSpace)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }
            Vector3 changeAmount = changeDir * additional;
            if (i != segmentCount - 1)
            {
                firstSeg.posNow -= changeAmount;
                this.m_RopeNodes[i] = firstSeg;
                secondSeg.posNow += changeAmount;
                this.m_RopeNodes[i - 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.m_RopeNodes[i - 1] = secondSeg;
            }
        }
        {
            RopeNode firstSeg = this.m_RopeNodes[m_RopeNodes.Count - 1];
            RopeNode secondSeg = this.m_RopeNodes[m_RopeNodes.Count - 2];
            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float additional = Mathf.Abs(dist - m_RopeSpace);
            Vector3 changeDir = Vector3.zero;
            if (dist > m_RopeSpace)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < m_RopeSpace)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }
            Vector3 changeAmount = changeDir * additional;
            firstSeg.posNow -= changeAmount;
            m_RopeNodes[m_RopeNodes.Count - 1] = firstSeg;
        }
        this.m_RopeNodes[0] = firstNode;

        for (int i = 0; i < segmentCount; ++i)
        {
            m_RopeNodes[i].node.position = m_RopeNodes[i].posNow;
        }
    }
    public struct RopeNode
    {
        public Vector3 posNow;
        public Vector3 posOld;
        public Transform node;
        public RopeNode(Vector3 pos, Transform go)
        {
            this.posNow = pos;
            this.posOld = pos;
            node = go;
        }
    }

}


// 上端固定，下面可交互版本
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{

    private List<RopeNode> m_RopeNodes = new List<RopeNode>();  // 绳子的节点和系挂的物体
    private float m_RopeSpace = 1.75f;                          // 绳子节点间的间隙
    private int segmentCount = 20;                              // 绳子节点和系挂物体的个数
    private GameObject node;                                    // 绳子节点
    private GameObject cube;                                    // 绳子系挂物
    private float quality;                                      // 每个节点的质量
    private Vector3 force;                                      // 绳子下面的系挂物受到的小球的冲击力

    // Use this for initialization
    void Start()
    {
        node = Resources.Load<GameObject>("Prefabs/Rope/node");
        cube = Resources.Load<GameObject>("Prefabs/Rope/Cube");
        Vector3 ropeStartPoint = transform.position;
        for (int i = 0; i < segmentCount - 1; ++i)
        {
            GameObject go = Instantiate(node, ropeStartPoint, Quaternion.identity) as GameObject;
            go.transform.parent = transform;
            go.transform.name = i.ToString();
            m_RopeNodes.Add(new RopeNode(ropeStartPoint, go.transform));
            ropeStartPoint.y -= m_RopeSpace;
            ropeStartPoint.x -= m_RopeSpace / 12;
        }
        GameObject temp = Instantiate(cube, ropeStartPoint, Quaternion.identity) as GameObject;
        temp.transform.parent = transform;
        m_RopeNodes.Add(new RopeNode(ropeStartPoint, temp.transform));
        ropeStartPoint.y -= m_RopeSpace;
        ropeStartPoint.x -= m_RopeSpace / 12;
        quality = 1.0f;
        force = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        SimulationRope();
    }
    /// <summary>
    /// 模拟绳子的运动
    /// </summary>
    private void SimulationRope()
    {
        Vector3 gravity = new Vector3(0, MyPhysics.Instance.G * quality, 0);
        // 处理绳子
        for (int i = 0; i < segmentCount - 1; ++i)
        {
            RopeNode firstSegment = this.m_RopeNodes[i];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += gravity * Time.deltaTime / quality;
            this.m_RopeNodes[i] = firstSegment;
            
        }
        // 处理绳子下面的物体
        gravity = new Vector3(0, MyPhysics.Instance.G * quality) * 3 + force;
        force = Vector3.zero;
        {
            RopeNode firstSegment = this.m_RopeNodes[m_RopeNodes.Count - 1];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += gravity * Time.deltaTime / (3*quality);
            this.m_RopeNodes[m_RopeNodes.Count - 1] = firstSegment;
        }
        for (int i = 0; i < 50; ++i)
        {
            ApplyConstraint();
        }
    }
    /// <summary>
    /// 给绳子下面的系挂物添加力
    /// </summary>
    /// <param name="force"> 冲击力</param>
    public void AddForce(Vector3 force)
    {
        this.force = force;
    }
    /// <summary>
    /// 断开绳子
    /// </summary>
    /// <param name="index"> 从index节点处断开绳子 </param>
    public void BreakTheRope(int index)
    {
        //print("index: " + index);
        //segmentCount = index + 1;
    }
    /// <summary>
    /// 给绳子添加约束
    /// </summary>
    private void ApplyConstraint()
    {
        RopeNode firstNode = this.m_RopeNodes[0];
        firstNode.posNow = transform.position;
        firstNode.node.position = firstNode.posNow;
        this.m_RopeNodes[0] = firstNode;


        for (int i = segmentCount - 1; i > 0; --i) 
        {
            RopeNode firstSeg = this.m_RopeNodes[i];
            RopeNode secondSeg = this.m_RopeNodes[i - 1];
            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float additional = Mathf.Abs(dist - m_RopeSpace);
            Vector3 changeDir = Vector3.zero;
            if (dist > m_RopeSpace)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < m_RopeSpace)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }
            Vector3 changeAmount = changeDir * additional;
            if (i != segmentCount - 1)
            {
                firstSeg.posNow -= changeAmount;
                this.m_RopeNodes[i] = firstSeg;
                secondSeg.posNow += changeAmount;
                this.m_RopeNodes[i - 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.m_RopeNodes[i - 1] = secondSeg;
            }
        }
        {
            RopeNode firstSeg = this.m_RopeNodes[m_RopeNodes.Count - 1];
            RopeNode secondSeg = this.m_RopeNodes[m_RopeNodes.Count - 2];
            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float additional = Mathf.Abs(dist - m_RopeSpace);
            Vector3 changeDir = Vector3.zero;
            if (dist > m_RopeSpace)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < m_RopeSpace)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }
            Vector3 changeAmount = changeDir * additional;
            firstSeg.posNow -= changeAmount;
            m_RopeNodes[m_RopeNodes.Count - 1] = firstSeg;
        }
        this.m_RopeNodes[0] = firstNode;

        for(int i=0;i< segmentCount; ++i)
        {
            m_RopeNodes[i].node.position = m_RopeNodes[i].posNow;
        }
    }
    public struct RopeNode
    {
        public Vector3 posNow;
        public Vector3 posOld;
        public Transform node;
        public RopeNode(Vector3 pos, Transform go)
        {
            this.posNow = pos;
            this.posOld = pos;
            node = go;
        }
    }

}

*/

// 双端都固定
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Rope : MonoBehaviour
//{

//    private List<RopeNode> m_RopeNodes = new List<RopeNode>();
//    private float m_RopeSpace = 1.7f;
//    private int segmentCount = 20;
//    private GameObject node;
//    private GameObject cube;
//    private float quality;  // 每个节点的质量

//    // Use this for initialization
//    void Start()
//    {
//        node = Resources.Load<GameObject>("Prefabs/Rope/node");
//        cube = Resources.Load<GameObject>("Prefabs/Rope/Cube");
//        Vector3 ropeStartPoint = transform.position;
//        for (int i = 0; i < segmentCount - 1; ++i)
//        {
//            GameObject go = Instantiate(node, ropeStartPoint, Quaternion.identity) as GameObject;
//            go.transform.parent = transform;
//            this.m_RopeNodes.Add(new RopeNode(ropeStartPoint, go.transform));
//            ropeStartPoint.y -= 4 * m_RopeSpace / 5;
//            ropeStartPoint.x -= 3 * m_RopeSpace / 5;
//        }
//        GameObject temp = Instantiate(cube, ropeStartPoint, Quaternion.identity) as GameObject;
//        temp.transform.parent = transform;
//        this.m_RopeNodes.Add(new RopeNode(ropeStartPoint, temp.transform));
//        ropeStartPoint.y -= 4 * m_RopeSpace / 5;
//        ropeStartPoint.x -= 3 * m_RopeSpace / 5;
//        quality = 1.5f;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        Simulate();
//    }
//    private void Simulate()
//    {
//        Vector3 forceGravity = new Vector3(0, MyPhysics.Instance.G * quality);
//        for (int i = 0; i < m_RopeNodes.Count; ++i)
//        {
//            RopeNode firstSegment = this.m_RopeNodes[i];
//            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
//            firstSegment.posOld = firstSegment.posNow;
//            firstSegment.posNow += velocity;
//            firstSegment.posNow += forceGravity * Time.deltaTime;
//            this.m_RopeNodes[i] = firstSegment;
//        }
//        for (int i = 0; i < 50; ++i)
//        {
//            ApplyConstraint();
//        }
//    }
//    private void ApplyConstraint()
//    {
//        RopeNode firstSegment = this.m_RopeNodes[0];
//        firstSegment.posNow = transform.position;
//        firstSegment.node.position = firstSegment.posNow;
//        this.m_RopeNodes[0] = firstSegment;

//        RopeNode lastSegment = this.m_RopeNodes[segmentCount - 1];
//        lastSegment.posNow = lastSegment.node.position;
//        this.m_RopeNodes[segmentCount - 1] = lastSegment;


//        for (int i = m_RopeNodes.Count - 1; i > 0; --i)
//        {
//            RopeNode firstSeg = this.m_RopeNodes[i];
//            RopeNode secondSeg = this.m_RopeNodes[i - 1];
//            float dist = (secondSeg.posNow - firstSeg.posNow).magnitude;
//            float additional = Mathf.Abs(dist - m_RopeSpace);
//            Vector3 changeDir = Vector3.zero;
//            if (dist > m_RopeSpace)
//            {
//                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
//            }
//            else if (dist < m_RopeSpace)
//            {
//                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
//            }
//            Vector3 changeAmount = changeDir * additional;
//            if (i != segmentCount - 1)
//            {
//                firstSeg.posNow += changeAmount;
//                this.m_RopeNodes[i] = firstSeg;
//                secondSeg.posNow -= changeAmount;
//                this.m_RopeNodes[i - 1] = secondSeg;
//            }
//            else
//            {
//                secondSeg.posNow -= changeAmount;
//                this.m_RopeNodes[i - 1] = secondSeg;
//            }
//        }

//        this.m_RopeNodes[0] = firstSegment;

//        for (int i = 0; i < m_RopeNodes.Count; ++i)
//        {
//            m_RopeNodes[i].node.position = m_RopeNodes[i].posNow;
//        }
//    }
//    public struct RopeNode
//    {
//        public Vector3 posNow;
//        public Vector3 posOld;
//        public Transform node;
//        public RopeNode(Vector3 pos, Transform go)
//        {
//            this.posNow = pos;
//            this.posOld = pos;
//            node = go;
//        }
//    }

//}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{

    private List<RopeNode> m_RopeNodes = new List<RopeNode>();
    private float m_RopeSpace = 1.7f;
    private int segmentCount = 20;
    private GameObject node;
    private float quality;  // 每个节点的质量

    // Use this for initialization
    void Start()
    {
        node = Resources.Load<GameObject>("Prefabs/Rope/node");
        Vector3 ropeStartPoint = transform.position;
        for (int i = 0; i < segmentCount; ++i)
        {
            GameObject go = Instantiate(node, ropeStartPoint, Quaternion.identity) as GameObject;
            go.transform.parent = transform;
            go.transform.name = i.ToString();
            this.m_RopeNodes.Add(new RopeNode(ropeStartPoint, go.transform));
            ropeStartPoint.y -= 4 * m_RopeSpace / 5;
            ropeStartPoint.x -= 3 * m_RopeSpace / 5;
        }
        quality = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        Simulate();
    }
    private void Simulate()
    {
        Vector3 forceGravity = new Vector3(0, MyPhysics.Instance.G * quality);
        for (int i = 0; i < m_RopeNodes.Count; ++i)
        {
            RopeNode firstSegment = this.m_RopeNodes[i];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.deltaTime;
            this.m_RopeNodes[i] = firstSegment;
        }
        for (int i = 0; i < 50; ++i)
        {
            ApplyConstraint();
        }
    }
    public void AddForce(Vector3 force)
    {

    }
    public void BreakTheRope(int index)
    {
        segmentCount = index + 1;
    }
    private void ApplyConstraint()
    {
        RopeNode firstSegment = this.m_RopeNodes[0];
        firstSegment.posNow = transform.position;
        firstSegment.node.position = firstSegment.posNow;
        this.m_RopeNodes[0] = firstSegment;

        RopeNode lastSegment = this.m_RopeNodes[segmentCount - 1];
        lastSegment.posNow = lastSegment.node.position;
        this.m_RopeNodes[segmentCount - 1] = lastSegment;


        for (int i = 0; i < segmentCount - 1; ++i)
        {
            RopeNode firstSeg = this.m_RopeNodes[i];
            RopeNode secondSeg = this.m_RopeNodes[i + 1];
            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float additional = Mathf.Abs(dist - m_RopeSpace);
            Vector3 changeDir = Vector3.zero;
            if (dist > m_RopeSpace)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < m_RopeSpace)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }
            Vector3 changeAmount = changeDir * additional;
            if (i != segmentCount - 1)
            {
                firstSeg.posNow -= changeAmount;
                this.m_RopeNodes[i] = firstSeg;
                secondSeg.posNow += changeAmount;
                this.m_RopeNodes[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                this.m_RopeNodes[i + 1] = secondSeg;
            }
        }
        this.m_RopeNodes[0] = firstSegment;

        for (int i = m_RopeNodes.Count - 1; i > segmentCount; --i)
        {
            RopeNode firstSeg = this.m_RopeNodes[i];
            RopeNode secondSeg = this.m_RopeNodes[i - 1];
            float dist = (secondSeg.posNow - firstSeg.posNow).magnitude;
            float additional = Mathf.Abs(dist - m_RopeSpace);
            Vector3 changeDir = Vector3.zero;
            if (dist > m_RopeSpace)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }
            else if (dist < m_RopeSpace)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            Vector3 changeAmount = changeDir * additional;
            if (i != segmentCount - 1)
            {
                firstSeg.posNow += changeAmount;
                this.m_RopeNodes[i] = firstSeg;
                secondSeg.posNow -= changeAmount;
                this.m_RopeNodes[i - 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow -= changeAmount;
                this.m_RopeNodes[i - 1] = secondSeg;
            }
        }
        this.m_RopeNodes[segmentCount - 1] = lastSegment;

        for (int i = 0; i < m_RopeNodes.Count; ++i)
        {
            m_RopeNodes[i].node.position = m_RopeNodes[i].posNow;
        }
    }
    public struct RopeNode
    {
        public Vector3 posNow;
        public Vector3 posOld;
        public Transform node;
        public RopeNode(Vector3 pos, Transform go)
        {
            this.posNow = pos;
            this.posOld = pos;
            node = go;
        }
    }

}
*/


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Rope : MonoBehaviour
//{

//    private List<RopeNode> m_RopeNodes = new List<RopeNode>();
//    private float m_RopeSpace = 1.8f;
//    private int segmentCount = 20;
//    private GameObject node;
//    private GameObject cube;
//    private float quality;  // 每个节点的质量

//    // Use this for initialization
//    void Start()
//    {
//        node = Resources.Load<GameObject>("Prefabs/Rope/node");
//        cube = Resources.Load<GameObject>("Prefabs/Rope/Cube");
//        //Vector3 mousePos = Input.mousePosition;
//        //mousePos.z = -Camera.main.transform.position.z;
//        //Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(mousePos);
//        Vector3 ropeStartPoint = transform.position;
//        for (int i = 0; i < segmentCount - 1; ++i)
//        {
//            GameObject go = Instantiate(node, ropeStartPoint, Quaternion.identity) as GameObject;
//            go.transform.parent = transform;
//            this.m_RopeNodes.Add(new RopeNode(ropeStartPoint, go.transform));
//            ropeStartPoint.y -= m_RopeSpace;
//        }
//        GameObject temp = Instantiate(cube, ropeStartPoint, Quaternion.identity) as GameObject;
//        temp.transform.parent = transform;
//        this.m_RopeNodes.Add(new RopeNode(ropeStartPoint, temp.transform));
//        ropeStartPoint.y -= m_RopeSpace;
//        quality = 1.5f;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        Simulate();
//    }
//    private void Simulate()
//    {
//        Vector3 forceGravity = new Vector3(0, MyPhysics.Instance.G * quality);
//        for (int i = 0; i < segmentCount; ++i)
//        {
//            RopeNode firstSegment = this.m_RopeNodes[i];
//            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
//            firstSegment.posOld = firstSegment.posNow;
//            firstSegment.posNow += velocity;
//            firstSegment.posNow += forceGravity * Time.deltaTime;
//            firstSegment.node.position = firstSegment.posNow;
//            this.m_RopeNodes[i] = firstSegment;
//        }
//        for (int i = 0; i < 50; ++i)
//        {
//            AppleConstraint();
//        }
//    }
//    private void AppleConstraint()
//    {
//        RopeNode firstSegment = this.m_RopeNodes[0];
//        //Vector3 mousePos = Input.mousePosition;
//        //mousePos.z = -Camera.main.transform.position.z;
//        //firstSegment.posNow = Camera.main.ScreenToWorldPoint(mousePos);
//        firstSegment.posNow = transform.position;
//        firstSegment.node.position = firstSegment.posNow;
//        this.m_RopeNodes[0] = firstSegment;

//        for (int i = 0; i < segmentCount - 1; ++i)
//        {
//            RopeNode firstSeg = this.m_RopeNodes[i];
//            RopeNode secondSeg = this.m_RopeNodes[i + 1];
//            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
//            float additional = Mathf.Abs(dist - m_RopeSpace);
//            Vector3 changeDir = Vector3.zero;
//            if (dist > m_RopeSpace)
//            {
//                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
//            }
//            else if (dist < m_RopeSpace)
//            {
//                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
//            }
//            Vector3 changeAmount = changeDir * additional;
//            if (i != 0)
//            {
//                firstSeg.posNow -= changeAmount;
//                firstSeg.node.position = firstSeg.posNow;
//                this.m_RopeNodes[i] = firstSeg;
//                secondSeg.posNow += changeAmount;
//                secondSeg.node.position = secondSeg.posNow;
//                this.m_RopeNodes[i + 1] = secondSeg;
//            }
//            else
//            {
//                secondSeg.posNow += changeAmount;
//                secondSeg.node.position = secondSeg.posNow;
//                this.m_RopeNodes[i + 1] = secondSeg;
//            }
//        }
//    }
//    public struct RopeNode
//    {
//        public Vector3 posNow;
//        public Vector3 posOld;
//        public Transform node;
//        public RopeNode(Vector3 pos, Transform go)
//        {
//            this.posNow = pos;
//            this.posOld = pos;
//            node = go;
//        }
//    }

//}

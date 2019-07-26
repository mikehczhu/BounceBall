using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{

    private List<RopeNode> RopeNodes = new List<RopeNode>();
    private float ropeSegLen = 1.7f;
    private int segmentCount = 20;
    private GameObject node;
    private GameObject cube;
    private float quality;  // 每个节点的质量

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
            this.RopeNodes.Add(new RopeNode(ropeStartPoint, go.transform));
            ropeStartPoint.y -= 4 * ropeSegLen / 5;
            ropeStartPoint.x -= 3 * ropeSegLen / 5;
        }
        GameObject temp = Instantiate(cube, ropeStartPoint, Quaternion.identity) as GameObject;
        temp.transform.parent = transform;
        this.RopeNodes.Add(new RopeNode(ropeStartPoint, temp.transform));
        ropeStartPoint.y -= 4 * ropeSegLen / 5;
        ropeStartPoint.x -= 3 * ropeSegLen / 5;
        quality = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        Simulate();
    }
    private void Simulate()
    {
        Vector2 forceGravity = new Vector2(0, MyPhysics.Instance.G * quality);
        for (int i = 0; i < segmentCount; ++i)
        {
            RopeNode firstSegment = this.RopeNodes[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.deltaTime;
            this.RopeNodes[i] = firstSegment;
        }
        for (int i = 0; i < 50; ++i)
        {
            ApplyConstraint();
        }
    }
    private void ApplyConstraint()
    {
        RopeNode firstSegment = this.RopeNodes[0];
        firstSegment.posNow = transform.position;
        firstSegment.node.position = firstSegment.posNow;
        this.RopeNodes[0] = firstSegment;

        RopeNode lastSegment = this.RopeNodes[segmentCount - 1];
        lastSegment.posNow = lastSegment.node.position;
        this.RopeNodes[segmentCount - 1] = lastSegment;


        for (int i = segmentCount - 1; i > 0; --i) 
        {
            RopeNode firstSeg = this.RopeNodes[i];
            RopeNode secondSeg = this.RopeNodes[i - 1];
            float dist = (secondSeg.posNow - firstSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - ropeSegLen);
            Vector2 changeDir = Vector2.zero;
            if (dist > ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            Vector2 changeAmount = changeDir * error;
            if (i != segmentCount - 1)
            {
                firstSeg.posNow += changeAmount;
                this.RopeNodes[i] = firstSeg;
                secondSeg.posNow -= changeAmount;
                this.RopeNodes[i - 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow -= changeAmount;
                this.RopeNodes[i - 1] = secondSeg;
            }
        }

        
        this.RopeNodes[0] = firstSegment;

        for(int i=0;i<segmentCount;++i)
        {
            RopeNodes[i].node.position = RopeNodes[i].posNow;
        }
    }
    public struct RopeNode
    {
        public Vector2 posNow;
        public Vector2 posOld;
        public Transform node;
        public RopeNode(Vector2 pos, Transform go)
        {
            this.posNow = pos;
            this.posOld = pos;
            node = go;
        }
    }

}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Rope : MonoBehaviour
//{

//    private List<RopeNode> RopeNodes = new List<RopeNode>();
//    private float ropeSegLen = 1.8f;
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
//            this.RopeNodes.Add(new RopeNode(ropeStartPoint, go.transform));
//            ropeStartPoint.y -= ropeSegLen;
//        }
//        GameObject temp = Instantiate(cube, ropeStartPoint, Quaternion.identity) as GameObject;
//        temp.transform.parent = transform;
//        this.RopeNodes.Add(new RopeNode(ropeStartPoint, temp.transform));
//        ropeStartPoint.y -= ropeSegLen;
//        quality = 1.5f;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        Simulate();
//    }
//    private void Simulate()
//    {
//        Vector2 forceGravity = new Vector2(0, MyPhysics.Instance.G * quality);
//        for (int i = 0; i < segmentCount; ++i)
//        {
//            RopeNode firstSegment = this.RopeNodes[i];
//            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
//            firstSegment.posOld = firstSegment.posNow;
//            firstSegment.posNow += velocity;
//            firstSegment.posNow += forceGravity * Time.deltaTime;
//            firstSegment.node.position = firstSegment.posNow;
//            this.RopeNodes[i] = firstSegment;
//        }
//        for (int i = 0; i < 50; ++i)
//        {
//            AppleConstraint();
//        }
//    }
//    private void AppleConstraint()
//    {
//        RopeNode firstSegment = this.RopeNodes[0];
//        //Vector3 mousePos = Input.mousePosition;
//        //mousePos.z = -Camera.main.transform.position.z;
//        //firstSegment.posNow = Camera.main.ScreenToWorldPoint(mousePos);
//        firstSegment.posNow = transform.position;
//        firstSegment.node.position = firstSegment.posNow;
//        this.RopeNodes[0] = firstSegment;

//        for (int i = 0; i < segmentCount - 1; ++i)
//        {
//            RopeNode firstSeg = this.RopeNodes[i];
//            RopeNode secondSeg = this.RopeNodes[i + 1];
//            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
//            float error = Mathf.Abs(dist - ropeSegLen);
//            Vector2 changeDir = Vector2.zero;
//            if (dist > ropeSegLen)
//            {
//                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
//            }
//            else if (dist < ropeSegLen)
//            {
//                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
//            }
//            Vector2 changeAmount = changeDir * error;
//            if (i != 0)
//            {
//                firstSeg.posNow -= changeAmount;
//                firstSeg.node.position = firstSeg.posNow;
//                this.RopeNodes[i] = firstSeg;
//                secondSeg.posNow += changeAmount;
//                secondSeg.node.position = secondSeg.posNow;
//                this.RopeNodes[i + 1] = secondSeg;
//            }
//            else
//            {
//                secondSeg.posNow += changeAmount;
//                secondSeg.node.position = secondSeg.posNow;
//                this.RopeNodes[i + 1] = secondSeg;
//            }
//        }
//    }
//    public struct RopeNode
//    {
//        public Vector2 posNow;
//        public Vector2 posOld;
//        public Transform node;
//        public RopeNode(Vector2 pos, Transform go)
//        {
//            this.posNow = pos;
//            this.posOld = pos;
//            node = go;
//        }
//    }

//}

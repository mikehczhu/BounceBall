using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {

    private List<RopeNode> RopeNodes = new List<RopeNode>();
    private float ropeSegLen = 0.4f;
    private int segmentCount = 20;
    private GameObject node;
    private GameObject cube;
    private float quality;  // 每个节点的质量

    // Use this for initialization
    void Start()
    {
        node = Resources.Load<GameObject>("Prefabs/node");
        cube = Resources.Load<GameObject>("Prefabs/Cube");
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(mousePos);
        for(int i=0;i<segmentCount - 1;++i)
        {
            GameObject go = Instantiate(node, ropeStartPoint, Quaternion.identity) as GameObject;
            go.transform.parent = transform;
            this.RopeNodes.Add(new RopeNode(ropeStartPoint,go.transform));
            ropeStartPoint.y -= ropeSegLen;
        }
        GameObject temp = Instantiate(cube, ropeStartPoint, Quaternion.identity) as GameObject;
        temp.transform.parent = transform;
        this.RopeNodes.Add(new RopeNode(ropeStartPoint, temp.transform));
        ropeStartPoint.y -= ropeSegLen;
        quality = 0.5f;
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
            firstSegment.node.position = firstSegment.posNow;
            this.RopeNodes[i] = firstSegment;
        }
        for (int i = 0; i < 50; ++i)
        {
            AppleConstraint();
        }
    }
    private void AppleConstraint()
    {
        RopeNode firstSegment = this.RopeNodes[0];
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        firstSegment.posNow = Camera.main.ScreenToWorldPoint(mousePos);
        firstSegment.node.position = firstSegment.posNow;
        this.RopeNodes[0] = firstSegment;

        for (int i = 0; i < segmentCount - 1; ++i)
        {
            RopeNode firstSeg = this.RopeNodes[i];
            RopeNode secondSeg = this.RopeNodes[i + 1];
            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - ropeSegLen);
            Vector2 changeDir = Vector2.zero;
            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }
            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount;
                firstSeg.node.position = firstSeg.posNow;
                this.RopeNodes[i] = firstSeg;
                secondSeg.posNow += changeAmount;
                secondSeg.node.position = secondSeg.posNow;
                this.RopeNodes[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                secondSeg.node.position = secondSeg.posNow;
                this.RopeNodes[i + 1] = secondSeg;
            }
        }
    }
    public struct RopeNode
    {
        public Vector2 posNow;
        public Vector2 posOld;
        public Transform node;
        public RopeNode(Vector2 pos,Transform go)
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

//    private LineRenderer lineRenderer;
//    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
//    private float ropeSegLen = 0.25f;
//    private int segmentLength = 30;
//    private float lineWidth = 0.1f;
//    public GameObject node;

//    // Use this for initialization
//    void Start()
//    {
//        this.lineRenderer = GetComponent<LineRenderer>();
//        Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        for (int i = 0; i < segmentLength; ++i)
//        {
//            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
//            ropeStartPoint.y -= ropeSegLen;
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        this.DrawRope();
//    }
//    private void FixedUpdate()
//    {
//        //Simulate();
//    }
//    private void Simulate()
//    {
//        Vector2 forceGravity = new Vector2(0, -1f);
//        for (int i = 0; i < segmentLength; ++i)
//        {
//            RopeSegment firstSegment = this.ropeSegments[i];
//            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
//            firstSegment.posOld = firstSegment.posNow;
//            firstSegment.posNow += velocity;
//            firstSegment.posNow += forceGravity * Time.deltaTime;
//            this.ropeSegments[i] = firstSegment;
//        }
//        //for(int i=0;i<50;++i)
//        //{
//        this.AppleConstraint();
//        //}
//    }
//    private void AppleConstraint()
//    {
//        RopeSegment firstSegment = this.ropeSegments[0];
//        Vector3 mousePos = Input.mousePosition;
//        mousePos.z = Camera.main.nearClipPlane;
//        firstSegment.posNow = Camera.main.ScreenToWorldPoint(mousePos);
//        this.ropeSegments[0] = firstSegment;

//        for (int i = 0; i < segmentLength - 1; ++i)
//        {
//            RopeSegment firstSeg = this.ropeSegments[i];
//            RopeSegment secondSeg = this.ropeSegments[i + 1];
//            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
//            float error = Mathf.Abs(dist - this.ropeSegLen);
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
//                firstSeg.posNow -= changeAmount * error;
//                this.ropeSegments[i] = firstSeg;
//                secondSeg.posNow += changeAmount * 0.5f;
//                this.ropeSegments[i + 1] = secondSeg;
//            }
//            else
//            {
//                secondSeg.posNow += changeAmount;
//                this.ropeSegments[i + 1] = secondSeg;
//            }
//        }
//    }
//    private void DrawRope()
//    {
//        float lineWidth = this.lineWidth;
//        lineRenderer.startWidth = lineWidth;
//        lineRenderer.endWidth = lineWidth;

//        Vector3[] ropePositions = new Vector3[this.segmentLength];
//        for (int i = 0; i < this.segmentLength; ++i)
//        {
//            ropePositions[i] = this.ropeSegments[i].posNow;
//        }
//        lineRenderer.positionCount = ropePositions.Length;
//        lineRenderer.SetPositions(ropePositions);
//    }
//    public struct RopeSegment
//    {
//        public Vector2 posNow;
//        public Vector2 posOld;
//        public RopeSegment(Vector2 pos)
//        {
//            this.posNow = pos;
//            this.posOld = pos;
//        }
//    }

//}

using UnityEngine;
using System.Collections;


public class Fluid : MonoBehaviour
{
    private GameObject particle;
    private int particleCount;
    public DynamicParticle.STATES particlesState = DynamicParticle.STATES.GAS; 

    void Start()
    {
        particle = Resources.Load<GameObject>("Prefabs/Fluid/particle");
        particleCount = 200;
        Vector3 startPos = transform.position;
        for(int i=0;i<particleCount;++i)
        {
            GameObject go = Instantiate(particle, startPos, Quaternion.identity);
            go.transform.parent = transform;
            DynamicParticle script = go.GetComponent<DynamicParticle>();
            script.SetState(particlesState);
            startPos.x -= 0.005f;
        }
    }

    void Update()
    {
    }
}

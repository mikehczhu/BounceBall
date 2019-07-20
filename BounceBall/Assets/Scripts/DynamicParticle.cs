using UnityEngine;
using System.Collections;

public class DynamicParticle : MonoBehaviour {	

	public enum STATES{WATER,GAS,LAVA,NONE};
	STATES currentState=STATES.NONE;
	public SpriteRenderer particleImage;
	public Color waterColor,gasColor,lavaColor;
	float GAS_FLOATABILITY=7.0f;

	void Awake(){ 
		if (currentState == STATES.NONE)
			SetState (STATES.WATER);
	}


	public void SetState(STATES newState)
    {
		if(newState!=currentState)
        {
			currentState=newState;
			GetComponent<Rigidbody2D>().velocity=new Vector2();
			switch(newState){
				case STATES.WATER:
					particleImage.color=waterColor;
					GetComponent<Rigidbody2D>().gravityScale=1.0f;
				break;
				case STATES.GAS:
					particleImage.color=gasColor;										
					GetComponent<Rigidbody2D>().gravityScale=0.0f;
					gameObject.layer=LayerMask.NameToLayer("Gas");
				break;					
				case STATES.LAVA:
					particleImage.color=lavaColor;															
					GetComponent<Rigidbody2D>().gravityScale=0.3f;
				break;	
				case STATES.NONE:
					Destroy(gameObject);
				break;
			}

		}		
	}
	void Update () {
		switch(currentState){
			case STATES.WATER: 
				MovementAnimation(); 
			break;
			case STATES.LAVA:
				MovementAnimation();
			break;
			case STATES.GAS:
				if(GetComponent<Rigidbody2D>().velocity.y<50)
                {
					GetComponent<Rigidbody2D>().AddForce (new Vector2(0,GAS_FLOATABILITY));
				}
			break;

		}	
	}
	void MovementAnimation(){
		Vector3 movementScale=new Vector3(1.0f,1.0f,1.0f);		
		movementScale.x+=Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x)/30.0f;
		movementScale.z+=Mathf.Abs(GetComponent<Rigidbody2D>().velocity.y)/30.0f;
		movementScale.y=1.0f;		
		particleImage.gameObject.transform.localScale=movementScale;
	}
	void OnCollisionEnter2D(Collision2D other){
		if(currentState==STATES.WATER && other.gameObject.tag=="DynamicParticle")
        { 
			if(other.collider.GetComponent<DynamicParticle>().currentState==STATES.LAVA)
            {
				SetState(STATES.GAS);
			}
		}

	}
	
}

using UnityEngine;
using System.Collections;

//	Controller for the Cake Monsters
//	It moves normally. Everytime it hits a block, it changes direction
//	
//	Whenever it gets hit by the bomb, depending on what type of cake it is (based on color) 
//		1. Red: dead immediately
//		2. Green: has 3 healths, so player needs to hit it 3 times. And everytime it gets hit it moves faster
//		3. Purple: dead immediately and spawn 1 red cake and 1 green cake
//		All of these detections are done in Destroy script. This controller provides the methods to execute each scenarios.


public class cakeController : MonoBehaviour {

	Animator anim;
	SpriteRenderer spriteRenderer;
	public float speed;
	public string type;

	// several flags used to control cake movement
	Vector3 position; // of the monster
	public bool isWalkingLeft; 
	public bool movingUpDownFlag;
	float direction;
	public bool stopMoving;
	public int hitCount = 0; // used by green

	// temp variables updated frequently
	string colliderTag = "";
	float randomNumber = 0.0f;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer>();
		speed = GlobalVars.monsterSpeed;

		position = transform.position;
		isWalkingLeft = anim.GetBool ("isWalkingLeft");
		if (isWalkingLeft)
			direction = -1.0f;
		else
			direction = 1.0f;
		movingUpDownFlag = false;
		stopMoving = false;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y)*(-1);
		if (GetComponent<Destroy> ().destroyIsChecked)
			stopMoving = true;
		// stop moving could also be set in freeze and changemovement below
		if (stopMoving)
			return;

		if (isWalkingLeft) {
			direction = -1.0f;
			anim.SetBool("isWalkingLeft",true);
		} else {
			direction = 1.0f;
			anim.SetBool("isWalkingLeft",false);
		}

		position = transform.position;
		if (movingUpDownFlag) {
			transform.position = new Vector3 (position.x, position.y + direction * speed, 0.0f);
		}
		else {
			transform.position = new Vector3 (position.x + direction * speed, position.y, 0.0f);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		colliderTag = coll.collider.gameObject.tag;
		//Debug.Log (gameObject.name+" - "+colliderTag);
		if (colliderTag == "destructible" || colliderTag == "nonDestructible" || colliderTag == "actualMap" || colliderTag == "bomb") {
			randomNumber = Random.Range(0.0f,1.0f);
			if (randomNumber <=0.5) { // either change vert-horz flags or simply change direction
				movingUpDownFlag = !movingUpDownFlag;
			}
			else {
				direction = -direction;
				isWalkingLeft = !isWalkingLeft;
				anim.SetBool ("isWalkingLeft", isWalkingLeft);
			}
			position = transform.position;
			transform.position = new Vector3(Mathf.Round(position.x),Mathf.Round(position.y),0.0f);

		} else if (colliderTag == "monster" || colliderTag == "item") { // allowing monsters to go thru each other
			Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), coll.gameObject.GetComponent<CircleCollider2D> ());
		} else {
			Debug.Log (gameObject.name+" collides to object with tag "+colliderTag);
		}				
	}


	// used by green cake
	public IEnumerator freeze(){
		Debug.Log (gameObject.name+" is hit ");
		stopMoving = true;
		anim.enabled = false;
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.7f); 
		yield return new WaitForSeconds(0.75f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.4f); 
		yield return new WaitForSeconds(0.75f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.2f); 
		yield return new WaitForSeconds(0.75f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,1.0f);
		speed *= 1.35f;
		anim.enabled = true;
		stopMoving = false;
		GetComponent<Destroy> ().destroyIsChecked = false;
		hitCount++;
	} 

	// used when boss cake spawns new cakes
	public IEnumerator changeMovement(){
		yield return new WaitForSeconds(1.0f);
		direction = -direction;
		isWalkingLeft = !isWalkingLeft;
		anim.SetBool ("isWalkingLeft", isWalkingLeft);
		position = transform.position;
		transform.position = new Vector3(Mathf.Round(position.x),Mathf.Round(position.y),0.0f);
		
	}
}

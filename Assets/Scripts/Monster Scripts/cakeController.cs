using UnityEngine;
using System.Collections;

// a controller script for common movement behavior of cake monster

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
	bool stopMoving;
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
		Debug.Log (gameObject.name+" - "+colliderTag);
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

		} else if (colliderTag == "monster") { // allowing monsters to go thru each other
			Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), coll.gameObject.GetComponent<CircleCollider2D> ());
		} else {
			Debug.Log (gameObject.name+" collides to object with tag "+colliderTag);
		}				
	}


	// used by green cake
	public IEnumerator freeze(){
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

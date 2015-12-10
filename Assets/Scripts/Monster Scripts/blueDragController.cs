using UnityEngine;
using System.Collections;

//	Controller for the Blue Dragon Monster = Boss
//	Every time interval (const at first but will decrease every time it gets hit), one of the following could happen:
//		1. It moves until its blocked by destructible or nondestructible objects. 
//		2. Perform special ability = Smash the ground and detonate all active bombs on the map immediately
//	Whenever it gets hit by the bomb, it looses one health (max at first is 5), and its attribute is updated. 
//		This is done in the freeze() method which is called by the Destroy script also attached to this monster. 
//			Basically the method creates the fading effect and then return the monster back to its normal visual state

public class blueDragController : MonoBehaviour {

	Animator anim;
	SpriteRenderer spriteRenderer;
	public float speed;
	public float rageRate;

	// several flags used to control movement
	Vector3 position; // of the monster
	public bool movingUpDownFlag;
	float direction; // unit in calculating position : -1 = move left/down, 1 = move right/up
	int idleValue; // -1 = left, 1 = right
	bool isMoving;
	public bool isDying;
	public int hitCount = 5; 
	public float idleTimer = 3.0f; // monster stay idle for 3 seconds then move
	float idleRate = 0.0f;

	// temp variables updated frequently
	string colliderTag = "";
	Vector3 newPos = Vector3.zero;
	float randomNumber = 0.0f;

	// Initialization and set up flags, variables to control monster movement and behaviors
	void Start () {
		anim = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer>();
		speed = GlobalVars.monsterSpeed * 2;

		position = transform.position;
		idleValue = anim.GetInteger ("idle"); // -1 by def = idle left

		if (idleValue == -1)
			direction = -1.0f;
		else
			direction = 1.0f;
		movingUpDownFlag = false;
		isMoving = false;
		rageRate = 0.15f;
		isDying = false;
		idleTimer = 3.0f - idleRate;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y)*(-1);

		if (GetComponent<Destroy> ().destroyIsChecked)
			isDying = true;
		if (isDying)
			return;

		if (idleTimer > 0.0f){
			idleTimer -= Time.deltaTime;
			return; // do nothing - let monster stay idle or simply freeze it in case its being destroyed?
		}

		if (isMoving) {
			position = transform.position;
			if (idleValue == -1){
				direction = -1.0f; anim.SetBool ("isWalkingLeft", true);
			}
			else{
				direction = 1.0f; anim.SetBool ("isWalkingRight", true);
			}
			if (movingUpDownFlag) {
				transform.position = new Vector3 (position.x, position.y + direction * speed, 0.0f);
			} else {
				transform.position = new Vector3 (position.x + direction * speed, position.y, 0.0f);
			}
		} else {
			if (GetComponent<Destroy> ().destroyIsChecked)
				return; // meaning its being destroyed - do nothing -  not moving or perform any animation 

			if (!anim.GetBool ("rage")) {
				randomNumber = Random.Range(0.0f,1.0f);
				Debug.Log("random num "+randomNumber);
				if (randomNumber <= rageRate) { // rage
					anim.SetBool ("rage",true);
					// we dont update timer and let it be like this so that monster is not moving while the attack anim is happening				
				}
				else { // enable the moving switch - move it until blocked
					if (idleValue == -1){
						anim.SetBool ("isWalkingLeft", true); // transition from idle left to walking left
					}
					else if(idleValue == 1){
						anim.SetBool ("isWalkingRight", true); // transition from idle right to walking right
					}
					else {
						Debug.Log("Unidentified idlevalue: "+ idleValue);
					}
					isMoving = true; 
				}
			}
			else {
				//Debug.Log ("Monster raging is happening - do nothing");
			}
		}
	}
	void OnCollisionEnter2D(Collision2D coll) {
		ContactPoint2D[] temp = coll.contacts;
		Debug.Log ("Tot coll "+ temp.Length);
		if (temp.Length == 1) {
			colliderTag = coll.collider.gameObject.tag;
			Debug.Log (gameObject.name + " collides with object with tag " + colliderTag);
			if (colliderTag == "destructible" || colliderTag == "nonDestructible" || colliderTag == "actualMap") {
				isMoving = false;
				// reset timer
				idleTimer = 3.0f - idleRate; // also means that it will stop moving
				
				if (idleValue == -1)
					anim.SetBool ("isWalkingLeft", false); // facing left
				else
					anim.SetBool ("isWalkingRight", false);
				// if colliding with anything except char - stop and change vert to horz or horz to ver
				movingUpDownFlag = !movingUpDownFlag;
				idleValue = (Random.Range (0.0f, 1.0f) <= 0.5f) ? idleValue : (-idleValue); // either keep direction its facing or change it
				
				anim.SetInteger ("idle", idleValue); // update facing direction
				
				Debug.Log("reset timer - updown flag" + movingUpDownFlag);
				position = transform.position;
				newPos = new Vector3 (Mathf.Round (position.x), Mathf.Round (position.y), 0.0f);
				//if (Vector3.Distance(newPos,coll.collider.gameObject.transform.position)<= 2.5f)
					//newPos = offsetPosPerStuck(newPos);
				transform.position = newPos;
				
			} else if (colliderTag == "monster" || colliderTag == "item") { // allowing monsters to go thru each other
				Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), coll.gameObject.GetComponent<CircleCollider2D> ());
			} else if(colliderTag == "bomb"){
				Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), coll.gameObject.GetComponent<CircleCollider2D> ());
				Destroy(coll.gameObject);
				if (hitCount>0) hitCount--;
				Debug.Log ("Yummy!!!");
			} else {
				Debug.Log (gameObject.name + " collides to object with tag " + colliderTag);
			}
		} else {
			foreach(ContactPoint2D cp in temp){
				Debug.Log("Metadata: " + cp.collider.gameObject.name + " - " + cp.collider.gameObject.tag);
			}
		}
	}

	// perform attack
	public void attack(){
		GameObject[] allBombs = GameObject.FindGameObjectsWithTag ("bomb");
		if (allBombs != null) {
			Debug.Log ("Active bombs count "+ allBombs.Length);
			foreach (GameObject dbomb in allBombs){
				if (dbomb != null) {
					if (!dbomb.GetComponent<bombController>().destroyIsChecked)
						dbomb.GetComponent<bombController>().customDestroyBomb(true,false, false, false, false, false, false); // spawn all sprites
				}
			}
		}
	}
	// turn off the rage flag -> return to whatever anim before that
	public void finishRage(){
		anim.SetBool ("rage",false);
		// reset timer
		idleTimer = 3.0f - idleRate;
	}

	// freeze effect for monster when hit - then update its attribute
	public IEnumerator freeze(){
		isDying = true;
		anim.enabled = false;
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.7f); 
		yield return new WaitForSeconds(0.35f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.4f); 
		yield return new WaitForSeconds(0.35f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.2f); 
		yield return new WaitForSeconds(0.35f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,1.0f);

		//update attributes
		speed *= 1.25f;
		rageRate += 0.15f;
		rageRate = Mathf.Min (rageRate,0.8f); // capped rageRate at 0.8f
		idleRate += 0.4f;
		anim.enabled = true;
		isDying = false;
		GetComponent<Destroy> ().destroyIsChecked = false;
		hitCount++;
	} 




























}

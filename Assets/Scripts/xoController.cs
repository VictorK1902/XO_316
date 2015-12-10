using UnityEngine;
using System.Collections;

//	Controller script for main character
//	It simply: 1. Controls how the character moves by accessing the animator controller (a component used to controll animation clips)
//			   2. Detect objects colliding with this main character and perform appropriate action 
//			   3. Spawn bomb when user hits spacebar 
//			   4. Update character attribute if user consumes an item by hitting it

public class xoController : MonoBehaviour {
	
	public Animator anim;
	public Rigidbody2D rbody;
	public bool destroyIsChecked = false; // a flag to make sure that only one instance of bomb/monster get to trigger death of character 
											// in case multiple boms/monster try to destroy character 
	SpriteRenderer spriteRenderer;
	Vector2 directionBeforeBlocked;
	Vector2 targetPoint; // point to move to
	public GameObject canvas;

	int bombCloneCount = 0;	// used to name the bomb clone 
	float speed;
	// Use this for initialization
	void Start () {
		rbody = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer>();
		anim.SetBool("isWalking", false);
		anim.SetFloat ("input_x", 0.0f);
		anim.SetFloat ("input_y", -1.0f);
		speed = GlobalVars.xoSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y)*(-1);
		// 1. Detect if player hits spacebar --> place a bomb
		if (Input.GetKeyDown ("space")) {
			Debug.Log (GlobalVars.bombCount +"-"+ GlobalVars.maxBomb);
			if (GlobalVars.bombCount >= GlobalVars.maxBomb) return;
			GlobalVars.bombCount++;
			bombCloneCount++;
			GameObject dBomb = Instantiate (Resources.Load ("Prefabs/bomb")) as GameObject;
			dBomb.transform.position = new Vector3(Mathf.Round(transform.position.x),Mathf.Round(transform.position.y),0.0f);
			dBomb.name = "bomb clone " + bombCloneCount;
			dBomb.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(dBomb.transform.position.y)*(-1);
		}

		// 2. movement detect and handling
		if (!anim.enabled) {
			rbody.MovePosition(rbody.position);
			return;
		}
		Vector2 movement = new Vector2 (Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
		if (movement != Vector2.zero) {
			anim.SetBool ("isWalking", true);
			anim.SetFloat ("input_x", movement.x);
			anim.SetFloat ("input_y", movement.y);
		} else {
			anim.SetBool("isWalking", false);
		}
		targetPoint = rbody.position + movement * speed;
		//Debug.Log (targetPoint);
		rbody.MovePosition (targetPoint);		
	}

	public void customDestroyCharacter(){
		GetComponent<CircleCollider2D> ().enabled = false;
		StartCoroutine (timerDestroy());
	}

	IEnumerator timerDestroy(){
		anim.enabled = false;
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.8f); 
		yield return new WaitForSeconds(0.25f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.6f); 
		yield return new WaitForSeconds(0.25f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.4f); 
		yield return new WaitForSeconds(0.25f);
		spriteRenderer.color = new Color (3.0f,1.0f,1.0f,0.2f); 
		yield return new WaitForSeconds(0.25f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.0f); 
		pauseGame ();
		Destroy (gameObject);
	}

	void pauseGame(){
		// 1. freeze all monsters
		GameObject[] allMonsters = GameObject.FindGameObjectsWithTag ("monster");
		if (allMonsters != null) {
			foreach (GameObject go in allMonsters){
				go.GetComponent<Animator>().enabled = false;
				pauseMonster(go);
			}
		}
		//canvas.SetActive (true);
	}

	public void pauseMonster(GameObject go){
		if (go.GetComponent<cakeController> () != null) {
			go.GetComponent<cakeController> ().stopMoving = true;
		}
		if (go.GetComponent<blueDragController> () != null) {
			go.GetComponent<blueDragController> ().isDying = true; // kinda cheating since its not theoretically "dying" - but simply stop moving- but since
																	// I used this flag to stop moving in blue drag so
		}
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
		Debug.Log ("Player collided with "+coll.collider.name + "-" +coll.collider.tag);
		if (coll.gameObject.tag == "monster") {
			customDestroyCharacter ();
		}
		if (coll.gameObject.tag == "item") {
			Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), coll.gameObject.GetComponent<CircleCollider2D> ());
			coll.gameObject.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder-1;
			StartCoroutine(delayItemConsume(coll.gameObject));
		}
	}

	IEnumerator delayItemConsume(GameObject obj){
		yield return new WaitForSeconds(0.25f);
		Destroy(obj);
		updatePlayerAttr (obj.name);
	}

	void updatePlayerAttr(string itemName){
		Debug.Log ("Consuming item "+ itemName);
		if (itemName.Contains ("boots")) {
			speed = speed * 1.05f;
			Debug.Log("New XO speed: "+ speed);
		} else if (itemName.Contains ("fire")) {
			GlobalVars.bombRadius++;
			Debug.Log("New XO bomb radius:"+ GlobalVars.bombRadius);
		} else if (itemName.Contains ("mask")) {
			speed = speed*0.5f; // decrease speed for 5 seconds
			Debug.Log("Slow XO speed: "+ speed);
			StartCoroutine(slowSpeed());
		} else if (itemName.Contains ("shotgun")) {
			GlobalVars.maxBomb++;
			Debug.Log("New XO maxbomb allowed:"+ GlobalVars.maxBomb);
		} else {
			Debug.Log("Unidentified name");
		}
	}

	IEnumerator slowSpeed(){
		yield return new WaitForSeconds(5.0f);
		speed = speed * 2;
	}

}

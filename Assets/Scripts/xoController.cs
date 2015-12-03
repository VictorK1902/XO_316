using UnityEngine;
using System.Collections;

// controller script for main character

public class xoController : MonoBehaviour {
	
	public Animator anim;
	public Rigidbody2D rbody;
	public bool destroyIsChecked = false; // a flag to make sure that only one instance of bomb/monster get to trigger death of character 
											// in case multiple boms/monster try to destroy character 

	SpriteRenderer spriteRenderer;
	Vector2 directionBeforeBlocked;
	Vector2 targetPoint; // point to move to
	public int maxBomb = 1;
		
	// Use this for initialization
	void Start () {
		rbody = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer>();
		anim.SetBool("isWalking", false);
		anim.SetFloat ("input_x", 0.0f);
		anim.SetFloat ("input_y", -1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y)*(-1);
		// 1. Bomb key detect
		if (Input.GetKeyDown ("space")) {
			GameObject dBomb = Instantiate (Resources.Load ("Prefabs/bomb")) as GameObject;
			dBomb.transform.position = new Vector3(Mathf.Round(transform.position.x),Mathf.Round(transform.position.y),0.0f);
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
		targetPoint = rbody.position + movement * GlobalVars.speed;
		//Debug.Log (targetPoint);
		rbody.MovePosition (targetPoint);
		
	}
	public void customDestroyCharacter(){
		StartCoroutine (timer());
	}
	IEnumerator timer(){
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
		Destroy (gameObject);
	}

	void OnCollisionEnter2D(Collision2D coll) {
		Debug.Log ("Player collided with "+coll.collider.name + "-" +coll.collider.tag);
		if (coll.gameObject.tag == "monster") {
			customDestroyCharacter ();
		}
		
	}



}

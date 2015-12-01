using UnityEngine;
using System.Collections;

public class xoController : MonoBehaviour {
	
	public Animator anim;
	public Rigidbody2D rbody;
	public bool destroyIsChecked = false;
	SpriteRenderer spriteRenderer;
	Vector2 directionBeforeBlocked;
	Vector2 targetPoint;
	
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
		if (!anim.enabled) {
			rbody.MovePosition(rbody.position);
			return;
		}
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y)*(-1);
		Vector2 movement = new Vector2 (Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
		if (movement != Vector2.zero) {
			anim.SetBool ("isWalking", true);
			anim.SetFloat ("input_x", movement.x);
			anim.SetFloat ("input_y", movement.y);
		} else {
			anim.SetBool("isWalking", false);
		}
		targetPoint = rbody.position + movement * 0.04f;
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




}

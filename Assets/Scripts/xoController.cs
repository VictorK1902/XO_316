using UnityEngine;
using System.Collections;

public class xoController : MonoBehaviour {
	
	public Animator anim;
	public Rigidbody2D rbody;
	Vector2 directionBeforeBlocked;
	Vector2 targetPoint;
	
	// Use this for initialization
	void Start () {
		rbody = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		anim.SetBool("isWalking", false);
		anim.SetFloat ("input_x", 0.0f);
		anim.SetFloat ("input_y", -1.0f);
	}
	
	// Update is called once per frame
	void Update () {
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
	void OnCollisionEnter2D(Collision2D col)
	{	

	}




}

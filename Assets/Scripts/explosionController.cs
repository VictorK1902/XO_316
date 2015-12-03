using UnityEngine;
using System.Collections;

// controller script for explosion sprite - simply got destroyed after timer = 0.65f

public class explosionController : MonoBehaviour {

	float timer = 0.65f;
	
	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y)*(-1)-1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer -= Time.deltaTime;
		//Debug.Log (Time.deltaTime +" - "+timer);
		if (timer < 0.0f) {
			Destroy(gameObject);
		}
	}
}

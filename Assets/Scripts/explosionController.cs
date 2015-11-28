using UnityEngine;
using System.Collections;

public class explosionController : MonoBehaviour {

	float timer = 0.65f;
	
	// Use this for initialization
	void Start () {
		
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

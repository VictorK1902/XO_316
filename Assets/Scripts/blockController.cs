using UnityEngine;
using System.Collections;

public class blockController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Debug.Log (GetComponent<Rigidbody2D> ().velocity);
		//Debug.Log (transform.position);
		//GetComponent<Rigidbody2D> ().velocity = Vector2.zero;	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy(){
		Debug.Log ("Object "+gameObject.name + " is destroyed");
	}
}

using UnityEngine;
using System.Collections;

public class renderingOrder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y)*(-1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

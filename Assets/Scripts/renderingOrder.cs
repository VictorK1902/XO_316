using UnityEngine;
using System.Collections;

// script to define sorting order of nondestructible obejects

public class renderingOrder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject[] nondestructibleObjects = GameObject.FindGameObjectsWithTag ("nonDestructible");
		if (nondestructibleObjects != null) {
			foreach (GameObject go in nondestructibleObjects){
				go.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(go.transform.position.y)*(-1);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

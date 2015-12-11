using UnityEngine;
using System.Collections;

//	Controller script to define sorting order of nondestructible and destructible objects
//	Sorting order basically makes sure that some object is rendered behind the other to makesure that everything appears
//		as 3/4 perspective	

public class renderingOrder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject[] returnedObjects = GameObject.FindGameObjectsWithTag ("nonDestructible");
		if (returnedObjects != null) {
			foreach (GameObject go in returnedObjects){
				go.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(go.transform.position.y)*(-1);
			}
		}
		returnedObjects = GameObject.FindGameObjectsWithTag ("destructible");
		if (returnedObjects != null) {
			foreach (GameObject go in returnedObjects){
				go.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(go.transform.position.y)*(-1);
			}
		}

		returnedObjects = GameObject.FindGameObjectsWithTag ("item");
		if (returnedObjects != null) {
			foreach (GameObject go in returnedObjects){
				go.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(go.transform.position.y)*(-1);
			}
		}

		//Physics2D.IgnoreCollision (GameObject.Find("Cake Monster").GetComponent<CircleCollider2D>(),GameObject.Find("Cake Monster (1)").GetComponent<CircleCollider2D>());

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

// Controller script for explosion sprite - simply got destroyed after timer = 0.65f
// Used ONLY BY DESTRUCTIBLE TERRAIN

public class explosionController : MonoBehaviour {

	float timer = 0.65f;
	float randomNum = 0.0f;
	public bool isblockExplosion;
	float itemDropRate;

	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y)*(-1)-1;
		itemDropRate = GlobalVars.itemDropRate;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer -= Time.deltaTime;
		//Debug.Log (Time.deltaTime +" - "+timer);
		if (timer < 0.0f) {
			Destroy(gameObject);
			if (isblockExplosion){
				randomNum = Random.Range(0.0f,1.0f);
				if (randomNum <= itemDropRate) spawnItem();
			}
		}
	}
	//Upon destroying the exp sprite - spawn item at the exact same location.
	void spawnItem(){
		GameObject item;
		randomNum = Random.Range (0.0f, 1.0f);
		Debug.Log ("Item drop rate "+ randomNum);
		// shotgun = fire = 30%, boots = mask = 20%
		if (randomNum < 0.3f) {
			item = Instantiate (Resources.Load ("Prefabs/Items/shotgun")) as GameObject;
		} else if (randomNum < 0.6f) {
			item = Instantiate (Resources.Load ("Prefabs/Items/fire")) as GameObject;
		} else if (randomNum < 0.8f) {
			item = Instantiate (Resources.Load ("Prefabs/Items/boots")) as GameObject;
		} else {
			item = Instantiate (Resources.Load ("Prefabs/Items/mask")) as GameObject;
		}
		item.transform.position = transform.position;
		item.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;

	}
}

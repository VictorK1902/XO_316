using UnityEngine;
using System.Collections;

// A destroy interface used by monster, item and destructibel terrain

public class Destroy : MonoBehaviour {

	bool isMonster, isItem, isDestructibleTerrain; // only one should be true per gameobject using this 
	public bool destroyIsChecked;
	public bool hasDeadAnim;
	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y)*(-1);
		// define proper flag for this object because 3 types of object use it: monster, item, and destructible terrain block
		// nondestructible terrain simply use a rendering object script to determine sorting order during run time
		if (gameObject.tag == "monster")
			isMonster = true;
		else
			isMonster = false;
		if (gameObject.tag == "item")
			isItem = true;
		else
			isItem = false;
		if (gameObject.tag == "destructible")
			isDestructibleTerrain = true;
		else
			isDestructibleTerrain = false;
		destroyIsChecked = false;
	}

	public void customDestroy(){
		Destroy (gameObject);
	}

	public void triggerDeath(){
		if (hasDeadAnim) { // for monster that actually has death animation clip - usually set in editor by hand
			GetComponent<Animator> ().SetBool ("isDead", true);
			GlobalVars.speed = GlobalVars.speed + 0.02f; // ughh this is just to play with character speed for now
		}
		else { // for those that dont -> use my hand-made effect
			StartCoroutine(timer());
		}
	}

	// we're not using this for now
	IEnumerator timer(){
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		GetComponent<Animator>().enabled = false;
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.8f); 
		yield return new WaitForSeconds(0.25f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.6f); 
		yield return new WaitForSeconds(0.25f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.4f); 
		yield return new WaitForSeconds(0.25f);
		spriteRenderer.color = new Color (3.0f,1.0f,1.0f,0.2f); 
		yield return new WaitForSeconds(0.25f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.0f);
		customDestroy ();
	}


	void OnTriggerEnter2D(Collider2D coll) {
		Debug.Log ("called from "+gameObject.name + " - collided with "+ coll.gameObject.name);
		StartCoroutine(secondtimer(coll.gameObject));
	}
	IEnumerator secondtimer(GameObject character){
		yield return new WaitForSeconds(0.15f);
		character.GetComponent<xoController> ().customDestroyCharacter ();
	}




}

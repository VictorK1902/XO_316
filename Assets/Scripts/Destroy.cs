using UnityEngine;
using System.Collections;

//	A destroy Interface used by monster, item and destructible terrain
//	Even though destructible terrain barely used this - It only used the destroyischecked flag..
//	Some notables methods includes:
//		1. triggerDeath(): beginning state of destroying an object - used by monster and item. If monster has a animation for being dead it will use that
//			If not, it will use my hand made method timerDestroy() 
//		2. timerDestroy(): a custom method to repesent the fading effect of the object as its being destroyed.
//		3. monsterCheck(): a method to control and determine what to do with the attaching monster (different monsters have different controllers)
//

public class Destroy : MonoBehaviour {

	public bool isMonster, isItem, isDestructibleTerrain; // only one should be true per gameobject using this 
	public bool destroyIsChecked;
	public bool hasDeadAnim;
	// Use this for initialization
	void Start () {
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

	// used by timer below and also by monster with death animation upon the animation finishes
	public void customDestroy(){
		Destroy (gameObject);
	}

	// This is used by monster and item
	public void triggerDeath(){
		bool goAheadAndDestroy = true;
		// 1. Check with the monster controller first if its ok to destroy the monster
		if (isMonster)
			goAheadAndDestroy = monsterCheck ();

		// 2. For item - this will always occur 
		// 		for monster - destroy only if the checking test passes
		if (goAheadAndDestroy) {
			if (isMonster) {
				GameObject go = GameObject.Find("XO_character");
				if (go!=null){
					GameObject.Find("XO_character").GetComponent<xoController>().monsterCount--;
					Debug.Log("MonsterCount "+ GameObject.Find("XO_character").gameObject.GetComponent<xoController>().monsterCount);
				}
			}
			if (hasDeadAnim) { // for monster that actually has death animation clip - usually set in editor by hand
				GetComponent<Animator> ().SetBool ("isDead", true);
				GlobalVars.xoSpeed = GlobalVars.xoSpeed*1.05f; // ughh this is just to play with character speed for now
			} else { // for those that dont -> use my hand-made effect
				StartCoroutine (timerDestroy ());
			}
		}
	}

	// wrapper to handle different kinds of monsters destroy behaviors - right now we're using the name of the controller
	//		of the monster to detect what kind of monster it is
	bool monsterCheck(){
		bool result = true; // default result

		// 1. Cake Monsters - we have 3 type: normal, green - move faster and die only getting caught by bomb 3 times, and boss - spawn 2 new cakes
		if (GetComponent<cakeController> () != null) {
			string type = GetComponent<cakeController>().type;
			if (type == "normal") result = true;
			else if (type == "green") {
				if (GetComponent<cakeController>().hitCount < 2){
					result = false;
					StartCoroutine(GetComponent<cakeController>().freeze());
				}
				else result = true;
			}
			else { // boss cake
				result = true;
				GameObject newCake1 = Instantiate (Resources.Load ("Prefabs/Monsters/Normal Cake Monster")) as GameObject;
				GameObject newCake2 = Instantiate (Resources.Load ("Prefabs/Monsters/Green Cake Monster")) as GameObject;
				newCake1.transform.position = gameObject.transform.position;
				newCake2.transform.position = gameObject.transform.position;
				newCake1.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(newCake1.transform.position.y)*(-1);
				newCake2.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(newCake2.transform.position.y)*(-1);
				StartCoroutine(newCake2.GetComponent<cakeController>().changeMovement());
				GameObject.Find("XO_character").GetComponent<xoController>().monsterCount++;
				GameObject.Find("XO_character").GetComponent<xoController>().monsterCount++;
			}
		}

		// 2. Boss Blue Drag
		if (GetComponent<blueDragController> () != null) {
			if (GetComponent<blueDragController> ().hitCount < 5){
				Debug.Log ("hitcount "+GetComponent<blueDragController> ().hitCount);
				StartCoroutine(GetComponent<blueDragController> ().freeze());
			}
		}
		return result;
	}

	// This is custom fading-effect for object being destroyed
	IEnumerator timerDestroy(){
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if (isMonster) GetComponent<Animator>().enabled = false;
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
}

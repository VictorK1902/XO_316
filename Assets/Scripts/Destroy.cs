using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {

	bool isMonster, isItem, isDestructibleTerrain;
	public bool destroyIsChecked;
	public bool hasDeadAnim;
	// Use this for initialization
	void Start () {
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
		if (hasDeadAnim) { // for monster that actually has death animation clip
			GetComponent<Animator> ().SetBool ("isDead", true);
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
}

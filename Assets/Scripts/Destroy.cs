using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public void customDestroy(){
		StartCoroutine (timer());
	}
	IEnumerator timer(){
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.8f); 
		yield return new WaitForSeconds(0.2f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.6f); 
		yield return new WaitForSeconds(0.2f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.4f); 
		yield return new WaitForSeconds(0.2f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.2f); 
		yield return new WaitForSeconds(0.2f);
		spriteRenderer.color = new Color (1.0f,1.0f,1.0f,0.0f); 
		Destroy (gameObject);
	}
}

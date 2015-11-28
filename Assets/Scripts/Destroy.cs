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
		yield return new WaitForSeconds(0.75f);
		Destroy (gameObject);
	}
}

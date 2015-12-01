using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalVars : MonoBehaviour {

	public List<GameObject> objectsToBeDestroyed;
	public int currentBombCount;

	// Use this for initialization
	void Start () {
		objectsToBeDestroyed = new List<GameObject>();
		currentBombCount = 4;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

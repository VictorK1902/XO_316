using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalVars : MonoBehaviour {

	public static List<GameObject> objectsToBeDestroyed = new List<GameObject>();
	public static int bombRadius = 4;
	public static float speed = 0.04f;
	public static float bombTimer = 3.0f;
	public static float horzBlockCounts = 19;
	public static float vertBlockCounts = 13;

	// Use this for initialization
	void Start () {
		//objectsToBeDestroyed = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

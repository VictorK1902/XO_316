using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A class that provides all static variables to be accessed by other components in the game
// Mostly related to attribute of the character, monster movement, item or the bomb.

public class GlobalVars : MonoBehaviour {

	// used by bombcontroller - contains objects to be destroyed. Refreshed everytime a chain bomb is finished 
	public static List<GameObject> objectsToBeDestroyed = new List<GameObject>(); 
																						
	public static int bombRadius = 1;
	public static float xoSpeed = 0.04f;
	public static float bombTimer = 2.15f;
	public static int maxBomb = 1;
	public static int bombCount = 0;

	public static float monsterSpeed = 0.02f;
	public static float itemDropRate = 0.2f;

	public bool isBossMap;

	public void restartGame(){
		Application.LoadLevel ("Level1");
		bombRadius = 1;
		xoSpeed = 0.04f;
		bombTimer = 2.0f;
		maxBomb = 1;
		bombCount = 0;
		monsterSpeed = 0.02f;
		itemDropRate = 0.2f;

	}

	void Start(){
		if (isBossMap) {
			itemDropRate = 0.5f;
			Debug.Log("Drop Rate double check "+ itemDropRate);
		}
	}

	void Update(){
		/*
		if (GameObject.Find ("XO_character") == null)
			StartCoroutine (GameObject.Find ("Fade").GetComponent<fading> ().timeFading ());
		else {
			Debug.Log("Not dead yet");
		}
		*/
	}

	
}

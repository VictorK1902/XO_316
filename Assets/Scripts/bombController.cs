using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bombController : MonoBehaviour {

	float timer = 2.0f;
	RaycastHit2D[]  linecast;

	int radius = 5;
	bool called = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer -= Time.deltaTime;
		//Debug.Log (Time.deltaTime +" - "+timer);
		if (timer < 0.0f) {
			Debug.Log ("Boom");
			gameObject.SetActive(false);
			if (!called){
				spawnExplosionSprite();
				called = true;
			}
			Destroy(gameObject);
		}
	}

	void spawnExplosionSprite(){
		Vector3 bombpos = transform.position;
		Vector3 bombCenter = bombpos + new Vector3(0.5f,0.5f,0.0f);

		GameObject expCenter = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
		expCenter.transform.position = bombpos;
		SpawnExplosion (bombCenter, new Vector3 (radius*(-1.0f)-0.4f,0.0f,0.0f), "left", bombpos);
		SpawnExplosion (bombCenter, new Vector3 (radius*(1.0f)+0.4f,0.0f,0.0f), "right", bombpos);
		SpawnExplosion (bombCenter, new Vector3 (0.0f, radius*(1.0f)+0.4f,0.0f), "up", bombpos);
		SpawnExplosion (bombCenter, new Vector3 (0.0f, radius*(-1.0f)-0.4f,0.0f), "down", bombpos);

	}

	void SpawnExplosion(Vector3 bombCenter, Vector3 end, string direction, Vector3 bombPos){
		Vector3 spawnPos = Vector3.zero;
		Vector3 tmp = offsetVector(direction);

		// line cast all return all objects hit by the line from startPoint to endPoint
		RaycastHit2D[] detectedObjects = Physics2D.LinecastAll(bombCenter, bombCenter + end);

		if (detectedObjects.Length == 0) {
			//maxout bomb radius
			for (int i = 0, j=1; i < radius; i++){
				GameObject explosionTempSprite = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
				explosionTempSprite.transform.position = bombPos + new Vector3 (j * tmp.x, j * tmp.y, 0.0f);
				j++;
			}
		} else { // collide at least one
			string tag = "";
			bool notDone = true;
			GameObject firstTerrainHit = detectedObjects[0].collider.gameObject; // will be updated
			int firstTerrainHitIndex = 0;
			//1. we look for the first terrain hit
			for (int i = 0; i < detectedObjects.Length && notDone;i++){
				tag = detectedObjects[i].collider.gameObject.tag;
				if (tag == "nonDestructible" || tag == "destructible"){
					// if we find it - then we save its meta data to process later
					spawnPos = calculateSpawnPosition(detectedObjects[i].collider.transform.position, direction);
					firstTerrainHit = detectedObjects[i].collider.gameObject;
					firstTerrainHitIndex = i;
					notDone = false;
				}
			} 
			//
			// note that monster might be hit before terrain
			//
			//2. now we spawn exp sprites - and destroy monster/character if theres any between bom pos and first hit terrain
			if (tag == "nonDestructible"){
				int count = (int) Vector2.Distance(spawnPos,bombPos);
				for (int i = 0, j=1; i < count; i++){
					GameObject explosionTempSprite = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
					explosionTempSprite.transform.position = bombPos + new Vector3 (j * tmp.x, j * tmp.y, 0.0f);
					j++;
				}
				// also we destroy any monster or character in between
				for (int i = 0; i < firstTerrainHitIndex;i++){
					// we need to call customDestroy 
					detectedObjects[i].collider.gameObject.GetComponent<Destroy>().customDestroy();
				}
			}
			else if (tag == "destructible") {
				int count = (int) Vector2.Distance(spawnPos,bombPos);
				for (int i = 0, j=1; i < count; i++){
					GameObject explosionTempSprite = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
					explosionTempSprite.transform.position = bombPos + new Vector3 (j * tmp.x, j * tmp.y, 0.0f);
					j++;
				}
				// spawn explosion sprite for wood block
				firstTerrainHit.SetActive(false); // hide it first
				GameObject tmp1 = Instantiate(Resources.Load("Prefabs/blockExplosion") as GameObject);
				tmp1.transform.position = firstTerrainHit.transform.position;			
				Destroy (firstTerrainHit);

				// also we destroy any monster or character in between
				for (int i = 0; i < firstTerrainHitIndex;i++){
					// we need to call customDestroy 
					detectedObjects[i].collider.gameObject.GetComponent<Destroy>().customDestroy();
				}

			}
			else { // "" by default - meaning all objects hit are either monster or character
				for (int i = 0, j=1; i < radius; i++){
					GameObject explosionTempSprite = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
					explosionTempSprite.transform.position = bombPos + new Vector3 (j * tmp.x, j * tmp.y, 0.0f);
					j++;
				}
				// Then we destroy any monster or character in between
				for (int i = 0; i < detectedObjects.Length;i++){
					// we need to call customDestroy 
					detectedObjects[i].collider.gameObject.GetComponent<Destroy>().customDestroy();
				}
			}
		}
	} // end



	// utilities
	Vector3 calculateSpawnPosition(Vector3 givenPoint, string direction){
		Vector3 result = Vector3.zero;
		if (direction == "left"){
			result = givenPoint + new Vector3(1.0f,0.0f,0.0f);
		} else if (direction == "right"){
			result = givenPoint + new Vector3(-1.0f,0.0f,0.0f);
		} else if (direction == "up"){
			result = givenPoint + new Vector3(0.0f,-1.0f,0.0f);
		} else { // down
			result = givenPoint + new Vector3(0.0f,1.0f,0.0f);
		}
		return result;
	} // end
	Vector3 offsetVector(string direction){
		Vector3 result = Vector3.zero;
		if (direction == "left"){
			result = new Vector3(-1.0f,0.0f,0.0f);
		} else if (direction == "right"){
			result = new Vector3(1.0f,0.0f,0.0f);
		} else if (direction == "up"){
			result = new Vector3(0.0f,1.0f,0.0f);
		} else { // down
			result =  new Vector3(0.0f,-1.0f,0.0f);
		}
		return result;
	} // end

} // end class

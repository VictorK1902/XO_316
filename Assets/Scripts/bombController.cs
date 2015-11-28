using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bombController : MonoBehaviour {

	public float timer = 2.0f;
	List<RaycastHit2D> toBeDestroyed; // list of temp objects to be destroyed later when line-casting against other colliders on bomb path
	int radius = 6; // bomb radius
	// Use this for initialization
	void Start () {
		toBeDestroyed = new List<RaycastHit2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer -= Time.deltaTime;
		//Debug.Log (Time.deltaTime +" - "+timer);
		if (timer < 0.0f) {
			Debug.Log ("Boom");
			customDestroyBomb();
		}
	}

	public void customDestroyBomb(){
		Debug.Log ("About to destroy: "+ gameObject.name);
		gameObject.SetActive(false);
		spawnExplosionSprite();
		Destroy(gameObject);
	}

	void spawnExplosionSprite(){
		Vector3 bombpos = transform.position;
		Vector3 bombCenter = bombpos + new Vector3(0.5f,0.5f,0.0f);

		GameObject expCenter = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
		expCenter.transform.position = bombpos;
		SpawnExplosionInGivenDirection (bombCenter, new Vector3 (radius*(-1.0f)-0.4f,0.0f,0.0f), "left", bombpos);
		SpawnExplosionInGivenDirection (bombCenter, new Vector3 (radius*(1.0f)+0.4f,0.0f,0.0f), "right", bombpos);
		SpawnExplosionInGivenDirection (bombCenter, new Vector3 (0.0f, radius*(1.0f)+0.4f,0.0f), "up", bombpos);
		SpawnExplosionInGivenDirection (bombCenter, new Vector3 (0.0f, radius*(-1.0f)-0.4f,0.0f), "down", bombpos);

	}

	void SpawnExplosionInGivenDirection(Vector3 bombCenter, Vector3 end, string direction, Vector3 bombPos){
		Vector3 spawnPos = Vector3.zero;
		Vector3 unitVector = findUnitVector(direction);
		toBeDestroyed.Clear ();
		// line cast all return all objects hit by the line from startPoint to endPoint
		RaycastHit2D[] detectedObjects = Physics2D.LinecastAll(bombCenter, bombCenter + end);

		if (detectedObjects.Length == 0) {
			//No obstacles on the path - simply spawn the exp sprites
			for (int i = 0, j=1; i < radius; i++){
				GameObject explosionTempSprite = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
				explosionTempSprite.transform.position = bombPos + new Vector3 (j * unitVector.x, j * unitVector.y, 0.0f);
				j++;
			}
		} else { // collide at least one obstacles
			string obstacleTag = "";
			bool notDone = true;
			GameObject firstTerrainHit = detectedObjects[0].collider.gameObject; // will be updated
			int bombCountOnPath = 0; // use this to make sure that only the last bomb on the path gets to destroy destructible terrain

			//1. we look for the first terrain hit - this is the boundary to spawn exp sprites
			for (int i = 0; i < detectedObjects.Length && notDone;i++){
				obstacleTag = detectedObjects[i].collider.gameObject.tag;
				if (obstacleTag == "nonDestructible" || obstacleTag == "destructible"){
					// if we find it - then we save its meta data to process later
					spawnPos = calculateSpawnPosition(detectedObjects[i].collider.transform.position, direction);
					firstTerrainHit = detectedObjects[i].collider.gameObject;
					notDone = false;
				}
				else { // items, monsters and character - possibly another bomb included - note that exp sprites dont have collider so they wont get caught here
					if (obstacleTag == "bomb") 
						bombCountOnPath++;	
					toBeDestroyed.Add(detectedObjects[i]);
				}
			} 

			//2. now we spawn exp sprites 
			int count = 0;
			// find how many sprites we need to spawn
			if (obstacleTag == "nonDestructible" || obstacleTag == "destructible")
				count = (int) Vector2.Distance(spawnPos,bombPos);
			else  // "" by default - meaning all objects hit are either monster or character or bomb --> spawn all sprites equal to radius value
				count = radius;
			// spawn exp sprites
			for (int i = 0, j=1; i < count; i++){
				GameObject explosionTempSprite = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
				explosionTempSprite.transform.position = bombPos + new Vector3 (j * unitVector.x, j * unitVector.y, 0.0f);
				j++;
			}
			// also we spawn exp sprite for destructible terrain block - only if this bomb is the last or only on the path in given direction
			if (obstacleTag == "destructible" && bombCountOnPath == 0){
				// spawn explosion sprite for wood block
				firstTerrainHit.SetActive(false); // hide it first
				GameObject tmp1 = Instantiate(Resources.Load("Prefabs/blockExplosion")) as GameObject;
				tmp1.transform.position = firstTerrainHit.transform.position;			
				Destroy (firstTerrainHit);
			}

			// 3. Now we destroy any monster or character in between - or bomb
			for (int i = 0; i < toBeDestroyed.Count; i++){
				// we need to call customDestroy 
				destroyBasedOnTag(toBeDestroyed[i].collider.gameObject);
			}
		}
	} // end



	// utilities
	void destroyBasedOnTag(GameObject theObject){
		// we need to call customDestroy 
		if (theObject.tag == "bomb")
			theObject.GetComponent<bombController>().customDestroyBomb();
		else if (theObject.tag == "character")
			theObject.GetComponent<xoController>().customDestroyCharacter();
		else // items and monsters
			theObject.GetComponent<Destroy>().customDestroy();
	}
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
	Vector3 findUnitVector(string direction){
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

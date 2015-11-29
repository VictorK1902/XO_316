using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bombController : MonoBehaviour {

	public float timer = 2.0f;
	List<RaycastHit2D> toBeDestroyed; // list of temp objects to be destroyed later when line-casting against other colliders on bomb path
	int radius = 5; // bomb radius
	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y)*(-1);
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
		SpawnExplosionInGivenDirection (bombCenter, new Vector3 (radius*(-1.0f)-0.5f,0.0f,0.0f), "left", bombpos);
		SpawnExplosionInGivenDirection (bombCenter, new Vector3 (radius*(1.0f)+0.5f,0.0f,0.0f), "right", bombpos);
		SpawnExplosionInGivenDirection (bombCenter, new Vector3 (0.0f, radius*(1.0f)+0.5f,0.0f), "up", bombpos);
		SpawnExplosionInGivenDirection (bombCenter, new Vector3 (0.0f, radius*(-1.0f)-0.5f,0.0f), "down", bombpos);

	}

	void SpawnExplosionInGivenDirection(Vector3 bombCenter, Vector3 end, string direction, Vector3 bombPos){
		Vector3 spawnPos = Vector3.zero;
		Vector3 unitVector = findUnitVector(direction);
		toBeDestroyed.Clear ();
		// line cast all return all objects hit by the line from startPoint to endPoint
		// we do 2 times for proper gameplay purpose

		// A. Line cast no 1 - consider this as a main line cast as it will clear off most of things - since it is the first one
		// terrains (destructible and non destructible) and bombs are perfectly aligned with virtual grid so 1st line cast will handle them for sure
		Vector3 startPoint = findStartPointToLineCast (direction,bombCenter,0.2f);
		RaycastHit2D[] detectedObjects_no1 = Physics2D.LinecastAll(startPoint, startPoint + end);

		if (detectedObjects_no1.Length == 0) {
			//No obstacles on the path - simply spawn the exp sprites
			for (int i = 0, j=1; i < radius; i++){
				GameObject explosionTempSprite = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
				explosionTempSprite.transform.position = bombPos + new Vector3 (j * unitVector.x, j * unitVector.y, 0.0f);
				j++;
			}
		} else { // collide at least one obstacles
			string obstacleTag = "";
			bool notDone = true;
			GameObject firstTerrainHit = detectedObjects_no1[0].collider.gameObject; // will be updated
			int bombCountOnPath = 0; // use this to make sure that only the last bomb on the path gets to destroy destructible terrain

			//1. we look for the first terrain hit - this is the boundary to spawn exp sprites
			for (int i = 0; i < detectedObjects_no1.Length && notDone;i++){
				obstacleTag = detectedObjects_no1[i].collider.gameObject.tag;
				if (obstacleTag  == "character") Debug.Log("catch character by " + gameObject.name);
				if (obstacleTag == "nonDestructible" || obstacleTag == "destructible"){
					// if we find it - then we save its meta data to process later
					spawnPos = calculateSpawnPosition(detectedObjects_no1[i].collider.transform.position, direction);
					firstTerrainHit = detectedObjects_no1[i].collider.gameObject;
					notDone = false;
				}
				else { // items, monsters and character - possibly another bomb included - note that exp sprites dont have collider so they wont get caught here
					if (obstacleTag == "bomb") 
						bombCountOnPath++;	
					toBeDestroyed.Add(detectedObjects_no1[i]);
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

		// B. Line cast no 2 - try to catch any obstacles not being caught yet. 
		startPoint = findStartPointToLineCast (direction,bombCenter,-0.2f);
		RaycastHit2D[] detectedObjects_no2 = Physics2D.LinecastAll(startPoint, startPoint + end);
		// we dont need to spawn exp sprites again - just catch any monster not being caught yet on the path
		if (detectedObjects_no2.Length != 0) {
			string obstacleTag = "";
			for (int i = 0; i < detectedObjects_no2.Length;i++){
				obstacleTag = detectedObjects_no2[i].collider.gameObject.tag;
				if (obstacleTag == "nonDestructible" || obstacleTag == "destructible" || obstacleTag == "bomb"){
					// do nothing 
					// these objects are definitely already handled during the first cast because they are perfectly aligned with grid snap
				}
				else { // items, monsters and character 
					destroyBasedOnTag(detectedObjects_no2[i].collider.gameObject);
				}
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
	Vector3 findStartPointToLineCast(string direction, Vector3 input, float offset){
		if (direction == "left" || direction == "right") 
			return (input + new Vector3(0.0f,offset,0.0f));
		else
			return (input + new Vector3(offset,0.0f,0.0f));
	}

} // end class

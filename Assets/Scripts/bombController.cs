using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bombController : MonoBehaviour {

	public float timer;
	List<GameObject> toBeDestroyed; // list of temp objects to be destroyed later when line-casting against other colliders on bomb path
	int radius = 0; // bomb radius
	bool isTriggeredByDef;
	float timedecrease;
	public bool destroyIsChecked = false;
	bool isFirstBomb;

	// Use this for initialization
	void Start () {
		GetComponent<CircleCollider2D> ().enabled = false;
		GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y)*(-1);
		isTriggeredByDef = true;
		timedecrease = Time.deltaTime;

		// set up bomb detonation controll variables
		toBeDestroyed = GlobalVars.objectsToBeDestroyed;
		radius = GlobalVars.bombRadius;
		timer = GlobalVars.bombTimer;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GameObject.Find ("XO_character") != null) {
			if (Vector2.Distance(GameObject.Find("XO_character").transform.position, transform.position)>=0.6)
				GetComponent<CircleCollider2D> ().enabled = true;
		}
		else 
			GetComponent<CircleCollider2D> ().enabled = true;

		timer -= timedecrease;
		//Debug.Log (Time.deltaTime +" - "+timer);
		if (timer < 0.0f) {
			Debug.Log (gameObject.name +" is triggered by default");
			customDestroyBomb(true, true, false, false, false, false, false);
		}
	}

	public void customDestroyBomb(bool firstbomb, bool triggerByDefault, bool skipCenter, bool skipLeft, bool skipRight, bool skipUp, bool skipDown){
		// update whether this bomb detonate by default or by chain bombing
		isTriggeredByDef = triggerByDefault;
		isFirstBomb = firstbomb;
		Debug.Log ("this is obj "+ gameObject.name + " - " + triggerByDefault.ToString() +"-"+isTriggeredByDef.ToString());
		if (!isTriggeredByDef)
			timedecrease = 0.0f; // turn off bomb timing - to avoid calling customdestroy more than once
		Debug.Log ("About to destroy: "+ gameObject.name);
		gameObject.SetActive(false);
		spawnExplosionSprite(skipCenter, skipLeft, skipRight, skipUp, skipDown);
		Destroy(gameObject);
	}

	void spawnExplosionSprite(bool skipCenter, bool skipLeft, bool skipRight, bool skipUp, bool skipDown){
		Vector3 bombpos = transform.position;
		Vector3 bombCenter = bombpos + new Vector3(0.5f,0.5f,0.0f);

		// these skip flags are used to avoid creating too many dup exp sprites
		if (!skipCenter) {
			GameObject expCenter = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
			expCenter.transform.position = bombpos;
			expCenter.name = gameObject.name + " exp center";
		}
		if (!skipLeft)
			SpawnExplosionInGivenDirection (bombCenter, new Vector3 (radius*(-1.0f)-0.4f,0.0f,0.0f), "left", bombpos);
		if (!skipRight)
			SpawnExplosionInGivenDirection (bombCenter, new Vector3 (radius*(1.0f)+0.4f,0.0f,0.0f), "right", bombpos);
		if (!skipUp)
			SpawnExplosionInGivenDirection (bombCenter, new Vector3 (0.0f, radius*(1.0f)+0.4f,0.0f), "up", bombpos);
		if (!skipDown)
			SpawnExplosionInGivenDirection (bombCenter, new Vector3 (0.0f, radius*(-1.0f)-0.4f,0.0f), "down", bombpos);

		Debug.Log ("Rolling out. This is gameobject "+ gameObject.name + " and it is root bomb? "+(isTriggeredByDef||isFirstBomb));
		if (isTriggeredByDef || isFirstBomb) { // meaning this is the root bomb that starts the chain bomb
			string dtag = "";
			Debug.Log("This is "+gameObject.name +" about to destroy " + toBeDestroyed.Count + " objects in <static> toBeDestroyed");
			for (int i = 0; i <toBeDestroyed.Count; i++) {
				dtag = toBeDestroyed [i].tag;
				if (dtag == "monster" || dtag == "item" || dtag == "character") {
					destroyBasedOnTag (toBeDestroyed [i]);
				} else if (dtag == "destructible") {
					// spawn explosion sprite for wood block
					toBeDestroyed [i].SetActive (false); // hide it first
					GameObject tmp1 = Instantiate (Resources.Load ("Prefabs/blockExplosion")) as GameObject;
					tmp1.transform.position = toBeDestroyed [i].transform.position;
					Destroy (toBeDestroyed [i]);
				}
			}
			toBeDestroyed.Clear();
			Debug.Log("This is "+gameObject.name + ". And toBeDestroyed is cleared");
		}
	}

	void SpawnExplosionInGivenDirection(Vector3 bombCenter, Vector3 end, string direction, Vector3 bombPos){
		Vector3 spawnPos = Vector3.zero;
		Vector3 unitVector = findUnitVector(direction);

		Vector3 firstHitDestructiblePos = Vector3.zero; // used in case of theres destructible hit - to make sure 2nd 
														// line cast doesnt go pass this point

		// LineCastAll return all objects hit by the line from startPoint to endPoint
		// we do 2 times for proper gameplay purpose

		// A. Line cast no 1 - consider this as a main line cast as it will detect most of things 
		// terrains (destructible and non destructible) and bombs are perfectly aligned with virtual grid, so 1st line cast will handle them for sure
		Vector3 startPoint = findStartPointToLineCast (direction,bombCenter,0.15f);
		RaycastHit2D[] detectedObjects_no1 = Physics2D.LinecastAll(startPoint, startPoint + end);

		if (detectedObjects_no1.Length == 0) {
			//No obstacles on the path - simply spawn the exp sprites
			for (int i = 0, j=1; i < radius; i++){
				GameObject explosionTempSprite = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
				explosionTempSprite.transform.position = bombPos + new Vector3 (j * unitVector.x, j * unitVector.y, 0.0f);
				explosionTempSprite.name = gameObject.name + " exp " + direction + " - "+ i;
				j++;
			}
		} else { // collide at least one obstacles
			string obstacleTag = "";
			bool notDone = true;

			firstHitDestructiblePos = Vector3.zero; // used in case of theres destructible hit - to make sure 2nd line cast doesnt go pass this point

			//1. we look for the first terrain hit - this is the boundary to spawn exp sprites
			for (int i = 0; i < detectedObjects_no1.Length && notDone;i++){
				obstacleTag = detectedObjects_no1[i].collider.gameObject.tag;
				if (obstacleTag == "nonDestructible" || obstacleTag == "destructible"){
					spawnPos = detectedObjects_no1[i].collider.transform.position - unitVector;//calculateSpawnPosition(detectedObjects_no1[i].collider.transform.position, direction);
					if (obstacleTag == "destructible"){
						GameObject tmp = detectedObjects_no1[i].collider.gameObject;
						firstHitDestructiblePos = tmp.transform.position;
						if (!tmp.GetComponent<Destroy>().destroyIsChecked){ // flag to make sure the object added to the tobedestroyed list only once
							tmp.GetComponent<Destroy>().destroyIsChecked = true;
							toBeDestroyed.Add(tmp);
							Debug.Log ("1st cast - Adding "+tmp.name +" from "+ gameObject.name+". Success - new list length "+ toBeDestroyed.Count);
							//printList(toBeDestroyed);
						}
					}
					notDone = false;
				} 
				else if(obstacleTag == "actualMap"){ // map boundary // actualMap
					Vector2 hitPoint = detectedObjects_no1[i].point;
					if (direction == "left" || direction == "right")
						spawnPos = new Vector3(Mathf.Round(hitPoint.x),Mathf.Floor(hitPoint.y),0.0f);
					else 
						spawnPos = new Vector3(Mathf.Floor(hitPoint.x),Mathf.Floor(hitPoint.y),0.0f);
					notDone = false;
				}
				else if (obstacleTag == "monster" || obstacleTag == "item"){
					GameObject tmp = detectedObjects_no1[i].collider.gameObject;
					if (!tmp.GetComponent<Destroy>().destroyIsChecked){ // flag to make sure the object added to the tobedestroyed list only once
						tmp.GetComponent<Destroy>().destroyIsChecked = true;
						toBeDestroyed.Add(tmp);
						Debug.Log ("1st cast - Adding "+tmp.name +" from "+ gameObject.name+". Success - new list length "+ toBeDestroyed.Count);
						//printList(toBeDestroyed);
					}
				}
				else if (obstacleTag == "character") {
					GameObject tmp = detectedObjects_no1[i].collider.gameObject;
					if (!tmp.GetComponent<xoController>().destroyIsChecked){ // flag to make sure the object added to the tobedestroyed list only once
						tmp.GetComponent<xoController>().destroyIsChecked = true;
						toBeDestroyed.Add(tmp);
						Debug.Log ("1st cast - Adding "+tmp.name +" from "+ gameObject.name+". Success - new list length "+ toBeDestroyed.Count);
						//printList(toBeDestroyed);
					}
				}
				else if (obstacleTag == "bomb") {
					if (!detectedObjects_no1[i].collider.gameObject.GetComponent<bombController>().destroyIsChecked) { // flag to make sure the object is destroyed only once
						detectedObjects_no1[i].collider.gameObject.GetComponent<bombController>().destroyIsChecked = true;
						if (direction == "left")
							detectedObjects_no1[i].collider.gameObject.GetComponent<bombController>().customDestroyBomb(false,false, true, false, true, false, false);
						else if (direction == "right")
							detectedObjects_no1[i].collider.gameObject.GetComponent<bombController>().customDestroyBomb(false,false, true, true, false, false, false);
						else if (direction == "up")
							detectedObjects_no1[i].collider.gameObject.GetComponent<bombController>().customDestroyBomb(false,false, true, false, false, false, true);
						else // down
							detectedObjects_no1[i].collider.gameObject.GetComponent<bombController>().customDestroyBomb(false,false, true, false, false, true, false);
					}
				}
				else { 
					Debug.Log("Unknown tag "+ obstacleTag);
				}
			} 

			//2. now we spawn exp sprites 
			int count = 0;
			// find how many sprites we need to spawn
			if (obstacleTag == "nonDestructible" || obstacleTag == "destructible" || obstacleTag == "actualMap")
				count = (int) Vector2.Distance(spawnPos,bombPos);
			else  // "" by default - meaning all objects hit are either monster or character or bomb --> spawn all sprites equal to radius value
				count = radius;
			// spawn exp sprites
			for (int i = 0, j=1; i < count; i++){
				GameObject explosionTempSprite = Instantiate (Resources.Load ("Prefabs/explosion")) as GameObject;
				explosionTempSprite.transform.position = bombPos + new Vector3 (j * unitVector.x, j * unitVector.y, 0.0f);
				explosionTempSprite.name = gameObject.name + " exp " + direction + " - "+ i;
				j++;
			}
		}

		// B. Line cast no 2 - try to catch any obstacles not being caught yet. 
		// if we catch a destructible the first time --> limit the radius to make sure line cast stay within this range
		startPoint = findStartPointToLineCast (direction,bombCenter,-0.15f);
		if (firstHitDestructiblePos != Vector3.zero) {
			Vector3 newEnd = firstHitDestructiblePos - bombPos + new Vector3(unitVector.x*0.4f,unitVector.y*0.4f,0.0f);
			Debug.Log("2nd line cast from "+gameObject.name+" "+bombPos.ToString("F3") + "-" + firstHitDestructiblePos.ToString("F3") + "-" + newEnd.ToString("F3"));
			end = newEnd;
		}
		RaycastHit2D[] detectedObjects_no2 = Physics2D.LinecastAll(startPoint, startPoint + end);
		// we dont need to spawn exp sprites again - just catch any monster not being caught yet on the path
		if (detectedObjects_no2.Length != 0) {
			string obstacleTag = "";
			for (int i = 0; i < detectedObjects_no2.Length;i++){
				obstacleTag = detectedObjects_no2[i].collider.gameObject.tag;
				if (obstacleTag == "nonDestructible" || obstacleTag == "destructible" || obstacleTag == "bomb" || obstacleTag == "actualMap"){
					// do nothing - as these objects are definitely already handled during the first cast because they are perfectly aligned with grid snap
				}
				else if (obstacleTag == "monster" || obstacleTag == "item"){
					GameObject tmp = detectedObjects_no2[i].collider.gameObject;
					if (!tmp.GetComponent<Destroy>().destroyIsChecked){
						tmp.GetComponent<Destroy>().destroyIsChecked = true;
						toBeDestroyed.Add(tmp);
						Debug.Log ("2nd cast - Adding "+tmp.name +" from "+ gameObject.name+". Success - new list length "+ toBeDestroyed.Count);
						//printList(toBeDestroyed);
					}
				}
				else if (obstacleTag == "character") {
					GameObject tmp = detectedObjects_no2[i].collider.gameObject;
					if (!tmp.GetComponent<xoController>().destroyIsChecked){
						tmp.GetComponent<xoController>().destroyIsChecked = true;
						toBeDestroyed.Add(tmp);
						Debug.Log ("2nd cast - Adding "+tmp.name +" from "+ gameObject.name+". Success - new list length "+ toBeDestroyed.Count);
						//printList(toBeDestroyed);
					}
				}
				else { 
					Debug.Log("Unknown tag "+ obstacleTag);
				}
			} 
		} // done
	} // end



	// utilities
	void destroyBasedOnTag(GameObject theObject){
		// we need to call customDestroy 
		if (theObject.tag == "character")
			theObject.GetComponent<xoController> ().customDestroyCharacter ();
		else { // items and monsters
			Debug.Log("Inside destroyBasedOnTag - is destroying object with name "+theObject.ToString());
			theObject.GetComponent<Destroy> ().triggerDeath ();
		}
	}
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
	void printList(List<GameObject> inputList){
		string res ="";
		for (int i =0; i<inputList.Count; i++)
			res = res +inputList[i]+"\n";
		Debug.Log (res);
	}
} // end class

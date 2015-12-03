using UnityEngine;
using System.Collections;

public class cameraController : MonoBehaviour {
	float vertExt, horzExt;

	float minX,maxX,minY,maxY;
	// Use this for initialization
	void Start () {
		vertExt = Camera.main.orthographicSize;
		horzExt = vertExt * Screen.width/Screen.height;

		minX = horzExt - GlobalVars.horzBlockCounts/2.0f;
		maxX = -horzExt + GlobalVars.horzBlockCounts/2.0f;
		minY = vertExt - GlobalVars.vertBlockCounts/2.0f;
		maxY = -vertExt + GlobalVars.vertBlockCounts/2.0f;

		Vector3 temp = GameObject.Find ("XO_character").transform.position;
		transform.position = new Vector3 (temp.x,temp.y,-10.0f);
	}

	void Update(){

	}

	// Update is called once per frame
	void LateUpdate () {
		Vector3 target = transform.position;
		target.x = Mathf.Clamp(target.x,minX,maxX);
		target.y = Mathf.Clamp(target.y,minY,maxY);
		transform.position = target;
	}

}

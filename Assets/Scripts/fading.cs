using UnityEngine;
using System.Collections;

public class fading : MonoBehaviour {

	public Texture2D fadeOutTextture;
	public float fadeSpeed = 1.0f;
	int drawDepth = -1000;
	float alpha = 1.0f;
	int fadeDir = -1;

	void OnGUI(){
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		alpha = Mathf.Clamp01 (alpha);
		GUI.color = new Color (GUI.color.r,GUI.color.g,GUI.color.b,alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture (new Rect(0,0,Screen.width,Screen.height),fadeOutTextture);
	}

	public float beginFade(int direction){
		Debug.Log ("Yasss");
		fadeDir = direction;
		return fadeSpeed;
	}
	
	void OnLevelWasLoaded(){
		Debug.Log ("just loaded "+ Application.loadedLevelName);
		beginFade (-1);
		Debug.Log ("done");
	}

	public IEnumerator timeFading(){
		float fadeTime = beginFade (1);
		yield return new WaitForSeconds(fadeTime);
		Debug.Log ("about to load level "+(Application.loadedLevel +1));
		Application.LoadLevel (Application.loadedLevel +1);
	}
}

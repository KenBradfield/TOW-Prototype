using UnityEngine;
using System.Collections;

/// <summary>
/// attached to player. creates crosshair in center of the screen
/// </summary>

public class Crosshair : MonoBehaviour {

	/*Variables start*/
	public Texture crosshairTex;
	private float crosshairDimension = 256;
	private float halfCrosshairWidth = 128;
	/*Variables end**/

	// Use this for initialization
	void Start () {
		if (networkView.isMine == false) {
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){
		//display crosshair while cursor is locked
		if (Screen.lockCursor == true) {
			GUI.DrawTexture(new Rect(Screen.width / 2 - halfCrosshairWidth, Screen.height / 2 - halfCrosshairWidth,
			                         crosshairDimension, crosshairDimension), crosshairTex);
		}
	}
}

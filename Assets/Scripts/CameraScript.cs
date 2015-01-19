using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the player
/// causes camera to follow CameraHead object
/// </summary>

public class CameraScript : MonoBehaviour {

	/*Variables Start*/
	private Camera myCamera;
	private Transform cameraHeadTransform;
	/*Variables End*/

	// Use this for initialization
	void Start () {
		if(networkView.isMine == true){
			myCamera = Camera.main;
			//sets camera to position of object CameraHead inside player
			cameraHeadTransform = transform.FindChild ("CameraHead");
		}else{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//make the camera follow the player's cameraHeadTransform
		myCamera.transform.position = cameraHeadTransform.position;
		myCamera.transform.rotation = cameraHeadTransform.rotation;
	}
}

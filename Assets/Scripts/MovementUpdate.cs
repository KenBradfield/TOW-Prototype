using UnityEngine;
using System.Collections;

/// <summary>
/// attached to each player so that every player iss up to date accross the network
/// 
/// Thisscript is closely based on a script written by M2H
/// </summary>
public class MovementUpdate : MonoBehaviour {

	/*Variables start*/
	private Vector3 lastPosition;
	private Quaternion lastRotation;
	private Transform myTransform;
	/*Variables End*/

	// Use this for initialization
	void Start () {
		if(networkView.isMine == true){
			myTransform = transform;

			//ensure players see everyone correctly the moment the spawn
			networkView.RPC("updateMovement", RPCMode.OthersBuffered, myTransform.position, myTransform.rotation);
		}else{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//if player has moved then update
		if (Vector3.Distance (myTransform.position, lastPosition) >= 0.1) {
			lastPosition = myTransform.position;
			networkView.RPC("updateMovement", RPCMode.OthersBuffered, myTransform.position, myTransform.rotation);
		}

		//if player has turned
		if (Quaternion.Angle (myTransform.rotation, lastRotation) >= 1) {
			lastRotation = myTransform.rotation;
			networkView.RPC("updateMovement", RPCMode.OthersBuffered, myTransform.position, myTransform.rotation);
		}
	}

	[RPC]
	void updateMovement (Vector3 newPosition, Quaternion newRotation){
		transform.position = newPosition;
		transform.rotation = newRotation;
	}
}

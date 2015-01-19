using UnityEngine;
using System.Collections;

/// <summary>
/// adds player's name (if allowed) to list
/// 
/// uses PlayerDatabase script to add to the player list, uses playerlabel to supply names, uses CommunicationWindow script to give the player name
/// </summary>
public class PlayerName : MonoBehaviour {

	/*variables start*/
	public string playerName;
	/*variables end**/

	//when player spawns retrieve their name from PlayerPrefs and ensure it is valid
	void Awake (){
		if (networkView.isMine == true) {
			playerName = PlayerPrefs.GetString ("playerName");

			//check if other players are already using that name
			//if true then assign a random number as their name and save it
			foreach(GameObject objNameCheck in GameObject.FindObjectsOfType(typeof(GameObject))){
				if(playerName == objNameCheck.name){
					float x = Random.Range(0, 1000);
					playerName = "(" + x.ToString() + ")";
					PlayerPrefs.SetString ("playerName", playerName);
				}
			}

			//update GameManager with the player's name so it's added to the list
			UpdateLocalGameManager(playerName);

			//ensure the name is given to the gameobject across the network
			networkView.RPC("UpdateMyNameEverywhere", RPCMode.AllBuffered, playerName);
		}
	}

	//tell PlayerDatabase to add the player's name
	void UpdateLocalGameManager (string pName){
		GameObject gameManager = GameObject.Find ("GameManager");
		PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase> ();
		dataScript.nameSet = true;
		dataScript.playerName = pName;

		//give player name to CommunicationWindow
		CommunicationWindow commScript = gameManager.GetComponent<CommunicationWindow>();
		commScript.playerName = pName;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[RPC]
	void UpdateMyNameEverywhere(string pName){
		//change object name to actual name
		gameObject.name = pName;
		playerName = pName;

		//Supply playerLabel script with player's name
		PlayerLabel labelScript = transform.GetComponent<PlayerLabel> ();
		labelScript.playerName = pName;
	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// script is attached to player. controls if mouse is locked or unlocked
/// 
/// accesses Multiplayer script, CommunicationWindow script
/// </summary>

public class CursorControl : MonoBehaviour {

	/*variables start*/
	private GameObject multiplayerManager;
	private MultiplayerScript multiScript;

	private GameObject gameManager;
	private CommunicationWindow commScript;
	/*variables end**/

	// Use this for initialization
	void Start () {
		if(networkView.isMine == true){
			multiplayerManager = GameObject.Find("MultiplayerManager");
			multiScript = multiplayerManager.GetComponent<MultiplayerScript>();
			gameManager = GameObject.Find("GameManager");
			commScript = gameManager.GetComponent<CommunicationWindow>();
		}else{
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (multiScript.showDisconnectWindow == false && commScript.unlockCursor == false) {
			Screen.lockCursor = true;
		}

		if (multiScript.showDisconnectWindow == true || commScript.unlockCursor == true) {
			Screen.lockCursor = false;
		}
	}
}

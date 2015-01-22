using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// script to manage the player list. attached to GameManager
/// 
/// accessed by PlayerName, HealthAndDamage script, SpawnScript
/// </summary>

public class PlayerDatabase : MonoBehaviour {

	/*Variables Start*/
	public List<PlayerDataClass> PlayerList = new List<PlayerDataClass>();

	//to add the player to the list
	public NetworkPlayer networkPlayer;
	public bool nameSet = false;
	public string playerName;

	//to update the player list with scores
	public bool scored = false;
	public int playerScore;

	//to update list with player's chosen team
	public bool joinTeam = false;
	public string playerTeam;
	/*Variables end**/

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//edit name
		if (nameSet == true) {
			//add name
			networkView.RPC("EditPlayerListWithName", RPCMode.AllBuffered, Network.player, playerName);
			nameSet = false;
		}

		//edit score
		if (scored == true) {
			networkView.RPC("EditPlayerListWithScore", RPCMode.AllBuffered, Network.player, playerScore);
			scored = false;
		}

		//include player on team they've joined
		if (joinTeam == true) {
			networkView.RPC ("EditPlayerListWithTeam", RPCMode.AllBuffered, Network.player, playerTeam);
			joinTeam = false;
		}
	}

	//when player connects to the server add them to the list using their network player ID
	void OnPlayerConnected (NetworkPlayer netPlayer){
		//add to list
		networkView.RPC ("AddPlayerToList", RPCMode.AllBuffered, netPlayer);
	}

	//Removes the player from the list on disconnect
	void OnPlayerDisconnected (NetworkPlayer netPlayer){
		networkView.RPC ("RemovePlayerFromList", RPCMode.AllBuffered, netPlayer);
	}

	[RPC]
	void AddPlayerToList (NetworkPlayer nPlayer){
		//to create a new entry in the PlayerList
		PlayerDataClass capture = new PlayerDataClass ();
		capture.networkPlayer = int.Parse (nPlayer.ToString ());
		PlayerList.Add (capture);
	}

	[RPC]
	void RemovePlayerFromList (NetworkPlayer nPlayer){
		//find then remove the player from the player list based on networkplayer ID
		for (int i = 0; i < PlayerList.Count; i++) {
			if(PlayerList[i].networkPlayer == int.Parse(nPlayer.ToString())){
				PlayerList.RemoveAt(i);
			}
		}
	}

	[RPC]
	void EditPlayerListWithName (NetworkPlayer nPlayer, string pName){
		//Find player based on NetworkPlayer ID and add their name
		for (int i = 0; i < PlayerList.Count; i++) {
			if(PlayerList[i].networkPlayer == int.Parse(nPlayer.ToString())){
				PlayerList[i].playerName = pName;
			}
		}
	}

	[RPC]
	void EditPlayerListWithScore (NetworkPlayer nPlayer, int pScore){
		//Find player based on NetworkPlayer ID and add their score
		for (int i = 0; i < PlayerList.Count; i++) {
			if(PlayerList[i].networkPlayer == int.Parse(nPlayer.ToString())){
				PlayerList[i].playerScore = pScore;
			}
		}
	}

	[RPC]
	void EditPlayerListWithTeam (NetworkPlayer nPlayer, string pTeam){
		//Find player based on NetworkPlayer ID and add their team
		for (int i = 0; i < PlayerList.Count; i++) {
			if(PlayerList[i].networkPlayer == int.Parse(nPlayer.ToString())){
				PlayerList[i].playerTeam = pTeam;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// attached to GameManager
/// 
/// accesses SpawnScript to see if it is the first spawn
/// </summary>
/// 
public class AssignHealth : MonoBehaviour {

	/*variables start*/
	private GameObject[] redTeamPlayers;
	private GameObject[] blueTeamPlayers;
	private float waitTime = 5;
	/*variables end**/

	// Use this for initialization
	void Start () {
	
	}

	void OnConnectedToServer(){
		StartCoroutine (AssignHealthOnJoiningGame ());
	}

	IEnumerator AssignHealthOnJoiningGame(){
		yield return new WaitForSeconds(waitTime);

		redTeamPlayers = GameObject.FindGameObjectsWithTag("RedTeamTrigger");
		blueTeamPlayers = GameObject.FindGameObjectsWithTag("BlueTeamTrigger");

		//Assign the buffered previous health value to other players
		foreach(GameObject red in redTeamPlayers){
			HealthAndDamage hdScript = red.GetComponent<HealthAndDamage>();
			hdScript.myHealth = hdScript.previousHealth;
		}
		foreach(GameObject blue in blueTeamPlayers){
			HealthAndDamage hdScript = blue.GetComponent<HealthAndDamage>();
			hdScript.myHealth = hdScript.previousHealth;
		}

		enabled = false;
	}
}

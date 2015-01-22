using UnityEngine;
using System.Collections;

/// <summary>
/// Script attached to SpawnManager allows players to spawn and respawn
/// 
/// accesses PlayerDataBase to supply yeams
/// 
/// accessed by FireBlaster script to determine player team, healthAndDamage script 
/// </summary>
public class SpawnScript : MonoBehaviour {

	/*Variables start*/
	private bool justConnectedToServer = false;
	public bool amIOnTheRedTeam = false;
	public bool amIOnTheBlueTeam = false;

	//used to define the team assignment window
	private Rect joinTeamRect;
	private string joinTeamWindowTitle = "Team Selection";
	private int joinTeamWindowWidth = 330;
	private int joinTeamWindowHeight = 100;
	private int joinTeamLeftIndent;
	private int joinTeamTopIndent;
	private int buttonHeight = 40;

	//defines the player prefabs
	public Transform redTeamPlayer;
	public Transform blueTeamPlayer;
	private int redTeamGroup = 0;
	private int blueTeamGroup = 1;

	//to get spawn points
	private GameObject[] redSpawnPoints;
	private GameObject[] blueSpawnPoints;

	//used in respawning the player if destroyed
	public bool iAmDestroyed = false;

	//used to determine if firt spawn
	public bool firstSpawn = false;
	/*Variable end*/

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnConnectedToServer (){
		justConnectedToServer = true;
	}

	//defines the window to join a team
	void JoinTeamWindow (int windowID){
		if (justConnectedToServer == true) {
			//if player joins red team
			if (GUILayout.Button ("Join Red Team", GUILayout.Height (buttonHeight))) {
				amIOnTheRedTeam = true;
				justConnectedToServer = false;
				SpawnRedTeamPlayer ();
				firstSpawn = true;
			}

			//if player joins blue team
			if (GUILayout.Button ("Join Blue Team", GUILayout.Height (buttonHeight))) {
				amIOnTheBlueTeam = true;
				justConnectedToServer = false;
				SpawnBlueTeamPlayer ();
				firstSpawn = true;
			}
		}

		//allows player to respawn
		if (iAmDestroyed == true) {
			if(GUILayout.Button("Respawn", GUILayout.Height(buttonHeight *2))){
				if( amIOnTheRedTeam == true){
					SpawnRedTeamPlayer ();
				}
				if(amIOnTheBlueTeam == true){
					SpawnBlueTeamPlayer();
				}
				iAmDestroyed = false;
			}
		}
	}

	//used to draw the GUI
	void OnGUI(){
		if (justConnectedToServer == true || iAmDestroyed == true) {
			Screen.lockCursor = false;
			joinTeamLeftIndent = Screen.width / 2 - joinTeamWindowWidth / 2;
			joinTeamTopIndent = Screen.width / 2 - joinTeamWindowHeight / 2;
			joinTeamRect = new Rect(joinTeamLeftIndent, joinTeamTopIndent,
			                        joinTeamWindowWidth, joinTeamWindowHeight);

			joinTeamRect = GUILayout.Window(0, joinTeamRect, JoinTeamWindow, joinTeamWindowTitle);
		}
	}

	//spawn in the red team player
	void SpawnRedTeamPlayer(){
		//get red spawn points
		redSpawnPoints = GameObject.FindGameObjectsWithTag ("SpawnRedTeam");

		//randomly select one of the spawn points.
		GameObject randomRedSpawn = redSpawnPoints [Random.Range (0, redSpawnPoints.Length)];

		//spawn the player
		Network.Instantiate (redTeamPlayer, randomRedSpawn.transform.position, randomRedSpawn.transform.rotation, redTeamGroup);


		//access playerdatabase and give it the team of the player
		GameObject gameManager = GameObject.Find ("GameManager");
		PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
		dataScript.joinTeam = true;
		dataScript.playerTeam = "red";
	}

	//spawn in the Blue team player
	void SpawnBlueTeamPlayer(){
		//get blue spawn points
		blueSpawnPoints = GameObject.FindGameObjectsWithTag ("SpawnBlueTeam");
		
		//randomly select one of the spawn points.
		GameObject randomBlueSpawn = blueSpawnPoints [Random.Range (0, blueSpawnPoints.Length)];
		
		//spawn the player
		Network.Instantiate (blueTeamPlayer, randomBlueSpawn.transform.position, randomBlueSpawn.transform.rotation, blueTeamGroup);

		//access playerdatabase and give it the team of the player
		GameObject gameManager = GameObject.Find ("GameManager");
		PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();
		dataScript.joinTeam = true;
		dataScript.playerTeam = "blue";
	}
}

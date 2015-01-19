using UnityEngine;
using System.Collections;

/// <summary>
/// attached to the game manager.
/// 
/// accesses spawnscript to check if player has joined
/// 
/// accessed by CursorControl script, PlayerName
/// </summary>
public class CommunicationWindow : MonoBehaviour {

	/*variables start*/
	public string playerName;

	//used to send a message
	private string messageToSend;
	private string communication;
	private bool showTextBox = false;
	private bool sendMessage = false;
	public bool unlockCursor = false;

	//used to define communication window
	private Rect windowRect;
	private int windowLeft = 10;
	private int windowTop;
	private int windowWidth = 300;
	private int windowHeight = 140;
	private int padding = 20;
	private int textFieldHeight = 30;
	private Vector2 scrollPosition;
	private GUIStyle myStyle = new GUIStyle();

	//quick references
	private GameObject spawnManager;
	private SpawnScript spawnScript;

	//for when another PlayerName joins
	public bool tellEveryoneIJoined = true;
	/*variables end**/

	void Awake(){
		Input.eatKeyPressOnTextFieldFocus = false;
		messageToSend = "";
		myStyle.normal.textColor = Color.white;
		myStyle.wordWrap = true;
	}
	// Use this for initialization
	void Start () {
		spawnManager = GameObject.Find ("SpawnManager");
		spawnScript = spawnManager.GetComponent<SpawnScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Network.peerType != NetworkPeerType.Disconnected) {
			if(Input.GetButtonDown("Communication") && showTextBox == false){
				showTextBox = true;
			}
			if(Input.GetButtonDown("Send Message") && showTextBox == true){
				sendMessage = true;
			}
		}
		//when player joins for the first time, announce it to the world
		if (Network.isClient && tellEveryoneIJoined == true && playerName != "") {
			networkView.RPC ("TellEveryonePlayerJoined", RPCMode.All, playerName);
			tellEveryoneIJoined = false;
		}
	}

	//defines the Comm window allowing for scrolling
	void CommLogWindow(int windowID){
		scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.Width (windowWidth - padding),
		                                            GUILayout.Height (windowHeight - padding - 5));
		GUILayout.Label (communication, myStyle);
		GUILayout.EndScrollView ();
	}

	//draws the comm window
	void OnGUI(){
		if (Network.peerType != NetworkPeerType.Disconnected) {
			windowTop = Screen.height - windowHeight - textFieldHeight;
			windowRect = new Rect(windowLeft, windowTop, windowWidth, windowHeight);

			//don't allow player/server to send messages or look at messages
			if(spawnScript.amIOnTheRedTeam ==true || spawnScript.amIOnTheBlueTeam == true || Network.isServer ==true){
				windowRect = GUI.Window (5, windowRect, CommLogWindow, "Communication Log");
				GUILayout.BeginArea (new Rect(windowLeft, windowTop + windowHeight, windowWidth, windowHeight));

				if(showTextBox == true){
					unlockCursor = true;
					Screen.lockCursor = false;
					GUI.SetNextControlName("MyTextField");
					messageToSend = GUILayout.TextField(messageToSend, GUILayout.Width (windowWidth));
					GUI.FocusControl("MyTextField");
					if(sendMessage == true){
						if(messageToSend != ""){
							if(Network.isClient == true){
								networkView.RPC ("SendMessageToEveryone", RPCMode.All, messageToSend, playerName);
							}

							if(Network.isServer == true){
								networkView.RPC("SendMessageToEveryone", RPCMode.All, messageToSend, "Server");
							}
						}
						sendMessage = false;
						showTextBox = false;
						unlockCursor = false;
						messageToSend = "";
					}
				}
				GUILayout.EndArea();
			}
		}
	}

	[RPC]
	void SendMessageToEveryone(string message, string pName){
		communication = pName + " : " + message + "\n" + "\n" + communication;
	}

	[RPC]
	void TellEveryonePlayerJoined(string pName){
		communication = pName + " has joined the game." + "\n" + "\n" + communication;
	}
}

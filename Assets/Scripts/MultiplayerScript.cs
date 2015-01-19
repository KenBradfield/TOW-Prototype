using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to multiplayer manager
/// basis of multiplayer system
/// </summary>
public class MultiplayerScript : MonoBehaviour {

	/*variables start*/
	private string titleMessage = "T.O.W. Prototype";
	private string connectToIP = "127.0.0.1";
	private int connectionPort = 26500;
	private bool useNAT = false;
	private string ipAddress;
	private string port;
	private int numOfPlayers = 10;
	public string playerName;
	public string serverName;
	public string serverNameForClient;
	private bool iWantToSetupAServer = false;
	private bool iWantToConnectToAServer = false;

	//variables for main menu
	private Rect connectionWindowRect;
	private int connectionWindowWidth = 400;
	private int connectionWindowHeight = 280;
	private int buttonHeight = 60;
	private int leftIndent;
	private int topIndent;

	//variables for server shutdown window
	private Rect serverDisWindowRect;
	private int serverDisWindowWidth = 300;
	private int serverDisWindowHeight = 150;
	private int serverDisWindowLeftIndent = 10;
	private int serverDisWindowTopIndent = 10;

	//variables for client shutdown window
	private Rect clientDisWindowRect;
	private int clientDisWindowWidth = 300;
	private int clientDisWindowHeight = 170;
	public bool showDisconnectWindow = false;
	/*variables end**/

	// Use this for initialization
	void Start () {
		//load preferences
		serverName = PlayerPrefs.GetString ("serverName");
		if (serverName == "") {
			serverName = "Server";
		}

		playerName = PlayerPrefs.GetString ("playerName");
		if (playerName == "") {
			playerName = "Player";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			showDisconnectWindow =!showDisconnectWindow;
		}
	}

	//Layout design of the connection window
	void ConnectWindow(int windowID){
		GUILayout.Space (15);

		if (iWantToSetupAServer == false && iWantToConnectToAServer == false) {
			if(GUILayout.Button("Setup a server", GUILayout.Height(buttonHeight))){
				iWantToSetupAServer = true;
			}

			GUILayout.Space(10);

			if(GUILayout.Button("Connect to a server", GUILayout.Height(buttonHeight))){
				iWantToConnectToAServer = true;
			}

			GUILayout.Space(10);

			if(Application.isWebPlayer == false && Application.isEditor == false){
				if(GUILayout.Button("Exit Prototype", GUILayout.Height(buttonHeight))){
					Application.Quit();
				}
			}
		}

		//user inputs server information
		if (iWantToSetupAServer == true) {
			//name
			GUILayout.Label ("Enter a name for your server");
			serverName = GUILayout.TextField(serverName);

			GUILayout.Space(5);

			//port number
			GUILayout.Label("Server Port");
			connectionPort = int.Parse (GUILayout.TextField(connectionPort.ToString()));

			GUILayout.Space(10);

			if(GUILayout.Button ("Start my own server", GUILayout.Height (30))){
				Network.InitializeServer(numOfPlayers, connectionPort, useNAT);
				PlayerPrefs.SetString("serverName", serverName);
				iWantToSetupAServer = false;
			}

			if(GUILayout.Button("Go Back", GUILayout.Height(30))){
				iWantToSetupAServer = false;
			}
		}

		//user connects to a server
		if (iWantToConnectToAServer == true) {
			//Name
			GUILayout.Label("Enter Player Name");
			playerName = GUILayout.TextField(playerName);

			GUILayout.Space(5);

			//ip address
			GUILayout.Label("Type in Server IP");
			connectToIP = GUILayout.TextField(connectToIP);

			GUILayout.Space(5);

			//port number
			GUILayout.Label("Server Port");
			connectionPort = int.Parse(GUILayout.TextField(connectionPort.ToString()));

			GUILayout.Space(5);

			if(GUILayout.Button ("Connect to server", GUILayout.Height (30))){
				//Ensure Valid name
				if(playerName ==""){
					playerName = "Player";
				}
				Network.Connect(connectToIP, connectionPort);
				PlayerPrefs.SetString("playerName", playerName);
				iWantToConnectToAServer = false;
			}

			GUILayout.Space(5);

			if(GUILayout.Button("Go Back", GUILayout.Height(30))){
				iWantToConnectToAServer = false;
			}
		}
	}

	//Design Disconnet Window for server
	void ServerDisconnectWindow(int windowID){
		GUILayout.Label ("Server name: " + serverName);

		//show players connected
		GUILayout.Label ("Number of players connected: " + Network.connections.Length);

		//show ping
		if (Network.connections.Length >= 1) {
			GUILayout.Label("Ping: " + Network.GetAveragePing (Network.connections[0]));
		}

		if (GUILayout.Button ("Shutdown server")) {
			Network.Disconnect();
		}
	}

	//Design Disconnect window for client
	void ClientDisconnectWindow(int windowID){
		GUILayout.Label ("Connect to server: " + serverName);
		GUILayout.Label ("Ping: " + Network.GetAveragePing (Network.connections [0]));

		GUILayout.Space (7);

		if (GUILayout.Button ("Disconnect", GUILayout.Height (25))) {
			Network.Disconnect();
		}

		GUILayout.Space (5);

		if (GUILayout.Button ("Return To Game", GUILayout.Height (25))) {
			showDisconnectWindow = false;
		}
	}

	//if a player disconnects for any reason the level is restarted
	void OnDisconnectedFromServer(){
		Application.LoadLevel (Application.loadedLevel);
	}

	//when player leaves, delete them from network so other players don't see them
	void OnPlayerDisconnected(NetworkPlayer networkPlayer){
		Network.RemoveRPCs (networkPlayer);
		Network.DestroyPlayerObjects (networkPlayer);
	}

	void OnPlayerConnected(NetworkPlayer networkPlayer){
		networkView.RPC ("TellPlayerServerName", networkPlayer, serverName);
	}

	//Design the position of the GUI
	void OnGUI(){
		if (Network.peerType == NetworkPeerType.Disconnected) {
			leftIndent = (Screen.width / 2) - (connectionWindowWidth / 2);
			topIndent = (Screen.height / 2) - (connectionWindowHeight / 2);

			connectionWindowRect = new Rect(leftIndent, topIndent, connectionWindowWidth, connectionWindowWidth);
			connectionWindowRect = GUILayout.Window(0, connectionWindowRect, ConnectWindow, titleMessage);
		}

		//if the game is running as a server then run the ServerDisconnetWindow function
		if(Network.peerType == NetworkPeerType.Server){
			serverDisWindowRect = new Rect(serverDisWindowLeftIndent, serverDisWindowTopIndent, serverDisWindowWidth, serverDisWindowHeight);
			serverDisWindowRect = GUILayout.Window(1, serverDisWindowRect, ServerDisconnectWindow, "");
		}

		//if connection type is client then show a window to disconnect
		if (Network.peerType == NetworkPeerType.Client && showDisconnectWindow == true) {
			clientDisWindowRect = new Rect(Screen.width / 2 - clientDisWindowWidth / 2,
			                               Screen.height / 2 - clientDisWindowHeight /2,
			                               clientDisWindowWidth, clientDisWindowHeight);

			clientDisWindowRect = GUILayout.Window(1, clientDisWindowRect, ClientDisconnectWindow, "");
		}
	}

	[RPC]
	//used to tell players the server name
	void TellPlayerServerName(string servername){
		serverName = servername;
	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// attached to GameManager to display Combat log
/// 
/// accessed by HealthAndDamage script
/// </summary>
public class CombatWindow : MonoBehaviour {

	/*variables end*/
	//variables affected by HealthAndDamage script
	public string attackerName;
	public string destroyedName;
	public bool addNewEntry = false;

	//the line to be displayed in the combat log
	private string combatLog;

	//size of combat log
	private int characterLimit = 10000;

	//defines the window
	public Rect windowRect;
	private int windowLeft = 10;
	private int windowTop = 150;
	private int windowWidth = 300;
	private int windowHeight = 150;
	private GUIStyle myStyle = new GUIStyle();

	//to scroll the combat log
	private float nextScrollTime = 0;
	private float scrollRate = 12;
	/*variables end**/

	// Use this for initialization
	void Start () {
		myStyle.fontStyle = FontStyle.Bold;
		myStyle.fontSize = 11;
		myStyle.normal.textColor = Color.green;
		myStyle.wordWrap = true;
	}

	void CombatWindowFunction(int windowID){
		GUILayout.Label (combatLog, myStyle);
	}

	void OnGUI(){
		if (Network.peerType != NetworkPeerType.Disconnected) {
			windowTop = Screen.height - 350;
			windowRect = new Rect(windowLeft, windowTop, windowWidth, windowHeight);

			//ifplayer was destroyed add an entry to the log
			if(addNewEntry == true){
				if(combatLog.Length < characterLimit){
					combatLog = attackerName + " >>> " + destroyedName + "\n" + combatLog;

					//combat log will scroll after some time
					nextScrollTime = Time.time + scrollRate;
					addNewEntry = false;
				}

				//Reset combatLog to stop it from getting too large
				if(combatLog.Length > characterLimit){
					combatLog = attackerName + " >>> " +destroyedName +"\n";
				}
			}
			windowRect = GUI.Window(4, windowRect, CombatWindowFunction, "Combat Log");

			//creates combat scrolling if enough time has passed
			if(Time.time > nextScrollTime && addNewEntry == false){
				combatLog = "\n" + combatLog;
				nextScrollTime = Time.time +scrollRate;
			}
		}
	}
}

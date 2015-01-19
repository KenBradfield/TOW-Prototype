using UnityEngine;
using System.Collections;

/// <summary>
/// attached to player. draws healthbar of other players above them.
/// 
/// accesses HealthAndDamage script to get other players health
/// 
/// accessed by PlayerName script
/// </summary>
public class PlayerLabel : MonoBehaviour {

	/*variables start*/
	public Texture healthTex;
	private Camera myCamera;
	private Transform myTransform;
	private Transform triggerTransform;
	private HealthAndDamage hdScript;

	//used to determine where and when to display the health bar
	private Vector3 worldPosition = new Vector3();
	private Vector3 screenPosition = new Vector3();
	private Vector3 cameraRelativePosition = new Vector3();
	private float minimumZ = 1.5f;

	//used to define health bar
	private int labelTop = 18;
	private int labelWidth = 110;
	private int labelHeight = 15;
	private int barTop = 1;
	private int healthBarHeight = 5;
	private int healthBarLeft =100;
	private float healthBarLength;
	private float adjustment = 1;

	//used to display player's name
	public string playerName;
	private GUIStyle myStyle = new GUIStyle();
	/*variables end**/

	void Awake(){
		//will run for other player characters
		if (networkView.isMine == false) {
			myTransform = transform;
			myCamera = Camera.main;
			
			//access healthanddamage script
			Transform triggerTransform = transform.FindChild ("Trigger");
			hdScript = triggerTransform.GetComponent<HealthAndDamage> ();
			
			if (myTransform.tag == "BlueTeam") {
				myStyle.normal.textColor = Color.blue;
			}
			if (myTransform.tag == "RedTeam") {
				myStyle.normal.textColor = Color.red;
			}
			myStyle.fontSize = 12;
			myStyle.fontStyle = FontStyle.Bold;
			
			//allow for long names
			myStyle.clipping = TextClipping.Overflow;
		} else {
			enabled = false;
		}
	}

	// Update is called once per frame
	void Update () {
		//Capture whether the player is in front of behind the camera
		cameraRelativePosition = myCamera.transform.InverseTransformPoint (myTransform.position);

		//figure out how long the health bar should be. // to avoid texture error the health cannot fall below 1
		if (hdScript.myHealth < 1) {
			healthBarLength = 1;
		}

		if (hdScript.myHealth >= 1) {
			healthBarLength = (hdScript.myHealth / hdScript.maxHealth) * 100;
		}
	}

	//used to display health of other players if they are in front of the camera
	void OnGUI(){
		if (cameraRelativePosition.z > minimumZ) {
			//set world position to above the player
			worldPosition = new Vector3(myTransform.position.x, myTransform.position.y + adjustment, myTransform.position.z);

			//set screen position to a point on the screen
			screenPosition = myCamera.WorldToScreenPoint(worldPosition);

			//draw the heath bar
			GUI.Box (new Rect(screenPosition.x - healthBarLeft / 2, Screen.height - screenPosition.y - barTop,
			                  100, healthBarHeight), "");
			GUI.DrawTexture(new Rect(screenPosition.x - healthBarLeft / 2, Screen.height - screenPosition.y - barTop,
			                         healthBarLength, healthBarHeight), healthTex);

			//Draw name of the player
			GUI.Label(new Rect(screenPosition.x - labelWidth / 2, Screen.height - screenPosition.y - labelTop,
			                   labelWidth, labelHeight), playerName, myStyle);
		}
	}
}

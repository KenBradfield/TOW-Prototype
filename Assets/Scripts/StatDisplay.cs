using UnityEngine;
using System.Collections;

/// <summary>
/// attached to player. allows them to see a graphic showing their health
/// 
/// accesses HealthAndDamae
/// </summary>

public class StatDisplay : MonoBehaviour {

	/*variables start*/
	public Texture healthTex;

	//used to calculate health
	private float health;
	private int healthForDisplay;

	//used in defining the StatDisplay box
	private int boxWidth = 160;
	private int boxHeight = 85;
	private int labelHeight = 20;
	private int labelWidth =35;
	private int padding = 10;
	private int gap = 120;
	private float healthBarLength;
	private int healthBarHeight = 15;
	private GUIStyle healthStyle = new GUIStyle();
	private float commonLeft;
	private float commonTop;

	//reference to health and damage script
	private HealthAndDamage hdScript;
	/*variables end**/

	// Use this for initialization
	void Start () {
		if (networkView.isMine == true) {
			Transform triggerTransform = transform.FindChild ("Trigger");
			hdScript = triggerTransform.GetComponent<HealthAndDamage> ();

			//set GUIStyle
			healthStyle.normal.textColor = Color.green;
			healthStyle.fontStyle = FontStyle.Bold;
		} else {
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//updates health to update the bar length
		health = hdScript.myHealth;

		//round up decimals for easy display
		healthForDisplay = Mathf.CeilToInt (health);
		healthBarLength = (health / hdScript.maxHealth) * 100; //percent of health bar to display
	}

	//Display the health bar
	void OnGUI(){
		commonLeft = Screen.width / 2 + 180;
		commonTop = Screen.height / 2 + 50;

		//draw box to contain health box
		GUI.Box (new Rect (commonLeft, commonTop, boxWidth, boxHeight), "");
		GUI.Box (new Rect (commonLeft + padding, commonTop + padding, 100, healthBarHeight), "");
		GUI.DrawTexture (new Rect (commonLeft + padding, commonTop + padding, healthBarLength, healthBarHeight), healthTex);

		//place health value to the right of the bar
		GUI.Label (new Rect (commonLeft + gap, commonTop + padding, labelWidth, labelHeight),
		          healthForDisplay.ToString (), healthStyle);
	}
}

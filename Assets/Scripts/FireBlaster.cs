using UnityEngine;
using System.Collections;

/// <summary>
/// This script is attached to the player
/// allows projectiles to fire
/// 
/// access the SpawnScript
/// </summary>
public class FireBlaster : MonoBehaviour {
	/*variables start*/
	public GameObject blaster;
	private Transform myTransform;
	private Transform cameraHeadTransform;
	private Vector3 launchPosition = new Vector3();
	private float fireRate = 0.2f;
	private float nextFire = 0;

	//determine which team the player was on
	private bool iAmOnTheRedTeam = false;
	private bool iAmOnTheBlueTeam = false;
	/*variables end***/

	// Use this for initialization
	void Start () {
		if (networkView.isMine == true) {
			myTransform = transform;
			cameraHeadTransform = myTransform.FindChild ("CameraHead");

			//access SpawnScript to get which team the player is on
			GameObject spawnManager = GameObject.Find("SpawnManager");
			SpawnScript spawnScript = spawnManager.GetComponent<SpawnScript>();
			if(spawnScript.amIOnTheRedTeam == true){
				iAmOnTheRedTeam = true;
			}
			if(spawnScript.amIOnTheBlueTeam == true){
				iAmOnTheBlueTeam = true;
			}
		} else {
				enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton ("Fire Weapon") && Time.time >nextFire && Screen.lockCursor == true) {
			nextFire = Time.time + fireRate;
			//positions projectile to be just in front of CameraHead
			launchPosition = cameraHeadTransform.TransformPoint(0,0,0.2f);

			//create projectile and tilt projectile so that it is in line with
			//where the player is looking
			if(iAmOnTheRedTeam == true){
				networkView.RPC ("SpawnProjectile", RPCMode.All, launchPosition, Quaternion.Euler(cameraHeadTransform.eulerAngles.x + 90,
				                                                                                  myTransform.eulerAngles.y, 0),
				                 myTransform.name, "red");
			}
			if(iAmOnTheBlueTeam == true){
				networkView.RPC ("SpawnProjectile", RPCMode.All, launchPosition, Quaternion.Euler(cameraHeadTransform.eulerAngles.x + 90,
				                                                                                  myTransform.eulerAngles.y, 0),
				                 myTransform.name, "blue");
			}
		}
	}

	[RPC]
	void SpawnProjectile (Vector3 position, Quaternion rotation, string originatorName, string team){
		//access BlasterScript and supply the player's name and team
		GameObject go = Instantiate (blaster, position, rotation) as GameObject;
		BlasterScript bScript = go.GetComponent<BlasterScript> ();
		bScript.myOriginator = originatorName;
		bScript.team = team;
	}
}

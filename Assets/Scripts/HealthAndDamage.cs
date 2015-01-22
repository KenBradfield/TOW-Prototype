using UnityEngine;
using System.Collections;

/// <summary>
/// attached to Trigger in player object
/// manages player's health & damage across the server
/// 
/// accesses PlayerDatabase script to check the playerList, Spawnscript to activate respawn on destroyed, CombatWindow to add next combat logs
/// accesses PlayerScore to inform it to update
/// 
/// accessed by BlasterScript, StatDisplay, PlayerLabel script
/// </summary>
public class HealthAndDamage : MonoBehaviour {

	/*variables start*/
	private GameObject parentObject;

	//used to figure out who is attacking
	public string myAttacker;
	public bool iWasJustAttacked;

	//used to figure out wha weapon is used
	public bool hitByBlaster = false;
	private float blasterDamage = 30;

	//used to prevent the player from being hit while being destroyed
	private bool destroyed = false;

	//used to mangae health
	public float myHealth = 100;
	public float maxHealth = 100;
	private float healthRegenRate = 1.3f;
	public float previousHealth = 100;
	/*variables end */

	// Use this for initialization
	void Start () {
		//set parent object to be destroyed
		parentObject = transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		//update the damage and health if attacked
		if (iWasJustAttacked == true) {
			GameObject gameManager = GameObject.Find ("GameManager");
			PlayerDatabase dataScript = gameManager.GetComponent<PlayerDatabase>();

			//only run hit detection if it is on the attacking players computer
			for(int i = 0; i < dataScript.PlayerList.Count; i++){
				if(myAttacker == dataScript.PlayerList[i].playerName){
					if(int.Parse(Network.player.ToString()) == dataScript.PlayerList[i].networkPlayer){
						if(hitByBlaster == true && destroyed == false){
							myHealth = myHealth - blasterDamage;

							//send out RPC so attacker can recieve score
							networkView.RPC ("UpdateMyCurrentAttackerEverywhere", RPCMode.Others, myAttacker);

							//send out RPC so player's health is reduced
							networkView.RPC ("UpdateMyCurrentHealthEverywhere", RPCMode.Others, myHealth);
							hitByBlaster = false;
						}
						if(myHealth <= 0 && destroyed == false){
							myHealth = 0;
							destroyed = true;
							GameObject attacker = GameObject.Find(myAttacker);
							PlayerScore scoreScript = attacker.GetComponent<PlayerScore>();
							scoreScript.iDestroyedAnEnemy = true;
							scoreScript.enemiesDestroyedInOneHit++;
						}
					}
				}
			}
			iWasJustAttacked = false;
		}
		//Each player is responsible for destroying themselves
		if (myHealth <= 0 && networkView.isMine == true) {
			//access SpawnScript to set iAmDestroyed to true so the player can respawn
			GameObject spawnManager = GameObject.Find ("SpawnManager");
			SpawnScript spawnScript = spawnManager.GetComponent<SpawnScript>();
			spawnScript.iAmDestroyed = true;

			//remove player's RPCs
			Network.RemoveRPCs(Network.player);

			//update combat log
			networkView.RPC("TellEveryoneWhoDestroyedWho", RPCMode.All, myAttacker, parentObject.name);

			networkView.RPC ("DestroySelf", RPCMode.All);
		}

		//If player's health has change then update the record across the network
		if (myHealth > 0 && networkView.isMine == true) {
			if(myHealth != previousHealth){
				networkView.RPC("UpdateMyHealthRecordEverywhere", RPCMode.AllBuffered, myHealth);
			}
		}

		//Regen
		if (myHealth < maxHealth) {
			myHealth = myHealth + healthRegenRate * Time.deltaTime;
		}

		//if health exceed max health while regenerating
		if (myHealth > maxHealth) {
			myHealth = maxHealth;
		}
	}

	[RPC]
	void UpdateMyCurrentAttackerEverywhere (string attacker){
		myAttacker = attacker;
	}

	[RPC]
	void UpdateMyCurrentHealthEverywhere (float health){
		myHealth = health;
	}

	[RPC]
	void DestroySelf(){
		Destroy (parentObject);
	}

	[RPC]
	void UpdateMyHealthRecordEverywhere(float health){
		previousHealth = health;
	}

	[RPC]
	void TellEveryoneWhoDestroyedWho (string attacker, string destroyed){
		GameObject gameManager = GameObject.Find ("GameManager");
		CombatWindow combatScript = gameManager.GetComponent<CombatWindow>();
		combatScript.attackerName = attacker;
		combatScript.destroyedName = destroyed;
		combatScript.addNewEntry = true;
	}
}

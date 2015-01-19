using UnityEngine;
using System.Collections;

/// <summary>
/// Script attached to blaster to control the behaviour
/// of the projectile
/// 
/// accessed by FireBlaster
/// </summary>

public class BlasterScript : MonoBehaviour {

	/*Start of variables*/
	private Transform myTransform;
	private float projectileSpeed = 10;
	private bool expended = false;
	private RaycastHit hit;
	private float range = 1.5f;
	private float expireTime = 5;

	//Explosion effect
	public GameObject blasterExplosion;

	//for hit detection
	public string team;
	public string myOriginator;
	/*end of variables*/

	// Use this for initialization
	void Start () {
		myTransform = transform;

		//timer will start to countdown to destroy the projectile
		StartCoroutine (DestroyMyselfAfterSomeTime());
	}
	
	// Update is called once per frame
	void Update () {
		//vector3 up moves projectile along the pointy end
		myTransform.Translate (Vector3.up * projectileSpeed * Time.deltaTime);

		//if it hits something
		if (Physics.Raycast (myTransform.position, myTransform.up, out hit, range) &&
						expended == false) {
						//if has tag floor
			if(hit.transform.tag == "Floor"){
				expended = true;

				Instantiate(blasterExplosion, hit.point, Quaternion.identity);

				myTransform.renderer.enabled = false;
				myTransform.light.enabled = false;
			}

			if(hit.transform.tag == "BlueTeamTrigger" || hit.transform.tag == "RedTeamTrigger"){
				expended = true;
				
				Instantiate(blasterExplosion, hit.point, Quaternion.identity);
				
				myTransform.renderer.enabled = false;
				myTransform.light.enabled = false;

				//access HealthAndDamage script and send attacked signal with ID
				if(hit.transform.tag == "BlueTeamTrigger" && team == "red"){
					HealthAndDamage hdScript = hit.transform.GetComponent<HealthAndDamage>();
					hdScript.iWasJustAttacked = true;
					hdScript.myAttacker = myOriginator;
					hdScript.hitByBlaster = true;
				}

				if(hit.transform.tag == "RedTeamTrigger" && team == "blue"){
					HealthAndDamage hdScript = hit.transform.GetComponent<HealthAndDamage>();
					hdScript.iWasJustAttacked = true;
					hdScript.myAttacker = myOriginator;
					hdScript.hitByBlaster = true;
				}
			}
		}
	}

	IEnumerator DestroyMyselfAfterSomeTime(){
		//when count has reached expire time it will destroy the projectile

		yield return new WaitForSeconds (expireTime);
		Destroy (myTransform.gameObject);
	}
}

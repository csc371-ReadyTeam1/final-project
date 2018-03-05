using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleShield : MonoBehaviour {

	public float lifeTime = 4.0f;
	private int hits;
	public int hitCount = 2;
	public GameObject player;


	//private Vector3 offset = new Vector3 (0.15f, 0.05f);

	void Start() {
		//this.transform.position += offset;
	}

	// Update is called once per frame
	void Update () {
		//this.transform.position = move with player...

		/*lifeTime -= Time.deltaTime;
		if (lifeTime <= 0) {
			gameObject.SetActive (false);
		}
		*/
		if (hitCount == hits) {
			gameObject.SetActive (false);
			player.GetComponent<PlayerPlatformerController> ().fullShieldEnable = false;
		}

	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Projectile p = collision.gameObject.GetComponent<Projectile>();
		if (p == null) return;
		hits++;
		Destroy(collision.gameObject);
	}
}

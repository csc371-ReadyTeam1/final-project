using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour {

	public float hitScale = 1.0f;

	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();

	}

	void OnMouseDown() {
		Debug.Log ("Activate trap is: " + orb.activateTrap);
		if (orb.activateTrap) {
			GetComponent<CircleCollider2D> ().radius = 0.5f;
			gameObject.layer = 0;
			rb.gravityScale = 2;
			GetComponent<Collider2D> ().isTrigger = true;
		}
		orb.activateTrap = false;
		Debug.Log ("Activate trap is: " + orb.activateTrap);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.layer == 8)
		{
			Debug.Log ("TODO: Should ignore collision from bullet...");
			//Physics.IgnoreCollision(GameObject.Find("GhostBullet").collider, GetComponent<Collider>());
		}
		PlayerPlatformerController pc = collision.gameObject.GetComponent<PlayerPlatformerController> ();

		if (pc == null)
			return;

		GetComponent<Collider2D> ().isTrigger = false;
		pc.ThrowBack (hitScale);
	}
		
}

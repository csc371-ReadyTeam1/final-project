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
		Debug.Log (orb.activateTrap);
		if (orb.activateTrap) {
			GetComponent<CircleCollider2D> ().radius = 0.5f;
			gameObject.layer = 0;
			rb.gravityScale = 2;
			GetComponent<Collider2D> ().isTrigger = true;
		}
		orb.activateTrap = false;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		PlayerPlatformerController pc = collision.gameObject.GetComponent<PlayerPlatformerController> ();

		if (pc == null)
			return;

		GetComponent<Collider2D> ().isTrigger = false;
		pc.ThrowBack (hitScale);
	}
		
}

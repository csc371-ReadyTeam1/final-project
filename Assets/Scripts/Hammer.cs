using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour {

	public float hitScale = 1.0f;
	public float stunTime = 0.5f;

	private Rigidbody2D rb;
	private CircleCollider2D cc;
	private bool activate = true;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		cc = GetComponent<CircleCollider2D> ();
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
		Projectile tnt = collision.gameObject.GetComponent<Projectile>();
		if (tnt != null && activate) {
			tnt.gameObject.SetActive (false);
			//rb.gravityScale = 1;
			rb.isKinematic = false;
			cc.isTrigger = true;
		}
		PlayerPlatformerController pc = collision.gameObject.GetComponent<PlayerPlatformerController> ();
		if (pc != null) {
			GetComponent<Collider2D> ().isTrigger = false;
			pc.Stun (stunTime);
			pc.ThrowBack (hitScale);
			activate = false;
		}
	}
		
}

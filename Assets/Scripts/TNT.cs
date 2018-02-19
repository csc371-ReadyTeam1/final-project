using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour {

	private Rigidbody2D rb2d;
	private CircleCollider2D cc2d;
	public float hitScale = 2.0f;
	Animator anim;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		cc2d = GetComponent<CircleCollider2D> ();
		anim = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {

	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 8)
		{
			Debug.Log ("TODO: Should ignore collision from bullet...");
			//Physics.IgnoreCollision(GameObject.Find("GhostBullet").collider, GetComponent<Collider>());
		}
		PlayerPlatformerController pc = collision.gameObject.GetComponent<PlayerPlatformerController> ();
		if (pc == null)
			return;
		anim.SetTrigger ("explode");
		pc.ThrowBack (hitScale);
	}

	void OnMouseDown() {
		Debug.Log ("Activate trap is: " + orb.activateTrap);
		if (orb.activateTrap) {
			gameObject.layer = 0;
			GetComponent<Collider2D> ().isTrigger = true;
			cc2d.radius = 0.5f;
			rb2d.gravityScale = 1;
		}
		orb.activateTrap = false;
		Debug.Log ("Activate trap is: " + orb.activateTrap);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingGround : MonoBehaviour {

	private CircleCollider2D cc;
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
		cc = GetComponent<CircleCollider2D> ();
		rb2d = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Projectile pr = collision.gameObject.GetComponent<Projectile>();
		if (pr != null) {
			Debug.Log("Projectile entered...");
			rb2d.bodyType = RigidbodyType2D.Dynamic;
			pr.gameObject.SetActive (false);
		}
	}
}

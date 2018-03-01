using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour {

	private Rigidbody2D rb2d;
	private CircleCollider2D cc2d;

	public AudioClip explosionSound;
	public float hitScale = 10.0f;
	public Animator anim;
	public float stunTime = 0.5f;

	private bool isActive = false;
	private float initialTime = 0.0f;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		cc2d = GetComponent<CircleCollider2D> ();
		anim = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {
		if (!isActive) {
			initialTime = Time.time;
		} else {
			if (Time.time - initialTime > 0.9f) {
				gameObject.SetActive(false);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Projectile tnt = collision.gameObject.GetComponent<Projectile>();
		if (tnt != null) {
			anim.SetTrigger ("explode");
			SoundManager.instance.PlaySingle (explosionSound);
			cc2d.radius = 0.5f;

			//gameObject.layer = 0;
			isActive = true;
			tnt.gameObject.SetActive (false);
			cc2d.isTrigger = true;
		}
		PlayerPlatformerController pc = collision.gameObject.GetComponent<PlayerPlatformerController> ();
		if (pc != null) {
			
			pc.Stun (stunTime);
			pc.ThrowBack (hitScale);
		}
	}
}

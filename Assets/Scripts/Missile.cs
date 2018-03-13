using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Missile : MonoBehaviour {

	private Transform target;
	private Rigidbody2D rb;


	public float speed = 2.0f;
	public float rotateSpeed = 200f;
	public float lifeTime = 15.0f;
	public float hitScale = 1.0f;

	public GameObject ShieldParticles;
	public GameObject explosion;

	// Use this for initialization
	void Start () {
		target = GameObject.FindWithTag ("Player").transform;
		rb = GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update () {
		Vector3 pos = GameObject.FindGameObjectWithTag ("Player").transform.position;
		if (pos.y < -5) {
			//gameController.instance.Died ();
		}
		Vector2 direction = (Vector2)target.position - rb.position;
		direction.Normalize ();

		float rotateAmount = Vector3.Cross (direction, transform.up).z;

		rb.angularVelocity = -rotateAmount * rotateSpeed;

		rb.velocity = transform.up * speed;

		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0){
			//gameController.instance.Scored ();
			Destroy (gameObject);
		}
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerPlatformerController pc = collision.gameObject.GetComponent<PlayerPlatformerController>();


		if (collision.CompareTag("Shield")) {
			//Explode ();
			Debug.Log("shield collision");
			ShieldParticles.gameObject.SetActive(true);
			Destroy (gameObject);
		}
		if (collision.CompareTag("Player")) {
			Debug.Log("shield fail");
			//if (pc == null) return;
			Instantiate (explosion, transform.position, Quaternion.Euler (0, 0, 0));
			pc.ThrowBack (hitScale);
            pc.Stun(hitScale);
			Destroy (gameObject);
		} 

	}
}
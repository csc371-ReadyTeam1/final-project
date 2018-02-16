using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    //How much relative 'power' is behind this projectile.
    //Affects how far the player is pushed back
    public float hitScale = 1.0f;

    //How fast this projectile travels
    public float speed = 1.0f;

    //How long the bullet persists until it is automatically destroyed
    public float lifeTime = 1.0f;

	private Rigidbody2D body;

	void Start() {
		body = GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update () {
		this.transform.position += this.transform.up * Time.deltaTime * speed;

		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0) {
			Destroy (gameObject);
		}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerPlatformerController pc = collision.gameObject.GetComponent<PlayerPlatformerController>();
        if (pc == null) return;

        pc.ThrowBack(hitScale);
        Destroy(gameObject);
    }
}

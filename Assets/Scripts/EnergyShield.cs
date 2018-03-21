using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyShield : MonoBehaviour {
	//How long the bullet persists until it is automatically destroyed
	public float lifeTime = 1.0f;

	private Vector3 offset = new Vector3 (0.15f, 0.05f);

	/* Contributors: Megan Washburn */
	void Start() {
		this.transform.position += offset;
	}

	// Update is called once per frame
	/* Contributors: Megan Washburn */
	void Update () {
		//this.transform.position = move with player...

		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0) {
			Destroy (gameObject);
		}
	}

	/* Contributors: Megan Washburn */
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Projectile p = collision.gameObject.GetComponent<Projectile>();
		if (p == null) return;

		Destroy(collision.gameObject);
	}
}

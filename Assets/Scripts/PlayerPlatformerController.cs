using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : MonoBehaviour {

    public float moveForce = 300.0f;
    public float maxSpeed = 1.0f;

    private Rigidbody2D body;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float horiz = Input.GetAxis("Horizontal");

        //Don't overspeed
        if (horiz * body.velocity.x < maxSpeed)
            body.AddForce(new Vector2(horiz, 0) * moveForce);

        //Slow down if we got too fast
        if (Mathf.Abs(body.velocity.x) > maxSpeed)
            body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * maxSpeed, body.velocity.y);

    }
}

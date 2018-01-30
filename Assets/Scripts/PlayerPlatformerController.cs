using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : MonoBehaviour {

    public float moveAccel = 300.0f;
    public float airMoveAccel = 50.0f;
    public float maxSpeed = 1.0f;
    public float jumpSpeed = 3.0f;
    public float jumpUpForce = 1.0f; //Additional force to apply while jump button held down
    public float friction = 0.83f;

    private Rigidbody2D body;
    private bool canJump = true;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    public void Jump()
    {
        if (!canJump) return;

        canJump = false;
        body.velocity = new Vector2(body.velocity.x, jumpSpeed);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        canJump = true;
    }

    // Update is called once per frame
    void FixedUpdate () {
        float horiz = Input.GetAxisRaw("Horizontal");

        //Don't overspeed
        float curVX = body.velocity.x;
        if (horiz * curVX < maxSpeed)
        {
            float accel = canJump ? moveAccel : airMoveAccel;
            float wishSpeed = horiz * accel * Time.fixedDeltaTime;

            //Cap velocity so we don't go over the max speed
            wishSpeed = wishSpeed - Mathf.Max(wishSpeed + curVX - maxSpeed, 0);
            body.velocity += new Vector2(wishSpeed, 0);
        }

        //Slow down if we got too fast
        if (Mathf.Abs(body.velocity.x) > maxSpeed)
            body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * maxSpeed, body.velocity.y);

        //Perform ground friction if we're on the ground
        if (canJump) //#TODO: "IsOnGround" function
        {
            float fricVel = Time.deltaTime * friction * Mathf.Sign(body.velocity.x);

            //If they're moving slow enough, just set their velocity to 0 immediately
            if (Mathf.Abs(body.velocity.x) < fricVel)
            {
                fricVel = body.velocity.x * Mathf.Sign(body.velocity.x);
            }
            
            body.velocity = new Vector2(body.velocity.x - fricVel, body.velocity.y);
        }
        
        //Jump a little bit higher the longer we hold the button
        if (!canJump && body.velocity.y > 0 && Input.GetButton("Jump"))
        {
            body.AddForce(new Vector2(0, jumpUpForce));
        }
    }
}

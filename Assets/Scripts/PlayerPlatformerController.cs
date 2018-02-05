using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : Pawn
{

    public float moveAccel = 300.0f;
    public float airMoveAccel = 50.0f;
    public float maxSpeed = 1.0f;
    public float jumpSpeed = 3.0f;
    public float jumpUpForce = 1.0f; //Additional force to apply while jump button held down
    public float friction = 0.83f;

    //Force multiplier that damage throwback applies to the character
    public float projectileForceScale = 10.0f;

    private Rigidbody2D body;
    private BoxCollider2D bodyCollider;
    private bool wishJump = false;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();

    }

    private void Update()
    {
        if (IsOnGround() && Controller.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    public void Jump()
    {
        wishJump = true;
    }

    public void ThrowBack(float hitScale)
    {
        body.AddForce(new Vector2(-1, 0.1f) * hitScale * projectileForceScale);
        CameraController.instance.ShakeScreen(hitScale);
    }

    public bool IsOnGround()
    {
        Vector3 bottom = transform.position - transform.up * (bodyCollider.size.y / 2 - bodyCollider.offset.y);
        return Physics2D.Linecast(bottom, transform.position + transform.up * -0.2f, 1 << 9); //9 == World Collision
    }

    // Update is called once per frame
    void FixedUpdate () {
        float horiz = Controller.GetAxisRaw("Horizontal");
        bool isOnGround = IsOnGround();

        //Jump if we want to
        if (isOnGround && wishJump)
        {
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
            isOnGround = false;
            wishJump = false;
        }

        //Don't overspeed
        float curVX = body.velocity.x;
        if (horiz * curVX < maxSpeed)
        {
            float accel = isOnGround ? moveAccel : airMoveAccel;
            float wishSpeed = horiz * accel * Time.fixedDeltaTime;

            //Cap velocity so we don't go over the max speed
            wishSpeed = wishSpeed - Mathf.Max(wishSpeed + curVX - maxSpeed, 0);
            body.velocity += new Vector2(wishSpeed, 0);
        }

        //Slow down if we got too fast
        if (Mathf.Abs(body.velocity.x) > maxSpeed)
            body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * maxSpeed, body.velocity.y);

        //Perform ground friction if we're on the ground
        if (isOnGround)
        {
            float fricVel = Time.deltaTime * friction * Mathf.Sign(body.velocity.x);

            //If they're moving slow enough, just set their velocity to 0 immediately
            if (Mathf.Abs(body.velocity.x) < Mathf.Abs(fricVel))
            {
                fricVel = Mathf.Abs(body.velocity.x) * Mathf.Sign(body.velocity.x);
            }
            
            body.velocity = new Vector2(body.velocity.x - fricVel, body.velocity.y);
        }
        
        //Jump a little bit higher the longer we hold the button
        if (!isOnGround && body.velocity.y > 0 && Controller.GetButton("Jump"))
        {
            body.AddForce(new Vector2(0, jumpUpForce));
        }
    }
}

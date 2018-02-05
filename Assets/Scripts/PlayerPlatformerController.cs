using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPlatformerController : Pawn
{
    public float moveAccel = 300.0f;
    public float airMoveAccel = 50.0f;
    public float maxSpeed = 1.0f;
    public float jumpSpeed = 3.0f;
    public float jumpUpForce = 1.0f; //Additional force to apply while jump button held down
    public float friction = 0.83f;

    //How long it takes before the hit combo resets
    public float comboResetTime = 1.0f;

    //How much more to scale hurt effects as combo increases
    public float hitComboThrowScale = 200;

    public int maxHitCombo = 4;
    public float stunDuration = 0.25f;

    //Force multiplier that damage throwback applies to the character
    public float projectileForceScale = 10.0f;

    public Text StunText;

    private Rigidbody2D body;
    private BoxCollider2D bodyCollider;
    private bool wishJump = false;

    private int hitCombo = 0;
    private float hitComboResetTime = 0;
    private float stunResetTime = 0;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();

    }

    public bool IsStunned()
    {
        return stunResetTime > 0;
    }

    private void Update()
    {
        if (!IsStunned() && IsOnGround() && Controller.GetButtonDown("Jump"))
        {
            Jump();
        }

        //Reset hit combo
        hitComboResetTime -= Time.deltaTime;
        if (hitCombo > 0 && hitComboResetTime < 0)
        {
            hitCombo = 0;
        }

        //Reset stun
        StunText.text = IsStunned() ? "STUNNED!!" : "";
        if (IsStunned())
        {
            StunText.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 13) * 5.0f);
            stunResetTime -= Time.deltaTime;
        }
    }

    public void Jump()
    {
        wishJump = true;
    }

    public void ThrowBack(float hitScale)
    {
        bool onGround = IsOnGround();
        float scale = hitScale * projectileForceScale + hitCombo * hitComboThrowScale;
        body.AddForce(new Vector2(-1, onGround ? 0.3f : 0.1f) * scale);
        CameraController.instance.ShakeScreen(hitScale + hitCombo * 0.1f);

        hitCombo++;
        Mathf.Min(hitCombo, maxHitCombo);
        hitComboResetTime = comboResetTime;

        //If they got hit tons of times in a row, temporarily stun
        if (!IsStunned() && hitCombo == maxHitCombo)
        {
            stunResetTime = stunDuration;
        }
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

        //Totally disable input if they're stunned
        if (IsStunned())
        {
            horiz = 0;
        }

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
        if (!isOnGround && body.velocity.y > 0 && Controller.GetButton("Jump") && !IsStunned())
        {
            body.AddForce(new Vector2(0, jumpUpForce));
        }
    }
}

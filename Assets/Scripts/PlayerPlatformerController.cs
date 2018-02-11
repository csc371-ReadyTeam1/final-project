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
	public float hitNum = 0;

	public GameObject ShieldPrefab;

	// SFX
	public AudioClip move1Sound;
	public AudioClip move2Sound;
	public AudioClip hit1Sound;
	public AudioClip hit2Sound;
	public AudioClip jumpSound;
	public AudioClip stunnedSound;
	public AudioClip shieldSound;

    //Force multiplier that damage throwback applies to the character
    public float projectileForceScale = 10.0f;

    private Rigidbody2D body;
    private bool canJump = true;
	private bool shieldReady = true;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
			SoundManager.instance.PlaySingle (jumpSound);
            Jump();
        }

		//Spawn shield
		if (Input.GetButtonDown ("Fire1")) {
			if (shieldReady) {
				SoundManager.instance.PlaySingle (shieldSound);
				Instantiate (ShieldPrefab, transform.position, Quaternion.Euler (0, 0, 0));
			}
		}
    }

    public void Jump()
    {
        if (!canJump) return;

        canJump = false;
        body.velocity = new Vector2(body.velocity.x, jumpSpeed);
    }

    public void ThrowBack(float hitScale)
    {
		hitNum++;

		SoundManager.instance.RandomSfx (hit1Sound, hit2Sound);

        body.AddForce(new Vector2(-1, 0.1f) * hitScale * projectileForceScale);
        CameraController.instance.ShakeScreen(hitScale);
    }

	IEnumerator WaitForStunToEnd(float hitTime, Color ogColor) {
		// Wait for hitTime seconds
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();

		yield return new WaitForSeconds(hitTime);

		renderer.color = ogColor;
	}

	public void Stun(float hitTime)
	{
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		Color ogColor = renderer.color;
		renderer.color = new Color(0.7f, 0.7f, 0.7f, 0.5f); // Set to opaque gray

		SoundManager.instance.PlaySingle (stunnedSound);

		StartCoroutine(WaitForStunToEnd(hitTime, ogColor));
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

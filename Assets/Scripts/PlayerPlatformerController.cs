using Cinemachine;
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



    /// <summary>
    /// Speed to use for double jumps while in air
    /// </summary>
    public float doubleJumpSpeed = 1.5f; 
    public float jumpUpForce = 1.0f; //Additional force to apply while jump button held down
    public float friction = 0.83f;

	// ShielfFull not used, half shielf uses full colllider - AB
	public GameObject ShieldPrefab;
	public GameObject ShieldFull;


	// SFX
	public AudioClip move1Sound;
	public AudioClip move2Sound;
	public AudioClip hit1Sound;
	public AudioClip hit2Sound;
	public AudioClip jumpSound;
	public AudioClip stunnedSound;
	public AudioClip shieldSound;

    //How long it takes before the hit combo resets
    public float comboResetTime = 1.0f;

    //How much more to scale hurt effects as combo increases
    public float hitComboThrowScale = 200;

    public int maxHitCombo = 4;
    public float stunDuration = 0.25f;

    //Force multiplier that damage throwback applies to the character
    public float projectileForceScale = 10.0f;

    /// <summary>
    /// How many times the player can jump before touching the ground
    /// </summary>
    public int NumJumps = 2;

    /// <summary>
    /// Allow jumping against walls?
    /// </summary>
    public bool WallJump = true;

    public Text StunText;
    public AudioClip HitSound;
    public AudioClip StunSound;

    private Rigidbody2D body;
    private BoxCollider2D bodyCollider;
    private AudioSource audioSrc;
    private bool wishJump = false;

    //Hit combo/stunning
    private int hitCombo = 0;
    private float hitComboResetTime = 0;
    private float stunResetTime = 0;

    //Jump info
    private int jumpCount = 0;
	private bool shieldReady = true;
    private Color ogColor;

    // Use this for initialization
    /* Contributors: Scott Kauker */
    void Start () 
    {
        body = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        audioSrc = GetComponent<AudioSource>();

        //Store original sprite color
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        ogColor = renderer.color;

        //For both virtual cameras, set ourselves as the follow object so they focus on us
        CameraController.instance.SetActiveFollower(gameObject);
        GameController.instance.VirtualMinigameCam.GetComponent<CinemachineVirtualCamera>().Follow = gameObject.transform;
    }

    /// <summary>
    /// When a player possesses this pawn, we want to set the color of the game object to match their color
    /// </summary>
    /* Contributors: Scott Kauker */
    public override void OnPossessed()
    {
        NametagCreator nametag = GetComponent<NametagCreator>();
        nametag.SetText(Controller.Name);
        nametag.SetColor(Controller.PlayerColor);
    }

    /* Contributors: Scott Kauker */
    public bool IsStunned()
    {
        return stunResetTime > 0;
    }

    /* Contributors: Scott Kauker */
    private void Update()
    {
        if (Controller == null) return;

        if (!IsStunned() && Controller.GetButtonDown("Jump"))
        {
			SoundManager.instance.PlaySingle (jumpSound);
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
        else
        {
            //Reset sprite color when stun ends
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.color = ogColor;    
        }

		//Spawn shield
		if (Controller.GetButtonDown ("Fire2")) {
			if (shieldReady) {
				SoundManager.instance.PlaySingle (shieldSound);
				Instantiate (ShieldPrefab, transform.position, Quaternion.Euler (0, 0, 0));
			}
		}
    }

    /* Contributors: Scott Kauker */
    public void Jump()
    {
        wishJump = true;
    }

    /* Contributors: Scott Kauker */
    public void ResetStun()
    {
        stunResetTime = 0;
    }

    /* Contributors: Scott Kauker */
    public void ThrowBack(float hitScale)
    {
        bool onGround = IsOnGround();
        float scale = hitScale * projectileForceScale + hitCombo * hitComboThrowScale;

        body.AddForce(new Vector2(-1, onGround ? 0.3f : 0.1f) * scale);
        CameraController.instance.ShakeScreen(hitScale + hitCombo * 0.1f);

        hitCombo = Mathf.Min(hitCombo + 1, maxHitCombo);
        hitComboResetTime = comboResetTime;

        //If they got hit tons of times in a row, temporarily stun
        if (!IsStunned() && hitCombo == maxHitCombo)
        {
            Stun(stunDuration);
            audioSrc.pitch = 1;
            audioSrc.PlayOneShot(StunSound);
        }
        else
        {
            audioSrc.pitch = 1 + (hitCombo * 1.0f / maxHitCombo);
            audioSrc.PlayOneShot(HitSound);
        }
    }

    /* Contributors: Scott Kauker */
    public void Stun(float hitTime)
	{
        //Activate stun mode, delay by hitTime
        stunResetTime = hitTime;

        //Temporarily change sprite color
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		Color ogColor = renderer.color;
		renderer.color = new Color(0.7f, 0.7f, 0.7f, 0.5f); // Set to opaque gray

		SoundManager.instance.PlaySingle (stunnedSound);
	}

    /* Contributors: Scott Kauker */
    private bool GroundCast(float horizontalOffset)
    {
        Vector3 bottom = transform.position - transform.up * (bodyCollider.size.y / 2 - bodyCollider.offset.y) * transform.localScale.y;
        bottom += new Vector3(horizontalOffset, 0, 0); //Apply horizontal offset

        Debug.DrawLine(bottom, bottom + transform.up * -0.03f * transform.localScale.y, Color.green);
        //9 == World Collision
        return Physics2D.Linecast(bottom, bottom + transform.up * -0.03f * transform.localScale.y, 1 << 9);
    }

    /* Contributors: Scott Kauker */
    public bool IsOnGround()
    {
        float offset = (bodyCollider.size.x / 2) * transform.localScale.x;
        return GroundCast(offset) || GroundCast(-offset);
    }

    // Update is called once per frame
    /* Contributors: Scott Kauker */
    void FixedUpdate () {
        if (Controller == null) { return; }
        float horiz = Controller.GetAxisRaw("Horizontal");
        bool isOnGround = IsOnGround();

        //Totally disable input if they're stunned
        if (IsStunned())
        {
            horiz = 0;
        }
		// Temp update to test full shield activating
		if (Input.GetKey ("v")) {
			ShieldFull.SetActive (true);
			//Debug.Log ("pressing V key");
		} else {
			ShieldFull.SetActive (false);
		} 

        //Reset jump count when they hit the ground
        if (isOnGround && body.velocity.y <= 0)
        {
            jumpCount = 0;
        }

        //Jump if we want to
        bool canJump = isOnGround || jumpCount < NumJumps;
        if (canJump && wishJump)
        {
            //Use a lower jump speed for airjumps
            float jumpVelocity = isOnGround ? jumpSpeed : doubleJumpSpeed;

            jumpCount++;
            body.velocity = new Vector2(body.velocity.x, jumpVelocity);
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

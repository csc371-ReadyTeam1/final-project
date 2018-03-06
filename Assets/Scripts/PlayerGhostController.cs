using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class PlayerGhostController : Pawn
{

    public Vector2 from;
    public Vector2 to;

    public float SmoothTime = 0.1f;
    public float MoveSpeed = 1.0f;
    public float MouseSmoothTime = 0.1f;
    public bool useMouseInput = false;

    public float fireRate = 0.25f;

	private GameObject gunImage;
	private GameObject missileImage;

    public GameObject BulletPrefab;
	public GameObject HomingPrefab;
	public GameObject ChargedProjectile;

	// SFX
	public AudioClip fireSound;
	public AudioClip reloadSound;
    public AudioClip bodySwitchSound;

	private GameObject[] chargedBullets;
	private Color ogColor;
	private Color greyedOut = new Color(0.7f, 0.7f, 0.7f, 0.5f);
    private float goalPos = 0.5f; //Between 0 and 1
    private float curPos;
	private int numBullets = 3;
	private int bulletIndex = 0;
	private float coolTime = 3.0f;
	private float timer = 0.0f;
	private int weaponNum = 1;
	public int numOfMissiles = 5;

    private float fireCooldown = 0;
    private bool isCoolingDown = false;

    //When the human is stunned, the ghost is able to 'take over' them
    private Vector2 altGoalPos;
    private Vector2 altCurPos;

	// Use this for initialization
	void Start () {
		gunImage = GameObject.Find ("gunImage");
		missileImage = GameObject.Find ("missileImage");
		missileImage.SetActive (false);
		chargedBullets = new GameObject[numBullets];
		Vector3 bulletPos = new Vector3 (2.5f, 1.95f);

		for (int i = 0; i < numBullets; i++) {
			bulletPos.y = bulletPos.y - 0.1f;

			chargedBullets[i] = Instantiate (ChargedProjectile, bulletPos, Quaternion.Euler (0, 0, 90));
		}

		// Saving color of first bullet for reference when reloading
		ogColor = chargedBullets [bulletIndex].GetComponent<SpriteRenderer>().color;

        //Hook into when the human player is stunned (letting the ghost take over)
        GameController.instance.OnHumanStunned += Instance_OnHumanStunned;
	}

    /// <summary>
    /// When a player possesses this pawn, we want to set the color of the game object to match their color
    /// </summary>
    public override void OnPossessed()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = Controller.PlayerColor;

        NametagCreator nametag = GetComponent<NametagCreator>();
        nametag.SetText(Controller.Name);
        nametag.SetColor(Controller.PlayerColor);
    }

    private void Instance_OnHumanStunned()
    {
        altGoalPos = Camera.main.WorldToViewportPoint(transform.position);
        altCurPos = altGoalPos;
    }

    private void reloadBullets() {
		SoundManager.instance.PlaySingle (reloadSound);

		for (int i = 0; i < numBullets; i++) {
			chargedBullets [i].GetComponent<SpriteRenderer>().color = ogColor;
		}
	}

	// Simple weapon switching function. As we add more weapons just increase the num!
	// For now,
	// Weapon 1: Regular gun/bullet
	// Weapon 2: Homing Missiles
	private void switchWeapons() {
		weaponNum++;
		gunImage.SetActive (false);
		missileImage.SetActive (true);
		if (weaponNum > 2) {
			weaponNum = 1;
			gunImage.SetActive (true);
			missileImage.SetActive (false);
		} else {
		}
	}

    //A better framerate-aware smooth lerp function.
    // Different from Vector3.SmoothDamp() since that has stuttering issues
    //#TODO: Function library?
    private float Damp(float a, float b, float smoothing, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Pow(smoothing, dt));
    }

    private Vector2 Damp(Vector2 a, Vector2 b, float smoothing, float dt)
    {
        return new Vector2(Damp(a.x, b.x, smoothing, dt),
            Damp(a.y, b.y, smoothing, dt));
    }

	IEnumerator Cooldown() {
        if (isCoolingDown) yield break;

        isCoolingDown = true;
        yield return new WaitForSeconds(coolTime);
        isCoolingDown = false;

        reloadBullets ();
		bulletIndex = 0;
	}

    void performMovement()
    {
		
        //Update edge points
        from = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.01f));
        to = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.99f));

        if (useMouseInput)
        {
            goalPos = Camera.main.ScreenToViewportPoint(Input.mousePosition).y;
        }
        else
        {
            goalPos += Controller.GetAxisRaw("Vertical") * MoveSpeed;
            goalPos = Mathf.Clamp(goalPos, 0, 1);
        }

        //Move the relative position
        curPos = Damp(curPos, goalPos, useMouseInput ? MouseSmoothTime : SmoothTime, Time.deltaTime);

        if (Controller.GetButtonDown ("Fire2")) {
			switchWeapons ();
		}

        //Spawn bullets
		if (Controller.GetButtonDown ("Fire1")) {
			if (bulletIndex != numBullets && weaponNum == 1) {
				SoundManager.instance.PlaySingle (fireSound);
				Instantiate (BulletPrefab, transform.position, Quaternion.Euler (0, 0, 90));

				// Show bullet was unloaded in stock; remove BulletCountdown
				chargedBullets [bulletIndex].GetComponent<SpriteRenderer> ().color = greyedOut;
				bulletIndex++;

                //Start reloading when it's the last shot
                if (bulletIndex == numBullets)
                {
                    StartCoroutine(Cooldown());// Cooldown. TODO: Do we want to let guns cooldown while using other weapons?
                }
			} else if (weaponNum == 2 && numOfMissiles > 0) { //Homing missiles
				Instantiate (HomingPrefab, transform.position, Quaternion.Euler (0, 0, 90));
				numOfMissiles--;
			}
        }

		if (weaponNum == 3) {
			orb.onWeapon3 = true;
		} else
			orb.onWeapon3 = false;
    }
    
    //The second movement mode of the ghost, when they are able to take over the human
    //This allows 2D movement across the screen
    void performAltMovement()
    {
        altGoalPos.x += Controller.GetAxisRaw("Horizontal") * MoveSpeed;
        altGoalPos.y += Controller.GetAxisRaw("Vertical") * MoveSpeed;

        altGoalPos.x = Mathf.Clamp01(altGoalPos.x);
        altGoalPos.y = Mathf.Clamp01(altGoalPos.y);

        //Move the relative position
        altCurPos = Damp(altCurPos, altGoalPos, SmoothTime, Time.deltaTime);
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (Controller == null) return;
        //Update goal pos
        if (GameController.instance.IsHumanStunned())
        {
            performAltMovement();
        }
        else
        {
            performMovement();
        }


        

        Vector2 normalPos = Vector2.Lerp(from, to, curPos);
        Vector2 altPos = Camera.main.ViewportToWorldPoint(altGoalPos);
        Vector2 actualPos = GameController.instance.IsHumanStunned() ? altPos : normalPos;




        //Actually set the world position of the ghost
        transform.position = actualPos;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (Controller == null) return;

        var ply = coll.gameObject.GetComponent<PlayerPlatformerController>();
        if (ply != null && ply.IsStunned())
        {
            GameController.instance.SwitchPlayers();
            ply.ResetStun(); //Reset the stun immediately
            SoundManager.instance.PlaySingle(bodySwitchSound, 0.5f);
        }
    }
}

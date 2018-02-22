﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostController : Pawn
{

    public Vector2 from;
    public Vector2 to;

    public float SmoothTime = 0.1f;
    public float MouseSmoothTime = 0.1f;
    public bool useMouseInput = false;

    public float fireRate = 0.25f;

    public GameObject BulletPrefab;
	public GameObject HomingPrefab;
	public GameObject ChargedProjectile;

	// SFX
	public AudioClip fireSound;
	public AudioClip reloadSound;

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

	// Use this for initialization
	void Start () {
		chargedBullets = new GameObject[numBullets];

		Vector3 bulletPos = new Vector3 (2.5f, 1.95f);

		for (int i = 0; i < numBullets; i++) {
			bulletPos.y = bulletPos.y - 0.1f;

			chargedBullets[i] = Instantiate (ChargedProjectile, bulletPos, Quaternion.Euler (0, 0, 90));
		}

		// Saving color of first bullet for reference when reloading
		ogColor = chargedBullets [bulletIndex].GetComponent<SpriteRenderer>().color;

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
		if (weaponNum > 3) {
			weaponNum = 1;
		}
	}

    //A better framerate-aware smooth lerp function.
    // Different from Vector3.SmoothDamp() since that has stuttering issues
    //#TODO: Function library?
    private float Damp(float a, float b, float smoothing, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Pow(smoothing, dt));
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
        if (useMouseInput)
        {
            goalPos = Camera.main.ScreenToViewportPoint(Input.mousePosition).y;
        }
        else
        {
            goalPos += Controller.GetAxis("Vertical");
            goalPos = Mathf.Clamp(goalPos, 0, 1);
        }

//<<<<<<< HEAD
/*
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        //Spawn bullets
        if (Controller.GetButtonDown("Fire1") && fireCooldown <= 0)
        {
            Instantiate(BulletPrefab, transform.position, Quaternion.Euler(0, 0, 90));
            fireCooldown = fireRate;
 */
//=======
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
//>>>>>>> 589b3040736f84074916e936e877bafe4b7b5711
        }

		if (weaponNum == 3) {
			orb.onWeapon3 = true;
		} else
			orb.onWeapon3 = false;
    }
	
	// Update is called once per frame
	void LateUpdate () {

        //Update goal pos
        performMovement();

        //Update edge points
        from = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.01f));
        to = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.99f));

        //Move the ghosty
        curPos = Damp(curPos, goalPos, useMouseInput ? MouseSmoothTime : SmoothTime, Time.deltaTime);

        transform.position = Vector2.Lerp(from, to, curPos);
	}
}

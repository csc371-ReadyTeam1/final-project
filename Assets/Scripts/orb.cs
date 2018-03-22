using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************/
/* Author: David Rifkin */
/************************/

public class orb : MonoBehaviour {

	public static bool onWeapon3 = false;
	public static bool activateTrap = false;

	private Animator anim;
	private bool mDown = false;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButtonDown(0) && onWeapon3) {
			
			Debug.Log ("Mouse clicked, mDown: " + mDown);
			if (!mDown) {
				anim.SetTrigger ("create");
				activateTrap = true;
				mDown = true;
			} else if (mDown) {
				anim.SetTrigger ("remove");
				activateTrap = false;
				mDown = false;
			}

		}
			
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = 1.0f;
		transform.position = Camera.main.ScreenToWorldPoint (mousePosition);
	}
}

using System.Collections;
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

    private float goalPos = 0.5f; //Between 0 and 1
    private float curPos;

    private float fireCooldown = 0;

	// Use this for initialization
	void Start () {
		
	}

    //A better framerate-aware smooth lerp function.
    // Different from Vector3.SmoothDamp() since that has stuttering issues
    //#TODO: Function library?
    private float Damp(float a, float b, float smoothing, float dt)
    {
        return Mathf.Lerp(a, b, 1 - Mathf.Pow(smoothing, dt));
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

        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        //Spawn bullets
        if (Controller.GetButtonDown("Fire1") && fireCooldown <= 0)
        {
            Instantiate(BulletPrefab, transform.position, Quaternion.Euler(0, 0, 90));
            fireCooldown = fireRate;
        }
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

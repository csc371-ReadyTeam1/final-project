using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostController : MonoBehaviour {

    public Vector2 from;
    public Vector2 to;

    public float SmoothTime = 0.1f;
    public float MouseSmoothTime = 0.1f;
    public bool useMouseInput = false;

    public GameObject BulletPrefab;

    private float goalPos = 0.5f; //Between 0 and 1
    private float curPos;

	// Use this for initialization
	void Start () {
		
	}

    void performMovement()
    {
        if (useMouseInput)
        {
            goalPos = Camera.main.ScreenToViewportPoint(Input.mousePosition).y;
        }
        else
        {
            goalPos += Input.GetAxis("P2_Vertical");
            goalPos = Mathf.Clamp(goalPos, 0, 1);
        }

        //Spawn bullets
        if (Input.GetButtonDown("P2_Fire1"))
        {
            Instantiate(BulletPrefab, transform.position, Quaternion.Euler(0, 0, 90));
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
        float vel = 0;
        curPos = Mathf.SmoothDamp(curPos, goalPos, ref vel, useMouseInput ? MouseSmoothTime : SmoothTime);

        transform.position = Vector2.Lerp(from, to, curPos);
	}
}

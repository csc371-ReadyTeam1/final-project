using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostController : MonoBehaviour {

    public Vector2 from;
    public Vector2 to;

    public float SmoothTime = 0.1f;

    private float goalPos; //Between 0 and 1
    private float curPos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {

        //Update goal pos
        goalPos = Camera.main.ScreenToViewportPoint(Input.mousePosition).y;

        //Update edge points
        
        from = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.01f));
        to = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0.99f));

        //Move the ghosty
        float vel = 0;
        curPos = Mathf.SmoothDamp(curPos, goalPos, ref vel, SmoothTime);

        transform.position = Vector2.Lerp(from, to, curPos);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject target;
    public float SmoothTime = 1.0f;
    public float MaxSmoothSpeed = 100.0f;

    private Vector3 offset;

	// Use this for initialization
	void Start () {
        offset = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 goalPos = new Vector3(target.transform.position.x, 0, 0) + offset;

        Vector3 curVel = Vector3.zero;
        Vector3 pos = Vector3.SmoothDamp(transform.position, goalPos, ref curVel,
            SmoothTime, MaxSmoothSpeed, Time.deltaTime);
        transform.position = pos;
    }
}

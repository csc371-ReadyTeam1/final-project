using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject target;
    public float SmoothTime = 1.0f;

    private Vector3 offset;

	// Use this for initialization
	void Start () {
        offset = transform.position;
    }

    //A better framerate-aware smooth lerp function.
    // Different from Vector3.SmoothDamp() since that has stuttering issues
    private Vector3 Damp(Vector3 a, Vector3 b, float smoothing, float dt)
    {
        return Vector3.Lerp(a, b, 1 - Mathf.Pow(smoothing, dt));
    }

    // Update is called once per frame
    void LateUpdate()
    {  
        Vector3 goalPos = new Vector3(target.transform.position.x, 0, 0) + offset;
        transform.position = Damp(transform.position, goalPos, SmoothTime, Time.deltaTime);
    }
}

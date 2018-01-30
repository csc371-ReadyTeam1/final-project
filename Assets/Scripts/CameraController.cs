using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject target;
    public float SmoothTime = 1.0f;
    public float screenShakeScale = 1.0f;

    public static CameraController instance;

    private float shakeScreenEndTime = 0.0f;
    private Vector3 offset;
    private Vector3 lastPos;

	// Use this for initialization
	void Start () {
        offset = transform.position;
        lastPos = offset;

        if (instance == null)
        {
            instance = this;
        }
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
        //Calculate screen shake
        float amt = Mathf.Max(shakeScreenEndTime - Time.time, 0) * screenShakeScale;
        Vector3 shakeOffset = new Vector3(Random.Range(-amt, amt), Random.Range(-amt, amt), 0);
        Vector3 goalPos = new Vector3(target.transform.position.x, 0, 0) + offset;

        lastPos = Damp(lastPos, goalPos, SmoothTime, Time.deltaTime);

        transform.position = lastPos + shakeOffset;
    }

    public void ShakeScreen(float intensity)
    {
        shakeScreenEndTime = Time.time + intensity;
    }
}

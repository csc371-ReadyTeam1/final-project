using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : CinemachineExtension
{
    //public GameObject target;
    //public float SmoothTime = 1.0f;
    public float screenShakeScale = 1.0f;

    public static CameraController instance;

    private float shakeScreenEndTime = 0.0f;
    private float defaultCamX = 0.0f;
    //private Vector3 offset;
    //private Vector3 lastPos;

    // Use this for initialization
    void Start ()
    {
        //offset = transform.position;
        //lastPos = offset;

        //#TODO: Store defaultCamX from the Frame Transposer
        //VirtualCamera.

        if (instance == null)
        {
            instance = this;
        }
    }

    //A better framerate-aware smooth lerp function.
    // Different from Vector3.SmoothDamp() since that has stuttering issues
    /*
    private Vector3 Damp(Vector3 a, Vector3 b, float smoothing, float dt)
    {
        return Vector3.Lerp(a, b, 1 - Mathf.Pow(smoothing, dt));
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
        {
            target = GameController.instance.platformer.gameObject;
            return;
        }

        //Calculate screen shake
        float amt = Mathf.Max(shakeScreenEndTime - Time.time, 0) * screenShakeScale;
        Vector3 shakeOffset = new Vector3(Random.Range(-amt, amt), Random.Range(-amt, amt), 0);
        Vector3 goalPos = new Vector3(target.transform.position.x, 0, 0) + offset;

        lastPos = Damp(lastPos, goalPos, SmoothTime, Time.deltaTime);

        //transform.position = lastPos + shakeOffset;
    }*/

    /// <summary>
    /// Callback from cinemachine so we can apply an offset to the camera without feeding back into
    /// the sytem. Used to implement screenshake.
    /// </summary>
    /// <param name="vcam"></param>
    /// <param name="stage"></param>
    /// <param name="state"></param>
    /// <param name="deltaTime"></param>
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Noise) return;

        //Calculate screen shake
        float amt = Mathf.Max(shakeScreenEndTime - Time.time, 0) * screenShakeScale;
        Vector3 shakeOffset = new Vector3(Random.Range(-amt, amt), Random.Range(-amt, amt), 0);

        state.PositionCorrection += shakeOffset;
    }

    public void ShakeScreen(float intensity)
    {
        shakeScreenEndTime = Time.time + intensity;
    }

    /// <summary>
    /// Temporarily override the horizontal position the player centers on in the camera.
    /// This is used during the minigame when we want the player centered, but the rest of
    /// the game the player tending to be on the left side.
    /// </summary>
    /// <param name="perc">x position of the focus center in camera space (0-1)</param>
    public void SetHorizontalOffsetOverride(float perc)
    {

    }

    /// <summary>
    /// Reset any active overrides for the horizontal position and use the default one
    /// specified as a property
    /// </summary>
    public void ResetHorizontalOffsetOverride()
    {

    }

    public void SetActiveFollower(GameObject obj)
    {
        VirtualCamera.Follow = obj.transform;
    }
}

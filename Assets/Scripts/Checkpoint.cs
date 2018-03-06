using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour {

    public Sprite CapturedSprite;
    public Text CaptureText;
    public float CaptureTime = 1.0f;

    /// <summary>
    /// Whether or not this checkpoint is required for the platformer to win
    /// </summary>
    public bool IsFinish = false;

    public GameObject ForcefieldToggle;

    float captureAmount;
    Pawn currentCapturer = null;
    bool isCaptured = false;
    SpriteRenderer render;
    private AudioSource activateSrc;

    // Use this for initialization
    void Start () {
        render = GetComponent<SpriteRenderer>();
        activateSrc = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (currentCapturer == null || isCaptured) return;

        captureAmount += Time.deltaTime;
        CaptureText.text = Mathf.Round(captureAmount * 100 / CaptureTime) + "% captured";

        if (captureAmount > CaptureTime)
        {
            Capture(currentCapturer.Controller);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Pawn pawn = collision.gameObject.GetComponent<Pawn>();
        if (pawn != null && pawn == GameController.instance.platformer)
        {
            currentCapturer = pawn;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Pawn pawn = collision.gameObject.GetComponent<Pawn>();
        if (pawn != null && pawn == currentCapturer)
        {
            currentCapturer = null;
        }
    }

    public void Capture(PlayerController capturer)
    {
        render.sprite = CapturedSprite;
        CaptureText.text = "CAPTURED!!!";
        isCaptured = true;

        activateSrc.Play();

        if (IsFinish)
        {
            GameController.instance.Finish(capturer);
        }
        else
        {
            ForcefieldToggle.SetActive(true);
        }
    }
}

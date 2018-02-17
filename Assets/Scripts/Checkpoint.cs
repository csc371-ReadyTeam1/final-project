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

    public AudioClip CaptureSound;

    public GameObject ForcefieldToggle;

    float captureAmount;
    bool isBeingCaptured = false;
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
        if (!isBeingCaptured || isCaptured) return;

        captureAmount += Time.deltaTime;
        CaptureText.text = Mathf.Round(captureAmount * 100 / CaptureTime) + "% captured";

        if (captureAmount > CaptureTime)
        {
            Capture();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isBeingCaptured = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isBeingCaptured = false;
        }
    }

    public void Capture()
    {
        render.sprite = CapturedSprite;
        CaptureText.text = "CAPTURED!!!";
        isCaptured = true;

        activateSrc.Play();

        if (IsFinish)
        {
            GameController.instance.Finish(false);
        }
        else
        {
            ForcefieldToggle.SetActive(true);
        }
    }
}

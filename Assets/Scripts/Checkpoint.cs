using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour {

    public Sprite CapturedSprite;
    public Text CaptureText;
    public float CaptureTime = 1.0f;

    float captureAmount;
    bool isBeingCaptured = false;
    bool isCaptured = false;
    SpriteRenderer render;

	// Use this for initialization
	void Start () {
        render = GetComponent<SpriteRenderer>();
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

        GameController.instance.Finish(false);
    }
}

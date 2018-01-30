using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public Text timerText;
    public Text winText;
    public float timeLimit = 60.0f;

    public static GameController instance;

    private float curTimeLeft;
    bool isPlaying = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start () {
        StartGame();
    }

    public void StartGame()
    {
        curTimeLeft = timeLimit;
        isPlaying = true;
    }

    public void Finish(bool timeOut)
    {
        isPlaying = false;

        winText.text = timeOut ? "SPIRIT WINS" : "BODY WINS";
        winText.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        //Only perform game logic if the game is active and playing
        if (!isPlaying) { return; }

        //Every frame, decrement however much time has passed from the 'time left'
        curTimeLeft -= Time.deltaTime;

        if (curTimeLeft < 0)
        {
            curTimeLeft = 0;
        }

        //Also update the timer text too
        float minutes = Mathf.Floor(curTimeLeft / 60.0f);
        float seconds = Mathf.Floor(curTimeLeft % 60);
        float ms = Mathf.Floor((curTimeLeft % 1.0f) * 10.0f);
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + "." + ms.ToString("00");

        if (curTimeLeft <= 0)
        {
            Finish(true);
        }
    }
}

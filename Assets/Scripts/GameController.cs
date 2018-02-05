using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    WAITING_FOR_PLAYERS,
    PLAYING,
    GAMEOVER
}

public class GameController : MonoBehaviour {

    public Text timerText;
    public Text winText;
    public float timeLimit = 60.0f;

    public GameObject GhostPrefab;
    public GameObject PlatformerPrefab;

    public GameObject platformer { get; private set; }
    public GameObject ghost { get; private set; }

    public static GameController instance;
    public List<PlayerController> Players = new List<PlayerController>();

    //#TODO: This is a pretty gross pattern, think of a better way architecturally to do this
    //Perhaps on game startup choose the player inputs, and those objects exist always throughout scenes?
    public static bool PlayersSwitched = true;

    private float curTimeLeft;
    GameState State = GameState.WAITING_FOR_PLAYERS;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start () {
        SetupPlayers();

        //Toggle switching players every time the game ends
        if (PlayersSwitched)
            SwitchPlayers();

        StartGame();
    }

    //#TODO: Add some sort of 'Press any key to join' phase.
    //This can be done by detecting which input index a key is pressed from, 
    //and creating/assigning a player controller to that index until all slots (2) are filled
    //For now, we just assume player 1 is index 1, and player 2 is index 2
    void SetupPlayers()
    {
        PlayerController pc1 = AddPlayer();
        PlayerController pc2 = AddPlayer();

        //Spawn and hook up the 'ghost'
        ghost = Instantiate(GhostPrefab);
        Pawn p1 = ghost.GetComponent<Pawn>();
        pc1.Possess(p1);

        //Spawn and hook up the platformer
        platformer = Instantiate(PlatformerPrefab);
        Pawn p2 = platformer.GetComponent<Pawn>();
        pc2.Possess(p2);
    }

    void SwitchPlayers()
    {
        Pawn pghost = ghost.GetComponent<Pawn>();
        Pawn pplat = platformer.GetComponent<Pawn>();

        PlayerController pcplat = pplat.Controller;
        pghost.Controller.Possess(pplat);

        pcplat.Possess(pghost);
    }

    void RestartLevel()
    {
        PlayersSwitched = !PlayersSwitched;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    PlayerController AddPlayer()
    {
        PlayerController pc = gameObject.AddComponent<PlayerController>();
        pc.SetInputIndex(Players.Count + 1);
        Players.Add(pc);

        return pc;
    }

    public void StartGame()
    {
        curTimeLeft = timeLimit;
        State = GameState.PLAYING;
    }

    public void Finish(bool timeOut)
    {
        State = GameState.GAMEOVER;

        winText.text = timeOut ? "SPIRIT WINS" : "PERSON WINS";
        winText.gameObject.SetActive(true);

        //Restart the level in 5 seconds with the players switched
        Invoke("RestartLevel", 5.0f);
    }
	
	// Update is called once per frame
	void Update () {

        if (State == GameState.WAITING_FOR_PLAYERS)
        {

        }

        //Only perform game logic if the game is active and playing
        if (State != GameState.PLAYING) { return; }

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

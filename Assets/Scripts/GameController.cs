using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState
{
    WAITING_FOR_PLAYERS,
    MINIGAME,
    PLAYING,
    GAMEOVER
}

public class GameController : MonoBehaviour {

    public Text timerText;
    public Text winText;
    public float timeLimit = 60.0f;

    public GameObject GhostPrefab;
    public GameObject PlatformerPrefab;
    public GameObject MinigameGhostPrefab;

    public Pawn platformer { get; private set; }
    public Pawn ghost { get; private set; }

    public PlayerController[] players { get; private set; }

    public static GameController instance;
    public List<PlayerController> Players = new List<PlayerController>();

    public delegate void StunAction();
    /// <summary>
    /// Called when the human has been stunned and cannot move
    /// </summary>
    public event StunAction OnHumanStunned;

    //#TODO: This is a pretty gross pattern, think of a better way architecturally to do this
    //Perhaps on game startup choose the player inputs, and those objects exist always throughout scenes?
    public static bool PlayersSwitched = true;

    private float curTimeLeft;
    private bool wasStunned = false;

    GameState State = GameState.WAITING_FOR_PLAYERS;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        players = new PlayerController[2]; //Hardcoded 2 player game, sue me

        gameObject.AddComponent<SoundManager>();
    }

    // Use this for initialization
    void Start () {
        SetupPlayers(); //Spawns the player controllers

        //Toggle switching players every time the game ends
        //if (PlayersSwitched)
        //    SwitchPlayers();

        //Setup the minigame players
        SpawnStartGhosts();

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

        players[0] = pc1;
        players[1] = pc2;

        //Spawn and hook up the 'ghost'
        ghost = Instantiate(GhostPrefab).GetComponent<Pawn>();
        //pc1.Possess(ghost);

        //Spawn and hook up the platformer
        platformer = Instantiate(PlatformerPrefab).GetComponent<Pawn>();
        //pc2.Possess(platformer);
    }

    /// <summary>
    /// In the beginning of the game, there's a minigame to decide who controls the platformer
    /// Spawn the temporary ghosts which will decide who will win
    /// </summary>
    void SpawnStartGhosts()
    {
        Vector3 centerPos = platformer.transform.position;
        Quaternion rot = Quaternion.identity;

        Pawn p1 = Instantiate(MinigameGhostPrefab, centerPos + new Vector3(0, 1, 0), rot).GetComponent<Pawn>();
        Pawn p2 = Instantiate(MinigameGhostPrefab, centerPos + new Vector3(0, -1, 0), rot).GetComponent<Pawn>();

        players[0].Possess(p1);
        players[1].Possess(p2);
    }



    public void SwitchPlayers()
    {
        Pawn pghost = ghost;
        Pawn pplat = platformer;

        PlayerController pcplat = pplat.Controller;
        pghost.Controller.Possess(pplat);

        pcplat.Possess(pghost);
    }

    public bool IsHumanStunned()
    {
        return platformer.GetComponent<PlayerPlatformerController>().IsStunned();
    }

    public void WinMinigame(PlayerController pc)
    {
        if (State != GameState.MINIGAME) return;

        //Remove the extra minigame ghosties
        foreach (PlayerController ply in players)
        {
            Destroy(ply.ControlledPawn);
        }

        //Possess the platformer/actual ghost
        PlayerController ghostPC = pc == players[0] ? players[1] : players[0];
 
        ghostPC.Possess(ghost);
        pc.Possess(platformer);

        State = GameState.PLAYING;
    }

    public void RestartLevel()
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
        State = GameState.MINIGAME;
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

        //Check game logic
        bool stunned = IsHumanStunned();
        if (stunned != wasStunned)
        {
            wasStunned = stunned;
            if (wasStunned && OnHumanStunned != null)
                OnHumanStunned();
        }

        //Debug command
        if (Input.GetButtonDown("Switch"))
        {
            SwitchPlayers();
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

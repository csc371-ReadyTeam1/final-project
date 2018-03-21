using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

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
    public Text player1Score;
    public Text player2Score;
    public float timeLimit = 60.0f; //Deprecated?
    public float countdownTime = 3.0f;

    public GameObject GhostPrefab;
    public GameObject PlatformerPrefab;
    public GameObject MinigameGhostPrefab;

    public GameObject VirtualMainCam;
    public GameObject VirtualMinigameCam;

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

    /// <summary>
    /// Reflects the current state of the game. 
    /// </summary>
    public GameState State { get; private set; }

    /* Contributors: Scott Kauker */
    private void Awake()
    {
        TransitionManager.Get().FadeIn(1.0f);

        State = GameState.WAITING_FOR_PLAYERS;
        if (instance == null)
        {
            instance = this;
        }

        players = new PlayerController[2]; //Hardcoded 2 player game, sue me

        gameObject.AddComponent<SoundManager>();
    }

    // Use this for initialization
    /* Contributors: Scott Kauker */
    void Start () {
        SetupPlayers(); //Spawns the player controllers

        //Toggle switching players every time the game ends
        //if (PlayersSwitched)
        //    SwitchPlayers();

        //Setup the minigame players
        SpawnStartGhosts();

        curTimeLeft = countdownTime;

        //StartGame(); //Started by countdown timer
    }

    //#TODO: Add some sort of 'Press any key to join' phase.
    //This can be done by detecting which input index a key is pressed from, 
    //and creating/assigning a player controller to that index until all slots (2) are filled
    //For now, we just assume player 1 is index 1, and player 2 is index 2
    /* Contributors: Scott Kauker */
    void SetupPlayers()
    {
        PlayerController pc1 = AddPlayer();
        pc1.Name = "Player 1"; //#TODO: Allow them to set their names?
        pc1.PlayerColor = new Color(0.95f, 0.26f, 0.26f);
        PlayerController pc2 = AddPlayer();
        pc2.Name = "Player 2";
        pc2.PlayerColor = new Color(0.26f, 0.45f, 0.95f);

        players[0] = pc1;
        players[1] = pc2;

        //Spawn and hook up the 'ghost'
        ghost = Instantiate(GhostPrefab).GetComponent<Pawn>();
        ghost.gameObject.SetActive(false); //Hide initially until the game starts
        //pc1.Possess(ghost);

        //Spawn and hook up the platformer
        platformer = Instantiate(PlatformerPrefab).GetComponent<Pawn>();
        //pc2.Possess(platformer);
    }

    /// <summary>
    /// In the beginning of the game, there's a minigame to decide who controls the platformer
    /// Spawn the temporary ghosts which will decide who will win
    /// </summary>
    /* Contributors: Scott Kauker */
    void SpawnStartGhosts()
    {
        Vector3 centerPos = platformer.transform.position;
        Quaternion rot = Quaternion.identity;

        Pawn p1 = Instantiate(MinigameGhostPrefab, centerPos + new Vector3(-1, 0, 0), rot).GetComponent<Pawn>();
        Pawn p2 = Instantiate(MinigameGhostPrefab, centerPos + new Vector3(1, 0, 0), rot).GetComponent<Pawn>();

        //Parent to human so if the human moves, they move too
        p1.gameObject.transform.SetParent(platformer.transform.transform);
        p2.gameObject.transform.SetParent(platformer.transform.transform);

        players[0].Possess(p1);
        players[1].Possess(p2);
    }


    /// <summary>
    /// Switches the roles of both players. 
    /// This immediately switches the bodies they are in, no matter what they are
    /// This affects their input controls and their role in the game
    /// </summary>
    /* Contributors: Scott Kauker */
    public void SwitchPlayers()
    {
        Pawn pghost = ghost;
        Pawn pplat = platformer;

        PlayerController pcplat = pplat.Controller;
        pghost.Controller.Possess(pplat);

        pcplat.Possess(pghost);
    }

    /// <summary>
    /// Returns true if the human character is currently stunned and cannot move.
    /// </summary>
    /// <returns></returns>
    /* Contributors: Scott Kauker */
    public bool IsHumanStunned()
    {
        return platformer.GetComponent<PlayerPlatformerController>().IsStunned();
    }

    /// <summary>
    /// Mark a specific player as the winner of the minigame
    /// This is used to determine which player should initially be in the human body
    /// </summary>
    /// <param name="pc"></param>
    /* Contributors: Scott Kauker */
    public void WinMinigame(PlayerController pc)
    {
        if (State != GameState.MINIGAME) return;

        //Remove the extra minigame ghosties
        foreach (PlayerController ply in players)
        {
            Destroy(ply.ControlledPawn.gameObject);
        }

        //Enable the ghost now
        ghost.gameObject.SetActive(true);

        //Possess the platformer/actual ghost
        PlayerController ghostPC = pc == players[0] ? players[1] : players[0];
 
        ghostPC.Possess(ghost);
        pc.Possess(platformer);

        //Playing state + camera
        VirtualMinigameCam.SetActive(false);
        State = GameState.PLAYING;
    }

    /// <summary>
    /// Immediately restart the current level. 
    /// Resets all objects and controls
    /// </summary>
    /* Contributors: Scott Kauker */
    public void RestartLevel()
    {
        PlayersSwitched = !PlayersSwitched;
        TransitionManager.Get().TransitionTo(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Add a new physical player into the scene in the form of a PlayerController object
    /// These objects provide the interface between the player and the object they'd like to control
    /// </summary>
    /// <returns>The newly created PlayerController object</returns>
    /* Contributors: Scott Kauker */
    PlayerController AddPlayer()
    {
        PlayerController pc = gameObject.AddComponent<PlayerController>();
        pc.SetInputIndex(Players.Count + 1);
        Players.Add(pc);

        return pc;
    }

    /// <summary>
    /// Given a player controller, get the 'other' player controller.
    /// Assumes a 2-player game, but a useful utility function for things that assume opposites.
    /// </summary>
    /// <param name="pc">The first player controller to get the enemy of</param>
    /// <returns>The "other" player controller.</returns>
    /* Contributors: Scott Kauker */
    public PlayerController GetOther(PlayerController pc)
    {
        return pc == players[0] ? players[1] : players[0];
    }

    /* Contributors: Scott Kauker */
    public void StartGame()
    {
        //curTimeLeft = timeLimit;
        State = GameState.MINIGAME;
    }

    /* Contributors: Scott Kauker */
    private IEnumerator InvokeRealtime(UnityAction action, float realSeconds)
    {
        if (action == null) yield return null;

        yield return new WaitForSecondsRealtime(realSeconds);
        action();
    }

    /* Contributors: Scott Kauker */
    public void Finish(PlayerController winner)
    {
        State = GameState.GAMEOVER;

        winText.text = winner.Name + " WINS";
        winText.color = winner.PlayerColor;
        winText.gameObject.SetActive(true);

        //Freeze the game, stop music
        Time.timeScale = 0;

        //Restart the level in 5 seconds with the players switched
        StartCoroutine(InvokeRealtime(RestartLevel, 5.0f));
    }

    // Update is called once per frame
    /* Contributors: Scott Kauker */
    void Update () {

        //Countdown to game start
        if (State == GameState.WAITING_FOR_PLAYERS)
        {
            float seconds = Mathf.Ceil(curTimeLeft);
            timerText.text = curTimeLeft > 0 ? seconds.ToString() : "GO!!";

            if (curTimeLeft <= -1)
            {
                timerText.gameObject.SetActive(false);
                StartGame();
            }

            //Every frame, decrement however much time has passed from the 'time left'
            curTimeLeft -= Time.deltaTime;
        }

        //Only perform game logic if the game is active and playing
        if (State != GameState.PLAYING) { return; }

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
        //Disabled for now, slight change in direction of game so the timer in this form isn't needed
        /*
        float minutes = Mathf.Floor(curTimeLeft / 60.0f);
        float seconds = Mathf.Floor(curTimeLeft % 60);
        float ms = Mathf.Floor((curTimeLeft % 1.0f) * 10.0f);
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + "." + ms.ToString("00");

        if (curTimeLeft <= 0)
        {
            Finish(true);
        }*/
    }
}

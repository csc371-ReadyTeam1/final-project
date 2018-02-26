using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPowerlessController : Pawn {

    enum ButtonState
    {
        CENTER,
        LEFT,
        RIGHT
    }

    public float stepSize = 0.05f;
    float completedPerc = 0;
    Vector2 goalPos;
    Vector2 startPos;

    private SpriteRenderer sprite;
    private NametagCreator nametag;

    private ButtonState prevState = ButtonState.CENTER;

	// Use this for initialization
	void Start () {
        goalPos = Vector2.zero;
        startPos = transform.localPosition;

        
    }

    void AddProgress()
    {
        completedPerc += stepSize;
        transform.localPosition = Vector2.Lerp(startPos, goalPos, completedPerc);

        if (completedPerc >= 1.0f)
        {
            GameController.instance.WinMinigame(Controller);
        }
    }

    /// <summary>
    /// When a player possesses this pawn, we want to set the color of the game object to match their color
    /// </summary>
    public override void OnPossessed()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = Controller.PlayerColor;

        nametag = GetComponent<NametagCreator>();
        nametag.SetText(Controller.Name);
        nametag.SetColor(Controller.PlayerColor);
    }

    private ButtonState GetButtonState()
    {
        float axis = Controller.GetAxisRaw("Horizontal");
        if (axis < 0) return ButtonState.LEFT;
        if (axis > 0) return ButtonState.RIGHT;
        return ButtonState.CENTER;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Don't allow input if we aren't in minigame mode
        if (GameController.instance.State != GameState.MINIGAME)
            return;

        //Add progress by pressing left right repeatedly
        //To make progress, every left must be matched by a right
        ButtonState newState = GetButtonState();
        if (newState != prevState && newState != ButtonState.CENTER)
        {
                prevState = newState;
                AddProgress();
        }

    }
}

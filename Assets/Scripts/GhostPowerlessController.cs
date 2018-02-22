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

    private ButtonState prevState = ButtonState.CENTER;

	// Use this for initialization
	void Start () {
        goalPos = GameController.instance.platformer.transform.position;
        startPos = transform.position;
    }

    void AddProgress()
    {
        completedPerc += stepSize;
        transform.position = Vector2.Lerp(startPos, goalPos, completedPerc);

        if (completedPerc >= 1.0f)
        {
            GameController.instance.WinMinigame(Controller);
        }
    }

    private ButtonState GetButtonState()
    {
        float axis = Controller.GetAxisRaw("Horizontal");
        if (axis < 0) return ButtonState.LEFT;
        if (axis > 0) return ButtonState.RIGHT;
        return ButtonState.CENTER;
    }
	
	// Update is called once per frame
	void Update () {

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

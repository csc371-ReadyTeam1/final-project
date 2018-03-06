using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Each physical player has a 'player controller'
//Player controllers can be attached to a 'pawn' (or game object) to provide input into it
//This allows to easily switching player inputs between different objects
//(Such as switching between the ghost and platformer)
public class PlayerController : MonoBehaviour {

    /// <summary>
    /// Retrieves the pawn this player controller is currently possessing. 
    /// This returns the Pawn MonoBehaviour, but the gameobject can be accessed via pawn.gameObject
    /// </summary>
    public Pawn ControlledPawn { get; private set; }

    /// <summary>
    /// Retrieves the name of this player. Usually will just be "Player 1" or "Player 2", 
    /// but allows support for players entering in custom names if we wanted that.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Unique identifying color for this player. 
    /// Will be shown on pawns they possessed.
    /// </summary>
    public Color PlayerColor { get; set; }

    //Which 'input' this player is getting input from
    public int InputIdx { get; private set; }
    private string InputIdxStr = "P0_";

    //Tell this player controller to 'control' a specific pawn
    //This is like saying "Player 1, start controlling the ghost pawn"
    public void Possess(Pawn p)
    {
        //If they were already controlling something, unpossess it
        if (ControlledPawn != null)
        {
            ControlledPawn.SetController(null);
            ControlledPawn = null;
        }

        //If the given input pawn is already possessed, unpossess that one too
        if (p != null && p.Controller != null)
        {
            p.Controller.Possess(null);
        }

        //Now actually possess the new pawn
        ControlledPawn = p;
        if (ControlledPawn != null)
        {
            ControlledPawn.SetController(this);
        }
    }

    //Set which input 'index' this player controller should receive input from
    //In our project, we have a bunch of different ways to receive input, and each 
    //way is prefixed with a PN_. For example, "P0_" are inputs from WASD/keybaord, and
    //P3_ could be inputs from a gamepad. This index specifies which one of those inputs to use.
    public void SetInputIndex(int idx)
    {
        InputIdx = idx;
        InputIdxStr = "P" + idx + "_";
    }

    //Given an action name, get the full unity input name we'll get input from
    private string getInputName(string action)
    {
        return InputIdxStr + action;
    }

    //Return the value of the virtual axis identified by axisname
    public float GetAxis(string axis)
    {
        return Input.GetAxis(getInputName(axis));
    }

    //Return the value of the virtual axis identified by axisname with no smoothing filter applied
    public float GetAxisRaw(string axis)
    {
        return Input.GetAxisRaw(getInputName(axis));
    }

    //Return true during the frame the user pressed down the virtual button identified by buttonName
    public bool GetButtonDown(string buttonName)
    {
        return Input.GetButtonDown(getInputName(buttonName));
    }

    //Return true while the virtual button identifed by buttonName is held down
    public bool GetButton(string buttonName)
    {
        return Input.GetButton(getInputName(buttonName));
    }

}

//Base class for a controllable 'pawn'
//Pawns receive input from the player controller and act on it
public class Pawn : MonoBehaviour
{
    public PlayerController Controller { get; private set; }

    public void SetController(PlayerController pc)
    {
        if (Controller)
        {
            OnUnpossessed();
            Controller = null;
        }

        Controller = pc;
        if (pc != null)
        {
            OnPossessed();
        }
    }

    public virtual void OnPossessed()
    {

    }

    public virtual void OnUnpossessed()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneSelectionByTime : MonoBehaviour {

	/* Initial Contribution by Scott Kaulker from MenuController Script, Changes to original by Alexander Bohlen*/
	/// <summary>
	/// How long to wait before we start accepting input
	/// </summary>
	public float InputWaitDelay = 5.0f;

	/// <summary>
	/// How long to fade in for
	/// </summary>
	public float FadeInTime = 1.0f;

	/// <summary>
	/// Which scene to load
	/// </summary>
	public string SceneName;

	private float startTime = 0.0f;

	// Use this for initialization
	void Start ()
	{
		startTime = Time.time;
		TransitionManager.Get().FadeIn(FadeInTime);
	}

	// Update is called once per frame
	void Update ()
	{
		// Changes by Alexander Bohlen
		//#TODO: Abstract the controller creation to not require gamemode logic
		if ( Time.time - startTime > InputWaitDelay)
		{
			TransitionManager.Get().TransitionTo(SceneName);

			//Play a sound? shrug
		}
	}
}

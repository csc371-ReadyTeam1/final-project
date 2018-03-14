using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    /// <summary>
    /// How long it takes to fade out the screen
    /// </summary>
    public float StartDelay = 1.0f;

    /// <summary>
    /// How long to wait before we start accepting input
    /// </summary>
    public float InputWaitDelay = 5.0f;

    /// <summary>
    /// Which scene to load
    /// </summary>
    public string SceneName;

    /// <summary>
    /// The texture to fade out to
    /// </summary>
    public Texture2D FadeTexture;

    private bool isFading = false;
    private float fadeStart = 0.0f;
    private float startTime = 0.0f;
	// Use this for initialization
	void Start ()
    {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isFading)
        {
            if (getFadePercent() > 1)
            {
                SceneManager.LoadScene(SceneName);
            }

            return;
        }

		//#TODO: Abstract the controller creation to not require gamemode logic
        if (Input.anyKey && Time.time - startTime > InputWaitDelay)
        {
            isFading = true;
            fadeStart = Time.time;

            //Play a sound? shrug
        }
	}

    private float getFadePercent()
    {
        if (!isFading) return 0;

        return (Time.time - fadeStart) / StartDelay;
    }

    private void OnGUI()
    {
        if (!isFading) return;

        GUI.color = new Color(1, 1, 1, getFadePercent());
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeTexture);
    }
}

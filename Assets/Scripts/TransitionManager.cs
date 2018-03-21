using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour {

    /// <summary>
    /// The texture to fade out to
    /// </summary>
    public Texture2D FadeTexture;

    // The current active instance of the manager
    private static TransitionManager instance;

    private string loadingScene;

    enum FadeMode
    {
        None,
        In,
        Out
    }

    private FadeMode fadeMode = FadeMode.None;
    private float fadeStart = 0.0f;
    private float startDelay = 1.0f;

    // Use this for initialization
    /* Contributors: Scott Kauker */
    void Start ()
    {
        instance = this;

        //Allow override, but default to black sprite
        if (FadeTexture == null)
            FadeTexture = Resources.Load("black") as Texture2D;

    }

    /// <summary>
    /// Get the current active instance of the transition manager
    /// </summary>
    /// <returns></returns>
    /* Contributors: Scott Kauker */
    public static TransitionManager Get()
    {
        if (instance == null)
        {
            instance = new GameObject().AddComponent<TransitionManager>();
        }

        return instance;
    }

    /// <summary>
    /// Return the percent faded the screen is
    /// </summary>
    /// <returns></returns>
    /* Contributors: Scott Kauker */
    public float GetFadePercent()
    {
        if (fadeMode == FadeMode.None) return 0;
        float p = (Time.realtimeSinceStartup - fadeStart) / startDelay;

        return fadeMode == FadeMode.Out ? p : 1 - p;
    }

    /// <summary>
    /// Return if we are currently in the middle of a fade transition
    /// </summary>
    /// <returns></returns>
    /* Contributors: Scott Kauker */
    public bool IsTransitioning()
    {
        return fadeMode == FadeMode.Out;
    }

    /// <summary>
    /// Perform a screen fade in from black
    /// </summary>
    /// <param name="time">How long to fade in</param>
    /* Contributors: Scott Kauker */
    public void FadeIn(float time)
    {
        fadeMode = FadeMode.In;
        fadeStart = Time.realtimeSinceStartup;
        startDelay = time;

        Time.timeScale = 0;
    }

    /// <summary>
    /// Transition to the given scene, fading out the screen
    /// </summary>
    /// <param name="SceneName">The name of the scene to load</param>
    /* Contributors: Scott Kauker */
    public void TransitionTo(string SceneName, float fadeTime = 1.0f)
    {
        if (fadeMode == FadeMode.Out) { return; }

        loadingScene = SceneName;
        fadeStart = Time.realtimeSinceStartup;
        fadeMode = FadeMode.Out;
        startDelay = fadeTime;
    }

    // Update is called once per frame
    /* Contributors: Scott Kauker */
    void Update ()
    {
        if (fadeMode == FadeMode.Out && GetFadePercent() > 1)
        {
            SceneManager.LoadScene(loadingScene);
        }
        else if (fadeMode == FadeMode.In && GetFadePercent() <= 0)
        {
            fadeMode = FadeMode.None;
            Time.timeScale = 1.0f;
        }
    }

    /* Contributors: Scott Kauker */
    private void OnGUI()
    {
        if (fadeMode == FadeMode.None) return;

        GUI.color = new Color(1, 1, 1, GetFadePercent());
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeTexture);
    }
}

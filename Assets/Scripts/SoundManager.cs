using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioSource sfxSource;
	public AudioSource musicSource;
	public static SoundManager instance = null;

	public float lowPitchRange = 0.95f;
	public float highPitchRange = 1.05f;

	// Initalization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}

	// For single sound effect audio clips
	public void PlaySingle (AudioClip clip) {
		sfxSource.clip = clip;
		sfxSource.Play ();
	}

	// Randomization of sounds and relative pitch for repeated sfx
	public void RandomSfx (params AudioClip [] clips) {
		int randomIndex = Random.Range (0, clips.Length);
		float randomPitch = Random.Range (lowPitchRange, highPitchRange);

		sfxSource.pitch = randomPitch;
		sfxSource.clip = clips [randomIndex];
		sfxSource.Play ();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioSource efxSource;
	public AudioSource musicSource;
	public static SoundManager instance = null; // this is a singleton

	public float lowPitchRange = .95f; // -5% variation
	public float highPitchRange = 1.05f; // +5% variation

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public void PlaySingle(AudioClip clip)
    {
		efxSource.clip = clip;
		efxSource.Play();
    }

	public void RandomizeSfx(params AudioClip[] clips) // params lets you pass a comma-seperated list of args of same type.
    {
		int randomIndex = Random.Range(0, clips.Length);
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);

		efxSource.pitch = randomPitch;
		efxSource.clip = clips[randomIndex];
		efxSource.Play();
    }
}

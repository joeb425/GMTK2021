using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioHandler : MonoBehaviour
{
	private AudioSource _sfxAudioSource;

	[SerializeField]
	public float maxFrequency = 0.1f;

	Dictionary<AudioClip, float> lastPlayed = new Dictionary<AudioClip, float>();

	// TODO Handle music in here?

	private void Start()
	{
		_sfxAudioSource = GetComponent<AudioSource>();
	}

	public void PlaySfx(AudioClip sfx)
	{
		if (sfx == null)
		{
			return;
		}

		if (lastPlayed.TryGetValue(sfx, out float lastTime))
		{
			if (Time.time - lastTime < maxFrequency)
			{
				return;
			}
		}
		else
		{
			lastPlayed.Add(sfx, Time.time);
		}

		lastPlayed[sfx] = Time.time;

		_sfxAudioSource.PlayOneShot(sfx);
	}
}
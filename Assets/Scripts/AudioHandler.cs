using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioHandler : MonoBehaviour
{
	private AudioSource _sfxAudioSource;
	
	// TODO Handle music in here?

	private void Start()
	{
		_sfxAudioSource = GetComponent<AudioSource>();
	}

	public void PlaySfx(AudioClip sfx)
	{
		_sfxAudioSource.PlayOneShot(sfx);
	}
}
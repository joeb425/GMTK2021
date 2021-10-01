using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Managers/AudioHandler")]
public class AudioHandler : ScriptableObject
{
	private AudioSource _sfxAudioSource;

	private AudioSource _music1;
	private AudioSource _music2;

	private GameObject audioGameObject;

	private bool bInitialized = false;

	[SerializeField]
	public float maxFrequency = 0.1f;

	Dictionary<AudioClip, float> lastPlayed = new Dictionary<AudioClip, float>();

	private void OnEnable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		if (!Application.isPlaying)
		{
			return;
		}

		if (audioGameObject != null)
		{
			return;
		}

		Debug.Log($"Audio handler init {Application.isPlaying}");

		audioGameObject = new GameObject();
		_sfxAudioSource = audioGameObject.AddComponent<AudioSource>();
		_sfxAudioSource.volume = 0.1f;
		_music1 = audioGameObject.AddComponent<AudioSource>();
		_music1.volume = 0.2f;
		_music2 = audioGameObject.AddComponent<AudioSource>();
		_music2.volume = 0.2f;
		DontDestroyOnLoad(audioGameObject);
	}

	public void PlayMusic(AudioClip music)
	{
		// Debug.Log($"Play music {music}");
		if (_music1.isPlaying)
		{
			_music2.clip = music;
			_music2.Play();
			audioGameObject.AddComponent<AudioFade>().StartCoroutine(CrossFade(_music1, _music2, 0.0f, 0.25f));
		}
		else if (_music2.isPlaying || !_music1.isPlaying && !_music2.isPlaying)
		{
			_music1.clip = music;
			_music1.Play();
			audioGameObject.AddComponent<AudioFade>().StartCoroutine(CrossFade(_music2, _music1, 0.0f, 0.25f));
			// audioOff.Stop();
		}
	}

	IEnumerator CrossFade(AudioSource audioOff, AudioSource audioOn, float low, float high)
	{
		float startTime = Time.time;
		float timeSinceStart = 0.0f;
		float startVolumeOff = audioOff.volume;
		float startVolumeOn = audioOn.volume;

		while (timeSinceStart < 1.0f)
		{
			timeSinceStart = Time.time - startTime;
			audioOff.volume = Mathf.Lerp(startVolumeOff, low, timeSinceStart);
			audioOn.volume = Mathf.Lerp(startVolumeOn, high, timeSinceStart);
			yield return null;
		}

		yield return StopMusic(audioOff);
	}

	IEnumerator StopMusic(AudioSource source)
	{
		source.Stop();
		yield return null;
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
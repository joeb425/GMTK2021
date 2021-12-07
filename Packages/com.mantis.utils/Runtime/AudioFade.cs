using System;
using System.Collections;
using UnityEngine;

namespace Mantis.Utils
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioFade : MonoBehaviour
	{
		private void Start()
		{
			// AudioSource audio = GetComponent<AudioSource>();
			// StartCoroutine(FadeIn(audio, 3f, Mathf.SmoothStep));
		}

		public static IEnumerator FadeOut(AudioSource source, float fadingTime, Func<float, float, float, float> Interpolate)
		{
			float startVolume = source.volume;
			float frameCount = fadingTime / Time.deltaTime;
			float framesPassed = 0;

			while (framesPassed <= frameCount)
			{
				var t = framesPassed++ / frameCount;
				source.volume = Interpolate(startVolume, 0, t);
				yield return null;
			}

			source.volume = 0;
			source.Pause();
		}

		public static IEnumerator FadeIn(AudioSource source, float fadingTime, Func<float, float, float, float> Interpolate)
		{
			source.Play();
			source.volume = 0;

			float resultVolume = 0.25f;
			float frameCount = fadingTime / Time.deltaTime;
			float framesPassed = 0;

			while (framesPassed <= frameCount)
			{
				var t = framesPassed++ / frameCount;
				source.volume = Interpolate(0, resultVolume, t);
				yield return null;
			}

			source.volume = resultVolume;
		}
	}
}
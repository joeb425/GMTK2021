using System;
using UnityEngine;
using UnityEngine.Events;

namespace Mantis.Utils
{
	public class PlayEvent : MonoBehaviour
	{
		[SerializeField]
		private UnityEvent Event;

		private void Start()
		{
			Debug.Log("Play event");
			Event?.Invoke();
		}
	}
}
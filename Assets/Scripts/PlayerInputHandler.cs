using UnityEngine;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerInputHandler : MonoBehaviour
	{
		public void OnMouseClick()
		{
			Debug.Log("On mouse click");
		}

		public void OnFastForward()
		{
			Debug.Log("On fast forward");
		}
	}
}
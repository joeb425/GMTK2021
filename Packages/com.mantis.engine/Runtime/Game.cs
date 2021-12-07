using UnityEngine;
using UnityEngine.UIElements;

namespace Mantis.Engine
{
	[RequireComponent(typeof(GameState))]
	[RequireComponent(typeof(UIHandler))]
	public class Game : MonoBehaviour
	{
		public static Game Get;

		public static System.Action OnGameInit;

		// [SerializeField]
		// public GameState MyGameState;

		[HideInInspector]
		public GameState gameState;

		[HideInInspector]
		public UIHandler uiHandler;

		[SerializeField]
		public AudioHandler audioHandler;

		public System.Action OnGameDestroyed;

		void Awake()
		{
			Get = this;

			// gameState = new GameState();
			gameState = GetComponent<GameState>();
			gameState.Init();

			uiHandler = GetComponent<UIHandler>();
			uiHandler.Init();

			// input = new InputHandler();
			// input.Init();

			OnGameInit?.Invoke();
		}

		void Update()
		{
			// input.GameUpdate();

			if (Input.GetKeyDown(KeyCode.Z))
			{
			}
		}

		private void OnDestroy()
		{
			Debug.Log("Game On Destroy");
			// input.Disable();
			Get = null;

			OnGameDestroyed?.Invoke();
		}
	}
}
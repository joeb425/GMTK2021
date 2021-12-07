using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mantis.Engine
{
	public abstract class GameState : MonoBehaviour
	{
		public static GameState Get;

		public abstract void Init();

		public void RestartGame()
		{
			Debug.Log("Restart game");
			SceneManager.LoadScene("Assets/Scenes/Game.unity", LoadSceneMode.Single);
		}

		public void GoToMainMenu()
		{
			SceneManager.LoadScene("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
		}
	}
}
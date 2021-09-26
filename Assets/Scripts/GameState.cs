using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngineInternal;

public class GameState
{
	public static GameState Get;

	public GameBoard Board;

	public event System.Action<int, int> OnCashChanged;
	public event System.Action<int, int> OnLivesChanged;
	public event System.Action OnGameOver;
	public System.Action OnLevelFinished;

	public int StartingCash;

	public int MaxLives;

	public int CurrentCash { get; private set; }

	public int CurrentLives { get; private set; }

	public bool IsGameOver = false;

	public bool SpendCash(int Cost)
	{
		if (CurrentCash > Cost)
		{
			SetCash(CurrentCash - Cost);
			return true;
		}

		return false;
	}

	public void SetCash(int newCash)
	{
		int previousCash = CurrentCash;
		CurrentCash = newCash;
		OnCashChanged?.Invoke(previousCash, newCash);
	}

	public void LoseLife()
	{
		if (CurrentLives > 0)
		{
			SetLives(CurrentLives - 1);
		}
	}

	public void SetLives(int newLives)
	{
		int previousLives = CurrentLives;
		CurrentLives = newLives;
		OnLivesChanged?.Invoke(previousLives, newLives);

		if (newLives <= 0)
		{
			OnGameOver?.Invoke();
			IsGameOver = true;
		}
	}

	public void Init()
	{
		Get = this;
		Game.Get.OnGameDestroyed += () => Get = null;

		CurrentCash = StartingCash;
		CurrentLives = MaxLives;
	}

	public void RestartGame()
	{
		Debug.Log("Restart game");
		SceneManager.LoadScene("Assets/Scenes/Game.unity", LoadSceneMode.Single);
	}

	public void GoToMainMenu()
	{
		SceneManager.LoadScene("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
	}

	public void FinishLevel()
	{
		if (IsGameOver) 
			return;

		Debug.Log("Level finished!");
		OnLevelFinished?.Invoke();
	}
}
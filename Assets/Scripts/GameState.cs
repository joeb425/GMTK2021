using System;
using System.Collections.Generic;
using HexLibrary;
using Mantis.Engine;
using Mantis.Hex;
using ObjectPools;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class GameState : BaseGameState
{
	public static GameState Get()
	{
		return Game.Get != null ? Game.Get.GetGameState<GameState>() : null;
	}

	public GameBoard Board;

	private static readonly float[] GameSpeeds =
	{
		0.5f,
		1.0f,
		2.0f,
		5.0f
	};

	public int gameSpeedIndex = 1;

	public System.Action<float> OnGameSpeedChanged;

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
		if (CurrentCash >= Cost)
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

	public override void Init()
	{
		Board.Initialize();

		CurrentCash = StartingCash;
		CurrentLives = MaxLives;
	}

	public override void GameUpdate()
	{
		Board.GameUpdate();
	}

	public void FinishLevel()
	{
		if (IsGameOver) 
			return;

		Debug.Log("Level finished!");
		OnLevelFinished?.Invoke();
	}

	public void SellSelectedTower()
	{
		if (!Board.GetTowerAtHex(Board.selectedTile, out Tower tower)) 
			return;

		Board.towerLayer.RemoveTile(tower.hex, out HexTileComponent removedTower);
		SetCash(CurrentCash + tower.towerData.towerSell);
		Object.Destroy(removedTower.gameObject);
	}

	public void SpeedUpGame()
	{
		SetGameSpeed(gameSpeedIndex + 1);
	}

	public void SlowDownGame()
	{
		SetGameSpeed(gameSpeedIndex - 1);
	}

	public void SetGameSpeed(int i)
	{
		gameSpeedIndex = Math.Clamp(i, 0, GameSpeeds.Length - 1);
		Time.timeScale = GameSpeeds[gameSpeedIndex];
		OnGameSpeedChanged(Time.timeScale);
	}
}
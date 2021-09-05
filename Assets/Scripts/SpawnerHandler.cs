using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


public class SpawnerHandler : MonoBehaviour
{
	[SerializeField]
	EnemyFactory enemyFactory = default;

	public Wave[] waves;
	public GameBoard board;

	[System.Serializable]
	public class Wave
	{
		public int enemyCount;
		public float spawnSpeed;
		public int enemyElementNo;
	}

	Wave currentWave;
	int currentWaveNumber = 0;
	int enemiesRemainingToSpawn;
	float nextSpawnTime;
	EnemyCollection enemies = new EnemyCollection();
	int endFlag = 0;

	private int _numAliveEnemies = 0;

	public void Start()
	{
		Debug.Log("starter");
		nextSpawnTime = Time.time;
		NextWave();
	}

	public void GameUpdate()
	{
		if (endFlag == 1)
		{
			return;
		}
		
		if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
		{
			enemiesRemainingToSpawn--;
			nextSpawnTime = Time.time + currentWave.spawnSpeed;
			
			Enemy enemy = enemyFactory.Get(currentWave.enemyElementNo);

			var spawnPoint = board.grid.flat.HexToWorld(board.enemyPath[0]);
			enemy.SpawnOn(spawnPoint);
			
			Game.SharedGame.enemies.Add(enemy);
			enemy.OnReachEnd += OnEnemyReachEnd;
			enemy.OnKilled += OnEnemyKilled;
			_numAliveEnemies += 1;
		}
	}

	public void OnEnemyReachEnd()
	{
		GameState.Get.LoseLife();
		OnEnemyDestroyed();
	}

	public void OnEnemyKilled()
	{
		OnEnemyDestroyed();
	}

	public void OnEnemyDestroyed()
	{
		_numAliveEnemies -= 1;

		if (enemiesRemainingToSpawn == 0 && _numAliveEnemies == 0)// && Game.SharedGame.enemies.enemies.Count == 0)
		{
			NextWave();
		}

		if (_numAliveEnemies <= 0 && endFlag == 1)
		{
			Debug.Log("Level finished!");
			GameState.Get.OnLevelFinished?.Invoke();
		}
	}

	void NextWave()
	{
		Debug.Log("Next Wave! " + currentWaveNumber);

		currentWaveNumber++;

		int waveIndex = currentWaveNumber - 1;
		if (waveIndex >= waves.Length)
		{
			endFlag = 1;
			return;
		}

		currentWave = waves[waveIndex];
		enemiesRemainingToSpawn = currentWave.enemyCount;
	}
}
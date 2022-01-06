using Mantis.AttributeSystem;
using Mantis.Engine.Misc;
using ObjectPools;
using UnityEngine;
using System.Collections.Generic;


public class SpawnerHandler : MonoBehaviour
{
	public GameBoard board;

	[System.Serializable]
	public class Wave
	{
		public int enemyCount;
		public float spawnSpeed;

		[SerializeField]
		public Enemy enemyPrefab;
	}

	Wave currentWave;
	int currentWaveNumber = 0;
	int enemiesRemainingToSpawn;
	float nextSpawnTime;
	int endFlag = 0;

	public System.Action<Wave, int> OnNextWave;

	private int _numAliveEnemies = 0;


	public List<GameplayEffect> mapMods; 

	public void Start()
	{
		Debug.Log("starter");
		nextSpawnTime = Time.time;
		NextWave();
		mapMods = GameData.Get().GetCurrentLevel().MapModifiers;

		// TODO load map mods table from elsewhere
		Debug.Log(mapMods);
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

			Enemy enemy = EnemyPool.Get().GetInstanceInactive(currentWave.enemyPrefab);
			var spawnPoint = board.grid.flat.HexToWorld(board.enemyPath[0]);
			enemy.SpawnOn(spawnPoint);
			enemy.gameObject.SetActive(true);

			enemy.OnReachEnd += OnEnemyReachEnd;
			enemy.OnKilled += OnEnemyKilled;
			_numAliveEnemies += 1;

			foreach (GameplayEffect ge in mapMods)
			{
				enemy.attributes.ApplyEffect(ge);
			}
		}
	}

	public void OnEnemyReachEnd()
	{
		GameState.Get().LoseLife();
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
			GameState.Get().FinishLevel();
		}
	}

	void NextWave()
	{
		Debug.Log("Next Wave! " + currentWaveNumber);
		LevelData currentLevel = GameData.Get().GetCurrentLevel();

		Debug.Assert(currentLevel != null, "currentLevel is null");

		currentWaveNumber++;

		int waveIndex = currentWaveNumber - 1;
		if (waveIndex >= currentLevel.waves.Count)
		{
			endFlag = 1;
			return;
		}

		currentWave = currentLevel.waves[waveIndex];
		enemiesRemainingToSpawn = currentWave.enemyCount;

		OnNextWave?.Invoke(currentWave, waveIndex);
	}
}
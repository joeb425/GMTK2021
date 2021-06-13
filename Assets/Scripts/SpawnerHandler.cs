using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnerHandler : MonoBehaviour
{

	[SerializeField]
	EnemyFactory enemyFactory = default;

	public Wave[] waves;
	private GameBoard board;

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

	public void Start()
	{
		Debug.Log("starter");
		nextSpawnTime = Time.time;
		NextWave();
	}
	public void GameUpdate(GameTile spawnPoint)
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
			enemy.SpawnOn(spawnPoint);
			//enemies.Add(enemy);
			Game.SharedGame.enemies.Add(enemy);
		}
		if (enemiesRemainingToSpawn ==0 && Game.SharedGame.enemies.enemies.Count == 0)
		{
			NextWave();
		}
	}
	void NextWave()
	{
		if (currentWaveNumber > waves.Length)
		{
			endFlag = 1;
			return;
		}
		currentWaveNumber++;
		currentWave = waves[currentWaveNumber - 1];
		enemiesRemainingToSpawn = currentWave.enemyCount;
	}

	//	void SpawnEnemy()
	//	{
	//		GameTile spawnPoint =
	//			board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
	//		Enemy enemy = enemyFactory.Get();
	//		enemy.SpawnOn(spawnPoint);
	//		enemies.Add(enemy);
	//	}



}

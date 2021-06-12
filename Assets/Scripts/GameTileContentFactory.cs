﻿using UnityEngine;

//using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class GameTileContentFactory : GameObjectFactory
{
	[SerializeField]
	GameTileContent destinationPrefab = default;

	[SerializeField]
	GameTileContent emptyPrefab = default;

	[SerializeField]
	GameTileContent wallPrefab = default;

	[SerializeField]
	GameTileContent spawnPointPrefab = default;

	[SerializeField]
	Tower towerPrefab = default;

	[SerializeField]
	GameTileContent buildPrefab = default;

	//Scene contentScene;

	public GameTileContent Get(GameTileContentType type)
	{
		switch (type)
		{
			case GameTileContentType.Destination: return Get(destinationPrefab);
			case GameTileContentType.Path: return Get(emptyPrefab);
			case GameTileContentType.Wall: return Get(wallPrefab);
			case GameTileContentType.SpawnPoint: return Get(spawnPointPrefab);
			case GameTileContentType.Tower: return Get(towerPrefab);
			case GameTileContentType.Build: return Get(buildPrefab);
		}

		Debug.Assert(false, "Unsupported type: " + type);
		return null;
	}

	public void Reclaim(GameTileContent content)
	{
		Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
		Destroy(content.gameObject);
	}

	GameTileContent Get(GameTileContent prefab)
	{
		GameTileContent instance = CreateGameObjectInstance(prefab);
		instance.OriginFactory = this;
		//MoveToFactoryScene(instance.gameObject);
		return instance;
	}


	/*void MoveToFactoryScene (GameObject o) {
		if (!contentScene.isLoaded) {
			if (Application.isEditor) {
				contentScene = SceneManager.GetSceneByName(name);
				if (!contentScene.isLoaded) {
					contentScene = SceneManager.CreateScene(name);
				}
			}
			else {
				contentScene = SceneManager.CreateScene(name);
			}
		}
		SceneManager.MoveGameObjectToScene(o, contentScene);
	}*/
}
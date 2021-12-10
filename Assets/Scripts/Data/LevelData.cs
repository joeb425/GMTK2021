using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
	[SerializeField]
	public TextAsset level;

	[SerializeField]
	public List<SpawnerHandler.Wave> waves;
}
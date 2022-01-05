using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
	[SerializeField]
	public TextAsset level;

	[SerializeField]
	public string displayName;

	[SerializeField]
	public Texture2D thumbnail;

	[SerializeField]
	public List<SpawnerHandler.Wave> waves;

	[SerializeField]
	public List<Tower> availableTowers;
}
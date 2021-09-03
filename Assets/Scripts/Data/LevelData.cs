using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
	[SerializeField]
	public List<TextAsset> levels;

	public bool IsLastLevel(int levelIndex)
	{
		return levelIndex >= levels.Count;
	}
}
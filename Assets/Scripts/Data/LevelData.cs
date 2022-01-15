using System.Collections.Generic;
using UnityEngine;
using Mantis.AttributeSystem;
using Mantis.Hex;

[CreateAssetMenu(menuName = "Data/LevelData")]
public class LevelData : ScriptableObject
{
	[SerializeField]
	public TextAsset level;

	[SerializeField]
	public HexTilePalette tilePalette;

	[SerializeField]
	public LevelColorPalette colorPalette;

	[SerializeField]
	public string displayName;

	[SerializeField]
	public Texture2D thumbnail;

	[SerializeField]
	public List<SpawnerHandler.Wave> waves;

	[SerializeField]
	public List<Tower> disabledTowers;

	[SerializeField]
	public List<GameplayEffect> MapModifiers;
}
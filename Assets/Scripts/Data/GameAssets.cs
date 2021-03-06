using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Data;
using Mantis.AttributeSystem;
using Mantis.Utils;
using Misc;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/GameAssets")]

public class GameAssets : ScriptableObject
{
	[SerializeField]
	public List<Material> spreadMaterials;

	[SerializeField]
	public AudioClip placeTowerSfx;

	[SerializeField]
	public GameplayEffect testEffect;

	[SerializeField]
	public ZoneData testZoneData;

	[SerializeField]
	public TileHighlighter tileHighlighter;

	[SerializeField]
	public MyAttributes myAttributes;
}

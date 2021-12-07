using System.Collections.Generic;
using Mantis.AttributeSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/ZoneData")]
public class ZoneData : ScriptableObject
{
	[SerializeField]
	public Color zoneColor;

	[SerializeField]
	public List<GameplayEffect> zoneEffects;
}
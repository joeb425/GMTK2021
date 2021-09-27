using System;
using System.Collections.Generic;
using System.Linq;
using Attributes;
using Unity.Profiling;
using UnityEngine;

namespace HexLibrary
{
	public enum HexTileType
	{
		Build,
		Path,
		Start,
		End
	}
	
	[Serializable]
	public class GroundTileComponent : HexTileComponent
	{
		[SerializeField]
		public bool IsObstacle;

		[SerializeField]
		public string SpreadType;

		[SerializeField]
		public HexTileType TileType;

		public Zone zone;

		public GameplayEffectList effectList = new GameplayEffectList();

		public void AddTowerEffect(GameplayEffect effect)
		{
			effectList.AddEffect(effect);
		}

		public void RemoveTowerEffect(GameplayEffect effect)
		{
			effectList.RemoveEffect(effect);
		}
	}
}
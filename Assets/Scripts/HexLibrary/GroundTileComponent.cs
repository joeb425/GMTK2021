using System;
using System.Collections.Generic;
using Attributes;
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

		public System.Action<Hex, GameplayEffect> OnTowerEffectAdded;
		public System.Action<Hex, GameplayEffect> OnTowerEffectRemoved;

		private List<GameplayEffect> towerEffects = new List<GameplayEffect>();

		public void ApplyEffectsToTower(Tower tower)
		{
			foreach (GameplayEffect effect in towerEffects)
			{
				tower.Attributes.ApplyEffect(effect);
			}
		}

		public void AddTowerEffect(GameplayEffect effect)
		{
			towerEffects.Add(effect);
			OnTowerEffectAdded?.Invoke(hex, effect);
		}

		public void RemoveTowerEffect(GameplayEffect effect)
		{
			towerEffects.Remove(effect);
			OnTowerEffectRemoved?.Invoke(hex, effect);
		}
	}
}
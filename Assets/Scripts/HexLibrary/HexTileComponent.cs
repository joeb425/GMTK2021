using System;
using Mantis.GameplayTags;
using UnityEngine;

namespace HexLibrary
{
	[Serializable]
	public class HexTileComponent : MonoBehaviour, IGameplayTag
	{
		[HideInInspector]
		public Hex hex;

		[SerializeField]
		public GameplayTag gameplayTag;
		public GameplayTag GetGameplayTag()
		{
			return gameplayTag;
		}

		public System.Action<Hex> OnPlacedOnHex;
		public System.Action<Hex> OnRemovedFromHex;

		public virtual void GameUpdate()
		{
		}

		public virtual void PlaceOnHex(Hex hex)
		{
			this.hex = hex;
			OnPlacedOnHex?.Invoke(hex);
		}

		public virtual void RemoveFromHex(Hex hex)
		{
			this.hex = null;
			OnRemovedFromHex?.Invoke(hex);
		}
	}
}
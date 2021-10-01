using System;
using Misc.GameplayTags;
using Misc.SerializableGuid;
using UnityEngine;

namespace HexLibrary
{
	[Serializable]
	public class HexTileComponent : MonoBehaviour
	{
		[HideInInspector]
		public Hex hex;

		[SerializeField]
		public SerializableGuid Guid;

		[SerializeField]
		public GameplayTag Tag;

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
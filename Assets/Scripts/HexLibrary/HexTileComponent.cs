﻿using System;
using UnityEngine;

namespace HexLibrary
{
	[Serializable]
	public class HexTileComponent : MonoBehaviour
	{
		[HideInInspector]
		public Hex hex;

		public System.Action<Hex> OnPlacedOnHex;
		public System.Action<Hex> OnRemovedFromHex;

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
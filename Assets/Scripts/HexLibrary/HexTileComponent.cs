using System;
using UnityEngine;

namespace HexLibrary
{
	[Serializable]
	public class HexTileComponent : MonoBehaviour
	{
		[HideInInspector]
		public Hex hex;
	}
}
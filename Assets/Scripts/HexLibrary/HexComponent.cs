using System;
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
	public class HexComponent : MonoBehaviour
	{
		[SerializeField]
		public bool IsObstacle;

		[SerializeField]
		public string SpreadType;

		[SerializeField]
		public HexTileType TileType;
	}
}
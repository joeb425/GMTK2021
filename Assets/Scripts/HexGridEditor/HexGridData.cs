using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.HexGridEditor
{
	[System.Serializable]
	public class JsonHexGrid
	{
		[SerializeField]
		public List<JsonHex> hexData = new List<JsonHex>();
	}

	[System.Serializable]
	public class JsonHex
	{
		[SerializeField]
		public Hex hex;

		[SerializeField]
		public int type;
	}
}
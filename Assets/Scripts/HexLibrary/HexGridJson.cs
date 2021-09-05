using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexLibrary
{
	[System.Serializable]
	public class JsonHexGrid
	{
		[SerializeField]
		public List<JsonHexLayer> hexLayer = new List<JsonHexLayer>();

		public JsonHexLayer GetLayer(string name)
		{
			return hexLayer.FirstOrDefault(layer => layer.layerName == name);
		}
	}

	[System.Serializable]
	public class JsonHexLayer
	{
		[SerializeField]
		public string layerName;

		[SerializeField]
		public List<JsonHex> hexData = new List<JsonHex>();

		public JsonHexLayer(string layerName) => this.layerName = layerName;
	}

	[System.Serializable]
	public class JsonHex
	{
		[SerializeField]
		public Hex hex;

		[SerializeField]
		public string guid;

		public JsonHex(Hex hex, string guid) 
		{
			this.hex = hex;
			this.guid = guid;
		}
	}
}
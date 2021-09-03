using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.HexGridEditor
{
	[System.Serializable]
	public class HexGridData
	{
		[SerializeField]
		public List<HexData> hexData = new List<HexData>();
	}

	[System.Serializable]
	public class HexData
	{
		[SerializeField]
		public Hex hex;

		[SerializeField]
		public int type;
	}

	// [System.Serializable]
	// public class HexCoordinate
	// {
	// 	public int q;
	// 	public int r;
	// 	public int s;
	//
	// 	public override int GetHashCode()
	// 	{
	// 		Hash128 hash = new Hash128();
	// 		hash.Append(q);
	// 		hash.Append(r);
	// 		hash.Append(s);
	// 		return hash.GetHashCode();
	// 	}
	//
	// 	public override bool Equals(object obj)
	// 	{
	// 		return Equals(obj as HexCoordinate);
	// 	}
	//
	// 	public bool Equals(HexCoordinate obj)
	// 	{
	// 		return q == obj.q && r == obj.r && s == obj.s;
	// 	}
	// }
}
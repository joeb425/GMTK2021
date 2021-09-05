using System;
using UnityEngine;

namespace DefaultNamespace.HexGrid
{
	[Serializable]
	public class HexGridComponent : MonoBehaviour
	{
		[SerializeField]
		public Hex hex;
	}
}
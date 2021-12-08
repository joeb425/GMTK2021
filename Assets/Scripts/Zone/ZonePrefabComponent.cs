using HexLibrary;
using Mantis.Hex;
using UnityEngine;

namespace DefaultNamespace.Zone
{
	public class ZonePrefabComponent : HexTileComponent
	{ 
		public ZoneData zoneData;

		void OnDrawGizmos()
		{
			if (!Application.isPlaying)
			{
				Gizmos.color = zoneData.zoneColor;
				Gizmos.DrawSphere(transform.position, 0.25f);
			}
		}
	}
}
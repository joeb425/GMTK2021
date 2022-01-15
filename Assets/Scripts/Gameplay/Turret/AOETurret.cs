using System.Collections.Generic;
using DefaultNamespace.Data;
using ObjectPools;
using UnityEditor.Media;
using UnityEngine;

public class AOETurret : BaseTurret
{
	private List<TargetPoint> _targetsInRadius;

	protected override void Attack()
	{
		foreach (TargetPoint targetToAttack in GetTargetsInRadius())
		{
			ApplyHit(targetToAttack);
		}

		Game.Get.GetAudioHandler().PlaySfx(tower.towerData.shootSfx);
	}

	public override bool CanAttack()
	{
		_targetsInRadius = GetTargetsInRadius();
		return _targetsInRadius.Count > 0;
	}
}
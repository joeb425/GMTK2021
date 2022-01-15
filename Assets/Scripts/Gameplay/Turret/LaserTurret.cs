using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using ObjectPools;
using UnityEditor.Media;
using UnityEngine;

public class LaserTurret : SingleTargetTurret
{
	[SerializeField]
	private LineRenderer lineRenderer;

	protected override void UpdateAttacking()
	{
		if (_target != null)
		{
			lineRenderer.enabled = true;
			Vector3 startPos = lineRenderer.transform.position;
			Vector3 endPos = _target.Position;
			endPos.y = startPos.y;
			lineRenderer.SetPosition(0, startPos);
			lineRenderer.SetPosition(1, endPos);
		}
		else
		{
			lineRenderer.enabled = false;
		}

		base.UpdateAttacking();
	}

	// protected override bool RotateToTarget()
	// {
	// 	Vector3 toTarget = _target.Position - transform.position;
	// 	turret.rotation = Quaternion.LookRotation(toTarget);
	// 	return false;
	// }

	protected override void Attack()
	{
		List<TargetPoint> targetsToAttack = new List<TargetPoint>();
		targetsToAttack.Add(_target);

		int numExtraTargets = Mathf.FloorToInt(tower.GetAttributes().GetCurrentValue(MyAttributes.Get().Split));
		if (numExtraTargets > 0)
		{
			Collider[] enemies = GetEnemiesInRadius();
			foreach (Collider enemyCollider in enemies)
			{
				if (numExtraTargets == 0)
				{
					break;
				}

				TargetPoint targetPoint = enemyCollider.GetComponent<TargetPoint>();
				if (targetPoint != _target)
				{
					numExtraTargets -= 1;
					targetsToAttack.Add(targetPoint);
				}
			}
		}

		foreach (TargetPoint targetToAttack in targetsToAttack)
		{
			ApplyHit(targetToAttack);
		}

		Game.Get.GetAudioHandler().PlaySfx(tower.towerData.shootSfx);
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (lineRenderer == null)
		{
			lineRenderer = GetComponentsInChildren<LineRenderer>().FirstOrDefault(comp => comp.name == "LaserRenderer");
		}
	}
}
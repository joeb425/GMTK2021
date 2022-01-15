using System.Collections.Generic;
using DefaultNamespace.Data;
using ObjectPools;
using UnityEditor.Media;
using UnityEngine;

public abstract class SingleTargetTurret : BaseTurret
{
	protected TargetPoint _target = null;

	protected bool IsFacingTarget()
	{
		Vector3 toTarget = _target.Position - transform.position;
		float angle = Quaternion.Angle(turretTransform.rotation, Quaternion.LookRotation(toTarget));
		return angle < 10;
	}

	public override bool CanAttack()
	{
		return (HasValidTarget() || AcquireTarget()) && RotateToTarget();
	}

	protected virtual bool RotateToTarget()
	{
		Vector3 toTarget = _target.Position - transform.position;
		toTarget.y = 0;
		float turnSpeed = tower.GetAttributes().GetCurrentValue(MyAttributes.Get().TurnSpeed);
		turretTransform.rotation = Quaternion.Lerp(turretTransform.rotation, Quaternion.LookRotation(toTarget), Time.deltaTime * turnSpeed);
		return IsFacingTarget();
	}

	protected override void Attack()
	{
		foreach (TargetPoint targetToAttack in GetTargetsInRadius())
		{
			ApplyHit(targetToAttack);
		}

		Game.Get.GetAudioHandler().PlaySfx(tower.towerData.shootSfx);
	}

	bool HasValidTarget()
	{
		if (!IsValidTarget(_target))
		{
			_target = null;
			return false;
		}

		Vector3 a = transform.position;
		Vector3 b = _target.Position;
		a.y = b.y;

		float range = tower.GetAttributes().GetCurrentValue(MyAttributes.Get().Range);
		if (Vector3.SqrMagnitude(b - a) > range * range)
		{
			_target = null;
			return false;
		}

		return true;
	}

	bool AcquireTarget()
	{
		Collider[] targets = GetEnemiesInRadius();

		foreach (Collider targetCollider in targets)
		{
			TargetPoint targetPoint = targetCollider.GetComponent<TargetPoint>();
			if (IsValidTarget(targetPoint))
			{
				_target = targetPoint;
				return true;
			}
		}

		_target = null;
		return false;
	}

	private void OnDrawGizmos()
	{
		if (_target != null)
		{
			Gizmos.DrawLine(transform.position, _target.Position);
		}
	}
}
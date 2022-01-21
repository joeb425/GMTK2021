using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using UnityEngine;

public abstract class SingleTargetTurret : BaseTurret
{
	protected TargetPoint _currentTarget = null;
	protected List<TargetPoint> _splitTargets = new();

	protected bool isFacingTarget = false;

	public Action<TargetPoint> OnStartFacingTarget;
	public Action<TargetPoint> OnStopFacingTarget;
	

	protected virtual void Awake()
	{
		targetingState.OnGainTarget += OnGainTarget;
		targetingState.OnLoseTarget += OnLoseTarget;
	}

	protected bool IsFacingTarget()
	{
		Vector3 toTarget = _currentTarget.Position - transform.position;
		float angle = Quaternion.Angle(turretTransform.rotation, Quaternion.LookRotation(toTarget));
		return angle < 25;
	}

	public override bool CanAttack()
	{
		return IsValidTarget(_currentTarget) && RotateToTarget();
	}

	protected virtual bool RotateToTarget()
	{
		Vector3 toTarget = _currentTarget.Position - transform.position;
		toTarget.y = 0;
		float turnSpeed = tower.GetAttributes().GetCurrentValue(MyAttributes.Get().TurnSpeed);
		// turretTransform.rotation = Quaternion.Lerp(turretTransform.rotation, Quaternion.LookRotation(toTarget), Time.deltaTime * turnSpeed);
		turretTransform.rotation = Quaternion.RotateTowards(turretTransform.rotation, Quaternion.LookRotation(toTarget), Time.deltaTime * 360.0f);

		bool isNowFacingTarget = IsFacingTarget();
		if (isNowFacingTarget != isFacingTarget)
		{
			isFacingTarget = isNowFacingTarget;
			if (isNowFacingTarget)
			{
				OnStartFacingTarget?.Invoke(_currentTarget);
			}
			else
			{
				OnStopFacingTarget?.Invoke(_currentTarget);
			}
		}
		
		return IsFacingTarget();
	}

	private void OnDrawGizmos()
	{
		if (_currentTarget != null)
		{
			Gizmos.DrawLine(transform.position, _currentTarget.Position);
		}
	}

	protected virtual void OnGainTarget(TargetPoint target)
	{
		// Debug.Log($"Gain {target}");
		if (!IsValidTarget(target))
		{
			return;
		}

		if (target == _currentTarget)
		{
			return;
		}

		// set as current target
		if (_currentTarget == null)
		{
			SetTarget(target);
		}
		else // add to split targets
		{
			int numExtraTargets = Mathf.FloorToInt(tower.GetAttributes().GetCurrentValue(MyAttributes.Get().Split));
			if (_splitTargets.Count < numExtraTargets)
			{
				AddSplitTarget(target);
			}
		}
	}

	private void OnLoseTarget(TargetPoint target)
	{
		if (target == null)
		{
			return;
		}
		
		if (_splitTargets.Contains(target))
		{
			RemoveSplitTarget(target);
		}
		else if (target == _currentTarget)
		{
			OnLoseCurrentTarget(target);
			FindNewTarget();
		}
	}

	public virtual void ClearCurrentTarget()
	{
		isFacingTarget = false;
		_currentTarget = null;

		for (var i = _splitTargets.Count - 1; i >= 0; i--)
		{
			RemoveSplitTarget(_splitTargets[i]);
		}

		_splitTargets = new List<TargetPoint>();
	}

	protected virtual void OnLoseCurrentTarget(TargetPoint target)
	{
	}

	private void FindNewTarget()
	{
		ClearCurrentTarget();

		List<TargetPoint> targetsInRadius = GetTargetsInRadius();
		targetsInRadius.Sort((a, b) => b.Enemy.GetProgress().CompareTo(a.Enemy.GetProgress()));

		int numSplit = Mathf.FloorToInt(tower.GetAttributes().GetCurrentValue(MyAttributes.Get().Split));
		for (var i = 0; i < targetsInRadius.Count; i++)
		{
			if (i == 0)
			{
				SetTarget(targetsInRadius[i]);
			}
			else if (i <= numSplit)
			{
				AddSplitTarget(targetsInRadius[i]);
			}
		}
	}

	protected virtual void SetTarget(TargetPoint target)
	{
		_currentTarget = target;
	}

	protected virtual void AddSplitTarget(TargetPoint target)
	{
		_splitTargets.Add(target);
	}

	protected virtual void RemoveSplitTarget(TargetPoint target)
	{
		_splitTargets.Remove(target);
	}
}
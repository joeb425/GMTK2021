using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Misc
{
	public class TargetingState : MonoBehaviour
	{
		private HashSet<TargetPoint> _allTargets = new();
		public HashSet<TargetPoint> GetAllTargets() => _allTargets;

		public Action<TargetPoint> OnGainTarget;
		public Action<TargetPoint> OnLoseTarget;

		private bool _triggerActive = true;
		public void SetTriggerActive(bool active) => _triggerActive = active;

		private void OnDestroy()
		{
			Debug.Log("Destroy?");
			foreach (TargetPoint target in _allTargets)
			{
				target.Enemy.OnKilled -= RemoveEnemy;
				target.Enemy.OnReachEnd -= RemoveEnemy;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (_triggerActive && other.TryGetComponent<TargetPoint>(out var target))
			{
				if (target.IsValid())
				{
					GainTarget(target);
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (_triggerActive && other.TryGetComponent<TargetPoint>(out var target))
			{
				RemoveTarget(target);
			}
		}

		private void GainTarget(TargetPoint target)
		{
			// Debug.Log("trigger enter");
			// _allTargets.Add(target);
			if (_allTargets.Contains(target))
			{
				Debug.LogError("tried to add target twice");
				return;
			}

			_allTargets.Add(target);

			target.Enemy.OnKilled += RemoveEnemy;
			target.Enemy.OnReachEnd += RemoveEnemy;
			OnGainTarget?.Invoke(target);
		}

		private void RemoveEnemy(Enemy enemy)
		{
			RemoveTarget(enemy.targetPoint);
		}

		private void RemoveTarget(TargetPoint target)
		{
			if (!_allTargets.Contains(target))
			{
				return;
			}

			target.Enemy.OnKilled -= RemoveEnemy;
			target.Enemy.OnReachEnd -= RemoveEnemy;

			_allTargets.Remove(target);
			OnLoseTarget?.Invoke(target);
		}

		private void OnDrawGizmos()
		{
			foreach (TargetPoint targetPoint in _allTargets)
			{
				Gizmos.DrawSphere(targetPoint.Position, 0.25f);
			}
		}
	}

	public class FirstEnemyComparison : IComparer<TargetPoint>
	{
		public int Compare(TargetPoint x, TargetPoint y)
		{
			return y.Enemy.GetProgress().CompareTo(x.Enemy.GetProgress());
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserTurret : SingleTargetTurret
{
	[SerializeField]
	private LaserBeamVFX laserBeamPrefab;

	private Dictionary<TargetPoint, LaserBeamVFX> _laserBeamVFXs = new();

	[SerializeField]
	private Transform _bulletSpawnPoint;

	protected override void Awake()
	{
		base.Awake();
		OnStartFacingTarget += (_) => SpawnLasers();
		OnStopFacingTarget += (_) => DestroyLasers();
	}

	private void DestroyLasers()
	{
		foreach (var laserBeamVfX in _laserBeamVFXs)
		{
			DestroyLaser(laserBeamVfX.Key);
		}

		_laserBeamVFXs = new Dictionary<TargetPoint, LaserBeamVFX>();
	}

	public void SpawnLasers()
	{
		CreateLaser(_currentTarget);
		foreach (TargetPoint splitTarget in _splitTargets)
		{
			CreateLaser(splitTarget);
		}
	}

	protected override void OnLoseCurrentTarget(TargetPoint target)
	{
		// Debug.Log($"Lose target {target}");
		base.OnLoseCurrentTarget(target);
		DestroyLaser(target);
	}

	protected override void AddSplitTarget(TargetPoint target)
	{
		base.AddSplitTarget(target);
		CreateLaser(target);
	}

	protected override void RemoveSplitTarget(TargetPoint target)
	{
		base.RemoveSplitTarget(target);
		DestroyLaser(target);
	}

	protected override void Attack()
	{
		ApplyHit(_currentTarget);

		// TODO potential bug if early split target dies from splash?
		for (var i = _splitTargets.Count - 1; i >= 0; i--)
		{
			ApplyHit(_splitTargets[i]);
		}

		Game.Get.GetAudioHandler().PlaySfx(tower.towerData.shootSfx);
	}

	private void CreateLaser(TargetPoint newTarget)
	{
		if (_laserBeamVFXs.ContainsKey(newTarget))
			return;

		LaserBeamVFX laser = Instantiate(laserBeamPrefab, _bulletSpawnPoint);
		laser.SetSourceAndTarget(_bulletSpawnPoint.transform, newTarget.Enemy.enemyModel.transform);
		_laserBeamVFXs.Add(newTarget, laser);
	}

	private void DestroyLaser(TargetPoint target)
	{
		if (!_laserBeamVFXs.ContainsKey(target))
			return;

		Destroy(_laserBeamVFXs[target].gameObject);
		_laserBeamVFXs.Remove(target);
	}


	protected override void OnValidate()
	{
		base.OnValidate();

		if (tower == null)
		{
			tower = GetComponent<Tower>();
		}

		if (_bulletSpawnPoint == null)
		{
			_bulletSpawnPoint = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "BulletSpawnPoint");
		}
	}
}
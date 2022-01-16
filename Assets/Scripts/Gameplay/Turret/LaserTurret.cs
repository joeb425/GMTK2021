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

	[SerializeField]
	private Transform hitVFX;

	[SerializeField]
	private ParticleSystem muzzleParticle;

	[SerializeField]
	private ParticleSystem hitParticle;

	protected override void UpdateAttacking()
	{
		if (_target != null)
		{
			Vector3 startPos = lineRenderer.transform.position;
			Vector3 endPos = _target.Position;
			endPos.y = startPos.y;
			lineRenderer.SetPosition(0, startPos);
			lineRenderer.SetPosition(1, endPos);
			hitParticle.transform.position = endPos;
		}

		base.UpdateAttacking();
	}

	protected override void OnGainTarget()
	{
		base.OnGainTarget();
		muzzleParticle.Play();
		hitParticle.Play();
		lineRenderer.enabled = true;
	}

	protected override void OnLoseTarget()
	{
		base.OnLoseTarget();
		muzzleParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		hitParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		lineRenderer.enabled = false;
	}

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

		if (hitVFX == null)
		{
			hitVFX = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "HitVFX");
		}

		if (hitParticle == null)
		{
			hitParticle = GetComponentsInChildren<ParticleSystem>().FirstOrDefault(t => t.name == "HitParticle");
		}

		if (muzzleParticle == null)
		{
			muzzleParticle = GetComponentsInChildren<ParticleSystem>().FirstOrDefault(t => t.name == "MuzzleParticle");
		}
	}
}
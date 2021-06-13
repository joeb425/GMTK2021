using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;
using Random = UnityEngine.Random;

// [RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SphereCollider))]
public class Tower : GameTileContent
{
	[SerializeField]
	Transform turret = default;
	
	[SerializeField, Range(1.5f, 10.5f)]
	float targetingRange = 1.5f;

	[SerializeField, Range(0.0f, 200f)]
	public float damage;

	[SerializeField, Range(0.0f, 200f)]
	public float baseAttackSpeed = 1.0f;

	public float numTargets = 2.0f;

	public float splash = .5f;

	private float attackTimeRemaining;

	public int numSegments = 64;

	public LineRenderer lineRenderer;

	private SphereCollider sphereCollider;

	private TargetPoint target = null;

	private void Start()
	{
		InitSphereCollider();
		InitLineRenderer();
	}

	public override void GameUpdate()
	{
		if (TrackTarget() || AcquireTarget())
		{
			Vector3 point = target.Position;
			turret.LookAt(point);
			UpdateAttacking();
		}
	}

	private void InitSphereCollider()
	{
		sphereCollider = gameObject.GetComponent<SphereCollider>();
		sphereCollider.radius = targetingRange;
	}

	bool TrackTarget()
	{
		if (target == null)
		{
			return false;
		}

		Vector3 a = transform.localPosition;
		Vector3 b = target.Position;
		if (Vector3.Distance(a, b) > targetingRange + 0.125f)
		{
			target = null;
			return false;
		}

		return true;
	}

	Collider[] GetEnemiesInRadius()
	{
		return Physics.OverlapSphere(transform.localPosition, targetingRange, 1 << 9);
	}

	bool AcquireTarget()
	{
		Collider[] targets = GetEnemiesInRadius();

		if (targets.Length > 0)
		{
			target = targets[0].GetComponent<TargetPoint>();
			// Debug.Log("Set target" + targets[0].gameObject);
			Debug.Assert(target != null, "Targeted non-enemy!", targets[0]);
			return true;
		}

		target = null;
		return false;
	}

	private float GetAttackSpeed()
	{
		return baseAttackSpeed;
	}

	void UpdateAttacking()
	{
		if (target != null)
		{
			attackTimeRemaining -= Time.deltaTime;
			if (attackTimeRemaining <= 0)
			{
				attackTimeRemaining = GetAttackSpeed() - attackTimeRemaining;
				Attack();
			}
		}
	}

	void Attack()
	{
		List<TargetPoint> targetsToAttack = new List<TargetPoint>();
		targetsToAttack.Add(target);

		Collider[] enemies = GetEnemiesInRadius();
		int remainingTargets = Mathf.FloorToInt(numTargets) - 1;

		foreach (Collider collider in enemies)
		{
			if (remainingTargets == 0)
			{
				break;
			}

			TargetPoint targetPoint = collider.GetComponent<TargetPoint>();
			if (targetPoint != target)
			{
				remainingTargets -= 1;
				targetsToAttack.Add(targetPoint);
			}
		}

		foreach (TargetPoint targetToAttack in targetsToAttack)
		{
			// Physics.OverlapSphere(transform.localPosition, targetingRange, 1 << 9);
			// Debug.DrawLine(turret.position, targetToAttack.Position, Color.red, 0.1f);
			// Collider[] splashedTargets = Physics.OverlapSphere(targetToAttack.Position, 2.0f, 1 << 9);
			//
			// float damage = 50.0f;
			//
			// foreach (Collider collider in splashedTargets)
			// {
			// 	TargetPoint aoeTarget = collider.GetComponent<TargetPoint>();
			// 	aoeTarget.Enemy.ApplyDamage(damage * splash);
			// }
			//
			// targetToAttack.Enemy.ApplyDamage(damage);


			GameObject bulletObject = BulletPool.SharedInstance.GetPooledObject();
			if (bulletObject != null)
			{
				bulletObject.transform.position = turret.position;
				bulletObject.transform.rotation = turret.rotation;
				bulletObject.SetActive(true);

				Bullet bullet = bulletObject.GetComponent<Bullet>();
				bullet.target = targetToAttack;
			}
		}
	}

	
	private void InitLineRenderer()
	{
		lineRenderer = gameObject.GetComponent<LineRenderer>();
		// Color c1 = new Color(0.5f, 0.5f, 0.5f, 1);
		// //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetColors(Color.red, Color.red);
		lineRenderer.SetWidth(0.1f, 0.1f);
		lineRenderer.positionCount = numSegments + 1;
		lineRenderer.useWorldSpace = false;
		lineRenderer.enabled = false;

		float deltaTheta = (float) (2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0; i < numSegments + 1; i++)
		{
			float x = targetingRange * Mathf.Cos(theta);
			float z = targetingRange * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, transform.position.y + 0.05f, z);
			lineRenderer.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}

	// public override void GameUpdate()
	// {
	// 	// Debug.Log("Searching for target...");
	// }

	private void OnDrawGizmos()
	{
		// if (target != null)
		// {
		// 	Gizmos.DrawLine(turret.localPosition, target.Position);
		// }
	}

	void OnDrawGizmosSelected()
	{
		// Gizmos.color = Color.yellow;
		// Vector3 position = transform.localPosition;
		// position.y += 0.01f;
		// Gizmos.DrawWireSphere(position, targetingRange);
	}
}
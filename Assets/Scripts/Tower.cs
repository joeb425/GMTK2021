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
	
	[SerializeField]
	public LineRenderer linePrefab;

	[SerializeField]
	public int Cost;

	private float attackTimeRemaining;

	public int numSegments = 50;

	private LineRenderer lineRenderer;

	private SphereCollider sphereCollider;

	private TargetPoint target = null;

	private List<Tower> linkedTowers = new List<Tower>();
	private List<GameObject> beams = new List<GameObject>();

	[SerializeField]
	TowerAttributes towerAttributes;

	[SerializeField]
	Transform linkBeam = default;

	[SerializeField]
	public List<Perk> towerPerks;
	
	private int towerLevel = 0;

	private void Start()
	{
		UpdateAttributes();

		UpdateTowerRangeCollider();
		// InitLineRenderer();
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

	private void UpdateTowerRangeCollider()
	{
		sphereCollider = gameObject.GetComponent<SphereCollider>();
		sphereCollider.radius = towerAttributes.finalTargetingRange;
	}

	bool TrackTarget()
	{
		if (target == null)
		{
			return false;
		}

		Vector3 a = transform.localPosition;
		Vector3 b = target.Position;
		if (Vector3.Distance(a, b) > towerAttributes.finalTargetingRange + 0.125f)
		{
			target = null;
			return false;
		}

		return true;
	}

	Collider[] GetEnemiesInRadius()
	{
		return Physics.OverlapSphere(transform.localPosition, towerAttributes.finalTargetingRange, 1 << 9);
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

	void UpdateAttacking()
	{
		if (target != null)
		{
			attackTimeRemaining -= Time.deltaTime;
			if (attackTimeRemaining <= 0)
			{
				attackTimeRemaining = towerAttributes.finalAttackSpeed - attackTimeRemaining;
				Attack();
			}
		}
	}

	void Attack()
	{
		List<TargetPoint> targetsToAttack = new List<TargetPoint>();
		targetsToAttack.Add(target);

		Collider[] enemies = GetEnemiesInRadius();
		int remainingTargets = Mathf.FloorToInt(towerAttributes.finalNumTargets) - 1;

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
			if (false)
			{
				Debug.DrawLine(transform.position, targetToAttack.Position, Color.red, 0.5f);
				ApplyHit(targetToAttack);
			}
			else
			{
				GameObject bulletObject = BulletPool.SharedInstance.GetPooledObject();
				if (bulletObject != null)
				{
					bulletObject.transform.position = turret.position;
					bulletObject.transform.rotation = turret.rotation;
					bulletObject.SetActive(true);

					Bullet bullet = bulletObject.GetComponent<Bullet>();
					bullet.towerAttributes = towerAttributes;
					bullet.target = targetToAttack;
					bullet.tower = this;
				}
			}
		}
	}

	//
	// private void InitLineRenderer()
	// {
	// 	// lineRenderer = gameObject.GetComponent<LineRenderer>();
	// 	// // Color c1 = new Color(0.5f, 0.5f, 0.5f, 1);
	// 	// // //lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
	// 	// // lineRenderer.SetColors(c1, c1);
	// 	// // lineRenderer.SetWidth(0.5f, 0.5f);
	// 	// lineRenderer.positionCount = numSegments + 1;
	// 	// lineRenderer.useWorldSpace = false;
	// 	//
	// 	// float x;
	// 	// float y;
	// 	// float z;
	// 	//
	// 	// float angle = 20f;
	// 	//
	// 	// for (int i = 0; i < (numSegments + 1); i++)
	// 	// {
	// 	// 	x = Mathf.Sin(Mathf.Deg2Rad * angle) * targetingRange;
	// 	// 	y = Mathf.Cos(Mathf.Deg2Rad * angle) * targetingRange;
	// 	//
	// 	// 	lineRenderer.SetPosition(i, new Vector3(y, 10, x));
	// 	//
	// 	// 	angle += (360f / numSegments);
	// 	// }
	// }

	// public override void GameUpdate()
	// {
	// 	// Debug.Log("Searching for target...");
	// }

	private void OnDrawGizmos()
	{
		// if (target != null)
		// {
		//	Gizmos.DrawLine(turret.localPosition, target.Position);
		// }
		foreach (Tower link in linkedTowers)
		{
			Gizmos.DrawLine(transform.position, link.transform.position);
		}	
	}

	void OnDrawGizmosSelected()
	{
		// Gizmos.color = Color.yellow;
		// Vector3 position = transform.localPosition;
		// position.y += 0.01f;
		// Gizmos.DrawWireSphere(position, targetingRange);
	}

	Vector3 linkBeamScale;
	public void LinkTower(Tower link)
	{
		//beams.Add(GameObject.)
		linkedTowers.Add(link);
		Debug.Log(linkedTowers.Count);
		Vector3 link_pos = link.transform.position;
		//Vector3 current_pos = this.transform.position;

		linkBeam.localRotation = turret.localRotation;
		float d = Vector3.Distance(turret.position, link_pos);
		linkBeamScale.z = d;
		linkBeam.localScale = linkBeamScale;

		List<LineRenderer> lineRenderer = new List<LineRenderer>();

		//lineRenderer = GetComponent<LineRenderer>();
		Color c1 = new Color(1f, 1f, 0.5f, 1);
		//lineRenderer.material = new Material(Shader.Find("Particles/Additive"));

		
		for (int i = 0; i < (linkedTowers.Count); i++)
		{
			lineRenderer.Add(Instantiate(linePrefab, transform));
			lineRenderer[i].SetColors(c1, c1);
			lineRenderer[i].SetWidth(0.2f, 0.2f);
			lineRenderer[i].positionCount = 2; // [v1,v2,v3,v4] --> v1v2, v2v3, v3v4, v1v2,v1v3,v1v4
			lineRenderer[i].SetPosition(0, transform.position + Vector3.up * 0.05f);
			lineRenderer[i].SetPosition(1, linkedTowers[i].transform.position + Vector3.up*0.05f);
		}

		//Handel bonuses here as well :D
		
		UpdateAttributes();
	}

	public void UpdateAttributes()
	{
		// clear old calculated value?
		List<Perk> allPerks = new List<Perk>();
		
		foreach (Tower link in linkedTowers)
		{
			foreach (Perk perk in link.towerPerks)
			{
				allPerks.Add(perk);
			}
		}

		// todo update attributes when other towers change level
		towerAttributes.UpdateAttributes(towerLevel, allPerks);
		
		UpdateTowerRangeCollider();
	}

	public void ApplyHit(TargetPoint targetPoint)
	{
		Collider[] splashedTargets = Physics.OverlapSphere(targetPoint.Position, 2.0f, 1 << 9);

		float damage = towerAttributes.finalDamage;

		foreach (Collider collider in splashedTargets)
		{
			TargetPoint aoeTarget = collider.GetComponent<TargetPoint>();
			aoeTarget.Enemy.ApplyDamage(damage * towerAttributes.finalSplash);
		}

		targetPoint.Enemy.ApplyDamage(damage);
	}
}
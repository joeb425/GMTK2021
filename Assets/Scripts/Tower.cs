using System;
using System.Collections.Generic;
using Attributes;
using UnityEngine;

[Serializable]
public class Tower : MonoBehaviour
{
	[SerializeField]
	public TowerData towerData;
	
	[SerializeField]
	Transform turret;

	[SerializeField]
	Transform bulletSpawnPoint;

	[SerializeField]
	public LineRenderer linePrefab;

	private float attackTimeRemaining;

	public int numSegments = 64;

	public LineRenderer radiusLineRenderer;

	private SphereCollider sphereCollider;

	private TargetPoint target = null;

	[SerializeField]
	public List<Perk> towerPerks;

	private bool hasRotator;

	public GameplayAttributeContainer Attributes = new GameplayAttributeContainer();

	private int _towerLevel = 0;

	private void Awake()
	{
		InitLineRenderer();

		Attributes.InitAttribute(AttributeType.Damage, towerData.damage);
		Attributes.InitAttribute(AttributeType.SplashPercent, towerData.splash);
		Attributes.InitAttribute(AttributeType.Range, towerData.attackRange);
		Attributes.InitAttribute(AttributeType.AttackSpeed, towerData.attackSpeed);
		Attributes.InitAttribute(AttributeType.Split, towerData.split);

		UpdateAttributes();

		RotatorComponent rotatorComponent = gameObject.GetComponent<RotatorComponent>();
		hasRotator = rotatorComponent != null;
	}

	public void InitLineRenderer()
	{
		radiusLineRenderer = gameObject.AddComponent<LineRenderer>();
		radiusLineRenderer.startColor = Color.red;
		radiusLineRenderer.endColor = Color.red;
		radiusLineRenderer.startWidth = 0.05f;
		radiusLineRenderer.endWidth = 0.05f;
		radiusLineRenderer.loop = true;
		radiusLineRenderer.positionCount = numSegments + 1;
		radiusLineRenderer.useWorldSpace = false;
		radiusLineRenderer.enabled = false;
	}

	public void GameUpdate()
	{
		if (TrackTarget() || AcquireTarget())
		{
			if (!hasRotator)
			{
				Vector3 point = target.Position;
				point.y = turret.transform.position.y;
				turret.LookAt(point);
			}

			UpdateAttacking();
		}
	}

	private void UpdateTowerRangeCollider()
	{
		sphereCollider = gameObject.GetComponent<SphereCollider>();
		sphereCollider.radius = Attributes.GetCurrentValue(AttributeType.Range);
	}

	bool TrackTarget()
	{
		if (target == null)
		{
			return false;
		}

		Vector3 a = transform.localPosition;
		Vector3 b = target.Position;
		if (Vector3.Distance(a, b) > Attributes.GetCurrentValue(AttributeType.Range) + 0.125f)
		{
			target = null;
			return false;
		}

		return true;
	}

	Collider[] GetEnemiesInRadius()
	{
		return Physics.OverlapSphere(transform.localPosition, Attributes.GetCurrentValue(AttributeType.Range), 1 << 9);
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
				attackTimeRemaining = Attributes.GetCurrentValue(AttributeType.AttackSpeed) - attackTimeRemaining;
				Attack();
			}
		}
	}

	void Attack()
	{
		List<TargetPoint> targetsToAttack = new List<TargetPoint>();
		targetsToAttack.Add(target);

		Collider[] enemies = GetEnemiesInRadius();
		int numExtraTargets = Mathf.FloorToInt(Attributes.GetCurrentValue(AttributeType.Split));

		foreach (Collider collider in enemies)
		{
			if (numExtraTargets == 0)
			{
				break;
			}
			
			TargetPoint targetPoint = collider.GetComponent<TargetPoint>();
			if (targetPoint != target)
			{
				numExtraTargets -= 1;
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
				GameObject bulletObject = BulletPool.Get.GetPooledObject();
				if (bulletObject != null)
				{
					bulletObject.transform.position = bulletSpawnPoint.position;
					bulletObject.transform.rotation = bulletSpawnPoint.rotation;
					bulletObject.SetActive(true);

					Bullet bullet = bulletObject.GetComponent<Bullet>();
					bullet.target = targetToAttack;
					bullet.tower = this;
					bullet.bulletMesh.mesh = towerData.bulletMesh;
					bullet.bulletSpeed = towerData.bulletSpeed;
					bullet.bulletMeshRenderer.materials[0] = towerData.bulletMaterial;
				}
			}
		}
	}

	private void UpdateRangeDisplay()
	{
		float deltaTheta = (float) (2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0; i < numSegments + 1; i++)
		{
			float range = Attributes.GetCurrentValue(AttributeType.Range);
			float x = range * Mathf.Cos(theta);
			float z = range * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, transform.position.y + 0.25f, z);
			radiusLineRenderer.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}

	// public override void GameUpdate()
	// {
	// 	// Debug.Log("Searching for target...");
	// }

	private void OnDrawGizmos()
	{
		//	Gizmos.DrawLine(turret.localPosition, target.Position);
	}

	public void UpdateAttributes()
	{
		// TODO bind to callbacks when attributes change
		Attributes.GetAttribute(AttributeType.Range).OnAttributeChanged += (attribute, oldValue) =>
		{
			if (attribute.attributeType == AttributeType.Range)
			{
				UpdateRangeDisplay();
			}
		};
		
		UpdateTowerRangeCollider();

		UpdateRangeDisplay();
	}

	public void ApplyHit(TargetPoint targetPoint)
	{
		Collider[] splashedTargets = Physics.OverlapSphere(targetPoint.Position, 2.0f, 1 << 9);

		float damage = Attributes.GetCurrentValue(AttributeType.Damage);

		foreach (Collider collider in splashedTargets)
		{
			TargetPoint aoeTarget = collider.GetComponent<TargetPoint>();
			float splashPercent = Attributes.GetCurrentValue(AttributeType.SplashPercent);
			aoeTarget.Enemy.ApplyDamage(damage * splashPercent);
		}

		targetPoint.Enemy.ApplyDamage(damage);
	}

	public void OnSelected(bool selected)
	{
		radiusLineRenderer.enabled = selected;
	}

	public bool CanUpgradeTower()
	{
		return _towerLevel < towerData.upgradeInfos.Length;
	}
	
	public void UpgradeTower()
	{
		if (!CanUpgradeTower())
		{
			return;
		}
		
		UpgradeInfo upgradeInfo = towerData.upgradeInfos[_towerLevel];
		if (GameState.Get.SpendCash(upgradeInfo.upgradeCost))
		{
			Attributes.ApplyEffect(upgradeInfo.upgradeEffect);
		}
	}
}
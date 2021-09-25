﻿using System;
using System.Collections.Generic;
using Attributes;
using HexLibrary;
using UnityEngine;

[Serializable]
public class Tower : MonoBehaviour
{
	[SerializeField]
	public TowerData towerData;
	
	[SerializeField]
	Transform turret;

	[SerializeField]
	public Transform bulletSpawnPoint;

	[SerializeField]
	public LineRenderer linePrefab;

	private float attackTimeRemaining;

	public int numSegments = 64;

	public LineRenderer radiusLineRenderer;

	private SphereCollider sphereCollider;

	private TargetPoint target = null;

	private bool hasRotator;

	public GameplayAttributeContainer Attributes = new GameplayAttributeContainer();

	private int _towerLevel = 0;

	public List<GameplayEffect> onHitEffects = new List<GameplayEffect>();

	private void Awake()
	{
		InitLineRenderer();

		InitAttributes();

		RotatorComponent rotatorComponent = gameObject.GetComponent<RotatorComponent>();
		hasRotator = rotatorComponent != null;

		Game.Get.gameState.Board.towerLayer.OnSelectedObjectChanged += (oldTower, newTower) =>
		{
			bool selectedThis = newTower == gameObject;
			OnSelected(selectedThis);
		};
	}

	public void OnTowerPlaced(GroundTileComponent groundTile)
	{
		for (int i = 0; i < 6; i++)
		{
			Hex neighborHex = groundTile.hex.Neighbor(i);
			if (Game.Get.gameState.Board.groundLayer.GetTile(neighborHex, out var neighborTile))
			{
				var neighbor = neighborTile.GetComponent<GroundTileComponent>();
				if (neighbor != null)
				{
					foreach (GameplayEffect effect in towerData.supportEffects)
					{
						neighbor.AddTowerEffect(effect);
					}
				}
			}
		}
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

		Attributes.Update(Time.deltaTime);
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
				GameObject bulletObject = BulletPool.Get.GetInstance();
				if (bulletObject != null)
				{
					bulletObject.transform.position = bulletSpawnPoint.position;
					bulletObject.transform.rotation = bulletSpawnPoint.rotation;

					Bullet bullet = bulletObject.GetComponent<Bullet>();
					bullet.target = targetToAttack;
					bullet.tower = this;
					bullet.bulletMesh.mesh = towerData.bulletMesh;
					bullet.bulletSpeed = towerData.bulletSpeed;
					bullet.bulletMeshRenderer.materials[0] = towerData.bulletMaterial;
					bullet.Init();
				}
			}
		}

		Game.Get.audio.PlaySfx(towerData.shootSfx);
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

	public void InitAttributes()
	{
		Attributes.InitAttribute(AttributeType.Damage, towerData.damage);
		Attributes.InitAttribute(AttributeType.SplashPercent, towerData.splash);
		Attributes.InitAttribute(AttributeType.Range, towerData.attackRange);
		Attributes.InitAttribute(AttributeType.AttackSpeed, towerData.attackSpeed);
		Attributes.InitAttribute(AttributeType.Split, towerData.split);
		onHitEffects = towerData.onHitEffects;
		
		// TODO bind to callbacks when attributes change
		Attributes.GetAttribute(AttributeType.Range).OnAttributeChanged += (attribute) =>
		{
			UpdateRangeDisplay();
			UpdateTowerRangeCollider();
		};

		UpdateTowerRangeCollider();
		UpdateRangeDisplay();
	}

	public void ApplyHit(TargetPoint targetPoint)
	{
		foreach (GameplayEffect effect in onHitEffects)
		{
			targetPoint.Enemy.Attributes.ApplyEffect(effect);
		}

		float damage = Attributes.GetCurrentValue(AttributeType.Damage);

		Collider[] splashedTargets = Physics.OverlapSphere(targetPoint.Position, 2.0f, 1 << 9);
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

	public void LinkTower()
	{
		// Link tower -> need to tell board link has occured and probably pass in coord of linking tile
		var link = 1;
		GameState.Get.Board.CreateLink(this);

		// 




	}

	public void UpdateAttributes()
	{

	}
}
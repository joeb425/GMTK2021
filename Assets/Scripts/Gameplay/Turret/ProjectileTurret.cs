using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using Mantis.AttributeSystem;
using ObjectPools;
using UnityEditor;
using UnityEditor.Media;
using UnityEngine;

public class ProjectileTurret : SingleTargetTurret
{
	[SerializeField]
	private Bullet bulletPrefab;

	[SerializeField]
	public Transform bulletSpawnPoint;

	public override void Init(Tower parentTower)
	{
		base.Init(parentTower);
		bulletSpawnPoint = tower.bulletSpawnPoint;
	}

	protected override void Attack()
	{
		GameplayAttributeContainer towerAttributes = tower.GetAttributes();
		List<TargetPoint> targetsToAttack = new List<TargetPoint>();
		targetsToAttack.Add(_target);

		int numExtraTargets = Mathf.FloorToInt(towerAttributes.GetCurrentValue(MyAttributes.Get().Split));
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
			if (bulletPrefab == null)
			{
				Debug.LogError($"no bullet set in {gameObject} {this}");
			}
			else
			{
				Bullet bulletObject = BulletPool.Get().GetInstance(bulletPrefab);
				if (bulletObject != null)
				{
					var bulletTransform = bulletObject.transform;
					bulletTransform.position = bulletSpawnPoint.position;
					bulletTransform.rotation = bulletSpawnPoint.rotation;

					Bullet bullet = bulletObject.GetComponent<Bullet>();
					bullet.target = targetToAttack;
					bullet.tower = tower;
					int chain = towerAttributes.GetCurrentValueAsInt(MyAttributes.Get().Chain);
					float chainRadius = towerAttributes.GetCurrentValue(MyAttributes.Get().ChainRadius);
					bullet.SetChain(chain, chainRadius);
					bullet.Init();
				}
			}
		}

		Game.Get.GetAudioHandler().PlaySfx(tower.towerData.shootSfx);
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (bulletSpawnPoint == null)
		{
			bulletSpawnPoint = GetComponentsInChildren<Transform>().First(t => t.name == "BulletSpawnPoint");
		}

		if (bulletPrefab == null)
		{
			bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Towers/BasicBullet.prefab").GetComponent<Bullet>();
		}
	}
}
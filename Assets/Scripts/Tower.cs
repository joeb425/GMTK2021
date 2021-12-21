using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using Mantis.AttributeSystem;
using HexLibrary;
using Mantis.AttributeSystem.UI;
using Mantis.GameplayTags;
using Mantis.Hex;
using Mantis.Utils;
using ObjectPools;
using UI.MainMenu;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(GameplayAttributeContainer))]
public class Tower : HexTileComponent
{
	[SerializeField]
	public TowerData towerData;

	[SerializeField]
	Transform turret;

	[SerializeField]
	public Transform bulletSpawnPoint;

	[SerializeField]
	public GameObject towerRangeDisplay;

	[SerializeField]
	public bool trackTarget = true;

	[SerializeField]
	public AnimationClip spawnAnimation;

	[SerializeField]
	public Material towerBuildMaterial;

	private float attackTimeRemaining;

	public int numSegments = 64;

	public LineRenderer radiusLineRenderer;

	private SphereCollider sphereCollider;

	private TargetPoint target = null;

	private bool hasRotator;

	public GameplayAttributeContainer Attributes;

	private int _towerLevel = 0;

	public List<GameplayEffect> onHitEffects = new List<GameplayEffect>();

	public GroundTileComponent groundTile;

	private Animation _animation;

	private VisualElement _towerDisplay;

	private Camera _mainCamera;

	private void Awake()
	{
		if (gameObject.transform.localScale != Vector3.one)
		{
			Debug.LogError("Tower should have scale 1.0");
		}
		
		InitLineRenderer();

		InitAttributes();

		RotatorComponent rotatorComponent = gameObject.GetComponent<RotatorComponent>();
		hasRotator = rotatorComponent != null;

		_animation = GetComponent<Animation>();
		_mainCamera = Camera.main;
		_towerDisplay = gameObject.GetComponent<UIDocument>().rootVisualElement;
		_towerDisplay.Q<AttributeLabel>("Damage").BindToGameplayAttribute(Attributes.GetAttribute(MyAttributes.Get().Damage));
		_towerDisplay.Q<AttributeLabel>("AttackSpeed").BindToGameplayAttribute(Attributes.GetAttribute(MyAttributes.Get().AttackSpeed));
	}

	private void OnDestroy()
	{
		if (GameState.Get() != null)
		{
			GameState.Get().Board.OnSelectedTileChanged -= OnSelected;
		}
	}

	public override void PlaceOnHex(Hex newHex, HexGridLayer newLayer) 
	{
		base.PlaceOnHex(newHex, newLayer);

		SetRadiusVisible(true);

		if (layer == GameState.Get().Board.tempTowerLayer)
		{
			PlaceOnTempLayer();
			return;
		}

		if (layer != GameState.Get().Board.towerLayer)
		{
			return;
		}

		GameState.Get().Board.GetGroundTileAtHex(hex, out groundTile);

		groundTile.effectList.SetContainer(Attributes);

		for (int i = 0; i < 6; i++)
		{
			Hex neighborHex = groundTile.hex.Neighbor(i);

			if (GameState.Get().Board.groundLayer.GetTileAtHex(neighborHex, out GroundTileComponent neighbor))
			{
				foreach (GameplayEffect effect in towerData.supportEffects)
				{
					neighbor.AddTowerEffect(effect);
				}
			}
		}

		// _animation.Play();

		GameState.Get().Board.OnSelectedTileChanged += OnSelected;
	}

	public void PlaceOnTempLayer()
	{
		MantisUtils.SetAllMaterials(turret, towerBuildMaterial);
	}

	public override void RemoveFromHex(Hex hex)
	{
		base.RemoveFromHex(hex);

		if (layer != GameState.Get().Board.towerLayer)
		{
			return;
		}

		for (int i = 0; i < 6; i++)
		{
			Hex neighborHex = groundTile.hex.Neighbor(i);

			if (GameState.Get().Board.groundLayer.GetTileAtHex(neighborHex, out GroundTileComponent neighbor))
			{
				foreach (GameplayEffect effect in towerData.supportEffects)
				{
					neighbor.RemoveTowerEffect(effect);
				}
			}
		}

		groundTile.effectList.SetContainer(null);

		GameState.Get().Board.OnSelectedTileChanged -= OnSelected;
	}

	public void InitLineRenderer()
	{
		radiusLineRenderer = gameObject.GetComponent<LineRenderer>();
		radiusLineRenderer.startColor = Color.red;
		radiusLineRenderer.endColor = Color.red;
		radiusLineRenderer.startWidth = 0.05f;
		radiusLineRenderer.endWidth = 0.05f;
		radiusLineRenderer.loop = true;
		radiusLineRenderer.positionCount = numSegments + 1;
		radiusLineRenderer.useWorldSpace = false;
		radiusLineRenderer.enabled = true;
	}

	public override void GameUpdate()
	{
		if (TrackTarget() || AcquireTarget())
		{
			if (!hasRotator && trackTarget)
			{
				Vector3 point = target.Position;
				point.y = turret.transform.position.y;
				turret.LookAt(point);
			}

			UpdateAttacking();
		}

		Attributes.GameUpdate();

		UpdateTowerDisplay();
	}

	private void UpdateTowerRange()
	{
		float range = Attributes.GetCurrentValue(MyAttributes.Get().Range);
		// update collider
		sphereCollider = gameObject.GetComponent<SphereCollider>();
		sphereCollider.radius = range;

		// update tower range display
		towerRangeDisplay.transform.localScale = new Vector3(range * 2.0f, towerRangeDisplay.transform.localScale.y, range * 2.0f);

		// update line renderer
		float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0; i < numSegments + 1; i++)
		{
			float x = range * Mathf.Cos(theta);
			float z = range * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, transform.position.y + 0.25f, z);
			radiusLineRenderer.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}

	private void UpdateTowerDisplay()
	{
		// Update tower display document
		Vector3 worldPosition = transform.position;
		Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(_towerDisplay.panel, worldPosition, _mainCamera);
		Rect layout = _towerDisplay.Children().First().layout;
		newPosition.x -= layout.width / 2;
		newPosition.y += 15.0f; // magic number 
		// Debug.Log(_towerDisplay);
		_towerDisplay.transform.position = newPosition;
	}

	bool TrackTarget()
	{
		if (!IsValidTarget(target))
		{
			target = null;
			return false;
		}

		Vector3 a = transform.position;
		Vector3 b = target.Position;
		a.y = b.y;
		if (Vector3.Distance(a, b) > Attributes.GetCurrentValue(MyAttributes.Get().Range))
		{
			target = null;
			return false;
		}

		return true;
	}

	Collider[] GetEnemiesInRadius()
	{
		return Physics.OverlapSphere(transform.position, Attributes.GetCurrentValue(MyAttributes.Get().Range), 1 << 9);
	}

	bool IsValidTarget(TargetPoint targetPoint)
	{
		if (targetPoint == null)
		{
			return false;
		}
		
		Enemy enemyTarget = targetPoint.Enemy;
		if (!enemyTarget || !enemyTarget.isActiveAndEnabled)
		{
			return false;
		}

		GameplayTagManager.instance.RequestTag("Status.IsAlive", out GameplayTag statusIsAlive);
		GameplayTagManager.instance.RequestTag("Status.Invulnerable", out GameplayTag statusInvulnerable);
		bool isAlive = enemyTarget.gameplayTagContainer.ContainsTag(statusIsAlive);
		bool isInvulnerable = enemyTarget.gameplayTagContainer.ContainsTag(statusInvulnerable);

		return isAlive && !isInvulnerable;
	}

	bool AcquireTarget()
	{
		Collider[] targets = GetEnemiesInRadius();
		
		foreach (Collider targetCollider in targets)
		{
			TargetPoint targetPoint = targetCollider.GetComponent<TargetPoint>();
			if (IsValidTarget(targetPoint))
			{
				target = targetPoint;
				return true;
			}
		}

		// if (targets.Length > 0)
		// {
		// 	target = targets[0].GetComponent<TargetPoint>();
		// 	// Debug.Log("Set target" + targets[0].gameObject);
		// 	Debug.Assert(target != null, "Targeted non-enemy!", targets[0]);
		// 	return true;
		// }

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
				attackTimeRemaining = Attributes.GetCurrentValue(MyAttributes.Get().AttackSpeed) - attackTimeRemaining;
				Attack();
			}
		}
	}

	void Attack()
	{
		List<TargetPoint> targetsToAttack = new List<TargetPoint>();
		targetsToAttack.Add(target);

		Collider[] enemies = GetEnemiesInRadius();
		int numExtraTargets = Mathf.FloorToInt(Attributes.GetCurrentValue(MyAttributes.Get().Split));

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
				if (towerData.bulletPrefab == null)
				{
					Debug.LogError("no bullet set in tower data");
				}
				else
				{
					Bullet bulletObject = BulletPool.Get().GetInstance(towerData.bulletPrefab);
					if (bulletObject != null)
					{
						bulletObject.transform.position = bulletSpawnPoint.position;
						bulletObject.transform.rotation = bulletSpawnPoint.rotation;

						Bullet bullet = bulletObject.GetComponent<Bullet>();
						bullet.target = targetToAttack;
						bullet.tower = this;
						bullet.Init();
					}
				}
			}
		}

		Game.Get.GetAudioHandler().PlaySfx(towerData.shootSfx);
	}

	private void UpdateRangeDisplay()
	{
		float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0; i < numSegments + 1; i++)
		{
			float range = Attributes.GetCurrentValue(MyAttributes.Get().Range);
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
		if (target != null)
		{
			Gizmos.DrawLine(transform.position, target.Position);
		}
	}

	public void InitAttributes()
	{
		Attributes = GetComponent<GameplayAttributeContainer>();
		Attributes.InitAttribute(MyAttributes.Get().Damage, towerData.damage);
		Attributes.InitAttribute(MyAttributes.Get().SplashPercent, towerData.splash);
		Attributes.InitAttribute(MyAttributes.Get().Range, towerData.attackRange);
		Attributes.InitAttribute(MyAttributes.Get().AttackSpeed, towerData.attackSpeed);
		Attributes.InitAttribute(MyAttributes.Get().Split, towerData.split);
		onHitEffects = towerData.onHitEffects;

		// TODO bind to callbacks when attributes change
		Attributes.GetAttribute(MyAttributes.Get().Range).OnAttributeChanged += (attribute) =>
		{
			UpdateTowerRange();
		};

		UpdateTowerRange();
	}

	public void ApplyHit(TargetPoint targetPoint)
	{
		foreach (GameplayEffect effect in onHitEffects)
		{
			targetPoint.Enemy.Attributes.ApplyEffect(effect);
		}

		float damage = Attributes.GetCurrentValue(MyAttributes.Get().Damage);

		Collider[] splashedTargets = Physics.OverlapSphere(targetPoint.Position, 2.0f, 1 << 9);
		foreach (Collider collider in splashedTargets)
		{
			TargetPoint aoeTarget = collider.GetComponent<TargetPoint>();
			if (IsValidTarget(aoeTarget))
			{
				float splashPercent = Attributes.GetCurrentValue(MyAttributes.Get().SplashPercent);
				DamageEnemy(aoeTarget.Enemy, damage * splashPercent);
			}
		}

		DamageEnemy(targetPoint.Enemy, damage);
	}

	public void DamageEnemy(Enemy enemy, float damage)
	{
		if (damage == 0) 
			return;

		EffectParameters effectParameters;
		effectParameters.source = Attributes;

		GameplayEffect damageEffect = ScriptableObject.CreateInstance<GameplayEffect>();
		GameplayAttributeModifier healthMod = new GameplayAttributeModifier(
			MyAttributes.Get().Health, 
			-1 * damage,
			AttributeOperator.Add);

		damageEffect.modifiers.Add(healthMod);

		enemy.Attributes.ApplyEffect(damageEffect, effectParameters);
	}

	protected void OnSelected(Hex _, Hex newSelection)
	{
		SetRadiusVisible(newSelection == hex);
	}

	protected void SetRadiusVisible(bool visible)
	{
		radiusLineRenderer.enabled = visible;
		towerRangeDisplay.SetActive(visible);
	}

	protected bool CanUpgradeTower()
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
		if (GameState.Get().SpendCash(upgradeInfo.upgradeCost))
		{
			Attributes.ApplyEffect(upgradeInfo.upgradeEffect);
		}
	}

	public void LinkTower()
	{
		// Link tower -> need to tell board link has occured and probably pass in coord of linking tile
		var link = 1;
		GameState.Get().Board.CreateLink(this);

		// 
	}

	public void UpdateAttributes()
	{
	}
}
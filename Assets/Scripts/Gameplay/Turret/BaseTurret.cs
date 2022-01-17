using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using Mantis.AttributeSystem;
using Mantis.GameplayTags;
using ObjectPools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GameplayAttributeContainer))]
public abstract class BaseTurret : MonoBehaviour
{
	[SerializeField]
	public Tower tower;

	[SerializeField]
	public Transform turretTransform;

	[SerializeField]
	UnityEvent onAttack;

	private float _attackTimeRemaining;

	public virtual void Init(Tower parentTower)
	{
		tower = parentTower;
		turretTransform = tower == null ? null : tower.turret;
	}

	public virtual void GameUpdate()
	{
		UpdateAttacking();
	}

	public abstract bool CanAttack();

	public Collider[] GetEnemiesInRadius()
	{
		return Physics.OverlapSphere(transform.position, tower.GetAttributes().GetCurrentValue(MyAttributes.Get().Range), 1 << 9);
	}

	public List<TargetPoint> GetTargetsInRadius()
	{
		return GetEnemiesInRadius()
			.Select(targetCollider => targetCollider.GetComponent<TargetPoint>())
			.Where(IsValidTarget).ToList();
	}

	public bool IsValidTarget(TargetPoint targetPoint)
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

		return tower.targetTagFilter.DoesFilterPass(enemyTarget.gameplayTagContainer);
	}

	protected virtual void UpdateAttacking()
	{
		_attackTimeRemaining -= Time.deltaTime;
		if (CanAttack() && _attackTimeRemaining <= 0)
		{
			_attackTimeRemaining = tower.GetAttributes().GetCurrentValue(MyAttributes.Get().AttackSpeed) - _attackTimeRemaining;
			Attack();
			onAttack?.Invoke();
		}
	}

	protected abstract void Attack();

	public void ApplyHit(TargetPoint targetPoint)
	{
		GameplayAttributeContainer attributes = tower.GetAttributes();
		foreach (GameplayEffect effect in tower.towerData.onHitEffects)
		{
			targetPoint.Enemy.attributes.ApplyEffect(effect);
		}

		float damage = attributes.GetCurrentValue(MyAttributes.Get().Damage);

		int splash = attributes.GetCurrentValueAsInt(MyAttributes.Get().SplashRadius);
		if (splash > 0)
		{
			Collider[] splashedTargets = Physics.OverlapSphere(targetPoint.Position, 2.0f, 1 << 9);
			foreach (Collider coll in splashedTargets)
			{
				TargetPoint splashTarget = coll.GetComponent<TargetPoint>();
				if (IsValidTarget(splashTarget))
				{
					float splashPercent = attributes.GetCurrentValue(MyAttributes.Get().SplashPercent);
					DamageEnemy(splashTarget.Enemy, damage * splashPercent);
				}
			}
		}

		DamageEnemy(targetPoint.Enemy, damage);
	}

	public void DamageEnemy(Enemy enemy, float damage)
	{
		if (damage == 0)
			return;

		EffectParameters effectParameters;
		effectParameters.source = tower.GetAttributes();

		GameplayEffect damageEffect = ScriptableObject.CreateInstance<GameplayEffect>();
		GameplayAttributeModifier healthMod = new GameplayAttributeModifier(
			MyAttributes.Get().Health,
			-1 * damage,
			AttributeOperator.Add);

		damageEffect.modifiers.Add(healthMod);

		enemy.attributes.ApplyEffect(damageEffect, effectParameters);
	}

	protected virtual void OnValidate()
	{
		if (turretTransform == null)
		{
			turretTransform = GetComponentsInChildren<Transform>().First(t => t.name == "Turret");
		}
	}
}
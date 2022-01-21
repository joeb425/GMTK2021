using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using Mantis.AttributeSystem;
using Mantis.GameplayTags;
using Misc;
using ObjectPools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

// [RequireComponent(typeof(GameplayAttributeContainer))]
[RequireComponent(typeof(TargetingState))]
public abstract class BaseTurret : MonoBehaviour
{
	[SerializeField]
	public Tower tower;

	[SerializeField]
	public Transform turretTransform;

	[SerializeField]
	UnityEvent onAttack;

	[SerializeField]
	public TargetingState targetingState;

	private float _attackTimer;

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

	// public Collider[] GetEnemiesInRadius()
	// {
	// 	return Physics.OverlapSphere(transform.position, tower.GetAttributes().GetCurrentValue(MyAttributes.Get().Range), 1 << 9);
	// }

	public List<TargetPoint> GetTargetsInRadius()
	{
		return targetingState.GetAllTargets().Where(IsValidTarget).ToList();
	}

	public bool IsValidTarget(TargetPoint targetPoint)
	{
		if (targetPoint == null || !targetPoint.IsValid())
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
		float attackSpeed = tower.GetAttributes().GetCurrentValue(MyAttributes.Get().AttackSpeed);
		_attackTimer += Time.deltaTime;

		float percent = _attackTimer / attackSpeed;
		tower.attackProgress.value = percent * 100;

		if (CanAttack() && percent > 1.0f)
		{
			_attackTimer = 0.0f;//Math.Min(attackSpeed, attackSpeed - _attackTimer);
			Attack();
			onAttack?.Invoke();
		}
	}

	protected abstract void Attack();

	public void ApplyHit(TargetPoint targetPoint)
	{
		if (!IsValidTarget(targetPoint))
		{
			return;
		}

		GameplayAttributeContainer attributes = tower.GetAttributes();
		foreach (GameplayEffect effect in tower.towerData.onHitEffects)
		{
			targetPoint.Enemy.attributes.ApplyEffect(effect);
		}

		float damage = attributes.GetCurrentValue(MyAttributes.Get().Damage);

		float splashPercent = attributes.GetCurrentValue(MyAttributes.Get().SplashPercent);
		if (splashPercent > 0)
		{
			float splashRadius = attributes.GetCurrentValue(MyAttributes.Get().SplashRadius);
			Collider[] splashedTargets = Physics.OverlapSphere(targetPoint.Position, splashRadius, 1 << 9);
			foreach (Collider coll in splashedTargets)
			{
				TargetPoint splashTarget = coll.GetComponent<TargetPoint>();
				if (IsValidTarget(splashTarget))
				{
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
		if (tower == null)
		{
			tower = GetComponent<Tower>();
		}
		
		if (turretTransform == null)
		{
			turretTransform = GetComponentsInChildren<Transform>().First(t => t.name == "Turret");
		}

		if (targetingState == null)
		{
			targetingState = GetComponentInChildren<TargetingState>();
		}
	}
}
using System.Collections.Generic;
using Mantis.GameplayTags;
using ObjectPools;
using UnityEngine;

public class Bullet : MonoBehaviour, IGameplayTag
{
	[SerializeField]
	public float bulletSpeed = 3.0f;

	[SerializeField]
	public GameplayTag gameplayTag;
	public GameplayTag GetGameplayTag()
	{
		return gameplayTag;
	}

	public Tower tower;

	public TargetPoint target;
	private HashSet<TargetPoint> _hitTargets = new HashSet<TargetPoint>();

	public Vector3 spawnPoint;
	public Vector3 lastKnownPos;

	private float _progress;
	private float _progressSpeed;

	private int _chain;
	private float _chainRadius = 1.0f;

	public void SetChain(int chain, float chainRadius)
	{
		_chain = chain;
		_chainRadius = chainRadius;
	}

	private TrailRenderer _trailRenderer;

	public void Awake()
	{
		_trailRenderer = GetComponent<TrailRenderer>();
	}

	public void Init()
	{
		SetTarget(target, tower.bulletSpawnPoint.position);
		_hitTargets = new HashSet<TargetPoint>();
	}

	public void SetTarget(TargetPoint newTarget, Vector3 startPos)
	{
		_progress = 0.0f;
		target = newTarget;

		var myTransform = transform;

		var position = myTransform.position;
		position.y = 5.0f;
		myTransform.position = position;

		lastKnownPos = newTarget.Position;
		lastKnownPos.y = position.y;

		spawnPoint = startPos;
		spawnPoint.y = position.y;
	}

	private void Update()
	{
		if (target != null)
		{
			lastKnownPos = target.Position;
			lastKnownPos.y = transform.position.y;
			float distance = (lastKnownPos - spawnPoint).magnitude;
			_progressSpeed = (1.0f / distance) * bulletSpeed;
		}


		_progress += Time.deltaTime * _progressSpeed;
		transform.LookAt(lastKnownPos);
		transform.position = Vector3.Lerp(spawnPoint, lastKnownPos, _progress);

		if (_progress >= 1.0f)
		{
			// check if the target is invalid before we even apply damage (to be used for chaining)
			bool targetIsDead = false;// target.IsValid();

			Explode();

			if (!targetIsDead && TryChaining())
			{
				return;
			}

			BulletPool.Get().ReclaimToPool(this);
			if (_trailRenderer)
			{
				_trailRenderer.Clear();
			}
		}
	}

	private void Explode()
	{
		if (target != null && tower != null)
		{
			_hitTargets.Add(target);
			tower.ApplyHit(target);
			PlaySound();
			PlayParticleEffect();
		}
	}

	public bool TryChaining()
	{
		if (_chain == 0)
		{
			return false;
		}

		if (AcquireChainTarget(out var targetPoint))
		{
			_chain -= 1;
			SetTarget(targetPoint, transform.position);
			return true;
		}

		_chain = 0;
		return false;
	}

	public bool AcquireChainTarget(out TargetPoint outTargetPoint)
	{
		Vector3 pos = transform.position;
		pos.y = 0.0f;
		Collider[] targets = Physics.OverlapSphere(pos, _chainRadius, 1 << 9);

		outTargetPoint = null;
		float? closestDist = null;
		
		foreach (TargetPoint targetPoint in _hitTargets)
		{
			Debug.Log(targetPoint);
		}
		
		foreach (Collider targetCollider in targets)
		{
			var targetPoint = targetCollider.GetComponent<TargetPoint>();

			if (target == targetPoint)
			{
				continue;
			}

			if (_hitTargets.Contains(targetPoint))
			{
				continue;
			}

			if (tower.IsValidTarget(targetPoint))
			{
				Vector3 toTarget = transform.position - targetPoint.Position;
				toTarget.y = 0;
				float dist = toTarget.sqrMagnitude;

				if (closestDist == null || dist < closestDist)
				{
					closestDist = dist;
					outTargetPoint = targetPoint;
				}
			}
		}

		return outTargetPoint != null;
	}

	private void PlaySound()
	{
	}

	private void PlayParticleEffect()
	{
	}
}
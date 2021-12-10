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

	public Vector3 spawnPoint;
	public Vector3 lastKnownPos;

	private float _progress;
	private float _progressSpeed;

	private TrailRenderer _trailRenderer;

	public void Awake()
	{
		_trailRenderer = GetComponent<TrailRenderer>();
	}

	public void Init()
	{
		_progress = 0.0f;

		lastKnownPos = target.Position;
		spawnPoint = tower.bulletSpawnPoint.position;
	}

	private void Update()
	{
		if (target != null)
		{
			lastKnownPos = target.Position;
			float distance = (lastKnownPos - spawnPoint).magnitude;
			_progressSpeed = (1.0f / distance) * bulletSpeed;
		}


		_progress += Time.deltaTime * _progressSpeed;
		transform.LookAt(lastKnownPos);
		transform.position = Vector3.Lerp(spawnPoint, lastKnownPos, _progress);

		if (_progress >= 1.0f)
		{
			Explode();
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
			tower.ApplyHit(target);
			PlaySound();
			PlayParticleEffect();
		}
	}

	private void PlaySound()
	{
	}

	private void PlayParticleEffect()
	{
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
	public Tower tower;
	
	public TargetPoint target;

	public Vector3 spawnPoint;
	public Vector3 lastKnownPos;

	public float bulletSpeed = 3.0f;

	[SerializeField]
	public MeshFilter bulletMesh;

	[SerializeField]
	public MeshRenderer bulletMeshRenderer;

	private float progress = 0.0f;
	private float progressSpeed = 1.0f;

	private TrailRenderer _trailRenderer;

	public void Awake()
	{
		_trailRenderer = GetComponent<TrailRenderer>();
	}

	public void Init()
	{
		progress = 0.0f;

		lastKnownPos = target.Position;
		spawnPoint = tower.bulletSpawnPoint.position;
	}

	private void Update()
	{
		if (target != null)
		{
			lastKnownPos = target.Position;
			float distance = (lastKnownPos - spawnPoint).magnitude;
			progressSpeed = (1.0f / distance) * bulletSpeed;
		}

		
		progress += Time.deltaTime * progressSpeed;
		transform.LookAt(lastKnownPos);
		transform.position = Vector3.Lerp(spawnPoint, lastKnownPos, progress);

		if (progress >= 1.0f)
		{
			Explode();
			BulletPool.Get.ReclaimToPool(gameObject);
			_trailRenderer.Clear();
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
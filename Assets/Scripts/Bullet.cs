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

	public void Init()
	{
		progress = 0.0f;

		lastKnownPos = target.Position;
		spawnPoint = tower.bulletSpawnPoint.position;

		if (tower)
		{
			float distance = (lastKnownPos - spawnPoint).magnitude;
			progressSpeed = (1.0f / distance) * bulletSpeed;
		}	
	}

	private void Update()
	{
		if (target != null)
		{
			lastKnownPos = target.Position;
		}

		progress += Time.deltaTime * progressSpeed;
		transform.LookAt(lastKnownPos);
		transform.position = Vector3.Lerp(spawnPoint, lastKnownPos, progress);

		if (progress >= 1.0f)
		{
			Explode();
			BulletPool.Get.ReclaimToPool(gameObject);
		}
	}

	private void Explode()
	{
		if (target != null && tower != null)
		{
			tower.ApplyHit(target);
		}
	}
}
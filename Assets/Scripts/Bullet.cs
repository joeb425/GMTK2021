using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
	public Tower tower;
	
	public TargetPoint target;
	public Vector3 lastKnownPos;

	public float bulletSpeed = 3.0f;

	[SerializeField]
	public MeshFilter bulletMesh;

	[SerializeField]
	public MeshRenderer bulletMeshRenderer;

	private float progress = 0.0f;
	private float progressSpeed = 1.0f;

	private void OnEnable()
	{
		progress = 0.0f;
		if (tower)
		{
			float distance = (tower.bulletSpawnPoint.position - target.Position).magnitude;
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
		transform.position = Vector3.Lerp(tower.bulletSpawnPoint.position, lastKnownPos, progress);

		if (progress >= 1.0f)
		{
			Explode();
			BulletPool.Get.AddBulletToPool(gameObject);
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
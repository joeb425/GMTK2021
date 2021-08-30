using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
	public Tower tower;
	
	public TargetPoint target;

	public float bulletSpeed = 3.0f;

	[SerializeField]
	public MeshFilter bulletMesh;

	[SerializeField]
	public MeshRenderer bulletMeshRenderer;

	private void Update()
	{
		if (target == null)
		{
			gameObject.SetActive(false);
			return;
		}

		transform.LookAt(target.Position);
		transform.position += transform.forward * Time.deltaTime * bulletSpeed;

		float dist = (transform.position - target.Position).magnitude;
		if (dist <= 0.5f)
		{
			Explode();
			gameObject.SetActive(false);
		}
	}

	private void Explode()
	{
		tower.ApplyHit(target);
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public Tower tower;
	
	public TargetPoint target;

	[SerializeField]
	public float bulletSpeed = 3.0f;

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
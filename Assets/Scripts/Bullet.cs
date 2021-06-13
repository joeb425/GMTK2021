using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public Tower tower;
	
	public TargetPoint target;

	public TowerAttributes towerAttributes;

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
		
		// Collider[] splashedTargets = Physics.OverlapSphere(target.Position, 2.0f, 1 << 9);
		//
		// float damage = towerAttributes.finalDamage;
		//
		// foreach (Collider collider in splashedTargets)
		// {
		// 	TargetPoint aoeTarget = collider.GetComponent<TargetPoint>();
		// 	aoeTarget.Enemy.ApplyDamage(damage * towerAttributes.finalSplash);
		// }
		//
		// target.Enemy.ApplyDamage(damage);
	}
}
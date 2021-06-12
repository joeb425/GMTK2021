using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public TargetPoint target;

	// Start is called before the first frame update
	void Start()
	{
	}

	private void Update()
	{
		if (target == null)
		{
			gameObject.SetActive(false);
			return;
		}

		transform.LookAt(target.Position);
		transform.position += transform.forward * Time.deltaTime * 3.0f;

		float dist = (transform.position - target.Position).magnitude;
		if (dist <= 0.5f)
		{
			Explode();
			gameObject.SetActive(false);
		}
	}

	private void Explode()
	{
		target.Enemy.ApplyDamage(50.0f);
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetPoint : MonoBehaviour
{
	public Enemy Enemy { get; private set; }

	public Vector3 Position => transform.position;

	public bool IsValid()
	{
		return Enemy != null && Enemy.isActiveAndEnabled && Enemy.IsAlive();
	}

	void Awake()
	{
		Enemy = transform.root.GetComponent<Enemy>();
	}
}
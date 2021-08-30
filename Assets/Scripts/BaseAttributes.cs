using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BaseAttributes
{
	[SerializeField, Range(0.0f, 50.0f)]
	public float baseTargetingRange;

	[SerializeField, Range(0.0f, 200f)]
	public float baseDamage;

	[SerializeField, Range(0.0f, 200f)]
	public float baseAttackSpeed;

	[SerializeField, Range(0.0f, 200f)]
	public float baseNumTargets;

	[SerializeField, Range(0.0f, 200f)]
	public float baseSplash;
}

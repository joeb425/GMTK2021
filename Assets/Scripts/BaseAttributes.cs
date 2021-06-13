using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttributes : MonoBehaviour
{
	[SerializeField, Range(1.5f, 10.5f)]
	public float baseTargetingRange = 1.5f;

	[SerializeField, Range(0.0f, 200f)]
	public float baseDamage;

	[SerializeField, Range(0.0f, 200f)]
	public float baseAttackSpeed = 1.0f;

	[SerializeField, Range(0.0f, 200f)]
	public float baseNumTargets = 2.0f;

	[SerializeField, Range(0.0f, 200f)]
	public float baseSplash = .5f;

}

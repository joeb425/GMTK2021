using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttributes : MonoBehaviour
{
	[SerializeField, Range(1.5f, 10.5f)]
	public float baseTargetingRange = 1.5f;

	public float finalTargetingRange;

	[SerializeField, Range(0.0f, 200f)]
	public float baseDamage;

	public float finalDamage;

	[SerializeField, Range(0.0f, 200f)]
	public float baseAttackSpeed = 1.0f;

	public float finalAttackSpeed;

	[SerializeField, Range(0.0f, 200f)]
	public float baseNumTargets = 2.0f;

	public float finalNumTargets;

	[SerializeField, Range(0.0f, 200f)]
	public float baseSplash = .5f;

	public float finalSplash;

	public void Reset()
	{
		finalTargetingRange = baseTargetingRange;
		// todo
	}

	public void ApplyPerks(List<Perk> perks)
	{
		// finalDamage = baseDamage * perk.DamageMod
		// set attributes
		
		
		//todo
	}
}

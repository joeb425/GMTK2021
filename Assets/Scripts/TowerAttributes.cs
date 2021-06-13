using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TowerAttributes
{
	[SerializeField]
	List<BaseAttributes> baseAttributesPerLevel;
	
	public float finalTargetingRange;

	public float finalDamage;

	public float finalAttackSpeed;

	public float finalNumTargets;

	public float finalSplash;
	
	public void UpdateAttributes(int level, List<Perk> perks)
	{
		BaseAttributes attributes = baseAttributesPerLevel[level];
		
		finalTargetingRange = attributes.baseTargetingRange;
		
		// todo
	}
}

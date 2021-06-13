using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttributes : MonoBehaviour
{
	[SerializeField]
	List<BaseAttributes> baseAttributesPerLevel;
	
	public float finalTargetingRange;

	public float finalDamage;

	public float finalAttackSpeed;

	public float finalNumTargets;

	public float finalSplash;

	public int level = 0;

	public void UpdateAttributes(int level, List<Perk> perks)
	{
		BaseAttributes attributes = baseAttributesPerLevel[level];
		
		finalTargetingRange = attributes.baseTargetingRange;
		
		// todo
	}
}

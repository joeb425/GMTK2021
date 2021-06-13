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

		float damageMod = 0.0f;
		float splashMod = 0.0f;
		float speedMod = 0.0f;
		float splitMod = 0.0f;
		float rangeMod = 0.0f;
		foreach (Perk perk in perks)
		{
			damageMod += perk.DamageMod;
			splashMod += perk.SplashMod;
			speedMod += perk.SpeedMod;
			splitMod += perk.SplitMod;
			rangeMod += perk.RangeMod;
		}

		finalTargetingRange = attributes.baseTargetingRange + rangeMod;
		finalDamage = attributes.baseDamage * (1.0f + damageMod);
		finalAttackSpeed = attributes.baseAttackSpeed / (1.0f + speedMod);
		finalNumTargets = attributes.baseNumTargets + splitMod;
		finalSplash = attributes.baseSplash + splashMod;
	}
}
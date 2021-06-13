using System;
using UnityEngine;

[Serializable]
public struct Perk
{
	[SerializeField]
	public float SpeedMod;

	[SerializeField]
	public float DamageMod;

	[SerializeField]
	public float SplashMod;

	[SerializeField]
	public float SplitMod;

	[SerializeField]
	public float RangeMod;
}
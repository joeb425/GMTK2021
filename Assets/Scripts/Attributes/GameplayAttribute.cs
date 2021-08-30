using System;
using UnityEngine;

namespace Attributes
{
	[Serializable]
	public enum AttributeType
	{
		Health,
		Range,
		Damage,
		AttackSpeed,
		Split,
		SplashPercent,
	}

	[Serializable]
	public class GameplayAttribute
	{
		[SerializeField]
		public AttributeType attributeType;

		[SerializeField]
		public float defaultValue;

		[HideInInspector]
		public float currentValue;

		public GameplayAttribute()
		{
			defaultValue = 0;
			currentValue = defaultValue;
		}

		public GameplayAttribute(AttributeType attributeType, float defaultValue)
		{
			this.attributeType = attributeType;
			this.defaultValue = defaultValue;
			this.currentValue = defaultValue;
		}
	}
}
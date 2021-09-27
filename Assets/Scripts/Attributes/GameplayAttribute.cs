using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Attributes
{
	[Serializable]
	public enum AttributeType
	{
		Health,
		MaxHealth,
		Range,
		Damage,
		AttackSpeed,
		Split,
		SplashPercent,
		Speed,
		Defence,
	}

	[Serializable]
	public class GameplayAttribute
	{
		[SerializeField]
		public AttributeType attributeType;

		[SerializeField]
		public float defaultValue;

		[HideInInspector]
		private float currentValue;

		public event System.Action<GameplayAttribute> OnAttributeChanged;

		public List<ActiveAttributeModifier> additiveModifiers = new List<ActiveAttributeModifier>();
		public List<ActiveAttributeModifier> multiplicativeModifiers = new List<ActiveAttributeModifier>();
		public List<ActiveAttributeModifier> overrideModifiers = new List<ActiveAttributeModifier>();

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

		public float GetBaseValue()
		{
			return currentValue;
		}

		public float GetValue()
		{
			foreach (var modifier in overrideModifiers.Where(modifier => modifier != null))
			{
				return modifier.modifier.value;
			}

			float totalAdditive = additiveModifiers.Where(modifier => modifier != null).Sum(modifier => modifier.modifier.value);
			float totalMultiplicative = 1 + multiplicativeModifiers.Where(modifier => modifier != null).Sum(modifier => modifier.modifier.value);

			return (GetBaseValue() + totalAdditive) * totalMultiplicative;
		}

		public void SetCurrentValue(float newValue)
		{
			float oldValue = currentValue;
			currentValue = newValue;
			OnAttributeChanged?.Invoke(this);
		}

		public void ApplyModifierDirectly(GameplayAttributeModifier modifier)
		{
			float oldValue = currentValue;
			switch (modifier.valueOperator)
			{
				case AttributeOperator.Add:
					currentValue += modifier.value;
					break;
				case AttributeOperator.Multiply:
					currentValue *= modifier.value;
					break;
				case AttributeOperator.Override:
					currentValue = modifier.value;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			OnAttributeChanged?.Invoke(this);
		}
		
		public ActiveAttributeModifier ApplyModifier(GameplayAttributeModifier modifier)
		{
			switch (modifier.valueOperator)
			{
				case AttributeOperator.Add:
				{
					ActiveAttributeModifier activeAdd = new ActiveAttributeModifier(modifier);
					additiveModifiers.Add(activeAdd);
					OnAttributeChanged?.Invoke(this);
					return activeAdd;
				}
				case AttributeOperator.Multiply:
				{
					ActiveAttributeModifier activeMult = new ActiveAttributeModifier(modifier);
					multiplicativeModifiers.Add(activeMult);
					OnAttributeChanged?.Invoke(this);
					return activeMult;
				}
				case AttributeOperator.Override:
					ActiveAttributeModifier activeOverride = new ActiveAttributeModifier(modifier);
					overrideModifiers.Add(activeOverride);
					OnAttributeChanged?.Invoke(this);
					return activeOverride;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public bool RemoveModifier(ActiveAttributeModifier active)
		{
			switch (active.modifier.valueOperator)
			{
				case AttributeOperator.Add:
					additiveModifiers.Remove(active);
					OnAttributeChanged?.Invoke(this);
					return true;
				case AttributeOperator.Multiply:
					multiplicativeModifiers.Remove(active);
					OnAttributeChanged?.Invoke(this);
					return true;
				case AttributeOperator.Override:
					overrideModifiers.Remove(active);
					OnAttributeChanged?.Invoke(this);
					return true;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return false;
		}
	}
}
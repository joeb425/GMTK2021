using System;
using System.Collections.Generic;
using UnityEngine;

namespace Attributes
{
	public class GameplayAttributeContainer
	{
		public Dictionary<AttributeType, GameplayAttribute> baseAttributes =
			new Dictionary<AttributeType, GameplayAttribute>();

		public Dictionary<AttributeType, GameplayAttribute> finalAttributes =
			new Dictionary<AttributeType, GameplayAttribute>();

		public List<GameplayAttributeModifier> modifiers = new List<GameplayAttributeModifier>();

		public void InitAttribute(AttributeType attributeType, float defaultValue)
		{
			baseAttributes.Add(attributeType,
				new GameplayAttribute(attributeType, defaultValue));

			finalAttributes.Add(attributeType,
				new GameplayAttribute(attributeType, defaultValue));
		}

		public void ApplyModifier(GameplayAttributeModifier modifier)
		{
			modifiers.Add(modifier);
			GameplayAttribute attribute = finalAttributes[modifier.attribute];
			float newValue = finalAttributes[modifier.attribute].currentValue + modifier.value;
			attribute.SetCurrentValue(newValue);
		}

		public void ApplyEffect(GameplayEffect effect)
		{
			foreach (GameplayAttributeModifier mod in effect.Modifiers)
			{
				ApplyModifier(mod);
			}
		}

		public GameplayAttribute GetAttribute(AttributeType attribute)
		{
			return finalAttributes[attribute];
		}
		
		public float GetCurrentValue(AttributeType attribute)
		{
			return finalAttributes[attribute].currentValue;
		}
	}
}
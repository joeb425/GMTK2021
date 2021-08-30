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

		public List<GameplayAttributeModifier> modifiers;

		public void InitAttribute(AttributeType attributeType, float defaultValue)
		{
			baseAttributes.Add(attributeType,
				new GameplayAttribute(attributeType, defaultValue));

			finalAttributes.Add(attributeType,
				new GameplayAttribute(attributeType, defaultValue));
		}

		public void ApplyModifier(GameplayAttributeModifier modifier)
		{
			// TODO
			// modifiers.Add(modifier);
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
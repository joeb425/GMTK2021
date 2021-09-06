using System;
using UnityEngine;

namespace Attributes
{
	public enum AttributeOperator
	{
		Add,
		Multiply,
		Override,
	}

	public class ActiveAttributeModifier
	{
		public GameplayAttributeModifier modifier;

		public ActiveAttributeModifier(GameplayAttributeModifier modifier)
		{
			this.modifier = modifier;
		}

		public override string ToString()
		{
			return $"{modifier} (i)";
		}
	}

	[Serializable]
	public class GameplayAttributeModifier
	{
		[SerializeField]
		public AttributeType attribute;
		
		[SerializeField]
		public float value;
		
		[SerializeField]
		public AttributeOperator valueOperator;

		public GameplayAttributeModifier(AttributeType attribute, float value, AttributeOperator valueOperator)
		{
			this.attribute = attribute;
			this.value = value;
			this.valueOperator = valueOperator;
		}

		public override string ToString()
		{
			return $"{attribute} {valueOperator} {value}";
		}
	}
}
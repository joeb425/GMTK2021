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

	[Serializable]
	public class GameplayAttributeModifier
	{
		[SerializeField]
		public AttributeType attribute;
		
		[SerializeField]
		public float value;
		
		[SerializeField]
		public AttributeOperator valueOperator;
	}
}
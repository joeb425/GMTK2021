using Attributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.MainMenu
{
	public class AttributeDisplay : VisualElement
	{
		private Label _attributeName;
		private Label _attributeValue;

		public new class UxmlFactory : UxmlFactory<AttributeDisplay, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		public AttributeDisplay()
		{
			RegisterCallback<AttachToPanelEvent>(OnAttach); // phil?=<ghenius 
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
			_attributeName = this.Q<Label>("AttributeName");
			_attributeValue = this.Q<Label>("AttributeValue");

			if (_attributeName == null || _attributeValue == null)
			{
				Debug.LogError("Failed to find attribute name or attribute value labels");
			}
		}

		public void BindToGameplayAttribute(GameplayAttribute attribute)
		{
			_attributeName.text = attribute.attributeType.ToString();
			_attributeValue.text = "" + attribute.GetValue();
		}
	}
}
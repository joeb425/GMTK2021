using Mantis.AttributeSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.MainMenu
{
	public class AttributeDisplay : VisualElement
	{
		private Label _attributeName;
		private Label _attributeValue;
		private bool _isInitialized = false;

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

		public void Init()
		{
			if (_isInitialized)
			{
				return;
			}

			_isInitialized = true;

			_attributeName = this.Q<Label>("AttributeName");
			_attributeValue = this.Q<Label>("AttributeValue");

			if (_attributeValue == null)
			{
				Debug.LogError("Failed to find attribute value label");
			}
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
			Init();
		}

		public void BindToGameplayAttribute(GameplayAttribute attribute)
		{
			Init();

			if (_attributeName != null)
			{
				_attributeName.text = attribute.attributeType.name;
			}

			_attributeValue.text = $"{attribute.GetValue()}";
			attribute.OnAttributeChanged += gameplayAttribute => _attributeValue.text = $"{attribute.GetValue()}";
		}

		public void BindToAttributeDefault(AttributeDefault defaultValue)
		{
			_attributeName.text = defaultValue.attribute.name;
			_attributeValue.text = $"{defaultValue.value}";
		}
	}
}
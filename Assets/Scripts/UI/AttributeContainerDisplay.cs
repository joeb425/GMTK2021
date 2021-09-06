using System.Collections.Generic;
using Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.MainMenu
{
	public class AttributeContainerDisplay : VisualElement
	{
		private VisualTreeAsset _attributeDisplay;
		private VisualElement _attributeContainer;

		public new class UxmlFactory : UxmlFactory<AttributeContainerDisplay, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		public AttributeContainerDisplay()
		{
			RegisterCallback<AttachToPanelEvent>(OnAttach);
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
			_attributeDisplay = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/AttributeDisplay.uxml");
			_attributeContainer = this.Q<VisualElement>("Container");
		}

		public void BindToAttributeContainer(GameplayAttributeContainer container)
		{
			_attributeContainer.Clear();

			foreach (KeyValuePair<AttributeType, GameplayAttribute> kvp in container.attributes)
			{
				VisualElement attributeDisplayInstance = _attributeDisplay.CloneTree();
				_attributeContainer.Add(attributeDisplayInstance);

				AttributeDisplay attributeDisplay = attributeDisplayInstance.Q<AttributeDisplay>();
				if (attributeDisplay == null)
				{
					Debug.Log("Failed to find attribute display from instance");
				}
				else
				{
					attributeDisplay.BindToGameplayAttribute(kvp.Value);
				}
			}
		}
	}
}
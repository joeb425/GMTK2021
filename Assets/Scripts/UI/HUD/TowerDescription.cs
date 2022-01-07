using Mantis.Utils.UI;
using UI.MainMenu;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerDescription : MantisVisualElement
{
	private Tower _currentTower;
	private Label _nameLabel;
	private Label _descriptionLabel;

	public AttributeContainerDisplay AttributeContainerDisplay { get; private set; }

	public new class UxmlFactory : UxmlFactory<TowerDescription, UxmlTraits>
	{
	}

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
	}

	protected override void OnAttach(AttachToPanelEvent evt)
	{
		AttributeContainerDisplay = this.Q<AttributeContainerDisplay>();
		_nameLabel = this.Q<Label>("NameLabel");
		_descriptionLabel = this.Q<Label>("DescriptionLabel");
		
		if (AttributeContainerDisplay == null)
		{
			Debug.Log("Failed to find attribute container display");
		}
	}

	public void BindToTower(Tower tower)
	{
		_currentTower = tower;

		bool towerIsNull = _currentTower == null;

		style.display = towerIsNull ? DisplayStyle.None : DisplayStyle.Flex;

		_nameLabel.text = towerIsNull ? "" : _currentTower.towerData.towerName;
		_descriptionLabel.text = towerIsNull ? "" : _currentTower.towerData.towerDescription;

		AttributeContainerDisplay.BindToAttributeContainer(towerIsNull ? null : tower.Attributes);
	}
}
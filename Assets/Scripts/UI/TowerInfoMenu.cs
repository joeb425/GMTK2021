using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class TowerInfoMenu : VisualElement
{
	private VisualTreeAsset _iconButton;

	public new class UxmlFactory : UxmlFactory<TowerInfoMenu, UxmlTraits>
	{
	}

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
	}


	public TowerInfoMenu()
	{

	}
}

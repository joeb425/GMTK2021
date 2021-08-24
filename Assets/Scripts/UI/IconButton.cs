using UnityEngine.UIElements;

namespace UI.MainMenu
{
	public class IconButton : VisualElement
	{
		public Label label;
		public string ItemGuid;

		public IconButton()
		{
			label = new Label();
			Add(label);
			label.text = "testing";
		}
	}
}
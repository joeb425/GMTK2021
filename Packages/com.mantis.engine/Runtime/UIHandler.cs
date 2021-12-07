using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mantis.Engine
{
	public class UIHandler : MonoBehaviour
	{
		[SerializeField]
		private UIDocument hud;

		private VisualElement _rootVisualElement;
		private List<VisualElement> _elementsBlockingMouse = new List<VisualElement>();

		public void Init()
		{
			ReadUIDocument();
		}

		public void ReadUIDocument()
		{
			_rootVisualElement = hud.rootVisualElement;
		}

		public void SetElementBlockingMouse(VisualElement elem, bool block)
		{
			if (block)
			{
				_elementsBlockingMouse.Add(elem);
			}
			else
			{
				_elementsBlockingMouse.Remove(elem);
			}
		}

		public bool IsMouseBlocked(Vector2 mousePos)
		{
			return _elementsBlockingMouse.Any(elem => elem.ContainsPoint(elem.WorldToLocal(mousePos)));
		}
	}
}
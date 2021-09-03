using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.HexGridEditor
{
	[ExecuteInEditMode]
	public class HexGrid : MonoBehaviour
	{
		public float width = 32.0f;
		public float height = 32.0f;

		[SerializeField]
		public GameObject buildPrefab;

		[SerializeField]
		public GameObject pathPrefab;

		public Layout flat;

		// public HexGridData _hexGridData = new HexGridData();
		public Dictionary<Hex, int> hexGrid = new Dictionary<Hex, int>();

		[SerializeField]
		public Plane gridPlane = new Plane(Vector3.up, Vector3.zero);
		
		public HexGrid()
		{
			flat = new Layout(Layout.flat, new Point(1.0, 1.0), new Point(0.0, 0.0));
		}

		private void Start()
		{
			// List<Hex> hexes = new List<Hex>();
			// hexes.Add(new Hex(0, 0, 0));
			// hexes.Add(new Hex(1, -1, 0));
			// // hexes.Add(new Hex(1, 0, -1));
			// hexes.Add(new Hex(-1, 0, 1));
			//
			// foreach (Hex hex in hexes)
			// {
			// 	var prefab = Instantiate(hexPrefab, transform.parent, true);
			// 	var world = flat.HexToPixel(hex);
			// 	prefab.transform.position = new Vector3((float)world.x, 0, (float)world.y);
			// 	Debug.Log("set hex pos");
			// }
		}
		private void Update()
		{
			// Debug.Log("updating");
			//
			// var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//
			// if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
			// {
			// 	Debug.Log(hit.point);
			// 	Point point = new Point(hit.point.x, hit.point.z);
			// 	Hex hex = flat.PixelToHex(point).HexRound();
			// 	Debug.Log(hex);
			// 	Point roundedWorld = flat.HexToPixel(hex);
			// 	_spawneHex.transform.position = new Vector3((float)roundedWorld.x, 0, (float)roundedWorld.y);
			// }
		}

		void OnDrawGizmos()
		{
			// TODO draw hex grid
			
			// Vector3 pos = Camera.current.transform.position;
			// for (float y = pos.y - 800.0f; y < pos.y + 800.0f; y += height)
			// {
			// 	Gizmos.DrawLine(
			// 		new Vector3(-1000000.0f, 0.0f, Mathf.Floor(y / height) * height),
			// 		new Vector3(1000000.0f, 0.0f, Mathf.Floor(y / height) * height));
			// }
			//
			// for (float x = pos.x - 1200.0f; x < pos.x + 1200.0f; x += width)
			// {
			// 	Gizmos.DrawLine(
			// 		new Vector3(Mathf.Floor(x / width) * width, 0.0f, -1000000.0f),
			// 		new Vector3(Mathf.Floor(x / width) * width, 0.0f, 1000000.0f));
			// }
		}
	}
}
using System;
using UnityEngine;

public class CameraOutline : MonoBehaviour
{
	[SerializeField]
	Mesh mesh;

	[SerializeField]
	Material material;

	private GameTile _selectedTile;

	private void Start()
	{
		GameState.Get.Board.OnSelectedTileChanged += OnSelectedTileChanged;
	}

	private void OnSelectedTileChanged(GameTile oldTile, GameTile newTile)
	{
		_selectedTile = newTile;
	}

	private void OnPostRender()
	{
		material.SetPass(0);
		if (_selectedTile)
		{
			Graphics.DrawMeshNow(mesh, _selectedTile.Content.transform.position + Vector3.up * 2.0f, Quaternion.identity);
		}
	}
}
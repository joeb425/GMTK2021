using System;
using HexLibrary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using UnityEngineInternal;

// TODO: Enhanced touch
// https://gamedev-resources.com/implementing-touch-with-input-systems-enhanced-touch-api/


public class InputHandler
{
	private GameInputs _gameInputs;
	public Ray touchRay;

	public Vector2 mousePos;
	
	private bool _isFastForwardEnabled = false;

	private readonly float _cameraSpeed = 0.025f;
	private readonly float _cameraLerpSpeed = 15.0f;
	private readonly float _cameraZoomSpeed = 10.0f;

	private float _desiredCameraSize;
	private Vector3 _desiredCameraLocation;

	public void Init()
	{
		TouchSimulation.Enable();

		_gameInputs = new GameInputs();
		_gameInputs.Enable();
		_gameInputs.Mouse.MouseClick.performed += ctx => MouseClick();
		_gameInputs.Keyboard.FastForward.performed += ctx => FastForward();

		Camera camera = Camera.main;
		_desiredCameraSize = camera.orthographicSize;

		_desiredCameraLocation = camera.transform.position;
	}

	public void Disable()
	{
		_gameInputs.Disable();
	}

	public void MouseClick()
	{
		if (Game.Get.GetUIHandler().IsMouseBlocked(mousePos))
		{
			return;
		}
		
		GameBoard board = GameState.Get.Board;
		if (board.hoveredTile != null)
		{
			board.SelectTile(board.hoveredTile);
		}
	}

	private void FastForward()
	{
		_isFastForwardEnabled = !_isFastForwardEnabled;
		Time.timeScale = _isFastForwardEnabled ? 2.0f : 1.0f;
	}

	public void GameUpdate()
	{
		Camera camera = Camera.main;
		
		// Update touch ray
		mousePos = _gameInputs.Mouse.MousePosition.ReadValue<Vector2>();
		touchRay = camera.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0.0f));

		UpdateCamera();
	}

	// TODO Move camera control logic elsewhere
	public void UpdateCamera()
	{
		Camera camera = Camera.main;
		
		// Update camera position
		if (!Game.Get.GetUIHandler().IsMouseBlocked(mousePos))
		{
			Vector2 cameraDelta = _gameInputs.Mouse.TouchDelta.ReadValue<Vector2>();
			_desiredCameraLocation -= new Vector3(cameraDelta.x, 0, cameraDelta.y) * _cameraSpeed;
			camera.transform.position = Vector3.Lerp(camera.transform.position, _desiredCameraLocation, Time.deltaTime *
				_cameraLerpSpeed);
		}

		// Update zoom
		float zoom = _gameInputs.Mouse.MouseWheel.ReadValue<float>();
		var orthographicSize = camera.orthographicSize;
		_desiredCameraSize -= zoom * 0.4f;
		camera.orthographicSize = Mathf.Lerp(orthographicSize, _desiredCameraSize, Time.deltaTime * _cameraZoomSpeed);
	}
}
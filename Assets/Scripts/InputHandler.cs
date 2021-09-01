using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputHandler
{
	private GameInputs _gameInputs;
	public Ray touchRay;
	private bool _isFastForwardEnabled = false;

	public static InputHandler Get;

	private readonly float _cameraSpeed = 0.025f;
	private readonly float _cameraLerpSpeed = 15.0f;
	private readonly float _cameraZoomSpeed = 10.0f;

	private float _desiredCameraSize;
	private Vector3 _desiredCameraLocation;

	public void Init()
	{
		Get = this;

		TouchSimulation.Enable();

		_gameInputs = new GameInputs();
		_gameInputs.Enable();
		_gameInputs.Mouse.MouseClick.performed += ctx => MouseClick();
		_gameInputs.Keyboard.FastForward.performed += ctx => FastForward();

		Camera camera = Camera.main;
		_desiredCameraSize = camera.orthographicSize;

		_desiredCameraLocation = camera.transform.position;
	}

	public void MouseClick()
	{
		GameBoard board = GameState.Get.Board;
		GameTile tile = board.GetTile(touchRay);
		if (tile != null)
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
		Vector2 mousePosition = _gameInputs.Mouse.MousePosition.ReadValue<Vector2>();
		touchRay = camera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0.0f));

		UpdateCamera();
	}

	// TODO Move camera control logic elsewhere
	public void UpdateCamera()
	{
		Camera camera = Camera.main;
		
		// Update camera position
		Vector2 cameraDelta = _gameInputs.Mouse.TouchDelta.ReadValue<Vector2>();
		_desiredCameraLocation -= new Vector3(cameraDelta.x, 0, cameraDelta.y) * _cameraSpeed;
		camera.transform.position = Vector3.Lerp(camera.transform.position, _desiredCameraLocation, Time.deltaTime *
			_cameraLerpSpeed);

		// Update zoom
		float zoom = _gameInputs.Mouse.MouseWheel.ReadValue<float>();
		var orthographicSize = camera.orthographicSize;
		_desiredCameraSize -= zoom * 0.4f;
		camera.orthographicSize = Mathf.Lerp(orthographicSize, _desiredCameraSize, Time.deltaTime * _cameraZoomSpeed);
	}
}
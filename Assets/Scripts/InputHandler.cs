using UnityEngine;

public class InputHandler
{
	private GameInputs _gameInputs;
	public Ray touchRay;
	private bool _isFastForwardEnabled = false;


	public static InputHandler Get;


	public void Init()
	{
		Get = this;
		
		_gameInputs = new GameInputs();
		_gameInputs.Enable();
		_gameInputs.Mouse.MouseClick.performed += ctx => MouseClick();
		_gameInputs.Keyboard.FastForward.performed += ctx => FastForward();
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
		Vector2 mousePosition = _gameInputs.Mouse.MousePosition.ReadValue<Vector2>();
		touchRay = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0.0f));
	}
}